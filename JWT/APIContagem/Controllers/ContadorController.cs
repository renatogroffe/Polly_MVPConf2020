using System;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using APIContagem.Models;

namespace APIContagem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize("Bearer")]
    public class ContadorController : ControllerBase
    {
        private static Contador _CONTADOR = new Contador();

        [HttpGet]
        public ResultadoContador Get([FromServices]IConfiguration configuration)
        {
            _CONTADOR.Incrementar();

            lock (_CONTADOR)
            {
                return new ()
                {
                    ValorAtual = _CONTADOR.ValorAtual,
                    MachineName = Environment.MachineName,
                    Local = "Teste",
                    Sistema = Environment.OSVersion.VersionString,
                    Saudacao = configuration["Saudacao"],
                    TargetFramework = Assembly
                        .GetEntryAssembly()?
                        .GetCustomAttribute<TargetFrameworkAttribute>()?
                        .FrameworkName
                };
            }
        }
    }
}