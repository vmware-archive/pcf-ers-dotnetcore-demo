# PCF Application Service (PAS) Base Demo for .NET CORE
Base application to demonstrate PCF PAS on .NET Core

## Credits and contributions
This is a .NET CORE port of the original ERS demo app for Java Spring https://github.com/Pivotal-Field-Engineering/pcf-ers-demo

## Introduction
This base application is intended to demonstrate some of the basic functionality of PCF PAS:

* PCF api, target, login, and push
* PCF environment variables
* Scaling, self-healing, router and load balancing
* RDBMS service and application auto-configuration
* Blue green deployments

## Getting Started

**Prerequisites**
- [Cloud Foundry CLI](http://info.pivotal.io/p0R00I0eYJ011dAUCN06lR2)
- [Git Client](http://info.pivotal.io/i1RI0AUe6gN00C010l12J0R)
- [.NET Core SDK](https://www.microsoft.com/net/download)

**Building**
```
$ git clone [REPO]
$ cd [REPO]
$ dotnet publish -o ../publish src/pcf-ers-dotnetcore-demo.csproj
``` 

### To run the application locally
The application is set to use an embedded SQLite database in non-PaaS environments, and to take advantage of Pivotal CF's auto-configuration for services. To use a MySQL Dev service in PCF, simply create and bind a service to the app and restart the app. No additional configuration is necessary when running locally or in Pivotal CF.

In Pivotal CF, it is assumed that a Pivotal MySQL service will be used.

```
$ dotnet run --project src/pcf-ers-dotnetcore-demo.csproj
```

Then go to the http://localhost:5000 in your browser

### Running on Cloud Foundry
Take a look at the manifest file for the recommended setting. Adjust them as per your environment.

## Labs/Demo Scripts summary
We have a [Labs](https://github.com/Pivotal-Field-Engineering/pcf-ers-dotnetcore-demo/tree/master/labs) folder to help you learn PCF. These labs can be used for workshops or self-training.    


