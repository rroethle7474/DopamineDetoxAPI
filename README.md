# DopamineDetoxAPI

## Technology Stack

DopamineDetoxAPI is a RESTful API built with:
- **.NET 8.0** - The latest version of Microsoft's development platform
- **Entity Framework Core 8.0.7** - ORM for database operations
- **ASP.NET Core Identity** - For user authentication and authorization
- **SQL Server** - Database backend
- **AutoMapper** - For object-to-object mapping
- **Swagger/OpenAPI** - API documentation and testing

This API serves as the backend for the Dopamine Detox application, which helps users monitor and control their content consumption habits.

## Database Structure

The application uses Entity Framework Core with a Code-First approach. The main `ApplicationDbContext` extends `IdentityDbContext<ApplicationUser>` to incorporate ASP.NET Core Identity for user management.

Key database entities include:
- **Users** - Extended Identity users
- **Content Types** - Categories of content
- **Topics & SubTopics** - Content organization hierarchy
- **Channels** - Content sources
- **Search Results** - User content history
- **Notes** - User annotations
- **Quotes** - Inspirational content
- **Email Templates** - For system notifications

## Authentication

### JWT Authentication

The application uses JSON Web Tokens (JWT) for authentication:

1. **JWT Service**: The `JwtService` handles token generation and verification:
   - `GenerateJwtToken`: Creates a signed JWT with user claims
   - Uses symmetric key encryption with HMAC SHA256
   - Configurable token expiration (default set in configuration)

2. **JWT Configuration**: Set up in Program.cs with the following validation parameters:
   - Issuer validation
   - Audience validation
   - Lifetime validation
   - Signing key validation

3. **Token Flow**:
   - User logs in with credentials
   - Server validates credentials and generates JWT
   - Client stores token and includes it in Authorization header for subsequent requests
   - Server validates token on protected endpoints

### Google Authentication

The application supports Google OAuth 2.0 for social login:

1. **Configuration**: Google authentication is configured in Program.cs:
   ```csharp
   .AddGoogle(options =>
   {
       options.ClientId = configuration["Authentication:Google:ClientId"] ?? "";
       options.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? "";
   });
   ```

2. **Integration**: The `JwtService` includes a `VerifyGoogleToken` method to validate Google ID tokens.

3. **Login Flow**:
   - User initiates Google login from client application
   - Client obtains Google ID token and sends it to the API
   - `UserController.GoogleLogin` endpoint verifies the token
   - If valid, the system either creates a new user or retrieves existing user
   - API generates a JWT for the authenticated user

## Email Services

The application includes a comprehensive email system primarily using SendGrid for delivery:

### Notification Service

The `NotificationService` is the main component for sending emails using SendGrid:

```csharp
public class NotificationService : INotificationService
{
    private readonly NotificationSettings _settings;
    private readonly ILogger<NotificationService> _logger;
    private readonly SendGridClient _sendGridClient;
    private readonly IEmailTemplateService _emailTemplateService;

    public NotificationService(
        IOptions<NotificationSettings> settings,
        ILogger<NotificationService> logger,
        IEmailTemplateService emailTemplateService)
    {
        _settings = settings.Value;
        _logger = logger;
        _sendGridClient = new SendGridClient(_settings.SendGridApiKey);
        _emailTemplateService = emailTemplateService;
    }

    // Email sending methods...
}
```

Key features:
- Uses SendGrid's API for reliable email delivery
- Supports single and bulk email sending
- Includes detailed logging for email delivery status
- Integrates with the EmailTemplateService for building email content

### Email Template Service

`EmailTemplateService` works with the Notification Service to build templated emails:

1. **Template Storage**: Email templates are stored in the database as `EmailTemplateEntity` records.

2. **Template Types**:
   - Weekly MVP Report
   - Reset Password
   - Header/Footer components
   - Search Result templates
   - Note templates

3. **Dynamic Content**:
   - Templates support placeholders for dynamic content
   - The service replaces placeholders with actual values
   - HTML templates support complex formatting

4. **Key Methods**:
   - `BuildUserMVPWeeklyEmail`: Creates a weekly report email with search results and notes
   - `BuildResetPasswordEmail`: Creates password reset emails with secure tokens

