using Microsoft.EntityFrameworkCore;
using StockTech.Application.DTOs.SystemSettings;
using StockTech.Application.Interfaces;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class SystemSettingService : ISystemSettingService
{
    private readonly IUnitOfWork _uow;

    public SystemSettingService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<SystemSettingDto?> GetByKeyAsync(string key)
    {
        var setting = await _uow.GetQueryable<SystemSetting>()
            .FirstOrDefaultAsync(s => s.Key == key);

        return setting is null ? null : new SystemSettingDto
        {
            Key = setting.Key,
            Value = setting.Value,
            Description = setting.Description
        };
    }

    public async Task<IEnumerable<SystemSettingDto>> GetAllAsync()
    {
        var settings = await _uow.GetQueryable<SystemSetting>()
            .ToListAsync();

        return settings.Select(s => new SystemSettingDto
        {
            Key = s.Key,
            Value = s.Value,
            Description = s.Description
        });
    }

    public async Task<SystemSettingDto> UpsertAsync(string key, UpdateSystemSettingDto dto)
    {
        var setting = await _uow.GetQueryable<SystemSetting>()
            .FirstOrDefaultAsync(s => s.Key == key);

        if (setting is null)
        {
            setting = new SystemSetting
            {
                Key = key,
                Value = dto.Value,
                Description = dto.Description
            };
            await _uow.AddAsync(setting);
        }
        else
        {
            setting.Value = dto.Value;
            if (dto.Description != null)
            {
                setting.Description = dto.Description;
            }
        }

        await _uow.CommitAsync();

        return new SystemSettingDto
        {
            Key = setting.Key,
            Value = setting.Value,
            Description = setting.Description
        };
    }
}
