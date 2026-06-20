using System.ComponentModel.DataAnnotations;
using Marketplace.Web.MVC.Models.ApiContracts.Usuarios;

namespace Marketplace.Web.MVC.Models.ViewModels;

public class CadastroViewModel
{
    [Required(ErrorMessage = "Nome obrigatório")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-mail obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha obrigatória")]
    [MinLength(8, ErrorMessage = "Senha deve ter no mínimo 8 caracteres")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Senha deve conter maiúscula, número e caractere especial (@$!%*?&)")]
    [DataType(DataType.Password)]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Documento obrigatório")]
    [Display(Name = "CPF / CNPJ")]
    public string Documento { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tipo de pessoa obrigatório")]
    [Display(Name = "Tipo de Pessoa")]
    public ETipoPessoa TipoPessoa { get; set; } = ETipoPessoa.PessoaFisica;

    public string? Telefone { get; set; }
}
