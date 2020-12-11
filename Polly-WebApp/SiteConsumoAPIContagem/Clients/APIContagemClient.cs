using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SiteConsumoAPIContagem.Models;

namespace SiteConsumoAPIContagem.Clients
{
    public class APIContagemClient
    {
        private HttpClient _client;
        private IConfiguration _configuration;
        private ILogger<APIContagemClient> _logger;

        public APIContagemClient(
            HttpClient client, IConfiguration configuration,
            ILogger<APIContagemClient> logger)
        {
            _client = client;
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            _configuration = configuration;
            _logger = logger;
        }

        public string ObterDadosContagem()
        {
            var resultado = _client.GetFromJsonAsync<ResultadoContador>(
                _configuration.GetSection("UrlAPIContagem").Value).Result;
            string jsonResultado = JsonSerializer.Serialize(resultado);
            _logger.LogInformation(
                $"Resultado: {jsonResultado}");

            LogFileHelper.WriteMessage(jsonResultado);

            return $"Valor atual do contador: {resultado.ValorAtual}";
        }
    }
}