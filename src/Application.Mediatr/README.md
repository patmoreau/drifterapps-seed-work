# Seeds of Application Mediatr

This package contains various utility classes when using the [Mediatr](https://github.com/jbogard/MediatR) library from
Jimmy Bogard in your domain driven design application layer.

## Installation

```bash
dotnet add package DrifterApps.Seeds.Application.Mediatr
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
