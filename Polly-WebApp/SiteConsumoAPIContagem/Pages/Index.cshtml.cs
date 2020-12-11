using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SiteConsumoAPIContagem.Clients;

namespace SiteConsumoAPIContagem.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(
            [FromServices]APIContagemClient client)
        {
            _logger.LogInformation("Acionado o método OnGet em Index.cshtml.cs...");
            TempData["ResultadoAPIContagem"] =
                client.ObterDadosContagem();
        }
    }
}
