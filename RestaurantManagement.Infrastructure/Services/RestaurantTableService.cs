using Microsoft.Extensions.Logging;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Domain.Interfaces.Repositories;

namespace RestaurantManagement.Infrastructure.Services
{
    /// <summary>
    /// Restaurant Table service implementation with logging and validation
    /// </summary>
    public class RestaurantTableService : IRestaurantTableService
    {
        private readonly IRestaurantTableRepository _restaurantTableRepository;
        private readonly ILogger<RestaurantTableService> _logger;

        public RestaurantTableService(
            IRestaurantTableRepository restaurantTableRepository,
            ILogger<RestaurantTableService> logger)
        {
            _restaurantTableRepository = restaurantTableRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get table by id
        /// </summary>
        public async Task<RestaurantTable?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting RestaurantTable {TableId}", id);

                var table = await _restaurantTableRepository.GetByIdAsync(id);

                if (table == null)
                    _logger.LogWarning("RestaurantTable {TableId} not found", id);
                else
                    _logger.LogInformation("Successfully retrieved RestaurantTable {TableId}", id);

                return table;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting RestaurantTable {TableId}", id);
                throw;
            }
        }

        /// <summary>
        /// Get all tables
        /// </summary>
        public async Task<IEnumerable<RestaurantTable>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Getting all RestaurantTables");

                var tables = await _restaurantTableRepository.GetAllAsync();

                _logger.LogInformation("Successfully retrieved {Count} RestaurantTables", tables.Count());

