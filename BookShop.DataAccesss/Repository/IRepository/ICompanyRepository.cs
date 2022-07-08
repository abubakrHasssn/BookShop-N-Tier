﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Models;

namespace BookShop.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company company);
    }
}
