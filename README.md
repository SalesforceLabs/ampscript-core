# Sage
Sage is a set of tools for Salesforce Marketing Cloud content development, analytics and security.

The goal of this project is to make writing, debugging and analyzing content easier.

## Debugging AMPscript using Sage
In Visual Studio, modify the [launchsettings.json](src/Sage.Engine/Properties/launchSettings.json) file to point to your ampscript file.

In VSCode, create a `launch.json` file which launches the sage engine. Example:
```json
    "configurations": [
        {
            "name": "Debug AMPscript",
            "type": "coreclr",
            "request": "launch",
            "program": "Sage.Engine.exe",
            "args": ["${file}", "-d"],
            "cwd": "${workspaceFolder}",
            "console": "internalConsole",
            "stopAtEntry": false
        }
    ]
```

## Marketing Cloud Supported Languages

| Language | Summary |
|----------|---------|
| AMPScript | ⚠️ Partial support |
| SSJS | ⛔ Not supported |
| GTL | ⛔ Not Supported |

### ⚠️ AMPScript Support
The language has full grammar support, but very little runtime support at the moment.

| Feature | Summary |
|----------|---------|
| Ampscript Blocks `%%[]%%` | ✔️ |
| Variables | ✔️ |
| For `for`| ✔️ |
| If `if`| ✔️ |
| Inline Ampscript `%%= =%%`| ⚠️ Grammar support only |
| Logic Operators `==`, `!=`, `&&`, etc | ⚠️ Grammar support only |
| Tag Syntax `<script language="ampscript">` | ⛔ Not Supported |
| System Strings `xtmonth, jobid, subscriberkey, etc` | ⛔ Not Supported |
| Personalization Strings `[Foo]` or `%%Foo%%` | ⛔ Not Supported |


## Supported Functions

If the function isn't listed here, then it's not supported.
| Function Area | Supported / Notes |
|----------|---------|
|[Marketing Cloud API](https://ampscript.guide/marketing-cloud-api-functions/)| ⛔ Not Supported |
|[Contact Model](https://ampscript.guide/content-model-functions/)| ⛔ Not Supported |
|[Content](https://ampscript.guide/content-functions/)| ⛔ Not Supported |
|[Data Extensions](https://ampscript.guide/data-extension-functions/)| ⛔ Not Supported |
|[Date and Time](https://ampscript.guide/date-and-time-functions/)| ⛔ Not Supported |
|[Einstein](https://ampscript.guide/einstein-email-recommendation-functions/)| ⛔ Not Supported |
|[Encryption](https://ampscript.guide/encryption-and-encoding-functions/)| ⛔ Not Supported |
|[HTTP](https://ampscript.guide/http-functions/)| ⛔ Not Supported |
|[Math](https://ampscript.guide/math-functions/)| ⛔ Not Supported |
|[Microsoft Dynamics](https://ampscript.guide/microsoft-dynamics-crm-functions/)| ⛔ Not Supported |
|[Sales and Service Cloud](https://ampscript.guide/sales-and-service-cloud-functions/)| ⛔ Not Supported |
|[Site](https://ampscript.guide/site-based-functions/)| ⛔ Not Supported |
|[Social](https://ampscript.guide/social-functions/)| ⛔ Not Supported |
|[String](https://ampscript.guide/string-functions/)| ⚠️ Partial Support |
|[Utility](https://ampscript.guide/utility-functions/)| ⚠️ Partial Support |
|[Content Syndication](https://ampscript.guide/content-syndication/)| ⛔ Not Supported |


## Roadmap
* Increase supported feature set for both static analysis and runtime execution
* A 95% compatibility target for supporting ampscript functions, with mockability for local execution.
* SSJS & GTL
* Static code analysis tools to identify and remediate performance or security issues

## Docs
-   [Code of Conduct](./CODE_OF_CONDUCT.md)
-   [LICENSE](./LICENSE)
