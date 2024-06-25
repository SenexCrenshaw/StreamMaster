using AutoMapper;

using StreamMaster.Domain.Attributes;

using System.Reflection;

namespace StreamMaster.Domain.Mappings;

public class IgnoreMappingProfile : Profile
{
    //private TypeMap? ResolveTypeMap(Type sourceType, Type destinationType)
    //{
    //    return ConfigurationProvider?.GetAllTypeMaps()
    //        .FirstOrDefault(tm => tm.SourceType == sourceType && tm.DestinationType == destinationType);
    //}
    //public IgnoreMappingProfile()
    //{
    //    var types = Assembly.GetExecutingAssembly().GetExportedTypes();

    //    foreach (var type in types)
    //    {
    //        var typeMap = this.CreateMap(type, type);
    //        var typeMapInfo = this.ResolveTypeMap(type, type);

    //        if (typeMapInfo != null)
    //        {
    //            foreach (var property in type.GetProperties())
    //            {
    //                if (property.GetCustomAttribute<IgnoreMapAttribute>() != null)
    //                {
    //                    typeMap.ForMember(property.Name, opt => opt.Ignore());
    //                }
    //            }
    //        }
    //    }

    //}
}

public class IPTVApplicationProfile : Profile
{
    public IPTVApplicationProfile()
    {
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());

    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        Type mapFromType = typeof(IMapFrom<>);

        string mappingMethodName = nameof(IMapFrom<object>.Mapping);

        bool HasInterface(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == mapFromType;
        }

        List<Type> types = assembly.GetExportedTypes().Where(t => t.GetInterfaces().Any(HasInterface)).ToList();

        Type[] argumentTypes = new Type[] { typeof(Profile) };

        foreach (Type? type in types)
        {
            object? instance = Activator.CreateInstance(type);

            MethodInfo? methodInfo = type.GetMethod(mappingMethodName);

            if (methodInfo != null)
            {
                methodInfo.Invoke(instance, new object[] { this });
                ApplyIgnoreMapAttribute(type);
            }
            else
            {
                List<Type> interfaces = type.GetInterfaces().Where(HasInterface).ToList();

                if (interfaces.Count > 0)
                {
                    foreach (Type? @interface in interfaces)
                    {
                        MethodInfo? interfaceMethodInfo = @interface.GetMethod(mappingMethodName, argumentTypes);

                        interfaceMethodInfo?.Invoke(instance, new object[] { this });
                        ApplyIgnoreMapAttribute(type);
                    }
                }
            }
        }
    }

    private void ApplyIgnoreMapAttribute(Type type)
    {
        var map = CreateMap(type, type);
        foreach (var property in type.GetProperties())
        {
            if (property.GetCustomAttribute<IgnoreMapAttribute>() != null)
            {
                map.ForMember(property.Name, opt => opt.Ignore());
            }
        }
    }
}
