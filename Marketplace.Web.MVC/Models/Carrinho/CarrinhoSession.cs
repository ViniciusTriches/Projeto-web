namespace Marketplace.Web.MVC.Models.Carrinho;

public class CarrinhoItem
{
    public Guid ProdutoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Quantidade { get; set; }
    public decimal Subtotal => Preco * Quantidade;
}

public class CarrinhoSession
{
    public List<CarrinhoItem> Itens { get; set; } = new();
    public decimal Total => Itens.Sum(i => i.Subtotal);
    public int TotalItens => Itens.Sum(i => i.Quantidade);
}
