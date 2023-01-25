using System.Threading.Tasks;
using Repository.DTO;

namespace Repository;

public interface IAdminRepository
{
    Task<AdminDashboard> GetAdminDashboardAsync();
}