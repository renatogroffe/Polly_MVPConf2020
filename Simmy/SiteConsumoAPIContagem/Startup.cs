using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;
using SiteConsumoAPIContagem.Clients;

namespace SiteConsumoAPIContagem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Repositório que contém a API REST consumida por este projeto:
            // https://github.com/renatogroffe/ASPNETCore5-REST_API-JSONConsoleLogger_APIContagem

            // Geração de uma mensagem simulado erro HTTP do tipo 500
            // (Internal Server Error)
            var resultInternalServerError = new HttpResponseMessage(
                HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(
                    "Erro gerado em simulação de caos com Simmy...")
            };

            // Criação da Chaos Policy com uma probabilidade
            // de 60% de erro
            var chaosPolicy = MonkeyPolicy
                .InjectResultAsync<HttpResponseMessage>(with =>
                    with.Result(resultInternalServerError)
                        .InjectionRate(0.6)
                        .Enabled(true)
                );
            
            // Configuração da Policy para Retry
            var retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .RetryAsync(3, onRetry: (message, retryCount) =>
                {
                    Console.Out.WriteLine($"Content: {message.Result.Content.ReadAsStringAsync().Result}");
                    Console.Out.WriteLine($"ReasonPhrase: {message.Result.ReasonPhrase}");
                    string msg = $"Retentativa: {retryCount}";
                    Console.Out.WriteLineAsync(msg);
                    LogFileHelper.WriteMessage(msg);
                });

            // Criação de um PolicyWrap agrupando as 2 Policies
            var policyWrap = Policy.WrapAsync(retryPolicy, chaosPolicy);

            services.AddHttpClient<APIContagemClient>()
                .AddPolicyHandler(policyWrap);

            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}