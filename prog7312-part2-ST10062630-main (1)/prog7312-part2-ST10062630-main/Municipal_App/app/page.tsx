/*
'use client'

import { useState } from 'react'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Progress } from '@/components/ui/progress'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { Badge } from '@/components/ui/badge'
import { CalendarDays, Search, AlertTriangle, Home, ArrowLeft, Upload, MapPin, Tag, FileText, CheckCircle } from 'lucide-react'

interface Issue {
    id: number
    location: string
    category: string
    description: string
    dateReported: string
    status: string
    attachedFiles: string[]
}

export default function MunicipalServicesApp() {
    const [currentView, setCurrentView] = useState<'home' | 'report' | 'success'>('home')
    const [formData, setFormData] = useState({
        location: '',
        category: '',
        description: ''
    })
    const [submittedIssue, setSubmittedIssue] = useState<Issue | null>(null)
    const [progress, setProgress] = useState(25)
    const [engagementMessage, setEngagementMessage] = useState("Providing detailed information helps us resolve your issue faster!")

    const categories = [
        'Sanitation',
        'Roads & Infrastructure',
        'Utilities',
        'Street Lighting',
        'Water Supply',
        'Waste Management',
        'Public Safety',
        'Other'
    ]

    const updateProgress = () => {
        let newProgress = 25
        let message = "Step 1 of 4: Provide Issue Details"
        let engagementMsg = "Providing detailed information helps us resolve your issue faster!"

        if (formData.location) newProgress += 25
        if (formData.category) newProgress += 25
        if (formData.description.length > 10) newProgress += 25

        if (newProgress >= 50) {
            message = "Step 2 of 4: Great progress!"
            engagementMsg = "You're doing great! Adding photos can help our team understand the issue better."
        }
        if (newProgress >= 75) {
            message = "Step 3 of 4: Almost done!"
            engagementMsg = "Excellent! Your detailed report will help us prioritize and resolve this issue quickly."
        }
        if (newProgress >= 100) {
            message = "Step 4 of 4: Ready to submit!"
            engagementMsg = "Perfect! Your comprehensive report is ready for submission."
        }

        setProgress(newProgress)
        setEngagementMessage(engagementMsg)
    }

    const handleInputChange = (field: string, value: string) => {
        setFormData(prev => ({ ...prev, [field]: value }))
        setTimeout(updateProgress, 100)
    }

    const handleSubmit = () => {
        if (!formData.location || !formData.category || !formData.description) {
            alert('Please fill in all required fields')
            return
        }

        const newIssue: Issue = {
            id: Math.floor(Math.random() * 10000),
            location: formData.location,
            category: formData.category,
            description: formData.description,
            dateReported: new Date().toLocaleDateString('en-ZA'),
            status: 'Submitted',
            attachedFiles: []
        }

        setSubmittedIssue(newIssue)
        setCurrentView('success')
    }

    const resetForm = () => {
        setFormData({ location: '', category: '', description: '' })
        setCurrentView('home')
        setProgress(25)
        setEngagementMessage("Providing detailed information helps us resolve your issue faster!")
    }

    if (currentView === 'home') {
        return (
            <div className="min-h-screen bg-gray-50">
                {/* Header ?}
                <header className="bg-blue-600 text-white shadow-lg">
                    <div className="container mx-auto px-4 py-4">
                        <h1 className="text-2xl font-bold flex items-center gap-2">
                            🏛️ Municipal Services
                        </h1>
                        <p className="text-blue-100 mt-1">Your gateway to efficient municipal service delivery in South Africa</p>
                    </div>
                </header>

                {/* Main Content ?}
                <main className="container mx-auto px-4 py-8">
                    <div className="text-center mb-8">
                        <h2 className="text-3xl font-bold text-gray-800 mb-4">Welcome to Municipal Services</h2>
                        <p className="text-lg text-gray-600">Your gateway to efficient municipal service delivery in South Africa</p>
                    </div>

                    <div className="grid md:grid-cols-3 gap-6 mb-8">
                        {/* Report Issues Card ?}
                        <Card className="hover:shadow-lg transition-shadow cursor-pointer">
                            <CardHeader className="text-center">
                                <AlertTriangle className="w-12 h-12 text-yellow-500 mx-auto mb-3" />
                                <CardTitle>Report Issues</CardTitle>
                            </CardHeader>
                            <CardContent className="text-center">
                                <p className="text-gray-600 mb-4">
                                    Report municipal issues such as potholes, water leaks, or sanitation problems in your area.
                                </p>
                                <Button
                                    onClick={() => setCurrentView('report')}
                                    className="w-full bg-blue-600 hover:bg-blue-700"
                                >
                                    Report an Issue
                                </Button>
                            </CardContent>
                        </Card>

                        {/* Local Events Card ?}
                        <Card className="hover:shadow-lg transition-shadow opacity-60">
                            <CardHeader className="text-center">
                                <CalendarDays className="w-12 h-12 text-blue-500 mx-auto mb-3" />
                                <CardTitle>Local Events & Announcements</CardTitle>
                            </CardHeader>
                            <CardContent className="text-center">
                                <p className="text-gray-600 mb-4">
                                    Stay updated with local events, community meetings, and municipal announcements.
                                </p>
                                <Button disabled className="w-full">
                                    Coming Soon
                                </Button>
                            </CardContent>
                        </Card>

                        {/* Service Status Card ?}
                        <Card className="hover:shadow-lg transition-shadow opacity-60">
                            <CardHeader className="text-center">
                                <Search className="w-12 h-12 text-green-500 mx-auto mb-3" />
                                <CardTitle>Service Request Status</CardTitle>
                            </CardHeader>
                            <CardContent className="text-center">
                                <p className="text-gray-600 mb-4">
                                    Track the progress of your submitted service requests and get real-time updates.
                                </p>
                                <Button disabled className="w-full">
                                    Coming Soon
                                </Button>
                            </CardContent>
                        </Card>
                    </div>

                    {/* Info Alert ?}
                    <Alert className="border-blue-200 bg-blue-50">
                        <AlertTriangle className="h-4 w-4 text-blue-600" />
                        <AlertDescription className="text-blue-800">
                            <strong>Getting Started:</strong> Welcome to the Municipal Services Application! This platform is designed to improve citizen engagement and service delivery. Currently, you can report issues to your local municipality. Additional features will be available soon.
                        </AlertDescription>
                    </Alert>
                </main>
            </div>
        )
    }

    if (currentView === 'report') {
        return (
            <div className="min-h-screen bg-gray-50">
                {/* Header ?}
                <header className="bg-blue-600 text-white shadow-lg">
                    <div className="container mx-auto px-4 py-4">
                        <h1 className="text-2xl font-bold flex items-center gap-2">
                            🏛️ Municipal Services
                        </h1>
                    </div>
                </header>

                <main className="container mx-auto px-4 py-8">
                    <div className="max-w-4xl mx-auto">
                        <Card className="shadow-lg">
                            <CardHeader className="bg-blue-600 text-white">
                                <CardTitle className="flex items-center gap-2">
                                    <AlertTriangle className="w-6 h-6" />
                                    Report Municipal Issue
                                </CardTitle>
                            </CardHeader>
                            <CardContent className="p-6">
                                {/* Progress Bar ?}
                                <div className="mb-6">
                                    <Progress value={progress} className="h-6" />
                                    <p className="text-sm text-gray-600 mt-2">
                                        {progress >= 100 ? "Step 4 of 4: Ready to submit!" :
                                            progress >= 75 ? "Step 3 of 4: Almost done!" :
                                                progress >= 50 ? "Step 2 of 4: Great progress!" :
                                                    "Step 1 of 4: Provide Issue Details"}
                                    </p>
                                </div>

                                <div className="grid md:grid-cols-2 gap-6">
                                    {/* Location Input ?}
                                    <div>
                                        <Label htmlFor="location" className="flex items-center gap-2 mb-2">
                                            <MapPin className="w-4 h-4" />
                                            Location of Issue *
                                        </Label>
                                        <Input
                                            id="location"
                                            placeholder="e.g., Corner of Main St and Church St, Johannesburg"
                                            value={formData.location}
                                            onChange={(e) => handleInputChange('location', e.target.value)}
                                            className="w-full"
                                        />
                                    </div>

                                    {/* Category Selection ?}
                                    <div>
                                        <Label className="flex items-center gap-2 mb-2">
                                            <Tag className="w-4 h-4" />
                                            Issue Category *
                                        </Label>
                                        <Select value={formData.category} onValueChange={(value) => handleInputChange('category', value)}>
                                            <SelectTrigger>
                                                <SelectValue placeholder="Select a category..." />
                                            </SelectTrigger>
                                            <SelectContent>
                                                {categories.map((category) => (
                                                    <SelectItem key={category} value={category}>
                                                        {category}
                                                    </SelectItem>
                                                ))}
                                            </SelectContent>
                                        </Select>
                                    </div>
                                </div>

                                {/* Description ?}
                                <div className="mt-6">
                                    <Label htmlFor="description" className="flex items-center gap-2 mb-2">
                                        <FileText className="w-4 h-4" />
                                        Detailed Description *
                                    </Label>
                                    <Textarea
                                        id="description"
                                        placeholder="Please provide a detailed description of the issue, including when you first noticed it and any relevant details..."
                                        value={formData.description}
                                        onChange={(e) => handleInputChange('description', e.target.value)}
                                        className="min-h-32"
                                        maxLength={1000}
                                    />
                                    <p className="text-sm text-gray-500 mt-1">
                                        {formData.description.length}/1000 characters
                                    </p>
                                </div>

                                {/* File Upload ?}
                                <div className="mt-6">
                                    <Label className="flex items-center gap-2 mb-2">
                                        <Upload className="w-4 h-4" />
                                        Attach Files (Images/Documents)
                                    </Label>
                                    <div className="border-2 border-dashed border-gray-300 rounded-lg p-6 text-center">
                                        <Upload className="w-8 h-8 text-gray-400 mx-auto mb-2" />
                                        <p className="text-gray-600">Click to upload or drag and drop</p>
                                        <p className="text-sm text-gray-500">Images, PDF, DOC, DOCX (Max 5 files)</p>
                                    </div>
                                </div>

                                {/* Engagement Message ?}
                                <Alert className="mt-6 border-blue-200 bg-blue-50">
                                    <AlertTriangle className="h-4 w-4 text-blue-600" />
                                    <AlertDescription className="text-blue-800">
                                        <strong>Engagement Tip:</strong> {engagementMessage}
                                    </AlertDescription>
                                </Alert>

                                {/* Action Buttons ?}
                                <div className="flex flex-col sm:flex-row gap-3 mt-8 justify-end">
                                    <Button
                                        variant="outline"
                                        onClick={() => setCurrentView('home')}
                                        className="flex items-center gap-2"
                                    >
                                        <ArrowLeft className="w-4 h-4" />
                                        Back to Main Menu
                                    </Button>
                                    <Button
                                        onClick={handleSubmit}
                                        className="bg-blue-600 hover:bg-blue-700 flex items-center gap-2"
                                    >
                                        <CheckCircle className="w-4 h-4" />
                                        Submit Issue Report
                                    </Button>
                                </div>
                            </CardContent>
                        </Card>
                    </div>
                </main>
            </div>
        )
    }

    if (currentView === 'success' && submittedIssue) {
        return (
            <div className="min-h-screen bg-gray-50">
                {/* Header ?}
                <header className="bg-blue-600 text-white shadow-lg">
                    <div className="container mx-auto px-4 py-4">
                        <h1 className="text-2xl font-bold flex items-center gap-2">
                            🏛️ Municipal Services
                        </h1>
                    </div>
                </header>

                <main className="container mx-auto px-4 py-8">
                    <div className="max-w-4xl mx-auto">
                        <Card className="shadow-lg">
                            <CardHeader className="bg-green-600 text-white">
                                <CardTitle className="flex items-center gap-2">
                                    <CheckCircle className="w-6 h-6" />
                                    Issue Reported Successfully!
                                </CardTitle>
                            </CardHeader>
                            <CardContent className="p-6">
                                <Alert className="border-green-200 bg-green-50 mb-6">
                                    <CheckCircle className="h-4 w-4 text-green-600" />
                                    <AlertDescription className="text-green-800">
                                        <strong>Thank you for your report!</strong><br />
                                        Your issue has been successfully submitted to the municipal services department.<br />
                                        <strong>Reference ID: #{submittedIssue.id}</strong>
                                    </AlertDescription>
                                </Alert>

                                <div className="grid md:grid-cols-2 gap-6">
                                    <div>
                                        <h3 className="font-semibold text-lg mb-3">Issue Details:</h3>
                                        <div className="space-y-2">
                                            <p><strong>Location:</strong> {submittedIssue.location}</p>
                                            <p><strong>Category:</strong> {submittedIssue.category}</p>
                                            <p><strong>Date Reported:</strong> {submittedIssue.dateReported}</p>
                                            <p><strong>Status:</strong> <Badge className="bg-blue-100 text-blue-800">{submittedIssue.status}</Badge></p>
                                        </div>
                                    </div>

                                    <div>
                                        <h3 className="font-semibold text-lg mb-3">What happens next?</h3>
                                        <ol className="list-decimal list-inside space-y-1 text-gray-700">
                                            <li>Your report is being reviewed</li>
                                            <li>A municipal officer will be assigned</li>
                                            <li>You'll receive updates on progress</li>
                                            <li>The issue will be resolved</li>
                                        </ol>
                                    </div>
                                </div>

                                <div className="mt-6">
                                    <h3 className="font-semibold text-lg mb-3">Description:</h3>
                                    <div className="bg-gray-50 p-4 rounded-lg border">
                                        {submittedIssue.description}
                                    </div>
                                </div>

                                <div className="flex flex-col sm:flex-row gap-3 mt-8 justify-center">
                                    <Button
                                        onClick={resetForm}
                                        className="bg-blue-600 hover:bg-blue-700 flex items-center gap-2"
                                    >
                                        <AlertTriangle className="w-4 h-4" />
                                        Report Another Issue
                                    </Button>
                                    <Button
                                        variant="outline"
                                        onClick={() => setCurrentView('home')}
                                        className="flex items-center gap-2"
                                    >
                                        <Home className="w-4 h-4" />
                                        Back to Home
                                    </Button>
                                </div>
                            </CardContent>
                        </Card>
                    </div>
                </main>
            </div>
        )
    }

    return null
}
*/