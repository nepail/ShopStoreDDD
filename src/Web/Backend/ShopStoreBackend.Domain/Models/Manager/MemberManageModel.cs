using System.Collections.Generic;

namespace ShopStoreBackend.Domain.Models.Manager
{
    public class MemberManageModel
    {
        public List<MemberManagerViewModel> MemberModel { get; set; }
    }

    public class MemberManagerViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Account { get; set; }
        public int Level { get; set; }
        public int Money { get; set; }
        public int IsSuspend { get; set; }
    }
}
