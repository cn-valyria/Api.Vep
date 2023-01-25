using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Repository.DTO;

namespace Repository;

public class AdminRepository : IAdminRepository
{
    private readonly string _connectionString;

    public AdminRepository(string connectionString) => _connectionString = connectionString;

    public async Task<AdminDashboard> GetAdminDashboardAsync()
    {
        using var sqlConnection = new MySqlConnection(_connectionString);
        const string query = @"
select
	(select count(1) from account) as TotalAccounts,
    (select count(1) from account where va = 'B') as TotalBuyers,
    (select count(1) from account where va = 'S' or va = 'N') as TotalSellers,
    (select count(1) from transaction) as TotalTransactions,
    (select coalesce(sum(rate), 0) from transaction where left(lu, 1) in ('F', 'D')) as UnusedCredit;";

        var dashboard = await sqlConnection.QuerySingleAsync<AdminDashboard>(query);

        var transactionsPerDayTask = GetTransactionsPerDayAsync();
        var pendingCreditPerDayTask = GetPendingCreditPerAllianceAsync();
        var accountsPerRoleTask = GetAccountsPerRoleAsync();
        var totalSlotsPerRoleTask = GetTotalSlotsPerRoleAsync();
        var freeSlotsPerListTask = GetFreeSlotsPerListAsync();
        var accountsPerAllianceTask = GetAccountsPerAllianceAsync();

        await Task.WhenAll(transactionsPerDayTask, pendingCreditPerDayTask, accountsPerRoleTask, totalSlotsPerRoleTask, freeSlotsPerListTask, accountsPerAllianceTask);

        dashboard.TransactionsPerDay = transactionsPerDayTask.Result;
        dashboard.PendingCreditByAlliance = pendingCreditPerDayTask.Result;
        dashboard.AccountsPerRole = accountsPerRoleTask.Result;
        dashboard.TotalSlotsPerRole = totalSlotsPerRoleTask.Result;
        dashboard.FreeSlotsPerList = freeSlotsPerListTask.Result;
        dashboard.AccountsPerAlliance = accountsPerAllianceTask.Result;

        return dashboard;
    }

    private async Task<List<TransactionCountByDay>> GetTransactionsPerDayAsync()
    {
        using var sqlConnection = new MySqlConnection(_connectionString);
        const string query = @"
select date(`date`) as DayTxnRecorded, count(1) as TxnCount
from transaction
join cybernations_db.aid on transaction.aid_id = aid.id
where `date` > curdate() - interval 5 day
group by date(`date`);";

        return (await sqlConnection.QueryAsync<TransactionCountByDay>(query)).ToList();
    }
    
    private async Task<List<CreditGroupsByAlliance>> GetPendingCreditPerAllianceAsync()
    {
        using var sqlConnection = new MySqlConnection(_connectionString);

        var results = new List<CreditGroupsByAlliance>();
        var rawData = await sqlConnection.QueryAsync<RawData>("get_pending_credit_per_alliance", CommandType.StoredProcedure);
        foreach (var grouping in rawData.GroupBy(x => x.AllianceName))
        {
            var newGroup = new CreditGroupsByAlliance
            {
                AllianceName = grouping.Key,
                CreditGroups = grouping.Select(data => new CreditGroup { DateRange = data.DateRange, TotalCredit = data.TotalCredit }).ToList()
            };

            results.Add(newGroup);
        }

        return results;
    }

    private class RawData
    {
        public string DateRange { get; set; }
        public string AllianceName { get; set; }
        public int TotalCredit { get; set; }
    }

    private async Task<List<AccountCountByRole>> GetAccountsPerRoleAsync()
    {
        using var sqlConnection = new MySqlConnection(_connectionString);
        const string query = @"select va as Role, count(1) as TotalAccounts from account group by va;";

        return (await sqlConnection.QueryAsync<AccountCountByRole>(query)).ToList();
    }

    private async Task<List<TotalSlotCountByRole>> GetTotalSlotsPerRoleAsync()
    {
        using var sqlConnection = new MySqlConnection(_connectionString);
        const string query = @"
select va as Role, sum(slots_full) as TotalSlots
from account
join aid_activity on account.id = aid_activity.account_id
group by va;";

        return (await sqlConnection.QueryAsync<TotalSlotCountByRole>(query)).ToList();
    }

    private async Task<List<FreeSlotCountByList>> GetFreeSlotsPerListAsync()
    {
        using var sqlConnection = new MySqlConnection(_connectionString);
        const string query = @"
select list.name as ListName, sum(slots_full) - sum(slots_used) as FreeSlots
from list
join list_recipient on list.id = list_recipient.list_id
join aid_activity on list_recipient.account_id = aid_activity.account_id
group by list.name;";

        return (await sqlConnection.QueryAsync<FreeSlotCountByList>(query)).ToList();
    }

    private async Task<List<AccountCountByAlliance>> GetAccountsPerAllianceAsync()
    {
        using var sqlConnection = new MySqlConnection(_connectionString);
        const string query = @"
select alliance.name as AllianceName, count(1) as TotalAccounts
from account
join cybernations_db.nation on account.nation_id = nation.id
join cybernations_db.alliance on nation.alliance_id = alliance.id
where account.va <> 'H'
group by alliance.name
order by count(1) desc;";

        return (await sqlConnection.QueryAsync<AccountCountByAlliance>(query)).ToList();
    }
}