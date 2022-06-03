using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAR_SPARE_PARTS.Models.Order;
using CAR_SPARE_PARTS.Models.Store;

namespace CAR_SPARE_PARTS.Classes
{
    public class OrdersData
    {

        int UserID { get; set; }

        public OrdersData(int userId)
        {
            UserID = userId;
        }

        public void AddOrder(Order order)
        {
            using (var dbContext = new OrderContext())
            {
                Order existOrder = dbContext.Orders.SingleOrDefault(o => o.UserID == UserID);
                if (existOrder != null)
                {
                    existOrder.Name = order.Name;
                    existOrder.LastName = order.LastName;
                    existOrder.MiddleName = order.MiddleName;
                    existOrder.Address = order.Address;
                    existOrder.Date = order.Date;
                    existOrder.PriceOfOrder = order.PriceOfOrder;
                }
                else
                {
                    dbContext.Orders.Add(order);
                }
                dbContext.SaveChanges();
            }
            using (var dbContext = new CartProductListContext())
            {
                dbContext.CartProductsList.RemoveRange(dbContext.CartProductsList);
                dbContext.SaveChanges();
            }
        }

        public Order GetExistOrder()
        {
            using (var dbContext = new OrderContext())
            {
                return dbContext.Orders.SingleOrDefault(o => o.UserID == UserID);
            }
        }

    }
}