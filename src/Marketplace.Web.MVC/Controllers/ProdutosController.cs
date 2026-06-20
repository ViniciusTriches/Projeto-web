using System.Security.Claims;
using Marketplace.Web.MVC.Models.ApiContracts.Avaliacoes;
using Marketplace.Web.MVC.Models.ViewModels;
using Marketplace.Web.MVC.Services.Carrinho;
using Marketplace.Web.MVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Web.MVC.Controllers;

public class ProdutosController(
    IProdutosClient produtosClient,
    ICategoriasClient categoriasClient,
    IAvaliacoesClient avaliacoesClient,
    CarrinhoService carrinhoService) : BaseController(carrinhoService)
{
    public async Task<IActionResult> Index(string? busca)
    {
        var produtos = string.IsNullOrWhiteSpace(busca)
            ? await produtosClient.ListarAsync()
            : await produtosClient.BuscarAsync(busca);

        var categorias = await categoriasClient.ListarAsync();

        return View(new HomeViewModel
        {
            Produtos = produtos,
            Categorias = categorias,
            TermoBusca = busca
        });
    }

    public async Task<IActionResult> Detalhe(Guid id)
    {
        var produto = await produtosClient.ObterPorIdAsync(id);
        if (produto == null)
        {
            TempData["Erro"] = "Produto não encontrado.";
            return RedirectToAction("Index");
        }

        var avaliacoes = await avaliacoesClient.ListarPorProdutoAsync(id);

        return View(new ProdutoDetalheViewModel
        {
            Produto = produto,
            Avaliacoes = avaliacoes,
            NovaAvaliacao = new NovaAvaliacaoViewModel { IdProduto = id }
        });
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
            TempData["Erro"] = "Sessão expirada. Faça login novamente.";
            return RedirectToAction("Detalhe", new { id = model.IdProduto });
        }

        var dto = new CriarAvaliacaoDto
        {
            IdCliente = clienteId,
            IdProduto = model.IdProduto,
            Nota = model.Nota,
            Descricao = model.Descricao
        };

        var resultado = await avaliacoesClient.CriarAsync(dto, accessToken);
        if (resultado == null)
            TempData["Erro"] = "Não foi possível registrar sua avaliação.";
        else
            TempData["Sucesso"] = "Avaliação enviada com sucesso!";

        return RedirectToAction("Detalhe", new { id = model.IdProduto });
    }
}
