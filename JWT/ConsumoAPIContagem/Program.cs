using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ConsumoAPIContagem.Clients;

namespace ConsumoAPIContagem
{
    class Program
    {
        private static async Task Interromper()
        {
            await Console.Out.WriteLineAsync(
                "Pressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        static async Task Main()
        {
			// Projeto com as APIs utilizadas nos testes:
			// https://github.com/renatogroffe/ASPNETCore5-REST_API-JWT_ContagemAcessos
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile($"appsettings.json");
            var config = builder.Build();

            var apiContagemClient =
                    new APIContagemClient(config);
            await apiContagemClient.Autenticar();
            while (true)
            {
                await apiContagemClient.ExibirResultadoContador();
                await Interromper();
            }
        }
    }
}