using System.Text.Json;

namespace Marketplace.Web.MVC.Models.ApiContracts.Estatisticas;

public class PainelHojeDto
{
    public JsonElement? Dados { get; set; }
}

public class MediaAvaliacaoDto
{
    public JsonElement? Dados { get; set; }
}
