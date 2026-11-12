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

public class AdapterFactoryFactoryTests
{
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
}

