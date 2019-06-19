using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace iSHARE.Api.Interfaces
{
    public interface IJobService
    {
        string EnqueueBackgroundJob(Expression<Func<Task>> callMethod);
        void AddOrUpdateRecurringJob(Expression<Func<Task>> callMethod, string cronExpression);
    }
}
