using System.ComponentModel.DataAnnotations;
using Marketplace.Web.MVC.Models.ApiContracts.Usuarios;

namespace Marketplace.Web.MVC.Models.ViewModels;

public class PerfilViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Nome obrigatório")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-mail obrigatório")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? NomeFantasia { get; set; }
    public string Documento { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public ETipoPessoa TipoPessoa { get; set; }
    public EUsuarioFuncao Funcao { get; set; }
    public EUsuarioStatus Status { get; set; }
    public DateTime CriadoEm { get; set; }
}
