using Marketplace.Web.MVC.Models.ApiContracts.Avaliacoes;
using Marketplace.Web.MVC.Models.ApiContracts.Pagamentos;
using Marketplace.Web.MVC.Models.ApiContracts.Pedidos;
using Marketplace.Web.MVC.Models.ApiContracts.Usuarios;
using Marketplace.Web.MVC.Models.ViewModels;

namespace Marketplace.Web.MVC.Services.Facades;

public interface IMarketplaceFacade
{
    Task<HomeViewModel> ObterCatalogoAsync(string? busca, int? categoriaId);
    Task<ProdutoDetalheViewModel?> ObterDetalheProdutoAsync(Guid produtoId);

    Task<(LoginResultado? Resultado, string? Erro)> AutenticarAsync(string email, string senha);
    Task<(UsuarioDto? Usuario, string? Erro)> RegistrarUsuarioAsync(CriarUsuarioRequest request);
    Task<UsuarioDto?> ObterPerfilAsync(Guid usuarioId, string accessToken);
    Task<UsuarioDto?> AtualizarPerfilAsync(Guid usuarioId, AtualizarUsuarioRequest request, string accessToken);

    Task<(ProdutoAvaliacaoDto? Avaliacao, string? Erro)> AvaliarProdutoAsync(CriarAvaliacaoDto dto, string accessToken);

    Task<List<TransportadoraDto>> ObterTransportadorasAsync();
    Task<object?> CalcularFreteAsync(FreteCalculoDto dto, string accessToken);
    Task<ResultadoCheckout> FinalizarCompraAsync(FinalizarCompraRequest request);

    Task<List<PedidoRetornoDto>> ObterPedidosDoUsuarioAsync(Guid usuarioId);

    Task<PainelEstatisticasViewModel> ObterPainelEstatisticasAsync();
}
