using App;
using App.Scopes;
using SpaceBattle.Lib;

var root = FindProjectRoot();
var rulesPath = Path.Combine(root, "SpaceBattle.Lib", "GameCore", "game-rules.txt");
var logPath = Path.Combine(root, "game-log.txt");
var parser = new TextRuleParser(new IRuleFactory[] { new BuiltInRuleFactory() });
var rules = parser.ParseFile(rulesPath);

Console.WriteLine("Загруженные правила:");
foreach (var rule in rules)
    Console.WriteLine($"  {rule.Name}={rule.Value}");

new App.Scopes.InitCommand().Execute();
var scope = Ioc.Resolve<object>("IoC.Scope.Create");
Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", scope).Execute();
var queue = new Queue<SpaceBattle.Lib.ICommand>();
queue.Enqueue(new DemoGameCommand("Подготовка игры"));
queue.Enqueue(new DemoGameCommand("Ход игрока 1"));
queue.Enqueue(new DemoGameCommand("Ход игрока 2"));
Ioc.Resolve<App.ICommand>("IoC.Register", "Game.TimeQuant", (object[] _) => (object)TimeSpan.FromSeconds(5)).Execute();
Ioc.Resolve<App.ICommand>("IoC.Register", "Game.CommandsQueue", (object[] _) => (object)queue).Execute();
var logger = new FileLogger(logPath);
Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Logger", (object[] _) => (object)logger).Execute();
Ioc.Resolve<App.ICommand>("IoC.Register", "ExceptionHandler.Handle", (object[] _) => (object)new NullCommand()).Execute();


Console.WriteLine("\nЗапуск демонстрационной игры...");
new Game(scope).Execute();
Console.WriteLine($"Игра завершена. Лог: {logPath}");
logger.Dispose();
Console.WriteLine("\n--- Лог игры ---");
Console.Write(File.ReadAllText(logPath));
Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();

static string FindProjectRoot()
{
    var directory = new DirectoryInfo(AppContext.BaseDirectory);
    while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Project.sln")))
        directory = directory.Parent;
    return directory?.FullName ?? Directory.GetCurrentDirectory();
}

sealed class DemoGameCommand(string name) : SpaceBattle.Lib.IDescribedCommand
{
    public string Description => name;
    public void Execute() => Console.WriteLine($"Выполнена команда: {name}");
}

