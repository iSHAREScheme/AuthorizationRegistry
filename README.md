# iSHARE Authorization Registry

**iSHARE** is a collaborative effort to improve conditions for data-sharing for organisations involved in the logistics sector. The functional scope of the iSHARE Scheme focuses on topics of identification, authentication and authorization.

## iSHARE Authorization Registry

The **Authorization Registry**: 

* Manages records of Delegation and Authorization of Entitled Party (role) and/or Service Consumer (role);
* Checks on the basis of the registered permission(s) whether a Human Service Consumer (role) or Machine Service Consumer (role) is authorized to take delivery of the requested service, and;
* Confirms the established powers towards the Service Provider (role). 

Within the iSHARE Scheme, the term Authorization Registry always refers to an external Authorization Registry (not part of the Service Provider (role) or Entitled Party (role)). 

The **Authorization Registry** is a role for which iSHARE Certification (iSHARE) is REQUIRED.

## Installation process for API

### 1. Install .NET Core 2.1

1. Click  [here](https://www.microsoft.com/net/download/dotnet-core/2.1) and download .NET Core 2.1.0 Runtime. (or SDK 2.1.300 for development)
2. Install the Runtime  


### 2. Clone or download the Authorization Registry repository:

#### Clone (If you have Git application installed on the local machine)
1. Navigate to the desired location on the disk where you want to clone the repository  
2. Run the following command `git clone https://github.com/iSHAREScheme/AuthorizationRegistry.git` (or clone the repository from the UI if possible)


#### Download (No Git application is installed on the local machine)
1. Go to the [repository](https://github.com/iSHAREScheme/AuthorizationRegistry)  
2. Click 'Clone or Download' > 'Download ZIP'  
3. Extract the zip file content  


### 3. Setup the development environment
1. On the developer machine set the environment variable 'ENVIRONMENT' with the value 'Development'
2. Go to the local Authorization Registry repository  
3. Navigate to NLIP.iShare.AuthorizationRegistry.Api  
4. In this folder create a new file named 'appsettings.Development.json'  
5. Look for a file named 'appsettings.Development.json.template' (this file contains the project configuration structure). Open it and copy it's content  
6. Paste the content in the 'appsettings.Development.json' file  
7. Complete all empty fields with the necessary information and save the changes  


### 4. Setup certificates
The certificates used in development should be installed for the current user on the 'Personal' profile. These certificates are required in order for the application to work properly. Own certificates can be used for local development but for the integration with the iSHARE Scheme, the certificates must be issued by the iSHARE Scheme.


## Build process

1. Open Command Prompt (or other command line interpreter)
2. Navigate to the local Authorization Registry repository
3. Run the following command: `dotnet build`
4. If the project is successfully built then you can go to the run process, otherwise redo the previous steps

## Run process
1. Open Command Prompt (or other command line interpreter)
2. Navigate to the local Authorization Registry repository
3. From this place navigate to NLIP.iShare.AuthorizationRegistry.Api
1. Run the following command: `dotnet run`
2. If the setup was successfull then the application is running
3. Open a browser tab and navigate to `localhost:61433/swagger`
4. If the API endpoints are displayed it means that the API is up and running properly
5. If you receive another error try to redo the installation process from step 3 to the end

## Instalation for SPA 
In order to install the SPA that manages the API you need to follow [this](https://github.com/iSHAREScheme/AuthorizationRegistry/tree/master/NLIP.iShare.AuthorizationRegistry.SPA/readme.md) steps.

## API References
1. https://ishareworks.atlassian.net/wiki/spaces/IS/pages/70222191/iSHARE+Scheme
2. https://app.swaggerhub.com/apis/iSHARE/iSHARE_Scheme_Specification/1.7