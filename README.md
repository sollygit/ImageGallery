# Image Gallery
Image Gallery Demo with .Net Core 3 and Azure Functions

Ensure to start Azure Storage Emulator for images to load.
You can also use Azure Stotage Explorer to search for the uploaded images.

Uncomment this line in Startup.cs to switch to FileStorageService: 
services.AddScoped<IStorageService, FileStorageService>();

Azure Services
- App Service - Web App
- Functions
- Azure Monitoring - Application Insights
- SQLDb
- Storage

Technologies
- ASP.NET Core 3.0
- Visual Studio 2017 or later (Visual Studio 2019 16.3 recommended)
- Entity Framework
- Azure
