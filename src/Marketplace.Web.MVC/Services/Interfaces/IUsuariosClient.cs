using Marketplace.Web.MVC.Models.ApiContracts.Usuarios;

namespace Marketplace.Web.MVC.Services.Interfaces;

public interface IUsuariosClient
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<(UsuarioDto? Usuario, string? Erro)> CriarUsuarioAsync(CriarUsuarioRequest request);
    Task<UsuarioDto?> ObterUsuarioAsync(Guid id, string accessToken);
    Task<UsuarioDto?> AtualizarUsuarioAsync(Guid id, AtualizarUsuarioRequest request, string accessToken);
}
