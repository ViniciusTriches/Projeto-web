using Marketplace.Web.MVC.Models.Perfil;

namespace Marketplace.Web.MVC.Services.Perfil;

public interface IEnderecoService
{
    EnderecoUsuario? Obter(Guid usuarioId);
    void Salvar(Guid usuarioId, EnderecoUsuario endereco);
}
