using Ecommerce.Models;

namespace Ecommerce.Repositories.TableRepository
{
    public interface ITableRepository
    {
        Task<IEnumerable<Table>> GetAll();
        Task<Table> GetById(int id);
        Task Create(Table model);
        Task Update(Table model);
        Task Delete(int id);

        Task UpdateTableOwner(string? userId, int tableId);
        Task<string> ValidTableName(string tableName);
        Task UpdateTableQr(Table model);

    }
}
