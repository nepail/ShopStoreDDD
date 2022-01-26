using System.Collections.Generic;

namespace ShopStoreBackend.Domain.Models.Manager
{
    public class OrderStatusModel
    {

        public OrderStatusModel()
        {
            CartgoState = new Dictionary<string, StatusProp>();
            SipState = new Dictionary<string, StatusProp>();
        }
        public Dictionary<string, StatusProp> CartgoState { get; set; }
        public Dictionary<string, StatusProp> SipState { get; set; }

        public class StatusProp
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Style { get; set; }
        }
    }
}
