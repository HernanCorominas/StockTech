using StockTech.Application.DTOs.Dashboard;

namespace StockTech.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetMetricsAsync();
}
