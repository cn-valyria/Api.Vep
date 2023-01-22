using System.Collections.Generic;
using System.Threading.Tasks;
using Repository.DTO;

namespace Repository;

public interface IListsRepository
{
    Task<List<AidListDetail>> GetAidListsAsync();

    Task<List<ListRecipientDetail>> GetListRecipientsForAccountAsync(int accountId);
}