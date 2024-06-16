using System.Reflection;
using ModelCache.Api.Attributes;
using ModelCache.Api.Models;

namespace ModelCache.Api;

public static class Extensions
{
    public static void ScanAssemblies(this IModelSynchronizationService service, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            LoadEffects(service, assembly);
        }
    }

    private static void LoadEffects(IModelSynchronizationService service, Assembly assembly)
    {
        var createEffects = assembly.GetTypes()
            .SelectMany(t => t.GetMethods().Where(m => m.GetCustomAttributes().Any(a => a is CreateEffect))).ToArray();
        AddCreateEffects(service, createEffects);
        var readEffects = assembly.GetTypes()
            .SelectMany(t => t.GetMethods().Where(m => m.GetCustomAttributes().Any(a => a is ReadEffect))).ToArray();
        AddReadEffects(service, readEffects);
        var readManyEffects = assembly.GetTypes()
            .SelectMany(t => t.GetMethods().Where(m => m.GetCustomAttributes().Any(a => a is ReadManyEffect)))
            .ToArray();
        AddReadManyEffects(service, readManyEffects);
        var readManyFromCacheEffects = assembly.GetTypes()
            .SelectMany(t => t.GetMethods().Where(m => m.GetCustomAttributes().Any(a => a is ReadManyFromCacheEffect)))
            .ToArray();
        AddReadManyFromCacheEffects(service, readManyFromCacheEffects);

        var updateEffects = assembly.GetTypes()
            .SelectMany(t => t.GetMethods().Where(m => m.GetCustomAttributes().Any(a => a is UpdateEffect))).ToArray();
        AddUpdateEffects(service, updateEffects);
        var deleteEffects = assembly.GetTypes()
            .SelectMany(t => t.GetMethods().Where(m => m.GetCustomAttributes().Any(a => a is DeleteEffect))).ToArray();
        AddDeleteEffects(service, deleteEffects);
    }


    private static void AddCreateEffects(IModelSynchronizationService service, IEnumerable<MethodInfo> createEffects)
    {
        // EffectMethods = readEffects.GroupBy(k => k.GetParameters().First().ParameterType.FullName ?? k.GetParameters().First().ParameterType.Name);
        foreach (var effect in createEffects)
        {
            if (!effect.IsStatic)
            {
                // Effect has to be static
                continue;
            }

            var attribute = effect.GetCustomAttributes().FirstOrDefault(a => a is CreateEffect) as CreateEffect;
            var modelType = attribute?.AssociatedType;
            if (modelType == null)
            {
                // Effect has no model type associated.
                continue;
            }

            var parameters = effect.GetParameters();
            var returnParameter = effect.ReturnParameter;
            var count = parameters.Length;
            if (parameters[0].ParameterType != typeof(IServiceProvider))
            {
                // First parameter must be IServiceProvider
                continue;
            }

            if (parameters[1].ParameterType != typeof(object))
            {
                // First parameter must be object[]
                continue;
            }

            if (returnParameter.ParameterType != typeof(Task<object>))
            {
                // return type must be object
                continue;
            }


            foreach (var parameterInfo in parameters)
            {
                var type = parameterInfo.ParameterType;
            }

            service.AddModelCreateEffect(modelType,
                (serviceProvider, model) =>
                    (Task<object>)effect.Invoke(null, new object[] { serviceProvider, model }));
        }
    }

    private static void AddReadEffects(IModelSynchronizationService service, IEnumerable<MethodInfo> readEffects)
    {
        // EffectMethods = readEffects.GroupBy(k => k.GetParameters().First().ParameterType.FullName ?? k.GetParameters().First().ParameterType.Name);
        foreach (var effect in readEffects)
        {
            if (!effect.IsStatic)
            {
                // Effect has to be static
                continue;
            }

            var attribute = effect.GetCustomAttributes().FirstOrDefault(a => a is ReadEffect) as ReadEffect;
            var modelType = attribute?.AssociatedType;
            if (modelType == null)
            {
                // Effect has no model type associated.
                continue;
            }

            var parameters = effect.GetParameters();
            var returnParameter = effect.ReturnParameter;
            var count = parameters.Length;
            if (parameters[0].ParameterType != typeof(IServiceProvider))
            {
                // First parameter must be IServiceProvider
                continue;
            }

            if (parameters[1].ParameterType != typeof(Guid))
            {
                // First parameter must be Guid
                continue;
            }

            if (parameters[2].ParameterType != typeof(object[]))
            {
                // First parameter must be object[]
                continue;
            }

            if (returnParameter.ParameterType != typeof(Task<object>))
            {
                // return type must be object
                continue;
            }

            service.AddModelReadEffect(modelType,
                (serviceProvider, guid, models) =>
                    (Task<object>)effect.Invoke(null, new object[] { serviceProvider, guid, models }));
        }
    }

    private static void AddReadManyEffects(IModelSynchronizationService service,
        IEnumerable<MethodInfo> readManyEffects)
    {
        // EffectMethods = readManyEffects.GroupBy(k => k.GetParameters().First().ParameterType.FullName ?? k.GetParameters().First().ParameterType.Name);
        foreach (var effect in readManyEffects)
        {
            if (!effect.IsStatic)
            {
                // Effect has to be static
                continue;
            }

            var attribute = effect.GetCustomAttributes().FirstOrDefault(a => a is ReadManyEffect) as ReadManyEffect;
            var modelType = attribute?.AssociatedType;
            if (modelType == null)
            {
                // Effect has no model type associated.
                continue;
            }

            var parameters = effect.GetParameters();
            var returnParameter = effect.ReturnParameter;
            var count = parameters.Length;
            if (parameters[0].ParameterType != typeof(IServiceProvider))
            {
                // First parameter must be IServiceProvider
                continue;
            }

            if (parameters[1].ParameterType != typeof(ModelSearchOptionsBase))
            {
                // First parameter must be ModelSearchOptionsBase
                continue;
            }

            if (parameters[2].ParameterType != typeof(Dictionary<string, object>))
            {
                // First parameter must be dictionary
                continue;
            }

            if (returnParameter.ParameterType != typeof(Task<object[]>))
            {
                // return type must be object
                continue;
            }

            service.AddModelReadManyEffect(modelType,
                (serviceProvider, searchOptions, dict) =>
                    (Task<object[]>)effect.Invoke(null, new object[] { serviceProvider, searchOptions, dict }));
        }
    }

    private static void AddReadManyFromCacheEffects(IModelSynchronizationService service,
        IEnumerable<MethodInfo> readManyFromCacheEffects)
    {
        // EffectMethods = readEffects.GroupBy(k => k.GetParameters().First().ParameterType.FullName ?? k.GetParameters().First().ParameterType.Name);
        foreach (var effect in readManyFromCacheEffects)
        {
            if (!effect.IsStatic)
            {
                // Effect has to be static
                continue;
            }

            var attribute =
                effect.GetCustomAttributes().FirstOrDefault(a => a is ReadManyFromCacheEffect) as
                    ReadManyFromCacheEffect;
            var modelType = attribute?.AssociatedType;
            if (modelType == null)
            {
                // Effect has no model type associated.
                continue;
            }

            var parameters = effect.GetParameters();
            var returnParameter = effect.ReturnParameter;
            var count = parameters.Length;
            if (parameters[0].ParameterType != typeof(IServiceProvider))
            {
                // First parameter must be IServiceProvider
                continue;
            }

            if (parameters[1].ParameterType != typeof(ModelSearchOptionsBase))
            {
                // First parameter must be ModelSearchOptionsBase
                continue;
            }

            if (parameters[2].ParameterType != typeof(Dictionary<string, object>))
            {
                // First parameter must be dictionary
                continue;
            }

            if (returnParameter.ParameterType != typeof(IQueryable<object>))
            {
                // return type must be object
                continue;
            }

            service.AddModelReadManyFromCacheEffect(modelType,
                (serviceProvider, searchOptions, dict) =>
                    (IQueryable<object>)effect.Invoke(null, new object[] { serviceProvider, searchOptions, dict }));
        }
    }

    private static void AddUpdateEffects(IModelSynchronizationService service, IEnumerable<MethodInfo> updateEffects)
    {
        // EffectMethods = readEffects.GroupBy(k => k.GetParameters().First().ParameterType.FullName ?? k.GetParameters().First().ParameterType.Name);
        foreach (var effect in updateEffects)
        {
            if (!effect.IsStatic)
            {
                // Effect has to be static
                continue;
            }

            var attribute = effect.GetCustomAttributes().FirstOrDefault(a => a is UpdateEffect) as UpdateEffect;
            var modelType = attribute?.AssociatedType;
            if (modelType == null)
            {
                // Effect has no model type associated.
                continue;
            }

            var parameters = effect.GetParameters();
            var returnParameter = effect.ReturnParameter;
            var count = parameters.Length;
            if (parameters[0].ParameterType != typeof(IServiceProvider))
            {
                // First parameter must be IServiceProvider
                continue;
            }

            if (parameters[1].ParameterType != typeof(object))
            {
                // First parameter must be model
                continue;
            }

            if (returnParameter.ParameterType != typeof(Task<object>))
            {
                // return type must be object
                continue;
            }

            service.AddModelUpdateEffect(modelType,
                (serviceProvider, model) =>
                    (Task<object>)effect.Invoke(null, new object[] { serviceProvider, model }));
        }
    }

    private static void AddDeleteEffects(IModelSynchronizationService service, IEnumerable<MethodInfo> deleteEffects)
    {
        // EffectMethods = readEffects.GroupBy(k => k.GetParameters().First().ParameterType.FullName ?? k.GetParameters().First().ParameterType.Name);
        foreach (var effect in deleteEffects)
        {
            if (!effect.IsStatic)
            {
                // Effect has to be static
                continue;
            }

            var attribute = effect.GetCustomAttributes().FirstOrDefault(a => a is DeleteEffect) as DeleteEffect;
            var modelType = attribute?.AssociatedType;
            if (modelType == null)
            {
                // Effect has no model type associated.
                continue;
            }

            var parameters = effect.GetParameters();
            var returnParameter = effect.ReturnParameter;
            var count = parameters.Length;
            if (parameters[0].ParameterType != typeof(IServiceProvider))
            {
                // First parameter must be IServiceProvider
                continue;
            }

            if (parameters[1].ParameterType != typeof(Guid))
            {
                // First parameter must be Guid
                continue;
            }

            if (returnParameter.ParameterType != typeof(Task<object>))
            {
                // return type must be object
                continue;
            }

            service.AddModelDeleteEffect(modelType,
                (serviceProvider, guid) =>
                    (Task<object>)effect.Invoke(null, new object[] { serviceProvider, guid }));
        }
    }
}