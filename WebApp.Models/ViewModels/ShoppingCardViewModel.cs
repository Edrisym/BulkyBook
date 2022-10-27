using System;
namespace WebAppMod.Models.ViewModels
{
    public class ShoppingCardViewModel
    {
        public IEnumerable<ShoppingCard> ListCard { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}


