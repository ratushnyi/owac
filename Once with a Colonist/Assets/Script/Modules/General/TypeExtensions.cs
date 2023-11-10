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
}