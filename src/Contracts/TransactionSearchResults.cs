using System.Collections.Generic;
using Models;

namespace api.Contracts;

public class TransactionSearchResults
{
    public TransactionCollection AidBasedTransactions { get; set; }
    public TransactionCollection ManualTransactions { get; set; }
}

public class TransactionCollection
{
    public int TotalCount { get; set; }
    public IEnumerable<Transaction> Results { get; set; }
}