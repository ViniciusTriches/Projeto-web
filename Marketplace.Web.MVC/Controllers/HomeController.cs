using System.Diagnostics;
using Marketplace.Web.MVC.Models;
using Marketplace.Web.MVC.Models.ViewModels;
using Marketplace.Web.MVC.Services.Carrinho;
using Marketplace.Web.MVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Web.MVC.Controllers;

public class HomeController(
    IProdutosClient produtosClient,
    ICategoriasClient categoriasClient,
    CarrinhoService carrinhoService) : BaseController(carrinhoService)
{
    public async Task<IActionResult> Index()
    {
        var produtos = await produtosClient.ListarAsync();
        var categorias = await categoriasClient.ListarAsync();

        var vm = new HomeViewModel
        {
            Produtos = produtos,
            Categorias = categorias
        };

        return View(vm);
    }

    public async Task<IActionResult> Buscar(string q)
    {
        var produtos = string.IsNullOrWhiteSpace(q)
            ? await produtosClient.ListarAsync()
            : await produtosClient.BuscarAsync(q);

        var categorias = await categoriasClient.ListarAsync();

        var vm = new HomeViewModel
        {
            Produtos = produtos,
            Categorias = categorias,
            TermoBusca = q
        };

        return View("Index", vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
