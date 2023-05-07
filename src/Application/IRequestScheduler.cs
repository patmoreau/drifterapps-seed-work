using MediatR;

namespace DrifterApps.Seeds.Application;

public interface IRequestScheduler
{
    string SendNow(IBaseRequest request, string description);

    void Schedule(IBaseRequest request, DateTimeOffset scheduleAt, string description);

    void Schedule(IBaseRequest request, TimeSpan delay, string description);

    void ScheduleRecurring(IBaseRequest request, string name, string cronExpression, string description);
}
