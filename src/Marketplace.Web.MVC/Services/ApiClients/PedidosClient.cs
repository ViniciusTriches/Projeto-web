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
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await http.GetFromJsonAsync<CarrinhoRetornoDto>($"/api/carrinho/{usuarioId}");
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
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var resp = await http.PostAsJsonAsync("/api/carrinho/AtualizarStatusPedido", dto);
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
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var resp = await http.DeleteAsync($"/api/carrinho/LimparCarrinho/{usuarioId}");
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
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var resp = await http.DeleteAsync($"/api/carrinho/DeletePedido/{pedidoId}");
            return resp.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao deletar pedido {PedidoId}", pedidoId);
            return false;
        }
    }
}
