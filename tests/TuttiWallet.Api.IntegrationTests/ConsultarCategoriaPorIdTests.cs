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

public class ConsultarCategoriaPorIdTests(ApiWebApplicationFactory factory)
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
        await CadastrarUsuarioAsync(client, "Maria", Email);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ConsultarCategoriaSemTokenRetornaUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/categorias/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ConsultarCategoriaPrincipalSemSubcategoriasRetornaOkComListaVazia()
    {
        var client = await CriarClienteAutenticadoAsync(Email, Senha);
        var categoriaId = await CriarCategoriaAsync(client, "Salário", tipoId: 1);

        var response = await client.GetAsync($"/api/categorias/{categoriaId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var conteudo = await response.Content.ReadFromJsonAsync<CategoriaDetalheResponse>();
        conteudo!.Id.Should().Be(categoriaId);
        conteudo.Nome.Should().Be("Salário");
        conteudo.TipoId.Should().Be(1);
        conteudo.CategoriaPaiId.Should().BeNull();
        conteudo.Subcategorias.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task ConsultarCategoriaPrincipalComSubcategoriasRetornaOkComListaPreenchida()
    {
        var client = await CriarClienteAutenticadoAsync(Email, Senha);
        var categoriaPrincipalId = await CriarCategoriaAsync(client, "Alimentação", tipoId: 2);
        var subcategoriaId = await CriarCategoriaAsync(client, "Restaurante", tipoId: 2, categoriaPaiId: categoriaPrincipalId);

        var response = await client.GetAsync($"/api/categorias/{categoriaPrincipalId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var conteudo = await response.Content.ReadFromJsonAsync<CategoriaDetalheResponse>();
        conteudo!.Subcategorias.Should().ContainSingle(s => s.Id == subcategoriaId && s.Nome == "Restaurante");
    }

    [Fact]
    public async Task ConsultarSubcategoriaRetornaOkComSubcategoriasNula()
    {
        var client = await CriarClienteAutenticadoAsync(Email, Senha);
        var categoriaPrincipalId = await CriarCategoriaAsync(client, "Alimentação", tipoId: 2);
        var subcategoriaId = await CriarCategoriaAsync(client, "Restaurante", tipoId: 2, categoriaPaiId: categoriaPrincipalId);

        var response = await client.GetAsync($"/api/categorias/{subcategoriaId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var conteudo = await response.Content.ReadFromJsonAsync<CategoriaDetalheResponse>();
        conteudo!.CategoriaPaiId.Should().Be(categoriaPrincipalId);
        conteudo.Subcategorias.Should().BeNull();
    }

    [Fact]
    public async Task ConsultarCategoriaInexistenteRetornaNotFound()
    {
        var client = await CriarClienteAutenticadoAsync(Email, Senha);

        var response = await client.GetAsync($"/api/categorias/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ConsultarCategoriaDeOutroUsuarioRetornaNotFound()
    {
        var clienteMaria = await CriarClienteAutenticadoAsync(Email, Senha);
        var categoriaDaMariaId = await CriarCategoriaAsync(clienteMaria, "Salário", tipoId: 1);

        // A API só permite criar o primeiro usuário do sistema via endpoint público; o cadastro de um
        // segundo usuário para este teste de isolamento é feito diretamente pelos serviços internos.
        var clienteJoao = await CriarClienteComSegundoUsuarioAutenticadoAsync();
        var response = await clienteJoao.GetAsync($"/api/categorias/{categoriaDaMariaId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task CadastrarUsuarioAsync(HttpClient client, string nome, string email) =>
        await client.PostAsJsonAsync("/api/usuarios/primeiro-acesso", new CadastrarPrimeiroUsuarioRequest
        {
            Nome = nome,
            Sobrenome = "Silva",
            Email = email,
            Celular = "(11) 98765-4321",
            Senha = Senha,
            ConfirmacaoSenha = Senha
        });

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

    private async Task<HttpClient> CriarClienteAutenticadoAsync(string email, string senha)
    {
        var client = factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync(
            "/api/autenticacao/login",
            new LoginRequest { Email = email, Senha = senha });
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
