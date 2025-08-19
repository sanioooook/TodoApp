namespace Todo.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Domain.Entities;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<TodoList> TodoLists => Set<TodoList>();
    public DbSet<TodoListShare> TodoListShares => Set<TodoListShare>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User -> TodoLists (1 : many)
        modelBuilder.Entity<TodoList>()
            .HasOne(t => t.Owner)
            .WithMany(u => u.TodoLists)
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        // TodoListShare composite key
        modelBuilder.Entity<TodoListShare>()
            .HasKey(s => new { s.TodoListId, s.UserId });

        // TodoList -> Shares (1 : many)
        modelBuilder.Entity<TodoListShare>()
            .HasOne(s => s.TodoList)
            .WithMany(l => l.Shares)
            .HasForeignKey(s => s.TodoListId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> Shares (1 : many)
        modelBuilder.Entity<TodoListShare>()
            .HasOne(s => s.User)
            .WithMany(u => u.SharedLists)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Default values for CreatedAt
        modelBuilder.Entity<User>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<TodoList>()
            .Property(l => l.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<TodoList>()
            .Property(l => l.UpdatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<TodoListShare>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("now()");
    }
}