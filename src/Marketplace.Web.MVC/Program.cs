using Marketplace.Web.MVC.Services.ApiClients;
using Marketplace.Web.MVC.Services.Auth;
using Marketplace.Web.MVC.Services.Carrinho;
using Marketplace.Web.MVC.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var config = builder.Configuration;

builder.Services.AddHttpClient<IUsuariosClient, UsuariosClient>(c =>
    c.BaseAddress = new Uri(config["Microservicos:Usuarios"]!));

builder.Services.AddHttpClient<IProdutosClient, ProdutosClient>(c =>
    c.BaseAddress = new Uri(config["Microservicos:Produtos"]!));

builder.Services.AddHttpClient<ICategoriasClient, CategoriasClient>(c =>
    c.BaseAddress = new Uri(config["Microservicos:Categorias"]!));

builder.Services.AddHttpClient<IPedidosClient, PedidosClient>(c =>
    c.BaseAddress = new Uri(config["Microservicos:Pedidos"]!));

builder.Services.AddHttpClient<IPagamentosClient, PagamentosClient>(c =>
    c.BaseAddress = new Uri(config["Microservicos:Pagamentos"]!));

builder.Services.AddHttpClient<IAvaliacoesClient, AvaliacoesClient>(c =>
    c.BaseAddress = new Uri(config["Microservicos:Avaliacoes"]!));

builder.Services.AddSingleton<JwtDecoder>();
builder.Services.AddScoped<CarrinhoService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Conta/Login";
        options.LogoutPath = "/Conta/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
