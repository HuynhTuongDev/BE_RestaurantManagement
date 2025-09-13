using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Application.Services
{
    public interface IMenuItemImageService
    {
        Task<MenuItemImage> UploadMenuItemImageAsync(int menuItemId, Stream fileStream, string fileName, CancellationToken cancellationToken = default);
    }
}
