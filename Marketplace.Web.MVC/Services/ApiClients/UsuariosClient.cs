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

    public async Task<UsuarioDto?> CriarUsuarioAsync(CriarUsuarioRequest request)
    {
        try
        {
            var resp = await http.PostAsJsonAsync("/api/usuario", request);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<UsuarioDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar usuario");
            return null;
        }
    }

    public async Task<UsuarioDto?> ObterUsuarioAsync(Guid id, string accessToken)
    {
        try
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var resp = await http.GetAsync($"/api/usuario/{id}");
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
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var resp = await http.PutAsJsonAsync($"/api/usuario/{id}", request);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<UsuarioDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao atualizar usuario {Id}", id);
            return null;
        }
    }
}
