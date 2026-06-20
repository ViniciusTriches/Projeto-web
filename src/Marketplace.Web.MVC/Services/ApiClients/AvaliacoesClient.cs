using System.Net.Http.Headers;
using System.Net.Http.Json;
using Marketplace.Web.MVC.Models.ApiContracts.Avaliacoes;
using Marketplace.Web.MVC.Services.Interfaces;

namespace Marketplace.Web.MVC.Services.ApiClients;

public class AvaliacoesClient(HttpClient http, ILogger<AvaliacoesClient> logger) : IAvaliacoesClient
{
    public async Task<List<ProdutoAvaliacaoDto>> ListarPorProdutoAsync(Guid idProduto)
    {
        try
        {
            return await http.GetFromJsonAsync<List<ProdutoAvaliacaoDto>>($"/api/avaliacao/produto/{idProduto}") ?? new();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao listar avaliacoes do produto {IdProduto}", idProduto);
            return new();
        }
    }

    public async Task<ProdutoAvaliacaoDto?> CriarAsync(CriarAvaliacaoDto dto, string accessToken)
    {
        try
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var resp = await http.PostAsJsonAsync("/api/avaliacao", dto);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<ProdutoAvaliacaoDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar avaliacao do produto {IdProduto}", dto.IdProduto);
            return null;
        }
    }
}
