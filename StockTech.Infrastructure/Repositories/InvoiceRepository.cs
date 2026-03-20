using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;

namespace StockTech.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    protected readonly StockTechDbContext _ctx;
    public InvoiceRepository(StockTechDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Invoice>> GetAllAsync() =>
        await _ctx.Invoices
            .Include(i => i.Client)
            .Include(i => i.Items).ThenInclude(item => item.Product)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();

    public async Task<(IEnumerable<Invoice> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search)
    {
        var query = _ctx.Invoices
            .Include(i => i.Client)
            .Include(i => i.Items).ThenInclude(item => item.Product)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(i => i.InvoiceNumber.Contains(search) || (i.Client != null && i.Client.Name.Contains(search)));
        }

        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(i => i.InvoiceDate)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to) =>
        await _ctx.Invoices
            .Include(i => i.Client)
            .Include(i => i.Items).ThenInclude(item => item.Product)
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();

    public async Task<Invoice?> GetByIdAsync(Guid id) =>
        await _ctx.Invoices
            .Include(i => i.Client)
            .Include(i => i.Items).ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(i => i.Id == id);

    public async Task AddAsync(Invoice invoice) => await _ctx.Invoices.AddAsync(invoice);

    public void Update(Invoice invoice) => _ctx.Invoices.Update(invoice);
}
