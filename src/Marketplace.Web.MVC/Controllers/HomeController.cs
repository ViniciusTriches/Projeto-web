using System.Diagnostics;
using Marketplace.Web.MVC.Models;
using Marketplace.Web.MVC.Services.Carrinho;
using Marketplace.Web.MVC.Services.Facades;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Web.MVC.Controllers;

public class HomeController(IMarketplaceFacade facade, CarrinhoService carrinhoService) : BaseController(carrinhoService)
{
    public async Task<IActionResult> Index()
        => View(await facade.ObterCatalogoAsync(null, null));

    public async Task<IActionResult> Buscar(string q)
        => View("Index", await facade.ObterCatalogoAsync(q, null));

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
