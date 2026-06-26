using Marketplace.Web.MVC.Models.ApiContracts.Avaliacoes;
using Marketplace.Web.MVC.Models.ApiContracts.Pagamentos;
using Marketplace.Web.MVC.Models.ApiContracts.Pedidos;
using Marketplace.Web.MVC.Models.ApiContracts.Usuarios;
using Marketplace.Web.MVC.Models.ViewModels;
using Marketplace.Web.MVC.Services.Auth;
using Marketplace.Web.MVC.Services.Interfaces;

namespace Marketplace.Web.MVC.Services.Facades;

public class MarketplaceFacade(
    IUsuariosClient usuarios,
    IProdutosClient produtos,
    ICategoriasClient categorias,
    IPedidosClient pedidos,
    IPagamentosClient pagamentos,
    IAvaliacoesClient avaliacoes,
    IEstatisticasClient estatisticas,
    JwtDecoder jwtDecoder,
    ILogger<MarketplaceFacade> logger) : IMarketplaceFacade
{
    public async Task<HomeViewModel> ObterCatalogoAsync(string? busca, int? categoriaId)
    {
        var vm = new HomeViewModel { TermoBusca = busca };
        try
        {
            var tarefaProdutos = string.IsNullOrWhiteSpace(busca)
                ? produtos.ListarAsync()
                : produtos.BuscarAsync(busca);

            // Cada client captura suas próprias exceções e retorna fallback seguro (lista vazia ou null),
            // portanto o WhenAll nunca lança mesmo que ambas as APIs estejam indisponíveis.
            var (listaProdutos, listaCategorias) = await (tarefaProdutos, categorias.ListarAsync()).WhenAll();

            vm.Produtos = listaProdutos ?? [];
            vm.Categorias = listaCategorias ?? [];

            if (categoriaId.HasValue)
                vm.Produtos = vm.Produtos.Where(p => p.IdCategoria == categoriaId.Value).ToList(); // cross-team: valores de IdCategoria podem não corresponder às categorias reais

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter catálogo");
            vm.ApiIndisponivel = true;
        }
        return vm;
    }

    public async Task<ProdutoDetalheViewModel?> ObterDetalheProdutoAsync(Guid produtoId)
    {
        try
        {
            var (produto, avaliacoesList) = await (
                produtos.ObterPorIdAsync(produtoId),
                avaliacoes.ListarPorProdutoAsync(produtoId)
            ).WhenAll();

            if (produto is null) return null;

            return new ProdutoDetalheViewModel
            {
                Produto = produto,
                Avaliacoes = avaliacoesList ?? [],
                NovaAvaliacao = new NovaAvaliacaoViewModel { IdProduto = produtoId }
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter detalhe do produto {Id}", produtoId);
            return null;
        }
    }

    public async Task<(LoginResultado? Resultado, string? Erro)> AutenticarAsync(string email, string senha)
    {
        try
        {
            var resp = await usuarios.LoginAsync(new LoginRequest(email, senha));
            if (resp is null) return (null, "E-mail ou senha incorretos.");

            var principal = jwtDecoder.Decode(resp.AccessToken);

            return (new LoginResultado
            {
                Principal = principal,
                AccessToken = resp.AccessToken,
                RefreshToken = resp.RefreshToken,
                ExpiresIn = resp.ExpiresIn,
                Nome = resp.Nome
            }, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao autenticar usuário");
            return (null, "Erro de conexão ao tentar entrar. Tente novamente.");
        }
    }

    public async Task<(UsuarioDto? Usuario, string? Erro)> RegistrarUsuarioAsync(CriarUsuarioRequest request)
        => await usuarios.CriarUsuarioAsync(request);

    public async Task<UsuarioDto?> ObterPerfilAsync(Guid usuarioId, string accessToken)
        => await usuarios.ObterUsuarioAsync(usuarioId, accessToken);

    public async Task<UsuarioDto?> AtualizarPerfilAsync(Guid usuarioId, AtualizarUsuarioRequest request, string accessToken)
        => await usuarios.AtualizarUsuarioAsync(usuarioId, request, accessToken);

    public async Task SolicitarResetSenhaAsync(string email)
        => await usuarios.EsqueciSenhaAsync(email);

    public async Task<(bool Sucesso, string? Erro)> RedefinirSenhaAsync(string email, string token, string novaSenha)
        => await usuarios.ResetarSenhaAsync(email, token, novaSenha);

    public async Task<(ProdutoAvaliacaoDto? Avaliacao, string? Erro)> AvaliarProdutoAsync(CriarAvaliacaoDto dto, string accessToken)
    {
        try
        {
            var result = await avaliacoes.CriarAsync(dto, accessToken);
            if (result is null) return (null, "Não foi possível registrar a avaliação.");
            return (result, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao avaliar produto");
            return (null, "Erro ao registrar avaliação.");
        }
    }

    public async Task<List<TransportadoraDto>> ObterTransportadorasAsync()
    {
        try { return await pagamentos.ListarTransportadorasAsync(); }
        catch (Exception ex) { logger.LogError(ex, "Erro ao listar transportadoras"); return []; }
    }

    public async Task<object?> CalcularFreteAsync(FreteCalculoDto dto, string accessToken)
    {
        try { return await pagamentos.CalcularFreteAsync(dto, accessToken); }
        catch (Exception ex) { logger.LogError(ex, "Erro ao calcular frete"); return null; }
    }

    public async Task<ResultadoCheckout> FinalizarCompraAsync(FinalizarCompraRequest req)
    {
        var resultado = new ResultadoCheckout
        {
            TotalProdutos = req.Carrinho.Total,
            ValorFrete = req.ValorFrete,
            MetodoPagamento = req.MetodoPagamento
        };

        try
        {
            var pedidoId = Guid.NewGuid();
            var valorTotal = (double)req.Carrinho.Total + req.ValorFrete;

            var pagamentoCriado = await pagamentos.CriarPagamentoAsync(new CriarPagamentoDto
            {
                PedidoId = pedidoId,
                CpfCliente = req.CpfCliente,
                ValorProdutos = (double)req.Carrinho.Total,
                ValorFrete = req.ValorFrete,
                NumeroParcelas = req.MetodoPagamento == MetodoPagamento.CartaoCredito ? req.NumeroParcelas : 1,
                MetodoPagamento = req.MetodoPagamento
            }, req.AccessToken);

            if (pagamentoCriado is null)
            {
                resultado.MensagemErro = "Erro ao criar pagamento. Tente novamente.";
                resultado.EtapaErro = "Pagamento";
                return resultado;
            }

            var transacao = await pagamentos.ProcessarTransacaoAsync(pagamentoCriado.PagamentoId, req.AccessToken);

            resultado.Sucesso = true;
            resultado.PedidoId = pedidoId;
            resultado.PagamentoId = pagamentoCriado.PagamentoId;
            resultado.ValorTotal = valorTotal;
            resultado.TransacaoAprovada = transacao?.StatusTransacao ?? false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao finalizar compra");
            resultado.MensagemErro = "Erro inesperado ao finalizar compra.";
            resultado.EtapaErro = "Checkout";
        }

        return resultado;
    }

    public async Task<List<PedidoRetornoDto>> ObterPedidosDoUsuarioAsync(Guid usuarioId)
    {
        try
        {
            var carrinho = await pedidos.ObterCarrinhoAsync(usuarioId, string.Empty);
            return carrinho?.PedidosModel ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter pedidos do usuário {Id}", usuarioId);
            return [];
        }
    }

    public async Task<PainelEstatisticasViewModel> ObterPainelEstatisticasAsync()
    {
        try
        {
            var dados = await estatisticas.ObterPainelHojeAsync();
            if (dados is null) return new PainelEstatisticasViewModel { Disponivel = false, MensagemErro = "Dados indisponíveis no momento." };
            return new PainelEstatisticasViewModel { Disponivel = true, DadosPainel = dados };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter painel de estatísticas");
            return new PainelEstatisticasViewModel { Disponivel = false, MensagemErro = "Serviço de estatísticas indisponível." };
        }
    }
}

// Extensão auxiliar para Task.WhenAll com tupla
file static class TaskExtensions
{
    public static async Task<(T1, T2)> WhenAll<T1, T2>(this (Task<T1> t1, Task<T2> t2) tasks)
    {
        await Task.WhenAll(tasks.t1, tasks.t2);
        return (tasks.t1.Result, tasks.t2.Result);
    }
}
