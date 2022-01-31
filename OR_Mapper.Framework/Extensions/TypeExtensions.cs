using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OR_Mapper.Framework.Extensions
{
    /// <summary>
    /// This class is used as extension class for types in order to get specific information about type.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Checks if type is internal.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>True if type is internal type.</returns>
        public static bool IsInternal(this Type type)
        {
            return type.IsValueType || type == typeof(string);
        }
        
        /// <summary>
        /// Gets internal properties from given type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>Property infos.</returns>
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
        
        /// <summary>
        /// Gets internal properties from given type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>Property infos.</returns>
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

        /// <summary>
        /// Checks if given type is from type List.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>True if type is from type List.</returns>
        public static bool IsList(this Type type)
        {
            return typeof(IList).IsAssignableFrom(type);
        }

        /// <summary>
        /// Checks if given type is Lazy type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>True if type is Lazy type.</returns>
        public static bool IsLazy(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>);
        }

        /// <summary>
        /// Gets underlying type for given (Lazy) type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>Underlying type.</returns>
        public static Type GetUnderlyingTypeForLazy(this Type type)
        {
            return type.IsLazy() ? type.GetGenericArguments().First() : type;
        }

        /// <summary>
        /// Gets underlying type for given (Lazy) type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>Underlying type.</returns>
        public static Type GetUnderlyingType(this Type type)
        {
            type = type.IsLazy() ? type.GetGenericArguments().First() : type;
            type = type.IsList() ? type.GetGenericArguments().First() : type;
            return type;
        }

        /// <summary>
        /// Gets corresponding property of given type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="correspondingType">Type of corresponding class.</param>
        /// <returns>Property info.</returns>
        public static PropertyInfo? GetCorrespondingPropertyOfType(this Type type, Type correspondingType)
        {
            var propInfo = type
                .GetProperties()
                .FirstOrDefault(x => x.PropertyType.GetUnderlyingType() == correspondingType);

            return propInfo;
        }
        
    }
}