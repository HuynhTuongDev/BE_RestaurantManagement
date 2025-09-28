using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<List<Feedback>> GetAllAsync();
    }
}