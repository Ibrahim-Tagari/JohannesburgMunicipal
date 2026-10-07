ReadMe:
======
PROG7312-Programming3B-POE (ST10062630)- Ibrahim Tagari:
========================================================

This document provides comprehensive instructions for compiling, running, and using this Municipal Services Application (Part 3 – Final POE) (Ibrahim Tagari [ST10062630]).
The application represents the culmination of all project phases and integrates reporting issues, local events and announcements, and service request status tracking into a cohesive municipal management system.

The system is implemented in C# .NET 9 (ASP.NET Core MVC) using Visual Studio 2022, and leverages a modular, service-oriented architecture designed for scalability and maintainability.

File Structure:
---------------

prog7312-part3-ST10062630/
│
├── ReadMe.txt
│
├── Municipal_App/
│   ├── Program.cs
│   ├── Startup.cs
│   │
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   │
│   ├── Controllers/
│   │   ├── AccountController.cs
│   │   ├── DatabaseController.cs
│   │   ├── EventsController.cs
│   │   ├── HomeController.cs
│   │   ├── IssuesController.cs
│   │   ├── NotificationsController.cs
│   │   ├── ServiceRequestStatusController.cs
│   │
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Migrations/
│   │   │   ├── 00000000000000_CreateIdentitySchema.cs
│   │   │   ├── 00000000000000_CreateIdentitySchema.Designer.cs
│   │   │   ├── ApplicationDbContextModelSnapshot.cs
│   │
│   ├── Models/
│   │   ├── ContactFormModel.cs
│   │   ├── DatabaseInfo.cs
│   │   ├── ErrorViewModel.cs
│   │   ├── EventRecommendation.cs
│   │   ├── Issue.cs
│   │   ├── ServiceRequest.cs
│   │   ├── ServiceRequestGraph.cs
│   │
│   ├── Services/
│   │   ├── DatabaseManagementService.cs
│   │   ├── EventDataStructureService.cs
│   │   ├── EventRecommendationService.cs
│   │   ├── IssueService.cs
│   │   ├── LocalEventService.cs
│   │   ├── NotificationService.cs
│   │   ├── PushSubscriptionService.cs
│   │   ├── ServiceRequestStatusService.cs
│   │   ├── UserService.cs
│   │   │
│   │   ├── Interfaces/
│   │   │   ├── IDatabaseManagementService.cs
│   │   │   ├── IEventDataStructureService.cs
│   │   │   ├── IEventRecommendationService.cs
│   │   │   ├── IIssueService.cs
│   │   │   ├── ILocalEventService.cs
│   │   │   ├── INotificationService.cs
│   │   │   ├── IPushSubscriptionService.cs
│   │   │   ├── IServiceRequestStatusService.cs
│   │   │   ├── IUserService.cs
│   │   │
│   │   ├── DataStructures/
│   │       ├── BinarySearchTree.cs
│   │       ├── AVLTree.cs
│   │       ├── RedBlackTree.cs
│   │       ├── MinHeap.cs
│   │       ├── GraphNode.cs
│   │       ├── ServiceRequestGraph.cs
│   │
│   ├── Views/
│   │   ├── Home/
│   │   │   ├── Index.cshtml
│   │   │   ├── About.cshtml
│   │   ├── Issues/
│   │   │   ├── ReportIssue.cshtml
│   │   │   ├── IssueList.cshtml
│   │   ├── Events/
│   │   │   ├── EventList.cshtml
│   │   │   ├── EventDetails.cshtml
│   │   ├── ServiceRequestStatus/
│   │       ├── Index.cshtml
│   │       ├── Details.cshtml
│   │
│   ├── wwwroot/
│   │   ├── css/
│   │   ├── js/
│   │   ├── images/
│   │
│   ├── Properties/
│   │   ├── launchSettings.json
│   │
│   ├── bin/
│   └── obj/
│
└── prog7312-part3-ST10062630.sln


