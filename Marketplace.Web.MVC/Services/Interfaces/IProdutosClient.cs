using Marketplace.Web.MVC.Models.ApiContracts.Produtos;

namespace Marketplace.Web.MVC.Services.Interfaces;

public interface IProdutosClient
{
    Task<List<ProdutoDto>> ListarAsync();
    Task<List<ProdutoDto>> BuscarAsync(string texto);
    Task<ProdutoDto?> ObterPorIdAsync(Guid id);
    Task<ProdutoDto?> ObterPorCodigoAsync(string codigo);
}
