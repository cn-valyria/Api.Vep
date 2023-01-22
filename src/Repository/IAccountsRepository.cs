using System.Collections.Generic;
using System.Threading.Tasks;
using Repository.DTO;

namespace Repository;

public interface IAccountsRepository
{
    Task<IEnumerable<AccountDetail>> GetAccountDetailsAsync();

    Task<AccountDetail> GetAccountByIdAsync(int accountId);

    Task<AccountDetail> CreateAccountAsync(AccountCreateRequest request);

    Task UpdateAccountAsync(AccountUpdateRequest account);

    Task RemoveAccountAsync(int accountId);

    Task<ProspectAccount> FindProspectAccountAsync(int nationId);
}