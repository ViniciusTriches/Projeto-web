using Marketplace.Web.MVC.Models.ApiContracts.Avaliacoes;
using Marketplace.Web.MVC.Models.ApiContracts.Produtos;

namespace Marketplace.Web.MVC.Models.ViewModels;

public class ProdutoDetalheViewModel
{
    public ProdutoDto Produto { get; set; } = null!;
    public List<ProdutoAvaliacaoDto> Avaliacoes { get; set; } = new();
    public double MediaNota => Avaliacoes.Count > 0 ? Avaliacoes.Average(a => a.Nota) : 0;
    public NovaAvaliacaoViewModel NovaAvaliacao { get; set; } = new();
}

public class NovaAvaliacaoViewModel
{
    public Guid IdProduto { get; set; }
    public int Nota { get; set; } = 5;
    public string? Descricao { get; set; }
}
