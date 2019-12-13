using PhoneStore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhoneStore.ViewModels
{
    public class MyOrderDetailViewModel
    {
        public MyOrderDetailViewModel()
        {
            
        }
        public MyOrderDetailViewModel(OrderModel order)
        {
            Order = order;
        }

        #region Properties
        public OrderModel Order { get; set; }
        #endregion
    }
}
