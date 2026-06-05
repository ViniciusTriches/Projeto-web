using Marketplace.Web.MVC.Models.ApiContracts.Categorias;

namespace Marketplace.Web.MVC.Services.Interfaces;

public interface ICategoriasClient
{
    Task<List<CategoriaDto>> ListarAsync();
    Task<CategoriaDto?> ObterAsync(int id);
}
