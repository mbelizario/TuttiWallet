using Serilog;
using Serilog.Events;

namespace TuttiWallet.Api.Logging;

public static class SerilogExtensions
{
    public static void UseSerilogConfigurado(this WebApplicationBuilder builder)
    {
        var seqUrl = builder.Configuration["Seq:Url"];

        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Aplicacao", "TuttiWallet.Api")
                .WriteTo.Console();

            if (!string.IsNullOrWhiteSpace(seqUrl))
                configuration.WriteTo.Seq(seqUrl);
        });
    }
}
