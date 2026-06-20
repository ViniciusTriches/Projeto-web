using Marketplace.Web.MVC.Services.Carrinho;
using Marketplace.Web.MVC.Services.Facades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Web.MVC.Controllers;

[Authorize]
public class EstatisticasController(IMarketplaceFacade facade, CarrinhoService carrinhoService) : BaseController(carrinhoService)
{
    public async Task<IActionResult> Index()
        => View(await facade.ObterPainelEstatisticasAsync());
}
