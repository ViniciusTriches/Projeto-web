namespace Marketplace.Web.MVC.Models.ApiContracts.Produtos;

public class ApiResponseWrapper<T>
{
    public bool Sucesso { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
}

public class ProdutoDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int QuantidadeEstoque { get; set; }
    public int EstoqueMinimoVenda { get; set; }
    public Guid IdCategoria { get; set; }
    public Guid IdImagemPrincipal { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool Destaque { get; set; }
    public bool Disponivel { get; set; }
    public bool Excluido { get; set; }
}
