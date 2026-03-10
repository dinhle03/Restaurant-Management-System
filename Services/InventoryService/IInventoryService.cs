using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.ViewModels;

namespace Ecommerce.Services.InventoryService
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryViewModel>> GetAll();
        Task<InventoryViewModel> GetById(int id);
        Task<StatusDTO> Create(Inventory model);
        Task<StatusDTO> Update(Inventory model);
        Task<StatusDTO> Delete(int id);
        Task<IEnumerable<InventoryViewModel>> Search(string keyword);

    }
}
