using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TuttiWallet.Application.Autenticacao;
using TuttiWallet.Application.Usuarios;
using TuttiWallet.Contracts.Autenticacao;
using TuttiWallet.Contracts.Categorias;
using TuttiWallet.Contracts.Usuarios;
using TuttiWallet.Domain;

namespace TuttiWallet.Api.IntegrationTests;

public class ExcluirCategoriaTests(ApiWebApplicationFactory factory)
    : IClassFixture<ApiWebApplicationFactory>, IAsyncLifetime
{
    private const string Email = "maria@exemplo.com";
    private const string Senha = "abcX24";

    public async Task InitializeAsync()
    {
        await using var conexao = new NpgsqlConnection(factory.ConnectionString);
        await conexao.OpenAsync();
        await conexao.ExecuteAsync("TRUNCATE TABLE Usuarios CASCADE;");

        var client = factory.CreateClient();
        await client.PostAsJsonAsync("/api/usuarios/primeiro-acesso", new CadastrarPrimeiroUsuarioRequest
        {
            Nome = "Maria",
            Sobrenome = "Silva",
            Email = Email,
            Celular = "(11) 98765-4321",
            Senha = Senha,
            ConfirmacaoSenha = Senha
        });
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ExcluirCategoriaSemTokenRetornaUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.DeleteAsync($"/api/categorias/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ExcluirCategoriaComSucessoRetornaNoContentENaoAApareceMaisNaListagem()
    {
        var client = await CriarClienteAutenticadoAsync();
        var categoriaId = await CriarCategoriaAsync(client, "Salário", tipoId: 1);

        var response = await client.DeleteAsync($"/api/categorias/{categoriaId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var consulta = await client.GetAsync($"/api/categorias/{categoriaId}");
        consulta.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ExcluirCategoriaInexistenteRetornaNotFound()
    {
        var client = await CriarClienteAutenticadoAsync();

        var response = await client.DeleteAsync($"/api/categorias/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ExcluirCategoriaDeOutroUsuarioRetornaNotFound()
    {
        var clienteMaria = await CriarClienteAutenticadoAsync();
        var categoriaDaMariaId = await CriarCategoriaAsync(clienteMaria, "Salário", tipoId: 1);

        // A API só permite criar o primeiro usuário do sistema via endpoint público; o cadastro de um
        // segundo usuário para este teste de isolamento é feito diretamente pelos serviços internos.
        var clienteJoao = await CriarClienteComSegundoUsuarioAutenticadoAsync();
        var response = await clienteJoao.DeleteAsync($"/api/categorias/{categoriaDaMariaId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ExcluirCategoriaPrincipalComSubcategoriaRetornaBadRequest()
    {
        var client = await CriarClienteAutenticadoAsync();
        var categoriaPaiId = await CriarCategoriaAsync(client, "Alimentação", tipoId: 2);
        await CriarCategoriaAsync(client, "Restaurante", tipoId: 2, categoriaPaiId: categoriaPaiId);

        var response = await client.DeleteAsync($"/api/categorias/{categoriaPaiId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ExcluirCategoriaComTransacaoVinculadaRetornaBadRequest()
    {
        var client = await CriarClienteAutenticadoAsync();
        var categoriaId = await CriarCategoriaAsync(client, "Restaurante", tipoId: 2);

        await InserirTransacaoAsync(client, categoriaId);

        var response = await client.DeleteAsync($"/api/categorias/{categoriaId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ExcluirSubcategoriaSemTransacoesRetornaNoContent()
    {
        var client = await CriarClienteAutenticadoAsync();
        var categoriaPaiId = await CriarCategoriaAsync(client, "Alimentação", tipoId: 2);
        var subcategoriaId = await CriarCategoriaAsync(client, "Restaurante", tipoId: 2, categoriaPaiId: categoriaPaiId);

        var response = await client.DeleteAsync($"/api/categorias/{subcategoriaId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task InserirTransacaoAsync(HttpClient client, Guid categoriaId)
    {
        using var escopo = factory.Services.CreateScope();
        var usuarioRepository = escopo.ServiceProvider.GetRequiredService<IUsuarioRepository>();
        var usuario = await usuarioRepository.ObterPorEmailAsync(Email);

        await using var conexao = new NpgsqlConnection(factory.ConnectionString);
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(
            """
            INSERT INTO Transacoes (Id, UsuarioId, CategoriaId, Tipo, Valor, DataOcorrencia)
            VALUES (@Id, @UsuarioId, @CategoriaId, 'despesa', 50, CURRENT_DATE);
            """,
            new { Id = Guid.NewGuid(), UsuarioId = usuario!.Id, CategoriaId = categoriaId });
    }

    private async Task<HttpClient> CriarClienteComSegundoUsuarioAutenticadoAsync()
    {
        using var escopo = factory.Services.CreateScope();
        var usuarioRepository = escopo.ServiceProvider.GetRequiredService<IUsuarioRepository>();
        var senhaHasher = escopo.ServiceProvider.GetRequiredService<ISenhaHasher>();
        var geradorToken = escopo.ServiceProvider.GetRequiredService<IGeradorToken>();

        var usuario = new Usuario(
            Guid.NewGuid(), "João", "Souza", "joao@exemplo.com", "11987654321", senhaHasher.GerarHash(Senha));
        await usuarioRepository.InserirAsync(usuario);

        var (token, _) = geradorToken.GerarTokenAcesso(usuario);

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    private async Task<HttpClient> CriarClienteAutenticadoAsync()
    {
        var client = factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync(
            "/api/autenticacao/login",
            new LoginRequest { Email = Email, Senha = Senha });
        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.AccessToken);

        return client;
    }

    private static async Task<Guid> CriarCategoriaAsync(HttpClient client, string nome, int tipoId, Guid? categoriaPaiId = null)
    {
        var response = await client.PostAsJsonAsync(
            "/api/categorias",
            new CriarCategoriaRequest { Nome = nome, TipoId = tipoId, CategoriaPaiId = categoriaPaiId });

        var conteudo = await response.Content.ReadFromJsonAsync<CategoriaResponse>();

        return conteudo!.Id;
    }
}
