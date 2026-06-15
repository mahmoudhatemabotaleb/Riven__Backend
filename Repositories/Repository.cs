using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using System.Linq.Expressions;

namespace RivenBackend.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext Context;
        protected readonly DbSet<T> DbSet;

        public Repository(AppDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

        public virtual async Task<IReadOnlyList<T>> GetAllAsync() => await DbSet.ToListAsync();

        public virtual async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await DbSet.Where(predicate).ToListAsync();

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) =>
            await DbSet.FirstOrDefaultAsync(predicate);

        public void Add(T entity) => DbSet.Add(entity);

        public void Update(T entity) => DbSet.Update(entity);

        public void Remove(T entity) => DbSet.Remove(entity);

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) =>
            await DbSet.AnyAsync(predicate);

        public async Task SaveChangesAsync() => await Context.SaveChangesAsync();
    }
}
