using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Repositories.RankAccount;

namespace Ecommerce.Services.RankAccount
{
    public class RankAccountService : IRankAccountService
    {
        private readonly IRankAccountRepository rankAccountRepository;

        public RankAccountService(IRankAccountRepository rankAccountRepository) 
        {
            this.rankAccountRepository = rankAccountRepository;
        }

        public async Task<CustomerRank> GetById(int id)
        {
            var item = await rankAccountRepository.GetByIdAsync(id);
            return item;
        }

        public async Task<StatusDTO> CreateAsync(CustomerRank model)
        {
            var validInput = await valid(model);
            if (validInput.IsSuccess == false) 
                return new StatusDTO { IsSuccess = false, Message = validInput.Message };
            await rankAccountRepository.CreateAsync(model);
            return new StatusDTO { IsSuccess = true, Message = "Tạo hạng thành công!" };
        }

        public async Task<StatusDTO> DeleteAsync(int id)
        {
            var customerRank = await rankAccountRepository.GetByIdAsync(id);
            if (customerRank == null) 
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy hạng cần xóa" };
            await rankAccountRepository.DeleteAsync(id);
            return new StatusDTO { IsSuccess = true, Message = "Xóa hạng thành công" };
        }

        public Task<IEnumerable<CustomerRank>> GetAllAsync() => rankAccountRepository.GetAllAsync();

        public async Task<StatusDTO> UpdateAsync(CustomerRank model)
        {
            if(model.RankId <= 0) 
                return new StatusDTO { IsSuccess = false, Message = "Mã hạng không hợp lệ" };
            var customerRank = await rankAccountRepository.GetByIdAsync(model.RankId);
            if (customerRank == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy hạng cần xóa" };
            await rankAccountRepository.UpdateAsync(model);
            return new StatusDTO { IsSuccess = true, Message = "Cập nhật thành công" };
        }

        public async Task<StatusDTO> valid(CustomerRank model)
        {
            if (model == null)
                return new StatusDTO { IsSuccess = false, Message = "Vui lòng nhập thông tin" };
            if (model.RankName == null)
                return new StatusDTO { IsSuccess = false, Message = "Vui lòng nhập tên hạng" };
            if (model.RankPoint.ToString() == null)
                return new StatusDTO { IsSuccess = false, Message = "Vui lòng nhập điểm hạng" };
            return new StatusDTO { IsSuccess = true, Message = "" };
        }
    }
}
