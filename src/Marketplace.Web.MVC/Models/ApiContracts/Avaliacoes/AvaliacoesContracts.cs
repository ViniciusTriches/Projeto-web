namespace Marketplace.Web.MVC.Models.ApiContracts.Avaliacoes;

public class ProdutoAvaliacaoDto
{
    public Guid Id { get; set; }
    public Guid IdCliente { get; set; }
    public Guid IdProduto { get; set; }
    public int Nota { get; set; }
    public string? Descricao { get; set; }
}

public class CriarAvaliacaoDto
{
    public Guid IdCliente { get; set; }
    public Guid IdProduto { get; set; }
    public int Nota { get; set; }
    public string? Descricao { get; set; }
}
