using System.Net.Http.Headers;
using System.Net.Http.Json;
using Marketplace.Web.MVC.Models.ApiContracts.Pagamentos;
using Marketplace.Web.MVC.Services.Interfaces;

namespace Marketplace.Web.MVC.Services.ApiClients;

public class PagamentosClient(HttpClient http, ILogger<PagamentosClient> logger) : IPagamentosClient
{
    public async Task<List<TransportadoraDto>> ListarTransportadorasAsync()
    {
        try
        {
            return await http.GetFromJsonAsync<List<TransportadoraDto>>("/api/Transportadora") ?? new();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao listar transportadoras");
            return new();
        }
    }

    public async Task<object?> CalcularFreteAsync(FreteCalculoDto dto, string accessToken)
    {
        try
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var resp = await http.PostAsJsonAsync("/api/Frete/calcular", dto);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<object>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao calcular frete");
            return null;
        }
    }

    public async Task<PagamentoDto?> CriarPagamentoAsync(CriarPagamentoDto dto, string accessToken)
    {
        try
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var resp = await http.PostAsJsonAsync("/api/Pagamento", dto);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagamentoDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar pagamento");
            return null;
        }
    }

    public async Task<TransacaoPagamentoDto?> ProcessarTransacaoAsync(Guid pagamentoId, string accessToken)
    {
        try
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var body = new { PagamentoId = pagamentoId };
            var resp = await http.PostAsJsonAsync("/api/Pagamento/transacao", body);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<TransacaoPagamentoDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao processar transacao do pagamento {PagamentoId}", pagamentoId);
            return null;
        }
    }
}
