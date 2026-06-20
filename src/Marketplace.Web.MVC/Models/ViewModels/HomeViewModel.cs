using Marketplace.Web.MVC.Models.ApiContracts.Categorias;
using Marketplace.Web.MVC.Models.ApiContracts.Produtos;

namespace Marketplace.Web.MVC.Models.ViewModels;

public class HomeViewModel
{
    public List<ProdutoDto> Produtos { get; set; } = new();
    public List<CategoriaDto> Categorias { get; set; } = new();
    public string? TermoBusca { get; set; }
    public bool ApiIndisponivel { get; set; }
}
