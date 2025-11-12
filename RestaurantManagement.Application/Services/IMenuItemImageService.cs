using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.DTOs.Common;

namespace RestaurantManagement.Application.Services
{
    public interface IMenuItemImageService
    {
        // Upload
        Task<MenuItemImageDto> UploadMenuItemImageAsync(int menuItemId, Stream fileStream, string fileName, CancellationToken cancellationToken = default);

        // Get
        Task<MenuItemImageDto?> GetImageByIdAsync(int imageId);
        Task<IEnumerable<MenuItemImageDto>> GetImagesByMenuItemIdAsync(int menuItemId);
        Task<PaginatedResponse<MenuItemImageDto>> GetPaginatedImagesByMenuItemIdAsync(int menuItemId, PaginationRequest pagination);

        // Delete
        Task<bool> DeleteImageAsync(int imageId);
    }
}
