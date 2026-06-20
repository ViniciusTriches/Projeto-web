using System.Security.Claims;
using Marketplace.Web.MVC.Models.ApiContracts.Usuarios;
using Marketplace.Web.MVC.Models.ViewModels;
using Marketplace.Web.MVC.Services.Carrinho;
using Marketplace.Web.MVC.Services.Facades;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Web.MVC.Controllers;

public class ContaController(IMarketplaceFacade facade, CarrinhoService carrinhoService) : BaseController(carrinhoService)
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var (resultado, erro) = await facade.AutenticarAsync(model.Email, model.Senha);
        if (resultado is null)
        {
            ModelState.AddModelError(string.Empty, erro ?? "E-mail ou senha inválidos.");
            return View(model);
        }

        await SignInWithResultado(resultado);

        TempData["Sucesso"] = $"Bem-vindo(a), {resultado.Nome}!";

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Cadastro()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");
        return View(new CadastroViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastro(CadastroViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var request = new CriarUsuarioRequest(
            model.Nome,
            model.Email,
            model.Senha,
            model.Documento,
            model.TipoPessoa,
            EUsuarioFuncao.Comprador,
            null,
            null,
            model.Telefone);

        var (usuario, erroApi) = await facade.RegistrarUsuarioAsync(request);
        if (usuario is null)
        {
            var mensagem = string.IsNullOrWhiteSpace(erroApi)
                ? "Erro ao criar conta. Verifique os dados e tente novamente."
                : $"Erro ao criar conta: {erroApi}";
            ModelState.AddModelError(string.Empty, mensagem);
            return View(model);
        }

        var (loginResultado, _) = await facade.AutenticarAsync(model.Email, model.Senha);
        if (loginResultado is not null)
            await SignInWithResultado(loginResultado);

        TempData["Sucesso"] = "Conta criada com sucesso! Bem-vindo(a) ao Marketplace.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Sucesso"] = "Você saiu com sucesso.";
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Perfil()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var accessToken = User.FindFirstValue("AccessToken");

        if (!Guid.TryParse(idStr, out var userId) || string.IsNullOrEmpty(accessToken))
            return RedirectToAction("Login");

        var usuario = await facade.ObterPerfilAsync(userId, accessToken);
        if (usuario is null)
        {
            TempData["Erro"] = "Não foi possível carregar o perfil. Tente novamente.";
            return RedirectToAction("Index", "Home");
        }

        return View(new PerfilViewModel
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            NomeFantasia = usuario.NomeFantasia,
            Documento = usuario.Documento,
            Telefone = usuario.Telefone,
            TipoPessoa = usuario.TipoPessoa,
            Funcao = usuario.Funcao,
            Status = usuario.Status,
            CriadoEm = usuario.CriadoEm
        });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Perfil(PerfilViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var accessToken = User.FindFirstValue("AccessToken") ?? string.Empty;
        var atualizado = await facade.AtualizarPerfilAsync(model.Id, new AtualizarUsuarioRequest(model.Id, model.Nome, model.Email), accessToken);

        if (atualizado is null)
        {
            TempData["Erro"] = "Erro ao atualizar perfil. Tente novamente.";
            return View(model);
        }

        TempData["Sucesso"] = "Perfil atualizado com sucesso!";
        return RedirectToAction("Perfil");
    }

    private async Task SignInWithResultado(Models.ViewModels.LoginResultado resultado)
    {
        var claims = new List<Claim>(resultado.Principal.Claims)
        {
            new("AccessToken", resultado.AccessToken),
            new("RefreshToken", resultado.RefreshToken)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true, ExpiresUtc = resultado.ExpiresIn });
    }
}
