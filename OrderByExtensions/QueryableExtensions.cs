using System;
using System.Collections.Generic;
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
            return property.IsAscending ? _PropertyCache<TSource>.OrderBy(source, property.PropertyName) : _PropertyCache<TSource>.OrderByDescending(source, property.PropertyName);
        }

        private static IOrderedQueryable<TSource> ThenByPropAndDirection<TSource>(this IOrderedQueryable<TSource> source, OrderByProperty property)
        {
            return property.IsAscending ? _PropertyCache<TSource>.ThenBy(source, property.PropertyName) : _PropertyCache<TSource>.ThenByDescending(source, property.PropertyName);
        }

        public static class _PropertyCache<TSource>
        {
            private static MethodInfo orderByMethod = ((Func<IQueryable<TSource>, Expression<Func<TSource, object>>, IOrderedQueryable<TSource>>)Queryable.OrderBy).Method.GetGenericMethodDefinition();
            private static MethodInfo orderByDescendingMethod = ((Func<IQueryable<TSource>, Expression<Func<TSource, object>>, IOrderedQueryable<TSource>>)Queryable.OrderByDescending).Method.GetGenericMethodDefinition();
            private static MethodInfo thenByMethod = ((Func<IOrderedQueryable<TSource>, Expression<Func<TSource, object>>, IOrderedQueryable<TSource>>)Queryable.ThenBy).Method.GetGenericMethodDefinition();
            private static MethodInfo thenByDescendingMethod = ((Func<IOrderedQueryable<TSource>, Expression<Func<TSource, object>>, IOrderedQueryable<TSource>>)Queryable.ThenByDescending).Method.GetGenericMethodDefinition();

            static Dictionary<string, Func<IQueryable<TSource>, IOrderedQueryable<TSource>>> OrderByFuncs = new Dictionary<string, Func<IQueryable<TSource>, IOrderedQueryable<TSource>>>();
            static Dictionary<string, Func<IOrderedQueryable<TSource>, IOrderedQueryable<TSource>>> ThenByFuncs = new Dictionary<string, Func<IOrderedQueryable<TSource>, IOrderedQueryable<TSource>>>();

            const string _ascendingFormat = "{0}-ASC";
            const string _descendingFormat = "{0}-DESC";

            static _PropertyCache()
            {
                var t = typeof(TSource);
                foreach (var property in t.GetProperties().Where(v => v.CanRead))
                {
                    var name = property.Name;

                    // expressions
                    var orderBy = orderByMethod.MakeGenericMethod(t, property.PropertyType);
                    var orderByDescending = orderByDescendingMethod.MakeGenericMethod(t, property.PropertyType);
                    var thenBy = thenByMethod.MakeGenericMethod(t, property.PropertyType);
                    var thenByDescending = thenByDescendingMethod.MakeGenericMethod(t, property.PropertyType);

                    // compiled
                    var orderByFunc = MakeExpressionBodyOrderBy(orderBy, property).Compile();
                    var orderByDescendingFunc = MakeExpressionBodyOrderBy(orderByDescending, property).Compile();
                    var thenByFunc = MakeExpressionBodyThenBy(thenBy, property).Compile();
                    var thenByDescendingFunc = MakeExpressionBodyThenBy(thenByDescending, property).Compile();

                    // stored in dictionary
                    OrderByFuncs[string.Format(_ascendingFormat, name)] = orderByFunc;
                    OrderByFuncs[string.Format(_descendingFormat, name)] = orderByDescendingFunc;
                    ThenByFuncs[string.Format(_ascendingFormat, name)] = thenByFunc;
                    ThenByFuncs[string.Format(_descendingFormat, name)] = thenByDescendingFunc;
                }
            }

            private static Expression<Func<IQueryable<TSource>, IOrderedQueryable<TSource>>> MakeExpressionBodyOrderBy(MethodInfo method, PropertyInfo prop)
            {
                var param = Expression.Parameter(typeof(IQueryable<TSource>));
                var propertyExpression = Functions.GetGenericExpression<TSource>(prop.Name);

                return Expression.Lambda<Func<IQueryable<TSource>, IOrderedQueryable<TSource>>>(
                    Expression.Call(null, method, param, propertyExpression),
                        param);
            }

            private static Expression<Func<IOrderedQueryable<TSource>, IOrderedQueryable<TSource>>> MakeExpressionBodyThenBy(MethodInfo method, PropertyInfo prop)
            {
                var param = Expression.Parameter(typeof(IOrderedQueryable<TSource>));
                var propertyExpression = Functions.GetGenericExpression<TSource>(prop.Name);

                return Expression.Lambda<Func<IOrderedQueryable<TSource>, IOrderedQueryable<TSource>>>(
                    Expression.Call(null, method, param, propertyExpression),
                        param);
            }

            public static IOrderedQueryable<TSource> OrderBy(IQueryable<TSource> query, string propertyName)
            {
                return OrderByFuncs[string.Format(_ascendingFormat, propertyName)](query);
            }

            public static IOrderedQueryable<TSource> OrderByDescending(IQueryable<TSource> query, string propertyName)
            {
                return OrderByFuncs[string.Format(_descendingFormat, propertyName)](query);
            }

            public static IOrderedQueryable<TSource> ThenBy(IOrderedQueryable<TSource> query, string propertyName)
            {
                return ThenByFuncs[string.Format(_ascendingFormat, propertyName)](query);
            }

            public static IOrderedQueryable<TSource> ThenByDescending(IOrderedQueryable<TSource> query, string propertyName)
            {
                return ThenByFuncs[string.Format(_descendingFormat, propertyName)](query);
            }
        }
    }
}