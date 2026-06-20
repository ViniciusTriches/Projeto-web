using System.Security.Claims;
using Marketplace.Web.MVC.Models.ViewModels;
using Marketplace.Web.MVC.Services.Carrinho;
using Marketplace.Web.MVC.Services.Facades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Web.MVC.Controllers;

[Authorize]
public class CheckoutController(IMarketplaceFacade facade, CarrinhoService carrinhoService) : BaseController(carrinhoService)
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

        return View(new CheckoutViewModel
        {
            Carrinho = carrinho,
            Transportadoras = await facade.ObterTransportadorasAsync()
        });
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
            model.Transportadoras = await facade.ObterTransportadorasAsync();
            return View("Index", model);
        }

        var accessToken = User.FindFirstValue("AccessToken") ?? string.Empty;
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
        {
            TempData["Erro"] = "Sessão expirada. Faça login novamente.";
            return RedirectToAction("Login", "Conta");
        }

        var resultado = await facade.FinalizarCompraAsync(new FinalizarCompraRequest
        {
            Carrinho = carrinho,
            Cep = model.Cep,
            Numero = model.Numero,
            Logradouro = model.Logradouro,
            Bairro = model.Bairro,
            Cidade = model.Cidade,
            Estado = model.Estado,
            Complemento = model.Complemento,
            TransportadoraId = model.TransportadoraId,
            ValorFrete = model.ValorFrete,
            MetodoPagamento = model.MetodoPagamento,
            NumeroParcelas = model.NumeroParcelas > 0 ? model.NumeroParcelas : 1,
            UsuarioId = usuarioId,
            AccessToken = accessToken,
            CpfCliente = User.FindFirstValue(ClaimTypes.Email)
        });

        if (!resultado.Sucesso)
        {
            TempData["Erro"] = resultado.MensagemErro ?? "Erro ao processar pedido.";
            model.Carrinho = carrinho;
            model.Transportadoras = await facade.ObterTransportadorasAsync();
            return View("Index", model);
        }

        await CarrinhoSvc.LimparAsync();

        return View("Confirmacao", new ConfirmacaoViewModel
        {
            PedidoId = resultado.PedidoId ?? Guid.Empty,
            PagamentoId = resultado.PagamentoId ?? Guid.Empty,
            TotalProdutos = resultado.TotalProdutos,
            ValorFrete = resultado.ValorFrete,
            ValorTotal = resultado.ValorTotal,
            MetodoPagamento = resultado.MetodoPagamento,
            Aprovado = resultado.TransacaoAprovada
        });
    }
}
