using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
            return _PropertyCache<TSource>.ThenByPropAndDirection(source, new OrderByProperty(propertyName, true));
        }

        public static IOrderedQueryable<TSource> ThenByDescending<TSource>(this IOrderedQueryable<TSource> source, string propertyName)
        {
            return _PropertyCache<TSource>.ThenByPropAndDirection(source, new OrderByProperty(propertyName, false));
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

            var returnValue = _PropertyCache<TSource>.OrderByPropAndDirection(source, orderByProperty);

            foreach (var thenByPropertyString in thenByPropertyNames)
            {
                returnValue = _PropertyCache<TSource>.ThenByPropAndDirection(returnValue, new OrderByProperty(thenByPropertyString, isAscending));
            }
            return returnValue;
        }

        private static class _PropertyCache<TSource>
        {
            public static IOrderedQueryable<TSource> OrderByPropAndDirection(IQueryable<TSource> source, OrderByProperty property)
            {
                return property.IsAscending ?
                    PropertyCache<TSource, IQueryable<TSource>, IOrderedQueryable<TSource>>.OrderBy(source, property.PropertyName) :
                    PropertyCache<TSource, IQueryable<TSource>, IOrderedQueryable<TSource>>.OrderByDescending(source, property.PropertyName);
            }

            public static IOrderedQueryable<TSource> ThenByPropAndDirection(IOrderedQueryable<TSource> source, OrderByProperty property)
            {
                return property.IsAscending ?
                    PropertyCache<TSource, IQueryable<TSource>, IOrderedQueryable<TSource>>.ThenBy(source, property.PropertyName) :
                    PropertyCache<TSource, IQueryable<TSource>, IOrderedQueryable<TSource>>.ThenByDescending(source, property.PropertyName);
            }
        }
    }
}