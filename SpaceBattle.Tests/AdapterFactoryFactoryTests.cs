using System.Reflection;
using SpaceBattle.Lib;

namespace SpaceBattle.Lib.Tests;

public interface IPositionAdapter
{
    int Position { get; set; }
}

public interface IOtherPositionAdapter
{
    int Position { get; set; }
}

public interface IValueAdapter
{
    int Value { get; set; }
}

public interface IMissingPropertyAdapter
{
    int Missing { get; }
}

public interface IMethodAdapter
{
    void Run();
}

public sealed class TestRuntimeAdapterProxy : AdapterFactoryFactory.RuntimeAdapterProxy
{
    public object? InvokeForTest(MethodInfo? method, object?[]? args = null) => Invoke(method, args);
}

public class AdapterFactoryFactoryTests
{
    private sealed class PlainItem
    {
        public int Value { get; set; }
    }

    private sealed class GameItem : IUObject
    {
        private readonly Dictionary<string, object> _properties = new();

        public void setProperty(string key, object value) => _properties[key] = value;

        public object getProperty(string key) => _properties[key];
    }

    [Fact]
    public void GetFactory_CreatesAdapterWithDelegatedProperties()
    {
        var item = new GameItem();
        item.setProperty("Position", 10);
        var factories = new AdapterFactoryFactory();
        var factory = factories.GetFactory(typeof(IPositionAdapter));
        var adapter = (IPositionAdapter)factory(item);

        Assert.Equal(10, adapter.Position);
        adapter.Position = 20;
        Assert.Equal(20, item.getProperty("Position"));
        Assert.Same(factory, factories.GetFactory(typeof(IPositionAdapter)));
    }

    [Fact]
    public void GetFactory_ReturnsTheSameCachedFactory()
    {
        var factories = new AdapterFactoryFactory();

        var first = factories.GetFactory(typeof(IPositionAdapter));
        var second = factories.GetFactory(typeof(IPositionAdapter));

        Assert.Same(first, second);
    }

    [Fact]
    public void RegisteredStrategy_IsSpecificToInterfaceAndProperty()
    {
        var item = new GameItem();
        item.setProperty("Position", 10);
        var factories = new AdapterFactoryFactory();
        factories.RegisterStrategy(typeof(IPositionAdapter), "Position", _ => 42);

        var adapter = (IPositionAdapter)factories.GetFactory(typeof(IPositionAdapter))(item);
        var otherAdapter = (IOtherPositionAdapter)factories.GetFactory(typeof(IOtherPositionAdapter))(item);

        Assert.Equal(42, adapter.Position);
        Assert.Equal(10, otherAdapter.Position);
    }

    [Fact]
    public void GetFactory_RejectsNullAndNonInterfaceTypes()
    {
        var factories = new AdapterFactoryFactory();

        Assert.Throws<ArgumentNullException>(() => factories.GetFactory(null!));
        Assert.Throws<ArgumentException>(() => factories.GetFactory(typeof(PlainItem)));
    }

    [Fact]
    public void RegisterStrategy_RejectsInvalidArguments()
    {
        var factories = new AdapterFactoryFactory();

        Assert.Throws<ArgumentNullException>(() => factories.RegisterStrategy(null!, "Value", _ => 1));
        Assert.Throws<ArgumentException>(() => factories.RegisterStrategy(typeof(IValueAdapter), " ", _ => 1));
        Assert.Throws<ArgumentNullException>(() => factories.RegisterStrategy(typeof(IValueAdapter), "Value", null!));
        Assert.Throws<ArgumentException>(() => factories.RegisterStrategy(typeof(PlainItem), "Value", _ => 1));
    }

    [Fact]
    public void GetFactory_RejectsNullItem()
    {
        var factory = new AdapterFactoryFactory().GetFactory(typeof(IValueAdapter));

        Assert.Throws<ArgumentNullException>(() => factory(null!));
    }

    [Fact]
    public void Adapter_DelegatesPropertiesToPlainObject()
    {
        var item = new PlainItem { Value = 7 };
        var adapter = (IValueAdapter)new AdapterFactoryFactory().GetFactory(typeof(IValueAdapter))(item);

        Assert.Equal(7, adapter.Value);
        adapter.Value = 13;
        Assert.Equal(13, item.Value);
    }

    [Fact]
    public void Adapter_ThrowsWhenPropertyIsMissingFromItem()
    {
        var adapter = (IMissingPropertyAdapter)new AdapterFactoryFactory()
            .GetFactory(typeof(IMissingPropertyAdapter))(new PlainItem());

        Assert.Throws<InvalidOperationException>(() => _ = adapter.Missing);
    }

    [Fact]
    public void Adapter_HandlesObjectMethods()
    {
        var item = new PlainItem { Value = 5 };
        var proxy = new TestRuntimeAdapterProxy();
        proxy.Initialize(item, new Dictionary<string, Func<object?[], object?>>());

        Assert.Equal(item.ToString(), proxy.InvokeForTest(typeof(object).GetMethod(nameof(ToString))));
        Assert.Equal(item.GetHashCode(), proxy.InvokeForTest(typeof(object).GetMethod(nameof(GetHashCode))));
        var equalsMethod = typeof(object).GetMethod(nameof(Equals), new[] { typeof(object) });
        Assert.NotNull(equalsMethod);
        Assert.True((bool)proxy.InvokeForTest(equalsMethod, new object?[] { proxy })!);
        Assert.False((bool)proxy.InvokeForTest(equalsMethod, new object?[] { item })!);
    }

    [Fact]
    public void Adapter_RejectsUnsupportedInterfaceMethods()
    {
        var adapter = (IMethodAdapter)new AdapterFactoryFactory()
            .GetFactory(typeof(IMethodAdapter))(new PlainItem());

        Assert.Throws<NotSupportedException>(adapter.Run);
    }

    [Fact]
    public void Proxy_RejectsMissingMethodInfo()
    {
        var proxy = new TestRuntimeAdapterProxy();
        proxy.Initialize(new PlainItem(), new Dictionary<string, Func<object?[], object?>>());

        Assert.Throws<ArgumentNullException>(() => proxy.InvokeForTest(null));
    }
}

