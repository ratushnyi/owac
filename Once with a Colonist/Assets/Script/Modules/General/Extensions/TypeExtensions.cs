using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class TypeExtensions
{
    public static IEnumerable<Type> GetAllTypesWithAttribute<T>()
        where T : Attribute
    {
        return typeof(T).GetAllTypesWithAttribute();
    }

    public static IEnumerable<Type> GetAllTypesWithAttribute(this Type baseType)
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes(), (assembly, type) => new { assembly, type })
            .Where(t => t.type.IsDefined(baseType))
            .Select(t => t.type);
    }
    
    public static void PopulateObject(object targetObject, object referenceObject)
    {
        if (referenceObject == null)
        {
            return;
        }
            
        var sourceProperties = referenceObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var targetProperties = targetObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(t => t.CanWrite);

        foreach (var targetProperty in targetProperties)
        {
            var sourceProperty = sourceProperties.FirstOrDefault(p => p.Name == targetProperty.Name);
            if (sourceProperty != null && targetProperty.PropertyType == sourceProperty.PropertyType)
            {
                var sourcesValue = sourceProperty.GetValue(referenceObject);
                targetProperty.SetValue(targetObject, sourcesValue);
            }
        }
    }
}