using Ecommerce.DTO;
using Ecommerce.Models;
using System.Security.Claims;

namespace Ecommerce.Services.TableService
{
    public interface ITableService
    {
        Task<IEnumerable<Table>> GetAll();
        Task<Table> GetById(int id);
        Task<StatusDTO> Create(Table model);
        Task<StatusDTO> Update(Table model);
        Task<StatusDTO> Delete(int id);

        Task<StatusDTO> CreateQRTable(int tableId);
        Task<QrResolveResult> ValidTableToken(string token, ClaimsPrincipal user);

    }
}
