using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace VetAppointment.Lib.Infra.Repo
{
    public interface IBaseRepository<T>
    {
        Task<bool> AnyAsync(Expression<Func<T, bool>> exp = null);
        Task InsertAsync(T item);
        Task Update(T item);
        Task Remove(T[] data);
        Task Remove(T data);
        Task InsertAsync(T[] data);
        Task Update(T[] data);
        Task<T> GetAsync(Expression<Func<T, bool>> exp = null);
        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> exp = null);
        IQueryable<T> Where(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, bool disableTracking = false);
    }
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly DbSet<T> table;
        protected readonly VetAppointmentDbContext _context;
        public BaseRepository(VetAppointmentDbContext context)
        {

            _context = context;
            table = _context.Set<T>();
        }
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> exp = null)
        {
            return exp == null ? await table.AnyAsync() : await table.AnyAsync(exp);
        }
        public async Task<T> GetAsync(Expression<Func<T, bool>> exp = null)
        {
            return exp == null ? await table.FirstOrDefaultAsync() : await table.FirstOrDefaultAsync(exp);
        }

        public async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> exp = null)
        {
            return exp == null ? await table.ToListAsync() : await table.Where(exp).ToListAsync();
        }

        public async Task InsertAsync(T item)
        {
            await table.AddAsync(item);
        }

        public async Task InsertAsync(T[] data)
        {
            await table.AddRangeAsync(data);
        }

        public Task Remove(T[] data)
        {
            table.RemoveRange(data);
            return Task.CompletedTask;
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, bool disableTracking = false)
        {
            IQueryable<T> query = table;

            if (predicate != null)
            {
                query = table.Where(predicate);
            }
            if (include != null)
            {
                query = include(query);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            return query;
        }

        public Task Update(T item)
        {
            table.Update(item);
            return Task.CompletedTask;

        }

        public Task Update(T[] data)
        {
            table.UpdateRange(data);
            return Task.CompletedTask;
        }

        public Task Remove(T data)
        {

            table.Remove(data);
            return Task.CompletedTask;
        }
    }
}
