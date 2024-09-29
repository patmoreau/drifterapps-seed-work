using DrifterApps.Seeds.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DrifterApps.Seeds.Application.Context;

public class StronglyTypedIdValueConverter<TStronglyTypedId>() :
    ValueConverter<TStronglyTypedId, Guid>(id => id.Value, value => StronglyTypedId<TStronglyTypedId>.Create(value))
    where TStronglyTypedId : StronglyTypedId<TStronglyTypedId>, new();
