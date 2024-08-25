using BillProcessorAPI.Data;
using BillProcessorAPI.Repositories.Interfaces;
using Infrastructure.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BillProcessorAPI.Repositories.Implementations
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly BillProcessorDbContext _context;

        public Repository(BillProcessorDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            await _context.Set<TEntity>().AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            await _context.Set<TEntity>().AddRangeAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().Any(predicate);
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().AnyAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().CountAsync(predicate);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);

            if (entity == null) return null;

            return entity;
        }

        public void Remove(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _context.Set<TEntity>().Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }
        public Task<TEntity> SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().SingleOrDefaultAsync(predicate);
        }

        public Task<TEntity> SingleOrDefaultNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(predicate);
        }

        public Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }
        public Task<TEntity> FirstOrDefaultNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(predicate);
        }
        public void UpdateRange(IEnumerable<TEntity> entity)
        {
            _context.Set<TEntity>().UpdateRange(entity);
        }

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> expression)
        {
            return _context.Set<TEntity>().AsQueryable().Where(expression);
        }

        public async Task<TEntity> AddAndReturnValue(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Validation error.");

            var add = await _context.AddAsync(entity);
            TEntity propertyInfo = add.Entity;
            await SaveChangesAsync();
            return propertyInfo;
        }
        
        public virtual async Task BeginTransaction(Func<Task> action, bool shouldCommitTransaction = true)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await action().ConfigureAwait(false);

                await SaveChangesAsync().ConfigureAwait(false);

                if(shouldCommitTransaction)
                    await transaction.CommitAsync().ConfigureAwait(false);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }

    }
}
