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

        public static PropertyInfo? GetCorrespondingPropertyOfType(this Type type, Type correspondingType)
        {
            return type.GetProperties().FirstOrDefault(x => 
                x.PropertyType == correspondingType || 
                x.PropertyType.IsList() && 
                x.PropertyType.GetGenericArguments().First() == correspondingType);
        }
        
    }
}