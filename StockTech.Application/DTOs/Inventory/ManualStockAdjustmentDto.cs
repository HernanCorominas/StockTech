using StockTech.Domain.Enums;

namespace StockTech.Application.DTOs.Inventory;

/// <summary>
/// DTO for submitting a manual stock adjustment (Entry, Adjustment, Transfer).
/// Quantity is signed: positive = add stock, negative = remove stock.
/// </summary>
public record ManualStockAdjustmentDto(
    Guid ProductId,
    Guid? VariantId,
    decimal Quantity,           // positive = entry, negative = exit
    TransactionType Type,       // Purchase=1, Sale=2, Adjustment=3, Transfer=4
    string? ReferenceNumber,
    Guid? BranchId
);
