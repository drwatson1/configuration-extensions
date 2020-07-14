# Configuration Extensions

The project contains two utilities to solve some general problems.

## AutoBind

[![NuGet](https://img.shields.io/nuget/v/Contrib.Extensions.Configuration.AutoBind.svg)](https://www.nuget.org/packages/Contrib.Extensions.Configuration.AutoBind)

The package contains helpers to make your code a bit less verbose when you add another configuration option class to your system. 

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

You can customize the section name by adding a class-level read-only property or field to the class with a name 'SectionName' as follow:

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
