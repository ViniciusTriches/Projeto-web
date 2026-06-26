using System.Security.Claims;
using Marketplace.Web.MVC.Models.ApiContracts.Usuarios;
using Marketplace.Web.MVC.Models.Perfil;
using Marketplace.Web.MVC.Models.ViewModels;
using Marketplace.Web.MVC.Services.Carrinho;
using Marketplace.Web.MVC.Services.Facades;
using Marketplace.Web.MVC.Services.Perfil;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Web.MVC.Controllers;

public class ContaController(
    IMarketplaceFacade facade,
    CarrinhoService carrinhoService,
    IEnderecoService enderecoService,
    ILogger<ContaController> logger) : BaseController(carrinhoService)
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
        logger.LogInformation("Login action iniciada. Response.HasStarted: {HasStarted}", HttpContext.Response.HasStarted);

        if (!ModelState.IsValid) return View(model);

        var (resultado, erro) = await facade.AutenticarAsync(model.Email, model.Senha);
        if (resultado is null)
        {
            ModelState.AddModelError(string.Empty, erro ?? "E-mail ou senha inválidos.");
            return View(model);
        }

        await SignInWithResultado(resultado);

        TempData["ToastSucesso"] = $"Bem-vindo(a), {resultado.Nome}!";

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

        TempData["ToastSucesso"] = "Conta criada com sucesso! Bem-vindo(a) ao Marketplace.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["ToastSucesso"] = "Você saiu com sucesso.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult EsqueciSenha() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EsqueciSenha(EsqueciSenhaViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await facade.SolicitarResetSenhaAsync(model.Email);

        TempData["ToastSucesso"] = "Enviamos um e-mail com o token de redefinição. Verifique sua caixa de entrada (e spam).";
        return RedirectToAction("RedefinirSenha");
    }

    [HttpGet]
    public IActionResult RedefinirSenha(string? token = null)
    {
        return View(new RedefinirSenhaViewModel { Token = token ?? string.Empty });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RedefinirSenha(RedefinirSenhaViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var (sucesso, erro) = await facade.RedefinirSenhaAsync(model.Email, model.Token, model.NovaSenha);
        if (!sucesso)
        {
            ModelState.AddModelError(string.Empty, erro ?? "Não foi possível redefinir a senha. O link pode ter expirado.");
            return View(model);
        }

        TempData["ToastSucesso"] = "Senha redefinida com sucesso! Faça login com sua nova senha.";
        return RedirectToAction("Login");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Perfil()
    {
        var (userId, accessToken) = ObterCredenciais();
        if (userId == Guid.Empty)
            return RedirectToAction("Login");

        var usuario = await facade.ObterPerfilAsync(userId, accessToken);
        if (usuario is null)
        {
            TempData["ToastErro"] = "Não foi possível carregar o perfil. Tente novamente.";
            return RedirectToAction("Index", "Home");
        }

        return View(new PerfilViewModel
        {
            Usuario = usuario,
            Endereco = enderecoService.Obter(userId) ?? new EnderecoUsuario()
        });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarDadosPessoais(PerfilViewModel model)
    {
        var (userId, accessToken) = ObterCredenciais();

        if (model.Usuario is null)
        {
            TempData["ToastErro"] = "Dados inválidos.";
            return RedirectToAction("Perfil");
        }

        var atualizado = await facade.AtualizarPerfilAsync(
            userId,
            new AtualizarUsuarioRequest(userId, model.Usuario.Nome, model.Usuario.Email),
            accessToken);

        if (atualizado is null)
        {
            TempData["ToastErro"] = "Não foi possível atualizar seus dados.";
            return RedirectToAction("Perfil");
        }

        TempData["ToastSucesso"] = "Dados atualizados com sucesso!";
        return RedirectToAction("Perfil");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AtualizarEndereco(PerfilViewModel model)
    {
        var (userId, _) = ObterCredenciais();
        enderecoService.Salvar(userId, model.Endereco);

        TempData["ToastSucesso"] = "Endereço salvo com sucesso!";
        return RedirectToAction("Perfil");
    }

    private (Guid UserId, string AccessToken) ObterCredenciais()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var token = User.FindFirstValue("AccessToken") ?? string.Empty;
        Guid.TryParse(idStr, out var userId);
        return (userId, token);
    }

    private async Task SignInWithResultado(Models.ViewModels.LoginResultado resultado)
    {
        logger.LogInformation("Iniciando SignInAsync para usuario {Nome}, ExpiresIn={ExpiresIn}",
            resultado.Nome, resultado.ExpiresIn);

        var claims = resultado.Principal.Claims
            .Where(c => c.Type != ClaimTypes.Name && c.Type != "AccessToken" && c.Type != "RefreshToken")
            .ToList();

        claims.Add(new Claim(ClaimTypes.Name, resultado.Nome));
        claims.Add(new Claim("AccessToken", resultado.AccessToken));
        claims.Add(new Claim("RefreshToken", resultado.RefreshToken));

        logger.LogInformation("Claims montados: {Claims}",
            string.Join(", ", claims.Select(c => $"{c.Type}={c.Value}")));

        var identity = new ClaimsIdentity(
            claims,
            authenticationType: CookieAuthenticationDefaults.AuthenticationScheme,
            nameType: ClaimTypes.Name,
            roleType: ClaimTypes.Role);

        var principal = new ClaimsPrincipal(identity);

        logger.LogInformation("Identity.IsAuthenticated antes do SignInAsync: {IsAuth}, Identity.Name: {Name}",
            principal.Identity?.IsAuthenticated, principal.Identity?.Name);

        try
        {
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = resultado.ExpiresIn
                });

            logger.LogInformation("SignInAsync concluido sem excecao. Response.HasStarted: {HasStarted}",
                HttpContext.Response.HasStarted);

            if (HttpContext.Response.Headers.TryGetValue("Set-Cookie", out var setCookieHeaders))
                logger.LogInformation("Set-Cookie presente na resposta: {Headers}", string.Join(" | ", setCookieHeaders!));
            else
                logger.LogWarning("Nenhum header Set-Cookie encontrado na resposta apos SignInAsync!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCECAO durante SignInAsync para usuario {Nome}", resultado.Nome);
            throw;
        }
    }
}
