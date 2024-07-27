using Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class OrderWithItemAndOrderingSpesification : Specification<Order>
    {
        public OrderWithItemAndOrderingSpesification(string email) : base(o=>o.BuyerEmail==email)
        {
            AddInclude(o=>o.OrderItems);
            AddInclude(o => o.DeliveryMethod );
            AddOrderByDescending(o => o.OrderDate);
        }
        public OrderWithItemAndOrderingSpesification(int id,string email) : base(o=>o.Id==id &&o.BuyerEmail == email)
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.DeliveryMethod);
        }
    }
}
