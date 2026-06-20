using System.Security.Claims;
using Marketplace.Web.MVC.Models.ApiContracts.Avaliacoes;
using Marketplace.Web.MVC.Models.ViewModels;
using Marketplace.Web.MVC.Services.Carrinho;
using Marketplace.Web.MVC.Services.Facades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Web.MVC.Controllers;

public class ProdutosController(IMarketplaceFacade facade, CarrinhoService carrinhoService) : BaseController(carrinhoService)
{
    public async Task<IActionResult> Index(string? busca)
        => View(await facade.ObterCatalogoAsync(busca, null));

    public async Task<IActionResult> Detalhe(Guid id)
    {
        var vm = await facade.ObterDetalheProdutoAsync(id);
        if (vm is null)
        {
            TempData["ToastErro"] = "Produto não encontrado.";
            return RedirectToAction("Index");
        }
        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Avaliar(NovaAvaliacaoViewModel model)
    {
        var accessToken = User.FindFirstValue("AccessToken");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(accessToken) || !Guid.TryParse(userId, out var clienteId))
        {
            TempData["ToastErro"] = "Sessão expirada. Faça login novamente.";
            return RedirectToAction("Detalhe", new { id = model.IdProduto });
        }

        var (_, erro) = await facade.AvaliarProdutoAsync(new CriarAvaliacaoDto
        {
            IdCliente = clienteId,
            IdProduto = model.IdProduto,
            Nota = model.Nota,
            Descricao = model.Descricao
        }, accessToken);

        TempData[erro is null ? "ToastSucesso" : "ToastErro"] = erro ?? "Avaliação enviada com sucesso!";
        return RedirectToAction("Detalhe", new { id = model.IdProduto });
    }
}
