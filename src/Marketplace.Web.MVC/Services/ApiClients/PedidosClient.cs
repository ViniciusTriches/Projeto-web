using System.Net.Http.Headers;
using System.Net.Http.Json;
using Marketplace.Web.MVC.Models.ApiContracts.Pedidos;
using Marketplace.Web.MVC.Services.Interfaces;

namespace Marketplace.Web.MVC.Services.ApiClients;

public class PedidosClient(HttpClient http, ILogger<PedidosClient> logger) : IPedidosClient
{
    public async Task<CarrinhoRetornoDto?> ObterCarrinhoAsync(Guid usuarioId, string accessToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/carrinho/{usuarioId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var resp = await http.SendAsync(request);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<CarrinhoRetornoDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter carrinho do usuario {UsuarioId}", usuarioId);
            return null;
        }
    }

    public async Task<bool> AtualizarStatusPedidoAsync(AtualizarStatusPedidoDto dto, string accessToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/carrinho/AtualizarStatusPedido");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = JsonContent.Create(dto);
            var resp = await http.SendAsync(request);
            return resp.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao atualizar status pedido {PedidoId}", dto.PedidoId);
            return false;
        }
    }

    public async Task<bool> LimparCarrinhoAsync(Guid usuarioId, string accessToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/carrinho/LimparCarrinho/{usuarioId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var resp = await http.SendAsync(request);
            return resp.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao limpar carrinho do usuario {UsuarioId}", usuarioId);
            return false;
        }
    }

    public async Task<bool> DeletarPedidoAsync(Guid pedidoId, string accessToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/carrinho/DeletePedido/{pedidoId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var resp = await http.SendAsync(request);
            return resp.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao deletar pedido {PedidoId}", pedidoId);
            return false;
        }
    }
}
