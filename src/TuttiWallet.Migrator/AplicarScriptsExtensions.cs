using System.Reflection;
using DbUp;
using DbUp.Engine;

namespace TuttiWallet.Migrator;

public static class AplicarScriptsExtensions
{
    public static DatabaseUpgradeResult AplicarScripts(this string connectionString) =>
        DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build()
            .PerformUpgrade();
}
