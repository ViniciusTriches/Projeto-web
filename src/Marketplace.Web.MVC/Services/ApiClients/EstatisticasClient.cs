using Marketplace.Web.MVC.Services.Interfaces;

namespace Marketplace.Web.MVC.Services.ApiClients;

public class EstatisticasClient(HttpClient http, ILogger<EstatisticasClient> logger) : IEstatisticasClient
{
    public async Task<object?> ObterPainelHojeAsync()
    {
        try
        {
            var resp = await http.GetAsync("/api/Estatistica/painel-hoje");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<object>();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "API de Estatísticas indisponível");
            return null;
        }
    }
}
