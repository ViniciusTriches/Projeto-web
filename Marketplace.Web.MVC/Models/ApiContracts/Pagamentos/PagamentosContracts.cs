namespace Marketplace.Web.MVC.Models.ApiContracts.Pagamentos;

public enum MetodoPagamento : int
{
    Pix = 1,
    Boleto = 2,
    CartaoCredito = 3,
    CartaoDebito = 4
}

public enum StatusPagamento : int
{
    Pendente = 1,
    Processando = 2,
    Aprovado = 3,
    Recusado = 4,
    Estornado = 5
}

public class PagamentoDto
{
    public Guid PagamentoId { get; set; }
    public Guid PedidoId { get; set; }
    public string? CpfCliente { get; set; }
    public double ValorProdutos { get; set; }
    public double ValorFrete { get; set; }
    public double ValorTotal { get; set; }
    public int NumeroParcelas { get; set; }
    public double ValorParcela { get; set; }
    public MetodoPagamento MetodoPagamento { get; set; }
    public StatusPagamento StatusPagamento { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? DataPagamento { get; set; }
}

public class TransacaoPagamentoDto
{
    public Guid IdTransacaoPagamento { get; set; }
    public Guid PagamentoId { get; set; }
    public double Valor { get; set; }
    public string? RetornoGateway { get; set; }
    public bool StatusTransacao { get; set; }
    public DateTime DataTransacao { get; set; }
}

public class FreteCalculoDto
{
    public Guid PedidoId { get; set; }
    public Guid? EnderecoEntregaId { get; set; }
    public Guid? TransportadoraId { get; set; }
    public string? CepOrigem { get; set; }
    public string? CepDestino { get; set; }
    public string? Logradouro { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
}

public class TransportadoraDto
{
    public Guid TransportadoraId { get; set; }
    public string? Nome { get; set; }
    public string? CodigoServico { get; set; }
    public double ValorBase { get; set; }
    public double ValorPorKg { get; set; }
    public int PrazoMinDias { get; set; }
    public int PrazoMaxDias { get; set; }
    public bool Ativo { get; set; }
}

public class CriarPagamentoDto
{
    public Guid PedidoId { get; set; }
    public string? CpfCliente { get; set; }
    public double ValorProdutos { get; set; }
    public double ValorFrete { get; set; }
    public int NumeroParcelas { get; set; }
    public MetodoPagamento MetodoPagamento { get; set; }
}
