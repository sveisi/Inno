using Inno.Models;
using System;
using System.Linq;

namespace Inno.Data.Queries
{
    public static class OrderQuery
    {
        /// <summary> سفارش باز و جاری مشتری-کاربر </summary>
        public static IQueryable<Order> CurrentOrder(this IQueryable<Order> query, Guid userId)
        {
            return query.Where(x => x.CreatedBy == userId && !x.ConfirmedAt.HasValue);
        }
    }
}
