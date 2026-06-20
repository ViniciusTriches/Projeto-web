using System.Security.Claims;
using Marketplace.Web.MVC.Services.Carrinho;
using Marketplace.Web.MVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Web.MVC.Controllers;

[Authorize]
public class PedidosController(
    IPedidosClient pedidosClient,
    CarrinhoService carrinhoService) : BaseController(carrinhoService)
{
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var accessToken = User.FindFirstValue("AccessToken");

        if (!Guid.TryParse(userId, out var usuarioId) || string.IsNullOrEmpty(accessToken))
        {
            TempData["Erro"] = "Sessão expirada. Faça login novamente.";
            return RedirectToAction("Login", "Conta");
        }

        var carrinho = await pedidosClient.ObterCarrinhoAsync(usuarioId, accessToken);
        return View(carrinho);
    }
}
