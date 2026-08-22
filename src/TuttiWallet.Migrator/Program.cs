using System.Reflection;
using DbUp;
using TuttiWallet.Migrator;

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? throw new InvalidOperationException("A variável de ambiente CONNECTION_STRING não foi definida.");

var bancoFoiCriado = connectionString.GarantirBancoDeDados();

if (!bancoFoiCriado)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Banco de dados já existia.");
    Console.ResetColor();
}

var upgrader =
    DeployChanges.To
        .PostgresqlDatabase(connectionString)
        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
        .LogToConsole()
        .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();
    return 1;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Migração aplicada com sucesso.");
Console.ResetColor();
return 0;
