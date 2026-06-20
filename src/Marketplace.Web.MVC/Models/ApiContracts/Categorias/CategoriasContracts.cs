namespace Marketplace.Web.MVC.Models.ApiContracts.Categorias;

public class CategoriaDto
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public int? ParentId { get; set; }
}

public class CreateCategoriaDto
{
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public int? ParentId { get; set; }
}