5. **Integration**:
   The Notification and Template services work together:
   ```csharp
   // Example workflow:
   var (emailBody, subject) = await _emailTemplateService.BuildResetPasswordEmail(user, encodedToken, baseUrl);
   await SendEmailAsync(user.Email, subject, emailBody);
   ```

### SendGrid Configuration

You'll need to set up a SendGrid account and configure it in your application:

1. **Create a SendGrid Account**: Sign up at [SendGrid](https://sendgrid.com/) and create an API key with appropriate permissions.

2. **Configure Settings**: Add SendGrid configuration to your `appsettings.json`:

```json
{
  // Other settings...
  
  "NotificationSettings": {
    "SendGridApiKey": "Your_SendGrid_API_Key",
    "SendGridFromEmail": "noreply@yourdomain.com",
    "SendGridFromName": "Your Application Name"
  }
}
```

### Legacy Email Service

> Note: While the codebase contains an SMTP-based `EmailService`, the application primarily uses the `NotificationService` with SendGrid for email delivery.

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server instance
- SMTP server for email functionality

### Configuration
The application requires the following configuration in appsettings.json:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your_SQL_Connection_String"
  },
  "Jwt": {
    "Key": "Your_Secret_Key_Min_16_Characters",
    "Issuer": "Your_Issuer",
    "Audience": "Your_Audience",
    "ExpireDays": "30"
  },
  "Authentication": {
    "Google": {
      "ClientId": "Your_Google_Client_Id",
      "ClientSecret": "Your_Google_Client_Secret"
    }
  },
  "AppSettings": {
    "Domain": "smtp.example.com",
    "Port": 587,
    "Username": "Your_SMTP_Username",
    "Password": "Your_SMTP_Password",
    "EnableSsl": true,
    "FromEmail": "noreply@example.com",
    "OPENAI_API_KEY": "Your_OpenAI_API_Key",
    "OpenAIImageTimeoutSeconds": 120
  },
  "NotificationSettings": {
    "SendGridApiKey": "Your_SendGrid_API_Key",
    "SendGridFromEmail": "noreply@yourdomain.com",
    "SendGridFromName": "Your Application Name"
  }
}
```

### Running the Application
1. Clone the repository
2. Configure NuGet package sources (see [NuGet Configuration](#nuget-configuration))
3. Set up the database connection in appsettings.json
4. Run database migrations: `dotnet ef database update`
5. Run the application: `dotnet run`
6. Access the Swagger UI at: https://localhost:5001/swagger

## NuGet Configuration

This project depends on custom NuGet packages hosted in an Azure DevOps Artifacts feed:

- **DopamineDetox.Domain** (Version 0.0.2) - Domain models and shared data structures (https://github.com/rroethle7474/DopamineDetox.Domain)
- **DopamineDetox.ServiceAgent** (Version 0.0.2) - Service agent components (https://github.com/rroethle7474/DopamineDetox.ServiceAgent)

### Setting Up the NuGet Feed

To restore these packages, you need to configure your NuGet client to use the Azure DevOps feed:

1. Ensure the project includes the `nuget.config` file with the required source: (replace value with where the local nuget packages are if setting up another nuget source feed)

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <clear />
        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
        <add key="Azure DevOps Packages" value="" />
    </packageSources>
</configuration>
```

2. **Authentication**: You may need to authenticate with Azure DevOps to access the feed:
   - In Visual Studio: Add the feed URL in Tools → NuGet Package Manager → Package Sources
   - Using .NET CLI: Run `dotnet nuget add source https://pkgs.dev.azure.com/rroethle/NugetPackages/_packaging/domain-models-services/nuget/v3/index.json --name "Azure DevOps Packages" --username [your-username] --password [your-personal-access-token]`
   - Or authenticate with an Azure DevOps Personal Access Token (PAT) that has package read permissions

3. **Visual Studio Authentication**: If using Visual Studio, it will prompt for Azure DevOps credentials when restoring packages.

### Package Details

These custom packages provide core functionality for the application:

- **DopamineDetox.Domain**: Contains shared models, DTOs, and domain entities used across the solution.
- **DopamineDetox.ServiceAgent**: Contains client libraries and service agents for external integrations.

If you encounter package restore issues, ensure you have proper access to the Azure DevOps organization and feed.