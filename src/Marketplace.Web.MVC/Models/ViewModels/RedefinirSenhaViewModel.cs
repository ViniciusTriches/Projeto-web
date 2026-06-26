using System.ComponentModel.DataAnnotations;

namespace Marketplace.Web.MVC.Models.ViewModels;

public class RedefinirSenhaViewModel
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha obrigatória")]
    [MinLength(8, ErrorMessage = "Senha deve ter no mínimo 8 caracteres")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Senha deve conter maiúscula, número e caractere especial (@$!%*?&)")]
    [DataType(DataType.Password)]
    public string NovaSenha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme a nova senha")]
    [Compare(nameof(NovaSenha), ErrorMessage = "As senhas não coincidem")]
    [DataType(DataType.Password)]
    public string ConfirmarSenha { get; set; } = string.Empty;
}
