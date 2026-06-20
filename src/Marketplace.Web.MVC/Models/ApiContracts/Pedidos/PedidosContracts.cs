namespace Marketplace.Web.MVC.Models.ApiContracts.Pedidos;

public class CarrinhoRetornoDto
{
    public Guid UsuarioId { get; set; }
    public List<PedidoRetornoDto> PedidosModel { get; set; } = new();
    public decimal ValorTotalCarrinho { get; set; }
}

public class PedidoRetornoDto
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public List<ProdutoPedidoDto> ProdutosModel { get; set; } = new();
    public DateTime DataPedido { get; set; }
    public int StatusPedido { get; set; }
    public string TextoStatusPedido { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string CEPEnderecoEntrega { get; set; } = string.Empty;
    public string NumeroEnderecoEntrega { get; set; } = string.Empty;
}

public class ProdutoPedidoDto
{
    public Guid Id { get; set; }
    public Guid PedidoId { get; set; }
    public Guid ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal Preco { get; set; }
    public bool Disponivel { get; set; }
    public bool Excluido { get; set; }
}

public class AtualizarStatusPedidoDto
{
    public Guid PedidoId { get; set; }
    public int StatusPedido { get; set; }
}
