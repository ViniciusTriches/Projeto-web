using System.Net.Http.Headers;
using System.Net.Http.Json;
using Marketplace.Web.MVC.Models.ApiContracts.Usuarios;
using Marketplace.Web.MVC.Services.Interfaces;

namespace Marketplace.Web.MVC.Services.ApiClients;

public class UsuariosClient(HttpClient http, ILogger<UsuariosClient> logger) : IUsuariosClient
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var resp = await http.PostAsJsonAsync("/api/autenticacao/login", request);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<LoginResponse>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao fazer login");
            return null;
        }
    }

    public async Task<(UsuarioDto? Usuario, string? Erro)> CriarUsuarioAsync(CriarUsuarioRequest request)
    {
        try
        {
            var resp = await http.PostAsJsonAsync("/api/usuario", request);
            if (!resp.IsSuccessStatusCode)
            {
                var erro = await resp.Content.ReadAsStringAsync();
                logger.LogWarning("Erro ao criar usuario: {Erro}", erro);
                return (null, erro);
            }
            var usuario = await resp.Content.ReadFromJsonAsync<UsuarioDto>();
            return (usuario, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar usuario");
            return (null, "Erro de conexão com o servidor.");
        }
    }

    public async Task<UsuarioDto?> ObterUsuarioAsync(Guid id, string accessToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/usuario/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var resp = await http.SendAsync(request);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<UsuarioDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter usuario {Id}", id);
            return null;
        }
    }

    public async Task<UsuarioDto?> AtualizarUsuarioAsync(Guid id, AtualizarUsuarioRequest request, string accessToken)
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Put, $"/api/usuario/{id}");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            req.Content = JsonContent.Create(request);
            var resp = await http.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<UsuarioDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao atualizar usuario {Id}", id);
            return null;
        }
    }

    public async Task EsqueciSenhaAsync(string email)
    {
        var payload = new EsqueciSenhaRequest(email);
        var payloadJson = System.Text.Json.JsonSerializer.Serialize(payload);
        logger.LogInformation("EsqueciSenha: email='{Email}' payload={Payload}", email, payloadJson);
        try
        {
            var resp = await http.PostAsJsonAsync("/api/autenticacao/esqueci-senha", payload);
            if (!resp.IsSuccessStatusCode)
            {
                var corpo = await resp.Content.ReadAsStringAsync();
                logger.LogWarning("EsqueciSenha: API retornou {Status} - {Corpo}", (int)resp.StatusCode, corpo);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Erro ao solicitar reset de senha para {Email}", email);
        }
    }

    public async Task<(bool Sucesso, string? Erro)> ResetarSenhaAsync(string email, string token, string novaSenha)
    {
        try
        {
            var resp = await http.PostAsJsonAsync("/api/autenticacao/resetar-senha", new ResetarSenhaRequest(email, token, novaSenha));
            if (!resp.IsSuccessStatusCode)
            {
                var erro = await resp.Content.ReadAsStringAsync();
                return (false, string.IsNullOrWhiteSpace(erro) ? "Link inválido ou expirado." : erro);
            }
            return (true, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao resetar senha");
            return (false, "Erro de conexão ao tentar redefinir a senha.");
        }
    }
}
