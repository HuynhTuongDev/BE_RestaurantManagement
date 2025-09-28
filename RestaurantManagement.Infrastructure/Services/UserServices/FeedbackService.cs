using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Services.IUserService.RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Domain.DTOs.UserDTOs;
using RestaurantManagement.Domain.Entities;
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
        public async Task<FeedbackDto> GetByIdAsync(int? id)
        {
            if (!id.HasValue || id.Value <= 0)
                throw new ArgumentException("Feedback ID must be provided and greater than zero.", nameof(id));
            var feedback = await _repository.GetByIdAsync(id.Value)
                ?? throw new KeyNotFoundException($"Feedback with ID {id} not found.");

            return new FeedbackDto
            {
                Id = feedback.Id,
                UserId = feedback.UserId,
                UserName = feedback.User.FullName,
                OrderId = feedback.OrderId,
                MenuItemId = feedback.MenuItemId,
                MenuItemName = feedback.MenuItem?.Name ?? "Unknown",
                Rating = feedback.Rating,
                Comment = feedback.Comment ?? string.Empty,
                IsApproved = feedback.IsApproved,
                CreatedAt = feedback.CreatedAt,
                UpdatedAt = feedback.UpdatedAt,
                Reply = feedback.Reply ?? string.Empty,
                RepliedAt = feedback.RepliedAt
            };
        }


        public async Task<FeedbackDto> CreateAsync(CreateFeedbackDto dto)
        {
            var entity = new Feedback
            {
                UserId = dto.UserId,
                OrderId = dto.OrderId,
                MenuItemId = dto.MenuItemId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                IsApproved = dto.IsApproved,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _repository.AddAsync(entity);

            return new FeedbackDto
            {
                Id = saved.Id,
                UserId = saved.UserId,
                UserName = saved.User?.FullName ?? string.Empty,
                OrderId = saved.OrderId,
                MenuItemId = saved.MenuItemId,
                MenuItemName = saved.MenuItem?.Name,
                Rating = saved.Rating,
                Comment = saved.Comment,
                IsApproved = saved.IsApproved,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Reply = saved.Reply,
                RepliedAt = saved.RepliedAt
            };
        }

        public async Task<FeedbackDto> UpdateFeedbackAsync(FeedbackUpdateDto updateDto)
        {
            var feedback = await _repository.GetByIdAsync(updateDto.Id);
            if (feedback == null)
                throw new KeyNotFoundException($"Feedback with ID {updateDto.Id} not found.");

            if (updateDto.Rating.HasValue) feedback.Rating = updateDto.Rating.Value;
            if (!string.IsNullOrEmpty(updateDto.Comment)) feedback.Comment = updateDto.Comment;
            if (updateDto.IsApproved.HasValue) feedback.IsApproved = updateDto.IsApproved.Value;
            if (!string.IsNullOrEmpty(updateDto.Reply))
            {
                feedback.Reply = updateDto.Reply;
                feedback.RepliedAt = updateDto.RepliedAt ?? DateTime.UtcNow;
            }

            feedback.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(feedback);

            return new FeedbackDto
            {
                Id = feedback.Id,
                UserId = feedback.UserId,
                UserName = feedback.User.FullName,
                OrderId = feedback.OrderId,
                MenuItemId = feedback.MenuItemId,
                MenuItemName = feedback.MenuItem?.Name,
                Rating = feedback.Rating,
                Comment = feedback.Comment,
                IsApproved = feedback.IsApproved,
                CreatedAt = feedback.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                Reply = feedback.Reply,
                RepliedAt = feedback.RepliedAt
            };
        }

    }
}
