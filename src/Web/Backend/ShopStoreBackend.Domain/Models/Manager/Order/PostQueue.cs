using System;
using System.Collections.Generic;
using System.Text;

namespace ShopStoreBackend.Domain.Models.Manager
{
    public class PostQueue
    {
        public string orderUser { get; set; }
        public string orderId { get; set; }
        public string statMsg { get; set; }
    }
}
