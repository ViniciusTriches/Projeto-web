using System.Net.Http.Json;
using Marketplace.Web.MVC.Models.ApiContracts.Categorias;
using Marketplace.Web.MVC.Services.Interfaces;

namespace Marketplace.Web.MVC.Services.ApiClients;

public class CategoriasClient(HttpClient http, ILogger<CategoriasClient> logger) : ICategoriasClient
{
    public async Task<List<CategoriaDto>> ListarAsync()
    {
        try
        {
            return await http.GetFromJsonAsync<List<CategoriaDto>>("/api/categoria") ?? new();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao listar categorias");
            return new();
        }
    }

    public async Task<CategoriaDto?> ObterAsync(int id)
    {
        try
        {
            return await http.GetFromJsonAsync<CategoriaDto>($"/api/categoria/{id}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter categoria {Id}", id);
            return null;
        }
    }
}
