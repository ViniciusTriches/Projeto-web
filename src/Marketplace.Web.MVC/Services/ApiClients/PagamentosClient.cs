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
            logger.LogWarning(ex, "Não foi possível carregar transportadoras");
            return new();
        }
    }

    public async Task<object?> CalcularFreteAsync(FreteCalculoDto dto, string accessToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Frete/calcular");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = JsonContent.Create(dto);
            var resp = await http.SendAsync(request);
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
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Pagamento");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = JsonContent.Create(dto);
            var resp = await http.SendAsync(request);
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
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Pagamento/transacao");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = JsonContent.Create(new { PagamentoId = pagamentoId });
            var resp = await http.SendAsync(request);
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
