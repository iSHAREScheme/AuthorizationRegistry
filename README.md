# iSHARE Authorization Registry

**iSHARE** is a collaborative effort to improve conditions for data-sharing for organisations involved in the logistics sector. The functional scope of the iSHARE Scheme focuses on topics of identification, authentication and authorization.

## iSHARE Authorization Registry

The **Authorization Registry**:

- Manages records of Delegation and Authorization of Entitled Party (role) and/or Service Consumer (role);
- Checks on the basis of the registered permission(s) whether a Machine Service Consumer (role) is authorized to take delivery of the requested service, and;
- Confirms the established powers towards the Service Provider (role).

Within the iSHARE Scheme, the term Authorization Registry always refers to an external Authorization Registry (not part of the Service Provider (role) or Entitled Party (role)).

The **Authorization Registry** is a role for which iSHARE Certification (iSHARE) is REQUIRED.

The **Authorization Registry**-code that is in this repository is not a 'production-ready' Authorization Registry, meaning it has a limited set of functionalities. It can be used in proof of concepts or pilots to showcase the iSHARE Authorization protocol, however many functionalities can be improved. Furthermore, it should be noted that only the request and return made to the /delegation endpoint (as described on our [Developer Portal](https://dev.ishareworks.org)) is specified within the iSHARE standards. How  an authorization registry registers policies and translates these into delegation evidence is up to the authorization registry. This code only provides one of the options to do so.

## Installation process for API

### Prerequisites

- Install [.NET Core 3.1.106 Runtime](https://dotnet.microsoft.com/download/dotnet-core/3.1) (or SDK 3.1.106 for development).

### Clone or download the Authorization Registry repository:

- `git clone https://github.com/iSHAREScheme/AuthorizationRegistry.git` (or download zip)

### Setup the development environment

1. Create environment variable 'ENVIRONMENT' with the value 'Development'
2. Navigate to iSHARE.AuthorizationRegistry.Api and create a new file named 'appsettings.Development.json'
3. Copy the content of 'appsettings.Development.json.template' into 'appsettings.Development.json' and complete all fields with the necessary information and save the changes
4. Into appsettings.Development.json file: 
    1. Change DigitalSigner -> PrivateKey value to the valid RSA private key value with the following format: "-----BEGIN RSA PRIVATE KEY-----\n...\n-----END RSA PRIVATE KEY-----". For this, you can use OpenSSL:
        1. Extract the private key from the certificate: `openssl pkcs12 -in "certificate.p12" -out "certificate.key.pem" -nodes -nocerts -password pass:your_password_here`
        2. Decrypt private key: `openssl rsa -in certificate.key.pem -out certificate.key.decr.pem`
        3. Extract the content from `certificate.key.decr.pem` and replace the endline characters with "\n"
    2. Change DigitalSigner -> RawPublicKey value to the valid certificate value with the following format: "-----BEGIN CERTIFICATE-----...-----END CERTIFICATE-----". For this you can use OpenSSL:
        1. Extract .pem: `openssl pkcs12 -in certificate.p12 -clcerts -nokeys -out certificate.pem -password pass:your_password_here`
        2. Extract the content from `certificate.pem` and remove the endline characters
    3. Save changes
5. Go to Resources\Development
    1. Open certificate_authorities.json
    2. Add the necessary certificate authorities in the following format: "-----BEGIN CERTIFICATE-----...-----END CERTIFICATE-----" (this value can be obtained from a .pem certificate by extracting the content and removing the line separators/endlines)
    3. Save

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
