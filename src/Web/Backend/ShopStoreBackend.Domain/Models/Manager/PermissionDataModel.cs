using System;
using System.Collections.Generic;

namespace ShopStoreBackend.Domain.Models.Manager
{
    public class PermissionDataModel
    {
        public class PermissionData
        {
            public PermissionData()
            {
                PermissionDetails = new List<PermissionDetail>();
            }

            public int UserId { get; set; }
            public int GroupId { get; set; }
            public List<PermissionDetail> PermissionDetails { get; set; }
        }

        public class PermissionDetail
        {
            public int MenuId { get; set; }
            public int PermissionsCode { get; set; }
        }

        public class PermissionModel
        {
            public int f_userId { get; set; } = 0;
            public int f_menuSubId { get; set; } = 0;
            public int f_permissionCode { get; set; } = 0;
            public int f_groupId { get; set; } = 0;
            public DateTime f_updateTime { get; set; }
            public int UpdateType { get; set; }
        }

    }
}
