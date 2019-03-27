## Version 2.0 Changes
Version 2 changes the concept of System.AzureErrorLog. Whilst previous versions have been static, from Version 2 the framework is marking the AzureErrorLog as abstract.

You must inherit and build a parent seal package.

**Settings**

Implement IAzureErrorLogSettings to structure how settings are provided. For .NET 4.5.2 this might be to implement reading from web.config, whilst .NET standard 2.0 this might be to read from appsettings.json.