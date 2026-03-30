namespace StockTech.Application.DTOs.SystemSettings;

public class UpdateSystemSettingDto
{
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Password { get; set; }
}
