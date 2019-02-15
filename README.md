# iSHARE Authorization Registry

**iSHARE** is a collaborative effort to improve conditions for data-sharing for organisations involved in the logistics sector. The functional scope of the iSHARE Scheme focuses on topics of identification, authentication and authorization.

## iSHARE Authorization Registry

The **Authorization Registry**:

- Manages records of Delegation and Authorization of Entitled Party (role) and/or Service Consumer (role);
- Checks on the basis of the registered permission(s) whether a Human Service Consumer (role) or Machine Service Consumer (role) is authorized to take delivery of the requested service, and;
- Confirms the established powers towards the Service Provider (role).

Within the iSHARE Scheme, the term Authorization Registry always refers to an external Authorization Registry (not part of the Service Provider (role) or Entitled Party (role)).

The **Authorization Registry** is a role for which iSHARE Certification (iSHARE) is REQUIRED.

## Installation process for API

### Prerequisites

- Install [.NET Core 2.1.4 Runtime](https://www.microsoft.com/net/download/dotnet-core/2.1) (or SDK 2.1.402 for development).

### Clone or download the Authorization Registry repository:

- `git clone https://github.com/iSHAREScheme/AuthorizationRegistry.git` (or download zip)

### Setup the development environment

1. Create environment variable 'ENVIRONMENT' with the value 'Development'
2. Navigate to iSHARE.AuthorizationRegistry.Api and create a new file named 'appsettings.Development.json'
3. Copy the content of 'appsettings.Development.json.template' into 'appsettings.Development.json' and complete all fields with the necessary information and save the changes
4. Into appsettings.json file, change PrivateKey and PublicKeys fields from PartyDetails with valid keys and save changes

## Build API

Navigate to the local Authorization Registry repository and run `dotnet build`

## Setup the database

Authorization Registry is using a SQL database that is created at runtime.
Various test records are inserted from JSON files present here

- `iSHARE.AuthorizationRegistry.Api\Seed\Identity\Development`
- `iSHARE.AuthorizationRegistry.Api\Seed\IdentityServer\Development`
- `iSHARE.AuthorizationRegistry.Data\Migrations\Seed\Development`

## Run process

1. Navigate to the local Authorization Registry repository, into iSHARE.AuthorizationRegistry.Api folder and run `dotnet run`
2. Open a browser tab and navigate to `localhost:61433/swagger`

## Installation for SPA

### Prerequisites

The software you'll need before installing Authorization Registry SPA:

- [Node.js](https://nodejs.org/en/)
- Angular CLI - `npm install -g @angular/cli`

### Install dependencies

Run `npm install` inside iSHARE.AuthorizationRegistry.SPA folder to get all dependencies downloaded locally.

### Run process

Run `ng serve -o`.

- NOTE: The Authorization Registry API must be running in order for the application to work correctly
- NOTE: The SPA will be available by default at http://localhost:4201/admin

### [Differences between the implementation and the official documentation](Differences.md)

## API References

1. https://ishareworks.atlassian.net/wiki/spaces/IS/pages/70222191/iSHARE+Scheme
2. https://dev.ishareworks.org/
