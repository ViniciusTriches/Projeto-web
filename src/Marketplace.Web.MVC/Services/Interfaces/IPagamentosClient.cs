using Marketplace.Web.MVC.Models.ApiContracts.Pagamentos;

namespace Marketplace.Web.MVC.Services.Interfaces;

public interface IPagamentosClient
{
    Task<List<TransportadoraDto>> ListarTransportadorasAsync();
    Task<object?> CalcularFreteAsync(FreteCalculoDto dto, string accessToken);
    Task<PagamentoDto?> CriarPagamentoAsync(CriarPagamentoDto dto, string accessToken);
    Task<TransacaoPagamentoDto?> ProcessarTransacaoAsync(Guid pagamentoId, string accessToken);
}
