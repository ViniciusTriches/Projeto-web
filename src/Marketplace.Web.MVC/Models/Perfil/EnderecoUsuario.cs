namespace Marketplace.Web.MVC.Models.Perfil;

// Endereço armazenado localmente em Session pois a API de Usuários não possui campo de endereço no banco de dados.
public class EnderecoUsuario
{
    public string Cep { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}
