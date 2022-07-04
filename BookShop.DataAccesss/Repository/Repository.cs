using BookShop.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BookShop.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace BookShop.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> DbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            DbSet = _context.Set<T>();
        }
        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = DbSet;
            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = DbSet;
            query = query.Where(filter);
            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
           DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            DbSet.RemoveRange(entity);
        }
    }
}
