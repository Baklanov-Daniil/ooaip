using App;
using System.Collections.Concurrent;

namespace SpaceBattle.Lib
{
    public class RegisterIocDependencyGameRepository : ICommand
    {
        public void Execute()
        {
            var gameItems = new ConcurrentDictionary<string, object>();

            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Item.Get", (object[] args) =>
            {
                var key = (string)args[0];
                
                if (gameItems.TryGetValue(key, out var item))
                {
                    return item;
                }
                
                throw new KeyNotFoundException($"Game item '{key}' not found.");
            }).Execute();

            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Item.Add", (object[] args) =>
            {
                var key = (string)args[0];
                var value = args[1];
                
                return new DelegateCommand(() =>
                {
                    gameItems.TryAdd(key, value);
                });
            }).Execute();

            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Item.Remove", (object[] args) =>
            {
                var key = (string)args[0];
                
                return new DelegateCommand(() =>
                {
                    gameItems.TryRemove(key, out _);
                });
            }).Execute();
        }
    }
}
