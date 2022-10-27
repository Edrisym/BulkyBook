using System;
using WebAppMod.Models;

namespace WebApp.DataAccess.Repository.IRepository
{
    public interface IShoppingCardRepository : IRepository<ShoppingCard>
    {
        int IncrementCount(ShoppingCard shoppingCard, int count);
        int DecrementCount(ShoppingCard shoppingCard, int count);

    }
}

