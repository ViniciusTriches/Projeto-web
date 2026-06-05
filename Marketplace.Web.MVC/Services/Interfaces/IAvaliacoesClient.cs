using Marketplace.Web.MVC.Models.ApiContracts.Avaliacoes;

namespace Marketplace.Web.MVC.Services.Interfaces;

public interface IAvaliacoesClient
{
    Task<List<ProdutoAvaliacaoDto>> ListarPorProdutoAsync(Guid idProduto);
    Task<ProdutoAvaliacaoDto?> CriarAsync(CriarAvaliacaoDto dto, string accessToken);
}
