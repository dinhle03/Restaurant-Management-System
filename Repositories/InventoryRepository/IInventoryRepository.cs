using Ecommerce.Models;
using Ecommerce.ViewModels;

namespace Ecommerce.Repositories.InventoryRepository
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<InventoryViewModel>> GetAll();
        Task<InventoryViewModel> GetById(int id);
        Task Create(Inventory model);
        Task Update(Inventory model);
        Task Delete(int id);
        Task<IEnumerable<InventoryViewModel>> Search(string keyword);
        Task<Inventory> GetEntityById(int id);

    }
}
