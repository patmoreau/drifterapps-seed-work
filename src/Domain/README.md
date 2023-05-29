# Seeds of domain

This package contains various utility classes for your domain driven design domain layer.

## Installation

```bash
dotnet add package DrifterApps.Seeds.Domain
```

## Usage

- [IAggregateRoot](./IAggregateRoot.cs) is an interface to identify the root of an aggregate;
- [IDomainPublisher](./IDomainPublisher.cs) is an interface indicating that the class is a domain event publisher;
- [IRepository](./IRepositoryOfT.cs) is a repository interface for saving an aggregate;
- [IUnitOfWork](./IUnitOfWork.cs) is an interface to implement a unit of work pattern.

### Exceptions

- [DomainException](./DomainException.cs) represents domain errors that occur during application execution;
- [DomainException<TContext>](./DomainExceptionOfT.cs) represents domain errors that occur during application execution
  of a certain context.
