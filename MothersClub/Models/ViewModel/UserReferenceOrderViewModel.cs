using System;

namespace MothersClub.Models.ViewModel
{
    public class UserReferenceOrderViewModel
    {
        public int orderId { get; set; }
        public decimal orderPrice { get; set; }
        public DateTime orderDate { get; set; }
        public int userId { get; set; }
    }

    public class CancelOrderViewModel
    {
        public int userId { get; set; }
        public int orderId { get; set; }
    }
}