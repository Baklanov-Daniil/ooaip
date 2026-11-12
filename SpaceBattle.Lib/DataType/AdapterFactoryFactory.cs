using System.Reflection;

namespace SpaceBattle.Lib;

public sealed class AdapterFactoryFactory
{
    private readonly Dictionary<Type, Func<object, object>> _factories = new();
    private readonly Dictionary<string, Func<object?[], object?>> _strategies = new();

    public void RegisterStrategy(Type interfaceType, string propertyName, Func<object?[], object?> strategy)
    {
        ArgumentNullException.ThrowIfNull(interfaceType);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        ArgumentNullException.ThrowIfNull(strategy);
        if (!interfaceType.IsInterface)
            throw new ArgumentException("Adapter type must be an interface.", nameof(interfaceType));

        _strategies[GetStrategyKey(interfaceType, propertyName)] = strategy;
    }

    public Func<object, object> GetFactory(Type interfaceType)
    {
        ArgumentNullException.ThrowIfNull(interfaceType);
        if (!interfaceType.IsInterface)
            throw new ArgumentException("Adapter type must be an interface.", nameof(interfaceType));

        if (_factories.TryGetValue(interfaceType, out var factory))
            return factory;

        var factoryType = typeof(RuntimeAdapterFactory<>).MakeGenericType(interfaceType);
        var instance = Activator.CreateInstance(factoryType, _strategies)
            ?? throw new InvalidOperationException($"Could not create adapter factory for {interfaceType}.");
        factory = (Func<object, object>)factoryType.GetMethod(nameof(RuntimeAdapterFactory<object>.Create))!
            .CreateDelegate(typeof(Func<object, object>), instance);
        _factories.Add(interfaceType, factory);
        return factory;
    }

    public static string GetStrategyKey(Type interfaceType, string propertyName) =>
        $"{interfaceType.FullName}.{propertyName}";

    private sealed class RuntimeAdapterFactory<TInterface> where TInterface : class
    {
        private readonly Dictionary<string, Func<object?[], object?>> _strategies;

        public RuntimeAdapterFactory(Dictionary<string, Func<object?[], object?>> strategies)
        {
            _strategies = strategies;
        }

        public object Create(object item)
        {
            ArgumentNullException.ThrowIfNull(item);
            var adapter = DispatchProxy.Create<TInterface, RuntimeAdapterProxy>();
            ((RuntimeAdapterProxy)(object)adapter).Initialize(item, _strategies);
            return adapter;
        }
    }

    public class RuntimeAdapterProxy : DispatchProxy
    {
        private object _item = null!;
        private Dictionary<string, Func<object?[], object?>> _strategies = null!;

        public void Initialize(object item, Dictionary<string, Func<object?[], object?>> strategies)
        {
            _item = item;
            _strategies = strategies;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            ArgumentNullException.ThrowIfNull(targetMethod);
            args ??= Array.Empty<object?>();
            var property = targetMethod.DeclaringType?.GetProperties()
                .FirstOrDefault(p => p.GetMethod == targetMethod || p.SetMethod == targetMethod);

            if (property is not null)
            {
                var key = GetStrategyKey(targetMethod.DeclaringType!, property.Name);
                if (_strategies.TryGetValue(key, out var strategy))
                    return strategy(new object[] { _item }.Concat(args).ToArray());

                if (_item is IUObject dynamicItem)
                {
                    if (targetMethod == property.GetMethod)
                        return dynamicItem.getProperty(property.Name);
                    dynamicItem.setProperty(property.Name, args[0]!);
                    return null;
                }

                var itemProperty = _item.GetType().GetProperty(property.Name);
                if (itemProperty is null)
                    throw new InvalidOperationException($"Property '{property.Name}' is not available on {_item.GetType()}.");
                if (targetMethod == property.GetMethod)
                    return itemProperty.GetValue(_item);
                itemProperty.SetValue(_item, args[0]);
                return null;
            }

            if (targetMethod.Name == nameof(ToString) && targetMethod.GetParameters().Length == 0)
                return _item.ToString();
            if (targetMethod.Name == nameof(GetHashCode) && targetMethod.GetParameters().Length == 0)
                return _item.GetHashCode();
            if (targetMethod.Name == nameof(Equals) && args.Length == 1)
                return ReferenceEquals(this, args[0]);
            throw new NotSupportedException($"Only adapter properties are supported: {targetMethod.Name}.");
        }
    }
}