System Requirements:
--------------------

To compile and run the application, ensure the following prerequisites are installed:

Requirement:	  | Version / Details:
---------------------------------------
Microsoft Windows | Windows 10 or later
Visual Studio	  | 2022 (Community/Professional)
.NET SDK	  | .NET 8.0 or later
Database	  | SQL Server Express or LocalDB
Browser	          | Any (Edge, Chrome, ect)

Installation Instructions:
--------------------------

* Download or Clone the Repository

* Extract the project folder prog7312-part3-ST10062630 into a known directory on your system.

* Ensure the relative folder structure is preserved.

* Open the Project

* Launch Visual Studio 2022.

* Select Open a project or solution → navigate to Municipal_App.sln inside the extracted folder.

* Restore Dependencies

* In Visual Studio, open the Package Manager Console and run: dotnet restore-
This retrieves all NuGet dependencies (e.g., Entity Framework Core, Microsoft Extensions, Logging).

* Configure the Database - 
Update the connection string in appsettings.json to match your SQL Server or LocalDB configuration.

* Apply initial migrations using: Update-Database

* Build and Run

* Press F5 or select Run → Start Debugging.

* The application will compile and automatically open in your default web browser at:

https://localhost:xxxx

Application Overview:
--------------------

Upon launch, users are presented with the Main Menu, offering three core options:

* Report Issues – allows citizens to submit issue reports including location, category, description, and attachments.

* Local Events & Announcements – displays community updates, with search and smart recommendation features based on previous queries.

* Service Request Status – enables users to view, track, and prioritise their submitted service requests using advanced data structures.

Using the “Service Request Status” Module:
------------------------------------------

Accessing the Module -

* From the main menu, click Service Request Status.

The page lists all submitted requests retrieved asynchronously from the database.

Functional Overview -

* Requests are internally organised using advanced data structures:

 - Binary Search Tree (BST): structured sorting and retrieval.

 - AVL Tree: balanced insertion/deletion ensuring logarithmic time complexity.

 - Red–Black Tree: dynamic self-balancing under continuous updates.

 - Min-Heap: prioritises high-urgency service requests.

 - Graph/MST: models dependencies between related requests and determines optimal resolution paths.

User Interaction - 

* Filter requests by status or keyword search.

* View dependency graphs (when applicable).

* Observe live status updates and priority orderings in the table view.

Application Architecture:
-------------------------

Layer	    | Description
---------------------------------------------------------------------------------------------------------
Controllers | Manage user input, navigation, and API calls. Example: ServiceRequestStatusController.cs
Services    | Contain core logic and data-structure implementations, e.g., ServiceRequestStatusService.cs.
Models	    | Define entities such as Issue, ServiceRequest, and ServiceRequestGraph.
Data	    | Handles persistence through ApplicationDbContext using Entity Framework Core.
Views	    | Razor Views providing the user interface for each module.

* This separation of concerns ensures scalability, modularity, and clean maintainability of the codebase.

Technical Highlights:
---------------------

Asynchronous Initialization-
* Uses InitializeDataStructuresAsync() to prevent UI blocking.

Logging -
* Integrated via dependency-injected ILogger for diagnostics and performance tracing.

Responsive UI Design -
* Consistent layout and colour scheme throughout the application.

Exception Handling -
* Graceful user feedback with descriptive error messages via try/catch blocks.

Troubleshooting:
----------------

Issue	                  | Resolution
---------------------------------------------------------------------------------------------------------------
Database connection error | Verify connection string and ensure SQL Server service is running.
Dependencies missing	  | Re-run dotnet restore in the Package Manager Console.
Browser shows 404 error	  | Ensure the startup project is set to Municipal_App.
Migration conflicts	  | Delete migration files and regenerate with Add-Migration Initial → Update-Database.


This ReadMe document provides the essential setup and operational guidance for the Municipal Services Application. Following the steps outlined ensures successful compilation and execution of the program.




