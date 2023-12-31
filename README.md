# CloudSalesApi

This is a REST Web API (.NET 6) project implemented as a cloud sales solution.

## Implementation

For faster development, and to avoid editing the database manually I chose the code-first approach.
I have implemented authentication using API key authentication assuming it is a system to system integration.
I used MS SQL Server for persistence.

The following endpoints were created by controllers:

##### Customers
- GET /api/customers/{customerNumber} 
- GET /api/customers/{customerNumber}/accounts

##### Accounts
- POST /api/accounts/{accountId}/order
- GET /api/accounts/{accountId}/purchased-software
- PUT /api/accounts/{accountId}/subscriptions/{serviceId}
- DELETE /api/accounts/{accountId}/subscriptions/{serviceId}
- PUT /api/accounts/{accountId}/subscriptions/{serviceId}/extend

##### SoftwareServices
- GET /api/software-services

Although not stated, I tried to stick to some conventions and practices, so I also implemented the following:
1. Unit tests for order service cases, software-services controller, and customer controller actions.
2. Integrated Swagger UI - the page is visible on app launch.
3. Configured Serilog for diagnostic logging.

## Project Setup

Clone the repository from the Github:
```sh
git clone https://github.com/m-kovacina/CloudSalesApi.git
```

Set up database.

Adjust connection string within appsettings.json file, for example:
```sh
"CloudSalesConnection": "Server=.\\SQLEXPRESS;Database=CloudSalesDB;TrustServerCertificate=True;Trusted_Connection=True;"
```

If we want to start from a clean database, it is necessary to run migrations using the following command.
```sh
dotnet ef database update
```

Insert some dummy data for testing
```sh
USE [CloudSalesDB]
GO

INSERT INTO [dbo].[Customers]
           ([CustomerIdentifier]
           ,[Name]
           ,[Email]
           ,[Type])
     VALUES
           ('CN000000001214' ,'Acme Corporation', 'office@acmecorp.com', 1),
		   ('CN000000001228' ,'Umbrella Corporation', 'office@umbrella.net', 1)
GO

INSERT INTO [dbo].[Accounts]
           ([CustomerId]
           ,[Name])
     VALUES
           (1,'Business Account'),
		   (1,'Business Plus Account'),
		   (2,'Business Account')
GO
```

Since the API key needs to be protected, within your project directory, use the following command to enable secret storage:
```sh
dotnet user-secrets init
```

Next, add the API key to secret storage using the following command, and use it later within the X-API-Key header when calling any endpoint:
```sh
dotnet user-secrets set "ApiKey" "{generate_api_key}"
```

Start the project from the command line or directly from the IDE:
```sh
dotnet run
```




