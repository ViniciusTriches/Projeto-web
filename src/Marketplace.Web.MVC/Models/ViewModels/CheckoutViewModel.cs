using System.ComponentModel.DataAnnotations;
using Marketplace.Web.MVC.Models.ApiContracts.Pagamentos;
using Marketplace.Web.MVC.Models.Carrinho;

namespace Marketplace.Web.MVC.Models.ViewModels;

public class CheckoutViewModel
{
    public CarrinhoSession Carrinho { get; set; } = new();
    public List<TransportadoraDto> Transportadoras { get; set; } = new();

    [Required(ErrorMessage = "CEP obrigatório")]
    [Display(Name = "CEP")]
    public string Cep { get; set; } = string.Empty;

    [Required(ErrorMessage = "Número obrigatório")]
    [Display(Name = "Número")]
    public string Numero { get; set; } = string.Empty;

    public string? Logradouro { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Complemento { get; set; }

    public Guid? TransportadoraId { get; set; }
    public double ValorFrete { get; set; }
    public int PrazoEntrega { get; set; }

    [Required(ErrorMessage = "Método de pagamento obrigatório")]
    public MetodoPagamento MetodoPagamento { get; set; } = MetodoPagamento.Pix;

    public int NumeroParcelas { get; set; } = 1;

    [Required(ErrorMessage = "CPF ou CNPJ obrigatório")]
    [Display(Name = "CPF / CNPJ")]
    public string CpfDocumento { get; set; } = string.Empty;
}

public class ConfirmacaoViewModel
{
    public Guid PedidoId { get; set; }
    public Guid PagamentoId { get; set; }
    public decimal TotalProdutos { get; set; }
    public double ValorFrete { get; set; }
    public double ValorTotal { get; set; }
    public MetodoPagamento MetodoPagamento { get; set; }
    public bool Aprovado { get; set; }
}
