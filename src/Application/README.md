# Seeds of Application

This package contains various utility classes for your domain driven design application layer.

## Installation

```bash
dotnet add package DrifterApps.Seeds.Application
```

## Registering with `MediatRServiceConfiguration`

Follow the instructions in the Mediatr documentation. But as of writing this, you can register the open generic
behaviors as such:

```csharp
    services.AddMediatR(serviceConfiguration =>
    {
        serviceConfiguration.RegisterServicesFromApplication();
    });
```
