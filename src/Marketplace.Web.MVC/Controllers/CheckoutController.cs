using System.Security.Claims;
using Marketplace.Web.MVC.Models.ApiContracts.Pagamentos;
using Marketplace.Web.MVC.Models.ViewModels;
using Marketplace.Web.MVC.Services.Carrinho;
using Marketplace.Web.MVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Web.MVC.Controllers;

[Authorize]
public class CheckoutController(
    IPagamentosClient pagamentosClient,
    CarrinhoService carrinhoService) : BaseController(carrinhoService)
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var carrinho = await CarrinhoSvc.ObterCarrinhoAsync();
        if (!carrinho.Itens.Any())
        {
            TempData["Erro"] = "Seu carrinho está vazio.";
            return RedirectToAction("Index", "Carrinho");
        }

        var transportadoras = await pagamentosClient.ListarTransportadorasAsync();

        var vm = new CheckoutViewModel
        {
            Carrinho = carrinho,
            Transportadoras = transportadoras
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirmar(CheckoutViewModel model)
    {
        var carrinho = await CarrinhoSvc.ObterCarrinhoAsync();
        if (!carrinho.Itens.Any())
        {
            TempData["Erro"] = "Seu carrinho está vazio.";
            return RedirectToAction("Index", "Carrinho");
        }

        if (!ModelState.IsValid)
        {
            model.Carrinho = carrinho;
            model.Transportadoras = await pagamentosClient.ListarTransportadorasAsync();
            return View("Index", model);
        }

        var accessToken = User.FindFirstValue("AccessToken") ?? string.Empty;
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdStr, out var usuarioId))
        {
            TempData["Erro"] = "Sessão expirada. Faça login novamente.";
            return RedirectToAction("Login", "Conta");
        }

        // Criar pedido via API de pedidos (usa endpoint interno da API)
        var pedidoId = Guid.NewGuid();

        // Criar pagamento
        var cpf = User.FindFirstValue(ClaimTypes.Email); // fallback
        var totalProdutos = (double)carrinho.Total;

        var criarPagamento = new CriarPagamentoDto
        {
            PedidoId = pedidoId,
            CpfCliente = cpf,
            ValorProdutos = totalProdutos,
            ValorFrete = model.ValorFrete,
            NumeroParcelas = model.NumeroParcelas > 0 ? model.NumeroParcelas : 1,
            MetodoPagamento = model.MetodoPagamento
        };

        var pagamento = await pagamentosClient.CriarPagamentoAsync(criarPagamento, accessToken);
        if (pagamento == null)
        {
            TempData["Erro"] = "Erro ao processar pagamento. Tente novamente.";
            model.Carrinho = carrinho;
            model.Transportadoras = await pagamentosClient.ListarTransportadorasAsync();
            return View("Index", model);
        }

        // Processar transação
        var transacao = await pagamentosClient.ProcessarTransacaoAsync(pagamento.PagamentoId, accessToken);

        // Limpar carrinho
        await CarrinhoSvc.LimparAsync();

        var confirmacao = new ConfirmacaoViewModel
        {
            PedidoId = pedidoId,
            PagamentoId = pagamento.PagamentoId,
            TotalProdutos = carrinho.Total,
            ValorFrete = model.ValorFrete,
            ValorTotal = totalProdutos + model.ValorFrete,
            MetodoPagamento = model.MetodoPagamento,
            Aprovado = transacao?.StatusTransacao ?? false
        };

        return View("Confirmacao", confirmacao);
    }
}
