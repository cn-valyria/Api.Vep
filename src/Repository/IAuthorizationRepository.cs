using System.Threading.Tasks;
using Repository.DTO;

namespace Repository;

public interface IAuthorizationRepository
{
    Task<AuthorizedAccount> Authorize(int nationId, string uniqueCode);
    Task<AuthorizedAccount> Authorize(string rulerName, string uniqueCode);
}