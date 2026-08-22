using DbUp;
using DbUp.Engine.Output;

namespace TuttiWallet.Migrator;

internal static class GarantirBancoDeDadosExtensions
{
    public static bool GarantirBancoDeDados(this string connectionString)
    {
        var log = new RegistradorDeCriacaoDeBanco();

        EnsureDatabase.For.PostgresqlDatabase(connectionString, log);

        return log.BancoFoiCriado;
    }

    private sealed class RegistradorDeCriacaoDeBanco : IUpgradeLog
    {
        private readonly IUpgradeLog log = new ConsoleUpgradeLog();

        public bool BancoFoiCriado { get; private set; }

        public void LogTrace(string format, params object[] args) => log.LogTrace(format, args);

        public void LogDebug(string format, params object[] args) => log.LogDebug(format, args);

        public void LogInformation(string format, params object[] args)
        {
            BancoFoiCriado = true;
            log.LogInformation(format, args);
        }

        public void LogWarning(string format, params object[] args) => log.LogWarning(format, args);

        public void LogError(string format, params object[] args) => log.LogError(format, args);

        public void LogError(Exception ex, string format, params object[] args) => log.LogError(ex, format, args);
    }
}
