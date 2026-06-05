using Marketplace.Web.MVC.Models.Carrinho;
using Marketplace.Web.MVC.Services.Carrinho;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Web.MVC.Controllers;

public class CarrinhoController(CarrinhoService carrinhoService) : BaseController(carrinhoService)
{
    public async Task<IActionResult> Index()
    {
        var carrinho = await carrinhoService.ObterCarrinhoAsync();
        return View(carrinho);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Adicionar(Guid produtoId, string nome, decimal preco, int quantidade = 1)
    {
        var item = new CarrinhoItem
        {
            ProdutoId = produtoId,
            Nome = nome,
            Preco = preco,
            Quantidade = quantidade
        };

        await carrinhoService.AdicionarItemAsync(item);
        TempData["Sucesso"] = $"{nome} adicionado ao carrinho!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remover(Guid produtoId)
    {
        await carrinhoService.RemoverItemAsync(produtoId);
        TempData["Sucesso"] = "Item removido do carrinho.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarQuantidade(Guid produtoId, int quantidade)
    {
        await carrinhoService.AtualizarQuantidadeAsync(produtoId, quantidade);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Limpar()
    {
        await carrinhoService.LimparAsync();
        TempData["Sucesso"] = "Carrinho esvaziado.";
        return RedirectToAction("Index");
    }
}
