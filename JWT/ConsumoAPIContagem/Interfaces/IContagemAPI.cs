using System.Threading.Tasks;
using Refit;
using ConsumoAPIContagem.Models;

namespace ConsumoAPIContagem.Interfaces
{
    public interface IContagemAPI
    {
        [Get("/Contador")]
        Task<ResultadoContador> ObterValorAtual(
            [Header("Authorization")]string token);       
    }
}