using System.Text.Json;
using Marketplace.Web.MVC.Models.Perfil;

namespace Marketplace.Web.MVC.Services.Perfil;

public class EnderecoSessionService(IHttpContextAccessor httpContextAccessor) : IEnderecoService
{
    private string Chave(Guid usuarioId) => $"endereco_{usuarioId}";

    public EnderecoUsuario? Obter(Guid usuarioId)
    {
        var json = httpContextAccessor.HttpContext?.Session.GetString(Chave(usuarioId));
        if (string.IsNullOrEmpty(json)) return null;
        return JsonSerializer.Deserialize<EnderecoUsuario>(json);
    }

    public void Salvar(Guid usuarioId, EnderecoUsuario endereco)
    {
        var json = JsonSerializer.Serialize(endereco);
        httpContextAccessor.HttpContext?.Session.SetString(Chave(usuarioId), json);
    }
}
