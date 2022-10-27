using System;
using WebApp.DataAccess.Repository.IRepository;
using WebAppMod.Models;

namespace WebApp.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;

        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Company obj)
        {
            _db.Companies.UpdateRange(obj);
        }
    }
}

