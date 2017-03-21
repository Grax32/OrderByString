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
            return _PropertyCache<TSource>.ThenByPropAndDirection(source, new OrderByProperty(propertyName, true));
        }

        public static IOrderedEnumerable<TSource> ThenByDescending<TSource>(this IOrderedEnumerable<TSource> source, string propertyName)
        {
            return _PropertyCache<TSource>.ThenByPropAndDirection(source, new OrderByProperty(propertyName, false));
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

            var returnValue = _PropertyCache<TSource>.OrderByPropAndDirection(source, orderByProperty);

            foreach (var thenByPropertyString in thenByPropertyNames)
            {
                returnValue = _PropertyCache<TSource>.ThenByPropAndDirection(returnValue, new OrderByProperty(thenByPropertyString, isAscending));
            }
            return returnValue;
        }

        private static class _PropertyCache<TSource>
        {
            public static IOrderedEnumerable<TSource> OrderByPropAndDirection(IEnumerable<TSource> source, OrderByProperty property)
            {
                return property.IsAscending ?
                    PropertyCache<TSource, IEnumerable<TSource>, IOrderedEnumerable<TSource>>.OrderBy(source, property.PropertyName) :
                    PropertyCache<TSource, IEnumerable<TSource>, IOrderedEnumerable<TSource>>.OrderByDescending(source, property.PropertyName);
            }

            public static IOrderedEnumerable<TSource> ThenByPropAndDirection(IOrderedEnumerable<TSource> source, OrderByProperty property)
            {
                return property.IsAscending ?
                    PropertyCache<TSource, IEnumerable<TSource>, IOrderedEnumerable<TSource>>.ThenBy(source, property.PropertyName) :
                    PropertyCache<TSource, IEnumerable<TSource>, IOrderedEnumerable<TSource>>.ThenByDescending(source, property.PropertyName);
            }
        }
    }
}