using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace OrderByExtensions
{
    internal class Functions
    {
        internal static Expression<Func<TSource, object>> GetExpression<TSource>(string propertyName)
        {
            var param = Expression.Parameter(typeof(TSource));

            return Expression.Lambda<Func<TSource, object>>(
                Expression.Convert(
                    Expression.Property(param, propertyName),
                    typeof(object)
                )
                , param);
        }
    }
}
