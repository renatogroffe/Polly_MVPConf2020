using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Retry;
using Refit;
using ConsumoAPIContagem.Extensions;
using ConsumoAPIContagem.Interfaces;
using ConsumoAPIContagem.Models;

namespace ConsumoAPIContagem.Clients
{
    public class APIContagemClient
    {
        private ILoginAPI _loginAPI;
        private IContagemAPI _contagemAPI;
        private IConfiguration _configuration;
        private Token _token;
        private AsyncRetryPolicy _jwtPolicy;

        public bool IsAuthenticatedUsingToken
        {
            get => _token?.Authenticated ?? false;
        }

        public APIContagemClient(IConfiguration configuration)
        {
            _configuration = configuration;
            string urlBase = _configuration.GetSection(
                "APIContagem_Access:UrlBase").Value;

            _loginAPI = RestService.For<ILoginAPI>(urlBase);
            _contagemAPI = RestService.For<IContagemAPI>(urlBase);
            _jwtPolicy = CreateAccessTokenPolicy();
        }

        public Task Autenticar()
        {
            try
            {
                // Envio da requisição a fim de autenticar
                // e obter o token de acesso
                _token = _loginAPI.PostCredentials(
                    new ()
                    {
                        UserID = _configuration.GetSection("APIContagem_Access:UserID").Value,
                        Password = _configuration.GetSection("APIContagem_Access:Password").Value
                    }).Result;
                return Console.Out.WriteLineAsync(
                    Environment.NewLine +
                    JsonSerializer.Serialize(_token));
            }
            catch
            {
                _token = null;
                return Console.Out.WriteLineAsync("Falha ao autenticar...");
            }
        }

        private AsyncRetryPolicy CreateAccessTokenPolicy()
        {
            return Policy
                .HandleInner<ApiException>(
                    ex => ex.StatusCode == HttpStatusCode.Unauthorized)
                .RetryAsync(1, async (ex, retryCount, context) =>
                {
                    var corAnterior = Console.ForegroundColor;

                    Console.ForegroundColor = ConsoleColor.Red;
                    await Console.Out.WriteLineAsync(
                        Environment.NewLine + "Token expirado ou usuário sem permissão!");
                    Console.ForegroundColor = corAnterior;

                    Console.ForegroundColor = ConsoleColor.Green;
                    await Console.Out.WriteLineAsync(
                        Environment.NewLine + "Execução de RetryPolicy...");
                    Console.ForegroundColor = corAnterior;

                    await Autenticar();
                    if (!(_token?.Authenticated ?? false))
                        throw new InvalidOperationException("Token inválido!");

                    context["AccessToken"] = _token.AccessToken;
                });
        }

        public Task ExibirResultadoContador()
        {
            var retorno = _jwtPolicy.ExecuteWithTokenAsync<ResultadoContador>(
                _token, async (context) =>
            {
                var resultado = await _contagemAPI.ObterValorAtual(
                  $"Bearer {context["AccessToken"]}");
                return resultado;
            });

            return Console.Out.WriteLineAsync(
                Environment.NewLine +
                "Retorno da API de Contagem: " +
                JsonSerializer.Serialize(retorno.Result));
        }
    }
}