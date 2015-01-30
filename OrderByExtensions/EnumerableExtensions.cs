        using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderByExtensions
{
    public static class EnumerableExtensions
    {
        static readonly char[] _splitOnComma = new[] { ',' };
        
        public static IOrderedEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> source, string propertyName, params string[] thenByPropertyNames)
        {
            return OrderByWithDefault(source, propertyName, true, thenByPropertyNames);
        }

        public static IOrderedEnumerable<TSource> OrderByDescending<TSource>(this IEnumerable<TSource> source, string propertyName, params string[] thenByPropertyNames)
        {
            return OrderByWithDefault(source, propertyName, false, thenByPropertyNames);
        }

        public static IOrderedEnumerable<TSource> ThenBy<TSource>(this IOrderedEnumerable<TSource> source, string propertyName)
        {
            return ThenByPropAndDirection(source, new OrderByProperty(propertyName, true));
        }

        public static IOrderedEnumerable<TSource> ThenByDescending<TSource>(this IOrderedEnumerable<TSource> source, string propertyName)
        {
            return ThenByPropAndDirection(source, new OrderByProperty(propertyName, false));
        }

        private static IOrderedEnumerable<TSource> OrderByWithDefault<TSource>(this IEnumerable<TSource> source, string propertyName, bool isAscending, params string[] thenByPropertyNames)
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


        private static IOrderedEnumerable<TSource> OrderByPropAndDirection<TSource>(this IEnumerable<TSource> source, OrderByProperty property)
        {
            var propFunc = Functions.GetExpression<TSource>(property.PropertyName).Compile();
            return property.IsAscending ? source.OrderBy(propFunc) : source.OrderByDescending(propFunc);
        }

        private static IOrderedEnumerable<TSource> ThenByPropAndDirection<TSource>(this IOrderedEnumerable<TSource> source, OrderByProperty property)
        {
            var propFunc = Functions.GetExpression<TSource>(property.PropertyName).Compile();
            return property.IsAscending ? source.ThenBy(propFunc) : source.ThenByDescending(propFunc);
        }
    }
}