using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;

namespace BookShop.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private ApplicationDbContext _context;
        public CoverTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(CoverType coverType)
        {
            _context.CoverTypes.Update(coverType);
        }
    }
}
