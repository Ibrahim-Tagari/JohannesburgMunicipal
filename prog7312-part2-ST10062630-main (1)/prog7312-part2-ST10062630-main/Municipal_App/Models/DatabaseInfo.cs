using System;
using System.Collections.Generic;

namespace Municipal_App.Models
{
    public class DatabaseInfo
    {
        public string DatabaseName { get; set; } = string.Empty;
        public string DatabaseType { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string? ConnectionString { get; set; }
        public string? DatabasePath { get; set; }
        public bool IsConnected { get; set; }
        public bool IsHealthy { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public DateTime? LastModified { get; set; }
        public string? Version { get; set; }
        public bool WALModeEnabled { get; set; }

        private long _size;
        public long Size
        {
            get => _size;
            set => _size = value;
        }

        public double SizeMB
        {
            get => _size / (1024.0 * 1024.0);
            set => _size = (long)(value * 1024.0 * 1024.0);
        }

        public int UserCount { get; set; }
        public int IssueCount { get; set; }
        public int EventCount { get; set; }
        public int NotificationCount { get; set; }

        public int TotalUsers { get; set; }
        public int TotalIssues { get; set; }
        public int TotalEvents { get; set; }
        public int TotalNotifications { get; set; }

        public Dictionary<string, object> Statistics { get; set; } = new Dictionary<string, object>();
    }
}
