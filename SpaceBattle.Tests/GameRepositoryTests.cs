using App;
using App.Scopes;

namespace SpaceBattle.Lib.Tests
{
    public class GameRepositoryTests : IDisposable
    {
        public GameRepositoryTests()
        {
            new InitCommand().Execute();
            var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
        }

        [Fact]
        public void Add_Get_Item()
        {
            var registerCmd = new RegisterIocDependencyGameRepository();
            var gameItem = new object();
            var id = "67";

            registerCmd.Execute();

            var cmdAdd = Ioc.Resolve<ICommand>("Game.Item.Add", id, gameItem);
            cmdAdd.Execute();
            var gameItemResult = Ioc.Resolve<object>("Game.Item.Get", id);

            Assert.Equal(gameItem, gameItemResult);
        }

        [Fact]
        public void Add_Remove_Item()
        {
            var registerCmd = new RegisterIocDependencyGameRepository();
            var gameItem = new object();
            var id = "42";

            registerCmd.Execute();

            var cmdAdd = Ioc.Resolve<ICommand>("Game.Item.Add", id, gameItem);
            cmdAdd.Execute();
            var cmdRemove = Ioc.Resolve<ICommand>("Game.Item.Remove", id);
            cmdRemove.Execute();

            Assert.Throws<KeyNotFoundException>(() => Ioc.Resolve<object>("Game.Item.Get", id));
        }

        public void Dispose()
        {
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
        }
    }
}