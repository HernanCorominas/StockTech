using Microsoft.AspNetCore.Mvc;
using StockTech.Application.DTOs.Categories;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;

namespace StockTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public CategoriesController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _uow.Categories.GetAllAsync();
        return Ok(categories.Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.IsActive)));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id)
    {
        var c = await _uow.Categories.GetByIdAsync(id);
        if (c == null) return NotFound();
        return Ok(new CategoryDto(c.Id, c.Name, c.Description, c.IsActive));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            IsActive = true
        };

        await _uow.Categories.AddAsync(category);
        await _uow.CommitAsync();

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, 
            new CategoryDto(category.Id, category.Name, category.Description, category.IsActive));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateCategoryDto dto)
    {
        if (id != dto.Id) return BadRequest();

        var category = await _uow.Categories.GetByIdAsync(id);
        if (category == null) return NotFound();

        category.Name = dto.Name.Trim();
        category.Description = dto.Description?.Trim();
        category.IsActive = dto.IsActive;

        _uow.Categories.Update(category);
        await _uow.CommitAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await _uow.Categories.GetByIdAsync(id);
        if (category == null) return NotFound();

        _uow.Categories.Delete(category);
        await _uow.CommitAsync();

        return NoContent();
    }
}
