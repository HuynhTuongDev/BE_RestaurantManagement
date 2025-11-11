using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.DTOs.UserDTOs;
using RestaurantManagement.Domain.Entities;
namespace RestaurantManagement.Application.Services.IUserService
{

    namespace RestaurantManagement.Domain.Interfaces
    {
        public interface IFeedbackService
        {
            Task<IEnumerable<FeedbackDto>> GetAllAsync();
            Task<PaginatedResponse<FeedbackDto>> GetPaginatedAsync(PaginationRequest pagination);
            Task<FeedbackDto> GetByIdAsync(int? id);
            Task<FeedbackDto> CreateAsync(CreateFeedbackDto dto);
            Task<FeedbackDto> UpdateFeedbackAsync(FeedbackUpdateDto updateDto);
            Task<FeedbackDto> UpdateCustomerFeedbackAsync(int userId, FeedbackUpdateDto updateDto);
            Task<bool> DeleteFeedbackAsync(int id, int userId, UserRole role);
        }
    }

}