                return tables;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all RestaurantTables");
                throw;
            }
        }

        /// <summary>
        /// Get paginated tables
        /// </summary>
        public async Task<PaginatedResponse<RestaurantTable>> GetPaginatedAsync(PaginationRequest pagination)
        {
            try
            {
                _logger.LogInformation(
                    "Getting paginated RestaurantTables - Page: {PageNumber}, Size: {PageSize}",
                    pagination.PageNumber,
                    pagination.PageSize);

                // Cast to base repository to access GetPaginatedAsync
                var baseRepo = _restaurantTableRepository as IBaseRepository<RestaurantTable>;
                if (baseRepo == null)
                {
                    _logger.LogError("Repository does not implement IBaseRepository");
                    throw new InvalidOperationException("Repository does not support pagination");
                }

                var paginatedTables = await baseRepo.GetPaginatedAsync(pagination);

                _logger.LogInformation(
                    "Retrieved {Count} tables out of {Total} - Page {PageNumber}/{TotalPages}",
                    paginatedTables.Data.Count(),
                    paginatedTables.TotalRecords,
                    paginatedTables.PageNumber,
                    paginatedTables.TotalPages);

                return paginatedTables;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated RestaurantTables");
                throw;
            }
        }

        /// <summary>
        /// Search table by table number
        /// </summary>
        public async Task<IEnumerable<RestaurantTable>> SearchAsync(int tableNumber)
        {
            try
            {
                _logger.LogInformation("Searching RestaurantTable with number: {TableNumber}", tableNumber);

                if (tableNumber <= 0)
                {
                    _logger.LogWarning("Invalid table number: {TableNumber}", tableNumber);
                    return new List<RestaurantTable>();
                }

                var tables = await _restaurantTableRepository.SearchAsync(tableNumber);

                _logger.LogInformation("Search found {Count} tables with number: {TableNumber}",
                    tables.Count(), tableNumber);

                return tables;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching RestaurantTable with number: {TableNumber}", tableNumber);
                throw;
            }
        }

        /// <summary>
        /// Search tables with pagination
        /// </summary>
        public async Task<PaginatedResponse<RestaurantTable>> SearchPaginatedAsync(
            int tableNumber, 
            PaginationRequest pagination)
        {
            try
            {
                _logger.LogInformation(
                    "Searching paginated RestaurantTables with number: {TableNumber} - Page: {PageNumber}, Size: {PageSize}",
                    tableNumber,
                    pagination.PageNumber,
                    pagination.PageSize);

                if (tableNumber <= 0)
                {
                    _logger.LogWarning("Invalid table number: {TableNumber}", tableNumber);
                    return PaginatedResponse<RestaurantTable>.Create(
                        new List<RestaurantTable>(),
                        pagination.PageNumber,
                        pagination.PageSize,
                        0);
                }

                // Use the base repository search
                var allTables = await _restaurantTableRepository.SearchAsync(tableNumber);

                // Calculate pagination
                var totalCount = allTables.Count();
                var paginatedData = allTables
                    .Skip(pagination.SkipCount)
                    .Take(pagination.PageSize)
                    .ToList();

                var result = PaginatedResponse<RestaurantTable>.Create(
                    paginatedData,
                    pagination.PageNumber,
                    pagination.PageSize,
                    totalCount);

                _logger.LogInformation(
                    "Found {Count} tables out of {Total} matching number: {TableNumber}",
                    result.Data.Count(),
                    result.TotalRecords,
                    tableNumber);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching paginated RestaurantTables with number: {TableNumber}", tableNumber);
                throw;
            }
        }

        /// <summary>
        /// Add restaurant table
        /// </summary>
        public async Task<RestaurantTable> AddAsync(RestaurantTableCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Creating RestaurantTable {TableNumber}", dto.TableNumber);

                ValidateTableDto(dto);

                var restaurantTable = new RestaurantTable
                {
                    TableNumber = dto.TableNumber,
                    Seats = dto.Seats,
                    Status = TableStatus.Available,
                    Location = dto.Location
                };

                await _restaurantTableRepository.AddAsync(restaurantTable);

                _logger.LogInformation("Successfully created RestaurantTable {TableNumber}", dto.TableNumber);

                return restaurantTable;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating RestaurantTable");
                throw;
            }
        }

        /// <summary>
        /// Update restaurant table
        /// </summary>
        public async Task UpdateAsync(int id, RestaurantTableCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Updating RestaurantTable {TableId}", id);

                var table = await _restaurantTableRepository.GetByIdAsync(id);
                if (table == null)
                {
                    _logger.LogWarning("RestaurantTable {TableId} not found", id);
                    throw new KeyNotFoundException($"Table {id} not found");
                }

                ValidateTableDto(dto);

                table.TableNumber = dto.TableNumber;
                table.Seats = dto.Seats;
                table.Location = dto.Location;

                await _restaurantTableRepository.UpdateAsync(table);

                _logger.LogInformation("Successfully updated RestaurantTable {TableId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating RestaurantTable {TableId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete restaurant table
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting RestaurantTable {TableId}", id);

                var table = await _restaurantTableRepository.GetByIdAsync(id);
                if (table == null)
                {
                    _logger.LogWarning("RestaurantTable {TableId} not found", id);
                    throw new KeyNotFoundException($"Table {id} not found");
                }

                await _restaurantTableRepository.DeleteAsync(id);

                _logger.LogInformation("Successfully deleted RestaurantTable {TableId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting RestaurantTable {TableId}", id);
                throw;
            }
        }

        /// <summary>
        /// Reserve table
        /// </summary>
        public async Task<bool> ReserveAsync(int id)
        {
            try
            {
                _logger.LogInformation("Reserving RestaurantTable {TableId}", id);

                var table = await _restaurantTableRepository.GetByIdAsync(id);
                if (table == null)
                {
                    _logger.LogWarning("RestaurantTable {TableId} not found", id);
                    return false;
                }

                if (table.Status != TableStatus.Available)
                {
                    _logger.LogWarning("RestaurantTable {TableId} is not available. Status: {Status}",
                        id, table.Status);
                    return false;
                }

                table.Status = TableStatus.Reserved;

                await _restaurantTableRepository.UpdateAsync(table);

                _logger.LogInformation("Successfully reserved RestaurantTable {TableId}", id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reserving RestaurantTable {TableId}", id);
                throw;
            }
        }

        /// <summary>
        /// Cancel reservation
        /// </summary>
        public async Task<bool> CancelReservationAsync(int id)
        {
            try
            {
                _logger.LogInformation("Canceling reservation for RestaurantTable {TableId}", id);

                var table = await _restaurantTableRepository.GetByIdAsync(id);
                if (table == null)
                {
                    _logger.LogWarning("RestaurantTable {TableId} not found", id);
                    return false;
                }

                if (table.Status != TableStatus.Reserved)
                {
                    _logger.LogWarning("RestaurantTable {TableId} is not reserved. Status: {Status}",
                        id, table.Status);
                    return false;
                }

                table.Status = TableStatus.Available;

                await _restaurantTableRepository.UpdateAsync(table);

                _logger.LogInformation("Successfully canceled reservation for RestaurantTable {TableId}", id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling reservation for RestaurantTable {TableId}", id);
                throw;
            }
        }

        /// <summary>
        /// Validate restaurant table DTO
        /// </summary>
        private void ValidateTableDto(RestaurantTableCreateDto dto)
        {
            _logger.LogDebug("Validating RestaurantTableCreateDto");

            if (dto.TableNumber <= 0)
                throw new ArgumentException("Table number must be greater than 0");

            if (dto.Seats <= 0)
                throw new ArgumentException("Seats must be greater than 0");

            if (dto.Seats > 20)
                throw new ArgumentException("Seats cannot exceed 20");
        }
    }
}
