using System.Threading.Tasks;
using Refit;
using ConsumoAPIContagem.Models;

namespace ConsumoAPIContagem.Interfaces
{
    public interface ILoginAPI
    {
        [Post("/login")]
        Task<Token> PostCredentials(User user);
    }
}