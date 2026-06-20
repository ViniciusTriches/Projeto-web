using System.Security.Claims;
using Marketplace.Web.MVC.Models.ApiContracts.Usuarios;
using Marketplace.Web.MVC.Models.ViewModels;
using Marketplace.Web.MVC.Services.Auth;
using Marketplace.Web.MVC.Services.Carrinho;
using Marketplace.Web.MVC.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Web.MVC.Controllers;

public class ContaController(
    IUsuariosClient usuariosClient,
    JwtDecoder jwtDecoder,
    CarrinhoService carrinhoService) : BaseController(carrinhoService)
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

        var response = await usuariosClient.LoginAsync(new LoginRequest(model.Email, model.Senha));
        if (response == null)
        {
            ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
            return View(model);
        }

        var principal = jwtDecoder.Decode(response.AccessToken);

        var claims = new List<Claim>(principal.Claims)
        {
            new("AccessToken", response.AccessToken),
            new("RefreshToken", response.RefreshToken)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var newPrincipal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, newPrincipal,
            new AuthenticationProperties { IsPersistent = true, ExpiresUtc = response.ExpiresIn });

        TempData["Sucesso"] = $"Bem-vindo(a), {response.Nome}!";

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

        var (usuario, erroApi) = await usuariosClient.CriarUsuarioAsync(request);
        if (usuario == null)
        {
            var mensagem = string.IsNullOrWhiteSpace(erroApi)
                ? "Erro ao criar conta. Verifique os dados e tente novamente."
                : $"Erro ao criar conta: {erroApi}";
            ModelState.AddModelError(string.Empty, mensagem);
            return View(model);
        }

        // Login automático após cadastro
        var loginResp = await usuariosClient.LoginAsync(new LoginRequest(model.Email, model.Senha));
        if (loginResp != null)
        {
            var principal = jwtDecoder.Decode(loginResp.AccessToken);
            var claims = new List<Claim>(principal.Claims)
            {
                new("AccessToken", loginResp.AccessToken),
                new("RefreshToken", loginResp.RefreshToken)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = true, ExpiresUtc = loginResp.ExpiresIn });
        }

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

        var usuario = await usuariosClient.ObterUsuarioAsync(userId, accessToken);
        if (usuario == null)
        {
            TempData["Erro"] = "Não foi possível carregar o perfil. Tente novamente.";
            return RedirectToAction("Index", "Home");
        }

        var vm = new PerfilViewModel
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
        };

        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Perfil(PerfilViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var accessToken = User.FindFirstValue("AccessToken") ?? string.Empty;
        var request = new AtualizarUsuarioRequest(model.Id, model.Nome, model.Email);
        var atualizado = await usuariosClient.AtualizarUsuarioAsync(model.Id, request, accessToken);

        if (atualizado == null)
        {
            TempData["Erro"] = "Erro ao atualizar perfil. Tente novamente.";
            return View(model);
        }

        TempData["Sucesso"] = "Perfil atualizado com sucesso!";
        return RedirectToAction("Perfil");
    }
}
