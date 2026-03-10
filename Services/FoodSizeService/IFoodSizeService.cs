using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.ViewModels;

namespace Ecommerce.Services.FoodSizeService
{
    public interface IFoodSizeService
    {
        Task<IEnumerable<FoodSizeViewModel>> GetAll();
        Task<FoodSizeViewModel> GetById(int id);
        Task<StatusDTO> Create(FoodSize model);
        Task<StatusDTO> Update(FoodSize model);
        Task<StatusDTO> Delete(int id);
        Task<IEnumerable<FoodSizeViewModel>> Search(string? keyword);

    }
}
