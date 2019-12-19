using System;
using System.Collections.Generic;
using System.Text;

namespace PhoneStore.Models
{
    public class OrderModel
    {
        public OrderModel()
        {
            Carts = new List<CartModel>();
            Shipments = new List<ShipmentDetailModel>();
        }

        public string Code { get; set; }
        public List<CartModel> Carts { get; set; }
        public List<ShipmentDetailModel> Shipments { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public PaymentType Payment { get; set; }
        public string UserEmail { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }

        public enum PaymentType
        {
            COD = 0,
            Store = 1,
            Bank = 2,
        }
        public enum OrderStatus
        {
            Ordered = 0,
            Accepted = 1,
            Deliverd = 2,
            Done = 3,
            Cancelled = 4,
        }
    }
}
