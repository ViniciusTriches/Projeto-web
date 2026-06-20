using System.Net.Http.Json;
using Marketplace.Web.MVC.Models.ApiContracts.Produtos;
using Marketplace.Web.MVC.Services.Interfaces;

namespace Marketplace.Web.MVC.Services.ApiClients;

public class ProdutosClient(HttpClient http, ILogger<ProdutosClient> logger) : IProdutosClient
{
    public async Task<List<ProdutoDto>> ListarAsync()
    {
        try
        {
            var resp = await http.GetFromJsonAsync<ApiResponseWrapper<List<ProdutoDto>>>("/api/produto/listar");
            return resp?.Sucesso == true ? resp.Data ?? new() : new();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao listar produtos");
            return new();
        }
    }

    public async Task<List<ProdutoDto>> BuscarAsync(string texto)
    {
        try
        {
            var resp = await http.GetFromJsonAsync<ApiResponseWrapper<List<ProdutoDto>>>($"/api/produto/buscar/{Uri.EscapeDataString(texto)}");
            return resp?.Sucesso == true ? resp.Data ?? new() : new();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao buscar produtos com texto {Texto}", texto);
            return new();
        }
    }

    public async Task<ProdutoDto?> ObterPorIdAsync(Guid id)
    {
        try
        {
            var resp = await http.GetFromJsonAsync<ApiResponseWrapper<ProdutoDto>>($"/api/produto/obtemPorId/{id}");
            return resp?.Sucesso == true ? resp.Data : null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter produto {Id}", id);
            return null;
        }
    }

    public async Task<ProdutoDto?> ObterPorCodigoAsync(string codigo)
    {
        try
        {
            var resp = await http.GetFromJsonAsync<ApiResponseWrapper<ProdutoDto>>($"/api/produto/obtem/{Uri.EscapeDataString(codigo)}");
            return resp?.Sucesso == true ? resp.Data : null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter produto por codigo {Codigo}", codigo);
            return null;
        }
    }
}
