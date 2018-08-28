using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NLIP.iShare.Abstractions
{
    public static class LinqExtensions
    {
        public static IQueryable<TEntity> GetPage<TEntity>(this IQueryable<TEntity> source, Query query)
            => query.Page.HasValue && query.PageSize.HasValue ? source.GetPage(query.Page.Value, query.PageSize.Value) : source;

        public static IQueryable<TEntity> GetPage<TEntity>(this IQueryable<TEntity> source, int page, int pageSize)
        {
            if (page > 0 && pageSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "The page size should be a positive value.");
            }

            return page == 0 ? source
                : source.Skip((page - 1) * pageSize).Take(pageSize);
        }
        public static IOrderedQueryable<TEntity> OrderBy<TEntity, TKey>(
            this IQueryable<TEntity> query,
            Expression<Func<TEntity, TKey>> orderByExpression,
            SortOrder order)
            => order == SortOrder.Desc ? query.OrderByDescending(orderByExpression) : query.OrderBy(orderByExpression);

        public static IOrderedQueryable<TEntity> ThenBy<TEntity, TKey>(
            this IOrderedQueryable<TEntity> query,
            Expression<Func<TEntity, TKey>> orderByExpression,
            bool descending) =>
            descending ? query.ThenByDescending(orderByExpression) : query.ThenBy(orderByExpression);   

        public static PagedResult<TEntity> ToPagedResult<TEntity>(this IEnumerable<TEntity> source, int total)
        {
            return new PagedResult<TEntity>
            {
                Data = source,
                Count = total
            };
        }
    }
}