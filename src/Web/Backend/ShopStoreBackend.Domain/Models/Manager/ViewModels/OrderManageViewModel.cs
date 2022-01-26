using System;

namespace ShopStoreBackend.Domain.Models.Manager.ViewModels
{
    public class OrderManageViewModel
    {
        private DateTime _date;

        public string Num { get; set; }
        public string MemberId { get; set; }
        public string MemberAccount { get; set; }
        public string Date { get => _date.ToString("yyyy/MM/dd HH:mm:ss"); set { } }
        public string Status { get; set; }
        public string StatusBadge { get; set; }
        public string ShippingMethod { get; set; }
        public string ShippingBadge { get; set; }
        public int Total { get; set; }
        public int IsPaid { get; set; }
        public int IsDel { get; set; }
    }
}
