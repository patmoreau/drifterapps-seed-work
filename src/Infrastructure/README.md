# Seeds of infrastructure

This package contains various utility classes for your domain driven design infrastructure layer.

## Installation

```bash
dotnet add package DrifterApps.Seeds.Infrastructure
```

## Registering with `ServiceCollection`

when using [Hangfire](https://www.hangfire.io/) as a IRequestScheduler, add the support using:

```csharp
    services.AddHangfireRequestScheduler();
```
