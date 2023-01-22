using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repository.DTO;

namespace Repository;

public interface ITransactionsRepository
{
    Task<DataCollection<TransactionDetail>> SearchTransactionsAsync(
        TransactionType transactionType,
        TransactionFilters filters,
        int limit,
        int offset
    );

    Task<List<TransactionDetail>> GetTransactionsByAccount(int accountId);

    Task<int> CreateTransactionAsync(TransactionCreateRequest transaction);

    Task UpdateTransactionAsync(TransactionUpdateRequest transaction);

    Task DeleteTransactionAsync(int transactionId);
}