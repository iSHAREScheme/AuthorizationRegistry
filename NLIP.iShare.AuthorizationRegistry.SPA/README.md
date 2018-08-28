# iSHARE Authorization Registry SPA

The Authorization Registry SPA is a web application provided by iSHARE Organization for managing the Authorization Registry API.
It allows human users to do management of policies and users.

## Installation process

### 1. Install Node.js

1. Click [here](https://nodejs.org/en/) and download Node.js
2. Install Node.js
3. Open a command line tool (like Command Prompt) and run `node -v` to confirm that you have installed Node.js properly
4. Run `npm -v` to confirm that you have installed npm

### 2. Install Angular CLI
1. Open a comand line tool and run `npm install -g @angular/cli`
2. Run `ng -v` to confirm that you have installed Angular CLI properly

### 3. Install dependencies

1. Open Command Prompt (or other command line tool)
2. Navigate to the local Authorization Registry repository
3. Navigate to NLIP.iShare.AuthorizationRegistry.SPA folder
4. Run `npm install` to get all dependencies downloaded locally

## Build process for SPA
1. Open Command Prompt (or other command line tool)
2. Navigate to the local Authorization Registry repository
3. Navigate to NLIP.iShare.Spas folder
4. Run `ng build` to start the build process
5. If the build is successfull then the everything is installed properly
6. If you receive build errors try to redo the install dependencies step and the build process

## Run process for SPA
1. Open Command Prompt (or other command line tool)
2. Navigate to the local Authorization Registry repository
3. Navigate to NLIP.iShare.Spas folder
4. Run `ng serve` to run the application
5. Open a browser tab and navigate to `localhost:4200`
6. If the login page is displayed it means that the SPA is up and running properly
7. If you receive an error (like a blank page) try to redo the run process  
* NOTE: The Authorization Registry API must be running in order for the application to work correctly

## API References
1. https://ishareworks.atlassian.net/wiki/spaces/IS/pages/70222191/iSHARE+Scheme
2. https://app.swaggerhub.com/apis/iSHARE/iSHARE_Scheme_Specification/1.7