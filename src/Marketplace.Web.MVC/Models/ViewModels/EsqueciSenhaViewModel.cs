using System.ComponentModel.DataAnnotations;

namespace Marketplace.Web.MVC.Models.ViewModels;

public class EsqueciSenhaViewModel
{
    [Required(ErrorMessage = "Informe seu e-mail")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = string.Empty;
}
