namespace Todo.Infrastructure.Repositories;

using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class ListRepository : IListRepository
{
    private readonly TodoDbContext _context;

    public ListRepository(TodoDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<TodoList?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TodoLists
            .Include(l => l.Shares)
            .ThenInclude(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TodoList>> GetForUserAsync(Guid userId, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _context.TodoLists
            .Where(l => l.OwnerId == userId)
            .Include(x => x.Shares)
            .ThenInclude(x => x.User)
            .Skip(skip).Take(take)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(TodoList list, CancellationToken cancellationToken = default)
    {
        await _context.TodoLists.AddAsync(list, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(TodoList list, CancellationToken cancellationToken = default)
    {
        _context.TodoLists.Update(list);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(TodoList list, CancellationToken cancellationToken = default)
    {
        _context.TodoLists.Remove(list);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddShareAsync(TodoListShare share, CancellationToken cancellationToken = default)
    {
        _context.TodoListShares.Add(share);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveShareAsync(TodoListShare share, CancellationToken cancellationToken = default)
    {
        _context.TodoListShares.Remove(share);
        await _context.SaveChangesAsync(cancellationToken);
    }
}