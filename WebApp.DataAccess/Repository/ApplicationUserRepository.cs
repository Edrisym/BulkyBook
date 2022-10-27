using System;
using WebApp.DataAccess.Repository.IRepository;
using WebAppMod.Models;
namespace WebApp.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //public void Update(Category obj)
        //{
        //    _db.Categories.Update(obj);
        //}
    }
}

