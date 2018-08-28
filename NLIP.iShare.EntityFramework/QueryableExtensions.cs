using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NLIP.iShare.Abstractions;

namespace NLIP.iShare.EntityFramework
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResult<TEntity>> ToPagedResult<TEntity>(this IQueryable<TEntity> source, int total)
        {
            return new PagedResult<TEntity>
            {
                Data = await source.ToListAsync().ConfigureAwait(false),
                Count = total
            };
        }
    }
}
