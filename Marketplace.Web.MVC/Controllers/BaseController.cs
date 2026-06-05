using Marketplace.Web.MVC.Services.Carrinho;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Marketplace.Web.MVC.Controllers;

public abstract class BaseController(CarrinhoService carrinhoService) : Controller
{
    public override async void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        var carrinho = await carrinhoService.ObterCarrinhoAsync();
        ViewBag.CarrinhoQtd = carrinho.TotalItens;
    }
}
