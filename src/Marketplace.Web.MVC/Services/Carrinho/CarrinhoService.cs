using System.Text.Json;
using Marketplace.Web.MVC.Models.Carrinho;
using Microsoft.Extensions.Caching.Distributed;

namespace Marketplace.Web.MVC.Services.Carrinho;

public class CarrinhoService(IDistributedCache cache, IHttpContextAccessor httpContextAccessor)
{
    private string ChaveCarrinho => $"carrinho_{ObterSessionId()}";

    private string ObterSessionId()
    {
        var ctx = httpContextAccessor.HttpContext!;
        var id = ctx.Session.GetString("CarrinhoSessionId");
        if (string.IsNullOrEmpty(id))
        {
            id = Guid.NewGuid().ToString();
            ctx.Session.SetString("CarrinhoSessionId", id);
        }
        return id;
    }

    public async Task<CarrinhoSession> ObterCarrinhoAsync()
    {
        var json = await cache.GetStringAsync(ChaveCarrinho);
        if (string.IsNullOrEmpty(json)) return new CarrinhoSession();
        return JsonSerializer.Deserialize<CarrinhoSession>(json) ?? new CarrinhoSession();
    }

    public async Task AdicionarItemAsync(CarrinhoItem item)
    {
        var carrinho = await ObterCarrinhoAsync();
        var existente = carrinho.Itens.FirstOrDefault(i => i.ProdutoId == item.ProdutoId);
        if (existente != null)
            existente.Quantidade += item.Quantidade;
        else
            carrinho.Itens.Add(item);
        await SalvarAsync(carrinho);
    }

    public async Task RemoverItemAsync(Guid produtoId)
    {
        var carrinho = await ObterCarrinhoAsync();
        carrinho.Itens.RemoveAll(i => i.ProdutoId == produtoId);
        await SalvarAsync(carrinho);
    }

    public async Task AtualizarQuantidadeAsync(Guid produtoId, int quantidade)
    {
        var carrinho = await ObterCarrinhoAsync();
        var item = carrinho.Itens.FirstOrDefault(i => i.ProdutoId == produtoId);
        if (item != null)
        {
            if (quantidade <= 0)
                carrinho.Itens.Remove(item);
            else
                item.Quantidade = quantidade;
        }
        await SalvarAsync(carrinho);
    }

    public async Task LimparAsync()
    {
        await cache.RemoveAsync(ChaveCarrinho);
    }

    private async Task SalvarAsync(CarrinhoSession carrinho)
    {
        var json = JsonSerializer.Serialize(carrinho);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        };
        await cache.SetStringAsync(ChaveCarrinho, json, options);
    }
}
