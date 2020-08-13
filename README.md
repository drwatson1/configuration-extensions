# Configuration Extensions

The project contains two utilities to solve some general problems.
These are not a silver bullet and don't solve all your problems, but do your coding a little bit easier.

## AutoBind

[![NuGet](https://img.shields.io/nuget/v/Contrib.Extensions.Configuration.AutoBind.svg)](https://www.nuget.org/packages/Contrib.Extensions.Configuration.AutoBind)

The package contains helpers to make your code a bit less verbose when you add a new configuration option class to your system. 

Install the package:

```
Install-Package Contrib.Extensions.Configuration.AutoBind
```

Use AutoBind:

```csharp
public class MyServerOptions
{
    // ...
}

// in the Startup.cs:
public void ConfigureServices(IServiceCollection services)
{
    // This call bind the MyServerOptions class to the configuration file section with the 'MyServerOptions' name
    services.AddOptions<MyServerOptions>.AutoBind();
}
```

You can customize the section name by adding a class-level read-only property or field to the class with the name 'SectionName' as follow:

```csharp
public class MyServerOptions
{
    // You can use const string field
    public const string SectionName = "MyCoolOptions";
    // or 
    public static readonly string SectionName = "AnotherCoolOptions";
    // or
    public static string SectionName { get; }  = "CoolOptions";
}
```

If the class contains one of these fields or properties, AutoBind uses it. If not,  it uses the class name.

## VariablesSubstitution

[![NuGet](https://img.shields.io/nuget/v/Contrib.Extensions.Configuration.VariablesSubstitution.svg)](https://www.nuget.org/packages/Contrib.Extensions.Configuration.VariablesSubstitution)

The package allows you to expand environment variables in configuration files.

### Basic usage

Install the package:

```
Install-Package Contrib.Extensions.Configuration.VariablesSubstitution
```

Define a configuration option class:

```csharp
public class MyServerOptions
{
    public string TempFolder { get; set; }
    public string DataFiles { get; set; }
    // other options
    // ...
}
```

Bind the class to a configuration file section and configure it:

```csharp
services.AddOptions<MyServerOptions>
    .AutoBind()
    .SubstituteVariables(); // This call makes it happen
```

Now you can add a configuration section and use environment variables in your `appsettings.json`:

```json
{
    "MyServerOptions": {
        "TempFolder": "%TEMP%/MyApp",
        "DataFiles": "%AppData%/MyApp/DataFiles"
    }
}
```

The substitution works for all nested options including any types of mutable lists. So, this will work too:

```csharp
public class MyServerOptions
{
    public class SubOptions
    {
        public string Proxy { get; set; }
    }

    public string TempFolder { get; set; }
    public string DataFiles { get; set; }
    public string[] StringList { get; set; }
    public SubOptions SubOptions { get; set; }
    public List<SubOptions> SubOptionsList { get; set; }
}
```

```json
{
    "MyServerOptions": {
        "TempFolder": "%TEMP%/MyApp",
        "DataFiles": "%AppData%/MyApp/DataFiles",
        "StringList": [
            "value1",
            "%USER%"
        ],
        "SubOptions": {
            "Proxy": "%PROXY_ADDRESS%"
        },
        "SubOptionsList": [
            {
                "Proxy": "%PROXY_ADDRESS%",
            },
            {
                "Proxy": "%PROXY_ADDRESS%",
            }
        ]
    }
}
```

### Customization

But what if you do want to use `$TEMP` or `$(TEMP)` instead of `%TEMP%`, or maybe don't want to use environment variables?

You can do it with two steps:

1. Implement the interface: 

```csharp
namespace Contrib.Extensions.Configuration.VariablesSubstitution
{
    public interface IVariablesSubstitution<T>
    {
        T Substitute(T value);
    }
}
```

2. Register the implementation in the DI-container BEFORE any call of SubstituteVariables:

```csharp
// Register your implementation
services.AddSingleton<IVariablesSubstitution<string>, YourImplementation>();

services.AddOptions<MyServerOptions>
    .AutoBind()
    .SubstituteVariables();
```

## Versions

| Date | Version | Description |
|-|-|-|
| 2020-08-13 | 1.1.0 | Support lists of nested options and strings
| 2020-07-17 | 1.0.0 | Initial release


Have fun!
