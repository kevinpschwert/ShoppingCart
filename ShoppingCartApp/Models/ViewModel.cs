using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCartApp.Models
{
    public class ViewModel
    {
        public Item myItem { get; set; }
        public ShoppingCart myShoppingCart { get; set; }
        public List<Order> OrderList { get; set; }
        public ICollection<OrderDetail> myOrderDetails { get; set; }
    }
}