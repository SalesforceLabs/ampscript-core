# Sage
Sage is a set of tools for Salesforce Marketing Cloud content development, analytics and security.

The goal of this project is to make writing, debugging and analyzing content easier.

Questions? Follow up on slack [#sage-discussion](https://salesforce-internal.slack.com/archives/C04H9CDV5RD)

## Subscriber Attributes
Subscriber attributes `%%Foo%%`/`[Foo]` are supported through a JSON file named `subscriber.json` in the working directory.

Supported within the JSON file is a key/value pair of the attribute name and value.

Example:
```json
{
    "FirstName": "Logan"
}
```

The attribute values can be referenced via:
```ampscript
%%[
    SET @FIRSTNAME = [FirstName]
]%%

Hello %%=V(@FIRSTNAME)=%%
or
Hello %%FirstName%%
```

## Referencing Content
Content functions can reference content local on disk.  The files must exist in a `Content` subdirectory of the working directory.

Example, if you have a file at `Content\EmbeddedContent.ampscript`, then you can reference it via:
```ampscript
%%=CONTENTBLOCKBYNAME("EmbeddedContent")=%%
```

## Data Extensions
Data Extension support is provided through CSV files on disk.  The files must exist in a `DataExtensions` subdirectory of the working directory. The CSV file must contain headers which represent the columns.

Example, if you have a file at `DataExtensions\Loyalty.csv`, then it will use this CSV file as the data extension.

`DataExtensions\Loyalty.csv`
```csv
EmailAddress,SubscriberKey,FirstName,LastName,LoyaltyLevel
donnie@northerntrailoutfitters.com,1,Donnie,Stanton,Silver
```

`Index.ampscript`
```ampscript
Hello %%=LOOKUP("Loyalty", "FirstName", "emailAddress", "donnie@northerntrailoutfitters.com")=%%
```

Outputs:
```ampscript
Hello Donnie
```

## Debugging AMPscript using Sage
There is an example at [Index.ampscript](src/Sage.Webhost/Index.ampscript) that you may modify and test.

In VSCode, create a `launch.json` file which launches the sage engine. Example:
```json
    "configurations": [
        {
            "name": "Launch (web)",
            "type": "coreclr",
            "request": "launch",
            "program": "<path to release>\\Sage.Webhost.dll",
            "cwd": "${workspaceFolder}",
            "stopAtEntry": false,
            "serverReadyAction": {
                "action": "openExternally",
                "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
            },
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            },
            "sourceFileMap": {
                "/Views": "${workspaceFolder}"
            }
        }
    ]
```

Future editor plugins (VSCode) are on the roadmap

## Marketing Cloud Supported Languages

| Language | Summary |
|----------|---------|
| AMPScript | ⚠️ Partial support |
| SSJS | ⛔ Not supported |
| GTL | ⛔ Not Supported |

### ⚠️ AMPScript Support
| Feature | Summary |
|----------|---------|
| Ampscript Blocks `%%[]%%` | ✔️ |
| Variables | ✔️ |
| For `for`| ✔️ |
| If `if`| ✔️ |
| Logic Operators `==`, `!=`, `&&`, etc | ✔️ |
| Inline Ampscript `%%= =%%`| ✔️ |
| Personalization Strings `[Foo]` or `%%Foo%%` | ✔️ |
| Tag Syntax `<script language="ampscript">` | ⛔ Not Supported |
| System Strings `xtmonth, jobid, subscriberkey, etc` | ⛔ Not Supported |


## Supported Functions

If the function isn't listed here, then it's not supported.
| Function Area | Supported / Notes |
|----------|---------|
|[Marketing Cloud API](https://ampscript.guide/marketing-cloud-api-functions/)| ⛔ Not Supported |
|[Contact Model](https://ampscript.guide/content-model-functions/)| ⛔ Not Supported |
|[Content](https://ampscript.guide/content-functions/)| ⚠️ Partial support |
|[Data Extensions](https://ampscript.guide/data-extension-functions/)| ⚠️ Partial Support |
|[Date and Time](https://ampscript.guide/date-and-time-functions/)| ⚠️ Partial Support |
|[Einstein](https://ampscript.guide/einstein-email-recommendation-functions/)| ⛔ Not Supported |
|[Encryption](https://ampscript.guide/encryption-and-encoding-functions/)| ⛔ Not Supported |
|[HTTP](https://ampscript.guide/http-functions/)| ⛔ Not Supported |
|[Math](https://ampscript.guide/math-functions/)| ✔️ |
|[Microsoft Dynamics](https://ampscript.guide/microsoft-dynamics-crm-functions/)| ⛔ Not Supported |
|[Sales and Service Cloud](https://ampscript.guide/sales-and-service-cloud-functions/)| ⛔ Not Supported |
|[Site](https://ampscript.guide/site-based-functions/)| ⛔ Not Supported |
|[Social](https://ampscript.guide/social-functions/)| ⛔ Not Supported |
|[String](https://ampscript.guide/string-functions/)| ⚠️ Partial Support |
|[Utility](https://ampscript.guide/utility-functions/)| ⚠️ Partial Support |
|[Content Syndication](https://ampscript.guide/content-syndication/)| ⛔ Not Supported |


## Roadmap
* VSCode plugin
* Increase supported feature
* A 95% compatibility target for supporting ampscript functions, with mockability for local execution.
* SSJS & GTL
* Static code analysis tools to identify and remediate performance or security issues

## Docs
-   [Code of Conduct](./CODE_OF_CONDUCT.md)
-   [LICENSE](./LICENSE)
