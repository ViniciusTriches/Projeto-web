using Marketplace.Web.MVC.Models.ApiContracts.Pedidos;

namespace Marketplace.Web.MVC.Services.Interfaces;

public interface IPedidosClient
{
    Task<CarrinhoRetornoDto?> ObterCarrinhoAsync(Guid usuarioId, string accessToken);
    Task<bool> AtualizarStatusPedidoAsync(AtualizarStatusPedidoDto dto, string accessToken);
    Task<bool> LimparCarrinhoAsync(Guid usuarioId, string accessToken);
    Task<bool> DeletarPedidoAsync(Guid pedidoId, string accessToken);
}
