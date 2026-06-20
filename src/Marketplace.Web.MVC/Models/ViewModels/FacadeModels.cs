using System.Security.Claims;
using Marketplace.Web.MVC.Models.ApiContracts.Pagamentos;
using Marketplace.Web.MVC.Models.Carrinho;

namespace Marketplace.Web.MVC.Models.ViewModels;

public class LoginResultado
{
    public ClaimsPrincipal Principal { get; set; } = null!;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresIn { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class FinalizarCompraRequest
{
    public CarrinhoSession Carrinho { get; set; } = new();
    public string Cep { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Logradouro { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Complemento { get; set; }
    public Guid? TransportadoraId { get; set; }
    public double ValorFrete { get; set; }
    public MetodoPagamento MetodoPagamento { get; set; }
    public int NumeroParcelas { get; set; } = 1;
    public Guid UsuarioId { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string? CpfCliente { get; set; }
}

public class ResultadoCheckout
{
    public bool Sucesso { get; set; }
    public Guid? PedidoId { get; set; }
    public Guid? PagamentoId { get; set; }
    public decimal TotalProdutos { get; set; }
    public double ValorFrete { get; set; }
    public double ValorTotal { get; set; }
    public MetodoPagamento MetodoPagamento { get; set; }
    public bool TransacaoAprovada { get; set; }
    public string? MensagemErro { get; set; }
    public string? EtapaErro { get; set; }
}

public class PainelEstatisticasViewModel
{
    public object? DadosPainel { get; set; }
    public bool Disponivel { get; set; }
    public string MensagemErro { get; set; } = string.Empty;
}
