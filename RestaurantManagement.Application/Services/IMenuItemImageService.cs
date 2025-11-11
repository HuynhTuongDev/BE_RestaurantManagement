using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Application.Services
{
    public interface IMenuItemImageService
    {
        // Upload
        Task<MenuItemImage> UploadMenuItemImageAsync(int menuItemId, Stream fileStream, string fileName, CancellationToken cancellationToken = default);

        // Get
        Task<MenuItemImage?> GetImageByIdAsync(int imageId);
        Task<IEnumerable<MenuItemImage>> GetImagesByMenuItemIdAsync(int menuItemId);
        Task<PaginatedResponse<MenuItemImage>> GetPaginatedImagesByMenuItemIdAsync(int menuItemId, PaginationRequest pagination);

        // Delete
        Task<bool> DeleteImageAsync(int imageId);
    }
}
