using System.Security.Claims;
using Marketplace.Web.MVC.Services.Carrinho;
using Marketplace.Web.MVC.Services.Facades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Web.MVC.Controllers;

[Authorize]
public class PedidosController(IMarketplaceFacade facade, CarrinhoService carrinhoService) : BaseController(carrinhoService)
{
    public async Task<IActionResult> Index()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
        {
            TempData["Erro"] = "Sessão expirada. Faça login novamente.";
            return RedirectToAction("Login", "Conta");
        }

        var pedidos = await facade.ObterPedidosDoUsuarioAsync(usuarioId);
        return View(pedidos);
    }
}
