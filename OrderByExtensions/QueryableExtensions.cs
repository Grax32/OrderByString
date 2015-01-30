using System;
using System.Linq;

namespace OrderByExtensions
{
    public static class QueryableExtensions
    {
        static readonly char[] _splitOnComma = new[] { ',' };

        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string propertyName, params string[] thenByPropertyNames)
        {
            return OrderByWithDefault(source, propertyName, true, thenByPropertyNames);
        }

        public static IOrderedQueryable<TSource> OrderByDescending<TSource>(this IQueryable<TSource> source, string propertyName, params string[] thenByPropertyNames)
        {
            return OrderByWithDefault(source, propertyName, false, thenByPropertyNames);
        }

        public static IOrderedQueryable<TSource> ThenBy<TSource>(this IOrderedQueryable<TSource> source, string propertyName)
        {
            return source.ThenByPropAndDirection(new OrderByProperty(propertyName, true));
        }

        public static IOrderedQueryable<TSource> ThenByDescending<TSource>(this IOrderedQueryable<TSource> source, string propertyName)
        {
            return source.ThenByPropAndDirection(new OrderByProperty(propertyName, false));
        }

        private static IOrderedQueryable<TSource> OrderByWithDefault<TSource>(this IQueryable<TSource> source, string propertyName, bool isAscending, params string[] thenByPropertyNames)
        {
            if (propertyName.Trim(',').Contains(","))
            {
                var names = propertyName
                    .Split(_splitOnComma, StringSplitOptions.RemoveEmptyEntries)
                    .Union(thenByPropertyNames);

                propertyName = names.First();
                thenByPropertyNames = names.Skip(1).ToArray();
            }

            var orderByProperty = new OrderByProperty(propertyName, isAscending);

            var returnValue = source.OrderByPropAndDirection(orderByProperty);

            foreach (var thenByPropertyString in thenByPropertyNames)
            {
                returnValue = returnValue.ThenByPropAndDirection(new OrderByProperty(thenByPropertyString, isAscending));
            }
            return returnValue;
        }

        private static IOrderedQueryable<TSource> OrderByPropAndDirection<TSource>(this IQueryable<TSource> source, OrderByProperty property)
        {
            var propExpression = Functions.GetExpression<TSource>(property.PropertyName);
            return property.IsAscending ? source.OrderBy(propExpression) : source.OrderByDescending(propExpression);
        }

        private static IOrderedQueryable<TSource> ThenByPropAndDirection<TSource>(this IOrderedQueryable<TSource> source, OrderByProperty property)
        {
            var propExpression = Functions.GetExpression<TSource>(property.PropertyName);
            return property.IsAscending ? source.ThenBy(propExpression) : source.ThenByDescending(propExpression);
        }


    }
}