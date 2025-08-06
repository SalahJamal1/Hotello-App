using Microsoft.EntityFrameworkCore;
using MyApplication.Contracts;
using MyApplication.Data;

namespace MyApplication.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly HotelDBContext _context;

    public GenericRepository(HotelDBContext context)
    {
        _context = context;
    }

    public async Task<T> GetAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        _context.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }
}