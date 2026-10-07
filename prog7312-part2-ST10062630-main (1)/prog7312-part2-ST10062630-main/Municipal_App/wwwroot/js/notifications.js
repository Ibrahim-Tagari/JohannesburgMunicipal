class NotificationManager {
    constructor() {
        this.isSupported = "Notification" in window && "serviceWorker" in navigator
        this.permission = this.isSupported ? Notification.permission : "denied"
        this.updateInterval = null
        this.init()
    }

    async init() {
        if (!this.isSupported) {
            console.warn("Push notifications are not supported in this browser")
            return
        }

        // Register service worker
        try {
            await navigator.serviceWorker.register("/sw.js")
            console.log("Service Worker registered successfully")
        } catch (error) {
            console.error("Service Worker registration failed:", error)
        }

        // Load initial notifications
        await this.loadNotifications()
        await this.updateUnreadCount()

        // Start periodic updates
        this.startPeriodicUpdates()

        // Setup event listeners
        this.setupEventListeners()
    }

    setupEventListeners() {
        // Notification dropdown toggle
        const notificationToggle = document.getElementById("notificationToggle")
        const notificationDropdown = document.getElementById("notificationDropdown")

        if (notificationToggle && notificationDropdown) {
            notificationToggle.addEventListener("click", (e) => {
                e.preventDefault()
                notificationDropdown.classList.toggle("show")
            })

            // Close dropdown when clicking outside
            document.addEventListener("click", (e) => {
                if (!notificationToggle.contains(e.target) && !notificationDropdown.contains(e.target)) {
                    notificationDropdown.classList.remove("show")
                }
            })
        }

        // Mark all as read button
        const markAllReadBtn = document.getElementById("markAllReadBtn")
        if (markAllReadBtn) {
            markAllReadBtn.addEventListener("click", () => this.markAllAsRead())
        }

        // Clear read notifications button
        const clearReadBtn = document.getElementById("clearReadBtn")
        if (clearReadBtn) {
            clearReadBtn.addEventListener("click", () => this.clearReadNotifications())
        }

        // Enable notifications button
        const enableNotificationsBtn = document.getElementById("enableNotificationsBtn")
        if (enableNotificationsBtn) {
            enableNotificationsBtn.addEventListener("click", () => this.requestPermission())
        }
    }

    async requestPermission() {
        if (!this.isSupported) {
            alert("Push notifications are not supported in this browser")
            return false
        }

        try {
            const permission = await Notification.requestPermission()
            this.permission = permission

            if (permission === "granted") {
                await this.subscribeToPush()
                this.updateNotificationUI()
                return true
            } else {
                alert("Notification permission denied. You can enable it in your browser settings.")
                return false
            }
        } catch (error) {
            console.error("Error requesting notification permission:", error)
            return false
        }
    }

    async subscribeToPush() {
        try {
            const registration = await navigator.serviceWorker.ready
            const subscription = await registration.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: this.urlBase64ToUint8Array("your-vapid-public-key-here"),
            })

            // Send subscription to server
            await fetch("/api/notifications/subscribe", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    endpoint: subscription.endpoint,
                    p256dh: subscription.getKey
                        ? btoa(String.fromCharCode(...new Uint8Array(subscription.getKey("p256dh"))))
                        : null,
                    auth: subscription.getKey ? btoa(String.fromCharCode(...new Uint8Array(subscription.getKey("auth")))) : null,
                }),
            })

            console.log("Successfully subscribed to push notifications")
        } catch (error) {
            console.error("Error subscribing to push notifications:", error)
        }
    }

    urlBase64ToUint8Array(base64String) {
        const padding = "=".repeat((4 - (base64String.length % 4)) % 4)
        const base64 = (base64String + padding).replace(/-/g, "+").replace(/_/g, "/")

        const rawData = window.atob(base64)
        const outputArray = new Uint8Array(rawData.length)

        for (let i = 0; i < rawData.length; ++i) {
            outputArray[i] = rawData.charCodeAt(i)
        }
        return outputArray
    }

    async loadNotifications() {
        try {
            const response = await fetch("/api/notifications")
            if (response.ok) {
                const notifications = await response.json()
                this.renderNotifications(notifications)
            }
        } catch (error) {
            console.error("Error loading notifications:", error)
        }
    }

    async updateUnreadCount() {
        try {
            const response = await fetch("/api/notifications/unread-count")
            if (response.ok) {
                const data = await response.json()
                this.updateUnreadBadge(data.count)
            }
        } catch (error) {
            console.error("Error updating unread count:", error)
        }
    }

    updateUnreadBadge(count) {
        const badge = document.getElementById("notificationBadge")
        if (badge) {
            if (count > 0) {
                badge.textContent = count > 99 ? "99+" : count.toString()
                badge.style.display = "inline-block"
            } else {
                badge.style.display = "none"
            }
        }
    }

    renderNotifications(notifications) {
        const container = document.getElementById("notificationList")
        if (!container) return

        if (notifications.length === 0) {
            container.innerHTML = `
                <div class="dropdown-item text-center text-muted py-3">
                    <i class="fas fa-inbox fa-2x mb-2"></i>
                    <div>No notifications</div>
                </div>
            `
            return
        }

        container.innerHTML = notifications
            .map(
                (notification) => `
            <div class="dropdown-item notification-item ${notification.isRead ? "read" : "unread"}" 
                 data-id="${notification.id}">
                <div class="d-flex justify-content-between align-items-start">
                    <div class="flex-grow-1">
                        <div class="notification-title fw-semibold">${notification.title}</div>
                        <div class="notification-message text-muted small">${notification.message}</div>
                        <div class="notification-time text-muted small">
                            <i class="fas fa-clock me-1"></i>
                            ${this.formatTime(notification.createdAt)}
                        </div>
                    </div>
                    <div class="notification-actions">
                        ${!notification.isRead
                        ? `
                            <button class="btn btn-sm btn-outline-primary mark-read-btn" 
                                    onclick="notificationManager.markAsRead(${notification.id})">
                                <i class="fas fa-check"></i>
                            </button>
                        `
                        : ""
                    }
                        <button class="btn btn-sm btn-outline-danger delete-btn" 
                                onclick="notificationManager.deleteNotification(${notification.id})">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
                ${notification.actionUrl
                        ? `
                    <div class="mt-2">
                        <a href="${notification.actionUrl}" class="btn btn-sm btn-primary">
                            ${notification.actionText || "View"}
                        </a>
                    </div>
                `
                        : ""
                    }
            </div>
        `,
            )
            .join("")
    }

    async markAsRead(notificationId) {
        try {
            const response = await fetch(`/api/notifications/${notificationId}/mark-read`, {
                method: "POST",
            })

            if (response.ok) {
                await this.loadNotifications()
                await this.updateUnreadCount()
            }
        } catch (error) {
            console.error("Error marking notification as read:", error)
        }
    }

    async markAllAsRead() {
        try {
            const response = await fetch("/api/notifications/mark-all-read", {
                method: "POST",
            })

            if (response.ok) {
                await this.loadNotifications()
                await this.updateUnreadCount()
            }
        } catch (error) {
            console.error("Error marking all notifications as read:", error)
        }
    }

    async deleteNotification(notificationId) {
        try {
            const response = await fetch(`/api/notifications/${notificationId}`, {
                method: "DELETE",
            })

            if (response.ok) {
                await this.loadNotifications()
                await this.updateUnreadCount()
            }
        } catch (error) {
            console.error("Error deleting notification:", error)
        }
    }

    async clearReadNotifications() {
        try {
            const response = await fetch("/api/notifications/read", {
                method: "DELETE",
            })

            if (response.ok) {
                await this.loadNotifications()
                await this.updateUnreadCount()
            }
        } catch (error) {
            console.error("Error clearing read notifications:", error)
        }
    }

    formatTime(dateString) {
        const date = new Date(dateString)
        const now = new Date()
        const diffInMinutes = Math.floor((now - date) / (1000 * 60))

        if (diffInMinutes < 1) return "Just now"
        if (diffInMinutes < 60) return `${diffInMinutes}m ago`
        if (diffInMinutes < 1440) return `${Math.floor(diffInMinutes / 60)}h ago`
        if (diffInMinutes < 10080) return `${Math.floor(diffInMinutes / 1440)}d ago`

        return date.toLocaleDateString()
    }

    startPeriodicUpdates() {
        // Update notifications every 30 seconds
        this.updateInterval = setInterval(() => {
            this.updateUnreadCount()
        }, 30000)
    }

    stopPeriodicUpdates() {
        if (this.updateInterval) {
            clearInterval(this.updateInterval)
            this.updateInterval = null
        }
    }

    updateNotificationUI() {
        const enableBtn = document.getElementById("enableNotificationsBtn")
        if (enableBtn) {
            if (this.permission === "granted") {
                enableBtn.style.display = "none"
            } else {
                enableBtn.style.display = "block"
            }
        }
    }
}

// Initialize notification manager when DOM is loaded
document.addEventListener("DOMContentLoaded", () => {
    window.notificationManager = new NotificationManager()
})
