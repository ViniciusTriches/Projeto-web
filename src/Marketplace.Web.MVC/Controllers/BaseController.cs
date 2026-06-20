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

    public override async void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        var carrinho = await CarrinhoSvc.ObterCarrinhoAsync();
        ViewBag.CarrinhoQtd = carrinho.TotalItens;
    }
}
