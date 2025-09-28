using RestaurantManagement.Domain.DTOs.UserDTOs;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<List<Feedback>> GetAllAsync();
    }
}