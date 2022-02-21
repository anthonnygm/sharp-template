# Sharp Template

C# .NET Core Template repository developed by using technologies, such as:

- ASP.NET Core
- Razor Pages
- EF Core 5
- Swagger
- JWT Authentication
- MailKit
- FluentValidation
- Argon2 Hash
- Xunit

## Deploy Test

- Run file `test-sharp-template.bat`, it will compile the application for release with all necessary files and run at http://localhost:5000. (using the settings contained in the appsettings.json file)

## Environment settings

- To run the project on your machine, using Visual Studio, it is necessary to configure the appsettings.json file, adjusting the connection string with the data of your SQL Server installation.

Sample file:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\sqlexpress;Database=sharp-template-db;User Id=sa;Password=!@#123qwe;"
  },
  "AppSettings": {
    "SmtpHost": "smtp.office365.com",
    "SmtpPort": "587",
    "EmailUser": "no-reply@sharptemplate.com",
    "EmailPassword": "!@#123qwe",
    "AuthTokenSecretKey": "489cd5dbc708c7e541de4d7cd91ce6d0f1613573b7fc5b40d3942ccb9555cf35",
    "EmailTokenSecretKey": "962012d09b8170d912f0669f6d7d9d07",
    "Argon2PasswordKey": "a946ce40cf936def"
  }
}
```
