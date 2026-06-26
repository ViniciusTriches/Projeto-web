using Marketplace.Web.MVC.Models.ApiContracts.Usuarios;
using Marketplace.Web.MVC.Models.Perfil;

namespace Marketplace.Web.MVC.Models.ViewModels;

public class PerfilViewModel
{
    public UsuarioDto? Usuario { get; set; }
    public EnderecoUsuario Endereco { get; set; } = new();
}
