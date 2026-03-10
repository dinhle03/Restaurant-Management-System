using Ecommerce.Models;
using Ecommerce.ViewModels;

namespace Ecommerce.Repositories.FoodSizeRepository
{
    public interface IFoodSizeRepository
    {
        Task<IEnumerable<FoodSizeViewModel>> GetAll();
        Task<FoodSizeViewModel> GetById(int id);
        Task Create(FoodSize model);
        Task Update(FoodSize model);
        Task Delete(int id);
        Task<string> ValidName(string foodName);
        Task<IEnumerable<FoodSizeViewModel>> Search(string? keyword);

    }
}
