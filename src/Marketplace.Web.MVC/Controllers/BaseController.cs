using Marketplace.Web.MVC.Services.Carrinho;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Marketplace.Web.MVC.Controllers;

public abstract class BaseController : Controller
{
    protected readonly CarrinhoService CarrinhoSvc;

    protected BaseController(CarrinhoService carrinhoService)
    {
        CarrinhoSvc = carrinhoService;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var carrinho = await CarrinhoSvc.ObterCarrinhoAsync();
        ViewBag.CarrinhoQtd = carrinho.TotalItens;
        await next();
    }
}
