# Banking Service
## Installation Instructions
###### WCF Service Application
In order for the service application to work, you need to follow the steps below:
> 1. Open terminal Developer Command Prompt for VS 2019
> 2. After that, run below command 
```
makecert.exe -sr CurrentUser -ss My -a sha1 -n CN=BankingServiceASM -sky exchange -pe
```
> 3. Change connection string in web.config file to fit into your local machine
###### Client Application - ASP.NET MVC