using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OR_Mapper.Framework.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsInternal(this Type type)
        {
            return type.IsValueType || type == typeof(string);
        }
        
        public static IEnumerable<PropertyInfo> GetInternalProperties(this Type type)
        {
            var properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (property.PropertyType.IsInternal())
                {
                    yield return property;
                }
            }
        }
        public static IEnumerable<PropertyInfo> GetExternalProperties(this Type type)
        {
            var properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (!property.PropertyType.IsInternal())
                {
                    yield return property;
                }
            }
        }

        public static bool IsList(this Type type)
        {
            return typeof(IList).IsAssignableFrom(type);
        }

        public static bool IsLazy(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>);
        }

        public static Type GetUnderlyingTypeForLazy(this Type type)
        {
            return type.IsLazy() ? type.GetGenericArguments().First() : type;
        }

        public static Type GetUnderlyingType(this Type type)
        {
            type = type.IsLazy() ? type.GetGenericArguments().First() : type;
            type = type.IsList() ? type.GetGenericArguments().First() : type;
            return type;
        }

        public static PropertyInfo? GetCorrespondingPropertyOfType(this Type type, Type correspondingType)
        {
            var test = type
                .GetProperties()
                .FirstOrDefault(x => x.PropertyType.GetUnderlyingType() == correspondingType);

            return test;
        }
        
    }
}