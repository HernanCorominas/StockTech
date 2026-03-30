using StockTech.Application.DTOs.SystemSettings;

namespace StockTech.Application.Interfaces;

public interface ISystemSettingService
{
    Task<SystemSettingDto?> GetByKeyAsync(string key);
    Task<IEnumerable<SystemSettingDto>> GetAllAsync();
    Task<SystemSettingDto> UpsertAsync(string key, UpdateSystemSettingDto dto);
}
