using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace OrderByExtensions
{
    static class PropertyCache<TSource, TQuery, TOrderedQuery>
    {
        private static class QueryableMethods
        {
            public static MethodInfo orderByMethod = ((Func<IQueryable<TSource>, Expression<Func<TSource, object>>, IOrderedQueryable<TSource>>)Queryable.OrderBy).Method.GetGenericMethodDefinition();
            public static MethodInfo orderByDescendingMethod = ((Func<IQueryable<TSource>, Expression<Func<TSource, object>>, IOrderedQueryable<TSource>>)Queryable.OrderByDescending).Method.GetGenericMethodDefinition();
            public static MethodInfo thenByMethod = ((Func<IOrderedQueryable<TSource>, Expression<Func<TSource, object>>, IOrderedQueryable<TSource>>)Queryable.ThenBy).Method.GetGenericMethodDefinition();
            public static MethodInfo thenByDescendingMethod = ((Func<IOrderedQueryable<TSource>, Expression<Func<TSource, object>>, IOrderedQueryable<TSource>>)Queryable.ThenByDescending).Method.GetGenericMethodDefinition();
        }

        private static class EnumerableMethods
        {
            public static MethodInfo orderByMethod = ((Func<IEnumerable<TSource>, Func<TSource, object>, IOrderedEnumerable<TSource>>)Enumerable.OrderBy).Method.GetGenericMethodDefinition();
            public static MethodInfo orderByDescendingMethod = ((Func<IEnumerable<TSource>, Func<TSource, object>, IOrderedEnumerable<TSource>>)Enumerable.OrderByDescending).Method.GetGenericMethodDefinition();
            public static MethodInfo thenByMethod = ((Func<IOrderedEnumerable<TSource>, Func<TSource, object>, IOrderedEnumerable<TSource>>)Enumerable.ThenBy).Method.GetGenericMethodDefinition();
            public static MethodInfo thenByDescendingMethod = ((Func<IOrderedEnumerable<TSource>, Func<TSource, object>, IOrderedEnumerable<TSource>>)Enumerable.ThenByDescending).Method.GetGenericMethodDefinition();
        }

        private static Type _type = typeof(TSource);
        private static bool isQueryable = typeof(TQuery) == typeof(IQueryable<TSource>);

        private static MethodInfo orderByMethod = isQueryable ? QueryableMethods.orderByMethod : EnumerableMethods.orderByMethod;
        private static MethodInfo orderByDescendingMethod = isQueryable ? QueryableMethods.orderByDescendingMethod : EnumerableMethods.orderByDescendingMethod;
        private static MethodInfo thenByMethod = isQueryable ? QueryableMethods.thenByMethod : EnumerableMethods.thenByMethod;
        private static MethodInfo thenByDescendingMethod = isQueryable ? QueryableMethods.thenByDescendingMethod : EnumerableMethods.thenByDescendingMethod;

        static Dictionary<string, Func<TQuery, TOrderedQuery>> OrderByFuncs = new Dictionary<string, Func<TQuery, TOrderedQuery>>();
        static Dictionary<string, Func<TOrderedQuery, TOrderedQuery>> ThenByFuncs = new Dictionary<string, Func<TOrderedQuery, TOrderedQuery>>();

        const string _ascendingFormat = "{0}-ASC";
        const string _descendingFormat = "{0}-DESC";

        private static Expression<Func<TIn, TOrderedQuery>> MakeSortExpression<TIn>(MethodInfo genericMethod, string propertyName)
        {
            var propertyExpression = Functions.GetGenericExpression<TSource>(propertyName);

            var propertyType = propertyExpression.Type.GetGenericArguments().Last();

            var method = genericMethod.MakeGenericMethod(typeof(TSource), propertyType);
            var param = Expression.Parameter(typeof(TIn));

            return Expression.Lambda<Func<TIn, TOrderedQuery>>(
                Expression.Call(null, method, param, propertyExpression),
                    param);
        }

        static Func<TIn, TOrderedQuery> GetCachedOrderCall<TIn>(Dictionary<string, Func<TIn, TOrderedQuery>> _cache, string formattedName, string propertyName, MethodInfo method)
        {
            Debug.WriteLine("{0} {1} by {2}", method.DeclaringType.FullName + "." + method.Name, _type.Name, formattedName);
            if (!_cache.ContainsKey(formattedName))
            {
                _cache[formattedName] = MakeSortExpression<TIn>(method, propertyName).Compile();
            }
            return _cache[formattedName];
        }

        public static TOrderedQuery OrderBy(TQuery query, string propertyName)
        {
            return GetCachedOrderCall(OrderByFuncs, string.Format(_ascendingFormat, propertyName), propertyName, orderByMethod)(query);
        }

        public static TOrderedQuery OrderByDescending(TQuery query, string propertyName)
        {
            return GetCachedOrderCall(OrderByFuncs, string.Format(_descendingFormat, propertyName), propertyName, orderByDescendingMethod)(query);
        }

        public static TOrderedQuery ThenBy(TOrderedQuery query, string propertyName)
        {
            return GetCachedOrderCall(ThenByFuncs, string.Format(_ascendingFormat, propertyName), propertyName, thenByMethod)(query);
        }

        public static TOrderedQuery ThenByDescending(TOrderedQuery query, string propertyName)
        {
            return GetCachedOrderCall(ThenByFuncs, string.Format(_descendingFormat, propertyName), propertyName, thenByDescendingMethod)(query);
        }
    }
}
