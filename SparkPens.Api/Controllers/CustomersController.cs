using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkPens.Api.Data;
using SparkPens.Api.DTOs;
using SparkPens.Api.Models;

namespace SparkPens.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _context;
    public CustomersController(AppDbContext context) => _context = context;

    // GET /api/customers (Pagination & Search)
    [HttpGet]
    public async Task<IActionResult> GetCustomers([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var query = _context.Customers.Where(c => !c.IsDeleted).AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(c => c.Name.Contains(search) || c.Email.Contains(search));

        var totalItems = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Ok(new { totalItems, page, pageSize, data = items });
    }

    // POST /api/customers (Create)
    [HttpPost]
    public async Task<IActionResult> Create(CustomerDto dto)
    {
        var customer = new Customer { 
            Name = dto.Name, Email = dto.Email, Phone = dto.Phone, 
            Address = dto.Address, Status = dto.Status 
        };
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCustomers), new { id = customer.Id }, customer);
    }

    // GET /api/customers/{id} (View Detail)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomer(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null || customer.IsDeleted) 
            return NotFound(new { message = "Customer tidak ditemukan atau sudah dihapus." });

        return Ok(customer);
    }

    // PUT /api/customers/{id} (Update - Baru!)
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CustomerDto dto)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null || customer.IsDeleted) return NotFound();

        customer.Name = dto.Name;
        customer.Email = dto.Email;
        customer.Phone = dto.Phone;
        customer.Address = dto.Address;
        customer.Status = dto.Status;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/customers/{id} (Soft Delete - Sesuai Kriteria)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();

        customer.IsDeleted = true;
        customer.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}