using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Linq.Expressions;

namespace MvcApplication1
{
    public static class Util
    {
        //Thanks to Ernesto for pointing out a small correction in method signature.   
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string sortExpression) where TEntity : class
        {
            var type = typeof(TEntity);
            // Remember that for ascending order GridView just returns the column name and for descending it returns column name followed by DESC keyword   
            // Therefore we need to examine the sortExpression and separate out Column Name and order (ASC/DESC)   
            string[] expressionParts = sortExpression.Split(' '); // Assuming sortExpression is like [ColoumnName DESC] or [ColumnName]   
            string[] orderByProperty = expressionParts[0].Split('.');
            string sortDirection = "asc";
            string methodName = "OrderBy";

            //if sortDirection is descending   
            if (expressionParts.Length > 1 && expressionParts[1] == "desc")
            {
                sortDirection = "Descending";
                methodName += sortDirection; // Add sort direction at the end of Method name   
            }
            var property = type.GetProperty(orderByProperty[0]);
            Type type2;
            PropertyInfo property2 = null;
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            MemberExpression propertyAccess3 = null;
            if (orderByProperty.Length > 1)
            {
                type2 = property.PropertyType;
                var parameter2 = Expression.Parameter(type2, "p");
                property2 = type2.GetProperty(orderByProperty[1]);
                propertyAccess3 = Expression.MakeMemberAccess(propertyAccess, property2);
            }
            LambdaExpression orderByExp;
            MethodCallExpression resultExp;
            if (propertyAccess3 != null)
            {
                orderByExp = Expression.Lambda(propertyAccess3, parameter);
                resultExp = Expression.Call(typeof(Queryable), methodName,
                               new Type[] { type, property2.PropertyType },
                               source.Expression, Expression.Quote(orderByExp));
            }
            else
            {
                orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), methodName,
                    new Type[] { type, property.PropertyType },
                    source.Expression, Expression.Quote(orderByExp));
            }
            return (IQueryable<TEntity>)source.Provider.CreateQuery(resultExp);
        }
    }
}
