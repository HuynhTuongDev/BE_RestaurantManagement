using RestaurantManagement.Domain.DTOs.UserDTOs;
using RestaurantManagement.Domain.Entities;
namespace RestaurantManagement.Application.Services.IUserService
{

    namespace RestaurantManagement.Domain.Interfaces
    {
        public interface IFeedbackService
        {
            Task<IEnumerable<FeedbackDto>> GetAllAsync();
            Task<FeedbackDto?> GetByIdAsync(int id);
            
        }
    }

}
