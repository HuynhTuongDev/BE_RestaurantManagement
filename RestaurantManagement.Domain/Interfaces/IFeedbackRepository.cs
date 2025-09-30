using RestaurantManagement.Domain.DTOs.UserDTOs;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<List<Feedback>> GetAllAsync();
        Task<Feedback?> GetByIdAsync(int id);
        Task<Feedback> AddAsync(Feedback feedback);
        Task<Feedback> UpdateAsync(Feedback feedback);
        Task<bool> DeleteAsync(Feedback feedback);
    }
}