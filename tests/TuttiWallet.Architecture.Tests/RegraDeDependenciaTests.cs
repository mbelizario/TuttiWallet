using System.Xml.Linq;
using FluentAssertions;

namespace TuttiWallet.Architecture.Tests;

public class RegraDeDependenciaTests
{
    private static readonly IReadOnlyDictionary<string, string[]> DependenciasPermitidas = new Dictionary<string, string[]>
    {
        ["TuttiWallet.Domain"] = [],
        ["TuttiWallet.Application"] = ["TuttiWallet.Domain"],
        ["TuttiWallet.Infrastructure"] = ["TuttiWallet.Application", "TuttiWallet.Domain"],
        ["TuttiWallet.Contracts"] = [],
        ["TuttiWallet.Migrator"] = [],
        ["TuttiWallet.Api"] = ["TuttiWallet.Application", "TuttiWallet.Infrastructure", "TuttiWallet.Contracts"],
        ["TuttiWallet.Web"] = ["TuttiWallet.Contracts"],
    };

    public static TheoryData<string> Projetos => new(DependenciasPermitidas.Keys);

    [Theory]
    [MemberData(nameof(Projetos))]
    public void ProjetoReferenciaApenasProjetosPermitidos(string projeto)
    {
        var referencias = ObterProjetosReferenciados(projeto);

        referencias.Should().BeSubsetOf(DependenciasPermitidas[projeto],
            $"{projeto} só pode referenciar: {DescreverPermitidos(projeto)} (ver regra de dependência no AGENTS.md)");
    }

    [Fact]
    public void TodoProjetoDeSrcTemRegraDeDependenciaDeclarada()
    {
        var projetosEmSrc = Directory
            .GetDirectories(Path.Combine(ObterRaizDoRepositorio(), "src"))
            .Select(Path.GetFileName);

        projetosEmSrc.Should().BeEquivalentTo(DependenciasPermitidas.Keys,
            "um projeto novo em src/ precisa ter suas dependências permitidas declaradas neste teste");
    }

    private static string DescreverPermitidos(string projeto)
    {
        var permitidos = DependenciasPermitidas[projeto];
        return permitidos.Length == 0 ? "nenhum projeto do repositório" : string.Join(", ", permitidos);
    }

    private static string[] ObterProjetosReferenciados(string projeto)
    {
        var caminhoCsproj = Path.Combine(ObterRaizDoRepositorio(), "src", projeto, $"{projeto}.csproj");

        return XDocument.Load(caminhoCsproj)
            .Descendants("ProjectReference")
            .Select(referencia => referencia.Attribute("Include")!.Value)
            .Select(caminho => Path.GetFileNameWithoutExtension(caminho.Replace('\\', '/')))
            .ToArray();
    }

    private static string ObterRaizDoRepositorio()
    {
        var diretorio = new DirectoryInfo(AppContext.BaseDirectory);
        while (diretorio is not null && !File.Exists(Path.Combine(diretorio.FullName, "TuttiWallet.slnx")))
            diretorio = diretorio.Parent;

        return diretorio?.FullName
            ?? throw new InvalidOperationException("Raiz do repositório (TuttiWallet.slnx) não encontrada.");
    }
}
