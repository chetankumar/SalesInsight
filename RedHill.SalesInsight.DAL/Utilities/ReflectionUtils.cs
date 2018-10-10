using RedHill.SalesInsight.DAL.Attributes;
using RedHill.SalesInsight.DAL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace RedHill.SalesInsight.DAL.Utilities
{
    public class ReflectionUtils
    {
        public static string GetName<TSource, TField>(Expression<Func<TSource, TField>> field)
        {
            if (object.Equals(field, null))
            {
                throw new NullReferenceException("Field is required");
            }

            MemberExpression expr = null;

            if (field.Body is MemberExpression)
            {
                expr = (MemberExpression)field.Body;
            }
            else if (field.Body is UnaryExpression)
            {
                expr = (MemberExpression)((UnaryExpression)field.Body).Operand;
            }
            else
            {
                const string Format = "Expression '{0}' not supported.";
                string message = string.Format(Format, field);

                throw new ArgumentException(message, "Field");
            }

            return expr.Member.Name;
        }

        public static bool IsNullableType(Type propertyType)
        {
            return propertyType.IsGenericType && propertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        public static string GetAttrValue(PropertyInfo propInfo, string attributeName)
        {
            DisplayAttribute attr = (DisplayAttribute)propInfo.GetCustomAttribute(typeof(DisplayAttribute));
            if (attr != null)
                return attr.Name;
            return propInfo.Name;
        }
    }
}
