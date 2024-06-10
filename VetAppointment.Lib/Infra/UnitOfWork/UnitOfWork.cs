using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetAppointment.Lib.Infra.Repo;

namespace VetAppointment.Lib.Infra.UnitOfWork
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task<int> SaveChangeAsync();
        IBaseRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        VetAppointmentDbContext DbContext { get; }
    }
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VetAppointmentDbContext _context;

        private Dictionary<Type, object> repositories;
        public UnitOfWork(VetAppointmentDbContext context)
        {
            _context = context;
        }

        public VetAppointmentDbContext DbContext => _context;

        public IBaseRepository<T> GetRepository<T>() where T : class
        {
            if (repositories == null)
            {
                repositories = new Dictionary<Type, object>();
            }

            var type = typeof(T);
            if (!repositories.ContainsKey(type))
            {
                repositories[type] = new BaseRepository<T>(_context);
            }

            return (IBaseRepository<T>)repositories[type];
        }

        public async Task<int> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private bool disposed = false;
        protected virtual async Task Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    await _context.DisposeAsync();
                }
            }
            this.disposed = true;
        }


        public async ValueTask DisposeAsync()
        {
            await Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void ChangeDatabase(string connectionString)
        {
            _context.Database.SetConnectionString(connectionString);
        }
    }
}
