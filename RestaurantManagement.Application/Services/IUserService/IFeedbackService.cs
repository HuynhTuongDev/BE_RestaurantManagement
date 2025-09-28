using RestaurantManagement.Domain.Entities;
namespace RestaurantManagement.Application.Services.IUserService
{

    namespace RestaurantManagement.Domain.Interfaces
    {
        public interface IFeedbackService
        {
            Task<IEnumerable<Feedback>> GetAllAsync();
        }
    }

}
