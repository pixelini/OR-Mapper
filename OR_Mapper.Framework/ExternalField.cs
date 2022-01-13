﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using OR_Mapper.Framework.Exceptions;
using OR_Mapper.Framework.Extensions;

namespace OR_Mapper.Framework
{
    public class ExternalField
    {
        public Model Model { get; set; }

        public Relation Relation { get; set; }


        public ExternalField(Model model, Relation relation)
        {
            Model = model;
            Relation = relation;
        }
        
        public object? GetValue(object obj)
        {
            var type = obj.GetType();

            // find property that has the desired underlying type
            var correspondingProperty = type
                .GetProperties()
                .First(x => Model.Member == x.PropertyType.GetUnderlyingType());

            // read the property's value
            var propertyValue = correspondingProperty.GetValue(obj);
            
            // if property is Lazy<>:
            // Call Lazy<>.Value to retrieve the inner value stored inside the lazy property
            if (correspondingProperty.PropertyType.IsLazy())
            {
                var innerValue = correspondingProperty.PropertyType
                    .GetProperties()
                    .First(x => x.Name == "Value")  // Find property Lazy<>.Value
                    .GetValue(propertyValue);  // Get inner value (calls Lazy<>.Value)
                
                propertyValue = innerValue;
            }

            return propertyValue;
        }

        public void SetValue(object obj, object? value)
        {
            var type = obj.GetType();
            
            // find property that has the desired underlying type
            var correspondingProperty = type
                .GetProperties()
                .First(x => Model.Member == x.PropertyType.GetUnderlyingType());
            
            // if property is Lazy<>:
            // Construct a Lazy<>(Db.LoadOneToMany()) type and pass 'value' as constructor parameter
            if (correspondingProperty.PropertyType.IsLazy())
            {
                value = Activator.CreateInstance(correspondingProperty.PropertyType, value);

                if (value is null)
                {
                    throw new InvalidEntityException(
                        $"Failed to create instance of {correspondingProperty.PropertyType.Name}");
                }
            }
            
            correspondingProperty.SetValue(obj, value);
        }
    }
}