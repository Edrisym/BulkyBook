using System;
using WebApp.DataAccess.Repository.IRepository;
using WebAppMod.Models;
namespace WebApp.DataAccess.Repository
{
    public class ShoppingCardRepository : Repository<ShoppingCard>, IShoppingCardRepository
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCardRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public int DecrementCount(ShoppingCard shoppingCard, int count)
        {
            shoppingCard.Count -= count;
            return shoppingCard.Count;
        }

        public int IncrementCount(ShoppingCard shoppingCard, int count)
        {
            shoppingCard.Count += count;
            return shoppingCard.Count;
        }
    }
}