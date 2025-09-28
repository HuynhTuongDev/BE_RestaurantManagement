using RestaurantManagement.Application.Services.IUserService.RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Domain.DTOs.UserDTOs;
using RestaurantManagement.Domain.Interfaces;

namespace RestaurantManagement.Infrastructure.Services.UserServices
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _repository;

        public FeedbackService(IFeedbackRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<FeedbackDto>> GetAllAsync()
        {
            var feedbacks = await _repository.GetAllAsync();

            return feedbacks.Select(f => new FeedbackDto
            {
                Id = f.Id,
                UserId = f.UserId,
                UserName = f.User?.FullName ?? string.Empty,
                OrderId = f.OrderId,
                MenuItemId = f.MenuItemId,
                MenuItemName = f.MenuItem?.Name,
                Rating = f.Rating,
                Comment = f.Comment,
                IsApproved = f.IsApproved,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt,
                Reply = f.Reply,
                RepliedAt = f.RepliedAt
            });
        }
    }
}
