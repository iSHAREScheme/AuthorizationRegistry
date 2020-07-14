using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace iSHARE.Api.Interfaces
{
    public interface IJobService
    {
        string EnqueueBackgroundJob(Expression<Func<Task>> callMethod);
        string ScheduleBackgroundJob(Expression<Func<Task>> callMethod, int delayInMinutes);
        void AddOrUpdateRecurringJob(Expression<Func<Task>> callMethod, string cronExpression);
        void AddOrUpdateRecurringJob<TCallee>(Expression<Func<TCallee, Task>> callMethod, string cronExpression);
    }
}
