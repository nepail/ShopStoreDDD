using System;
using System.Collections.Generic;

namespace ShopStoreBackend.Domain.Models.Manager.ViewModels
{
    public class UserManageViewModel
    {
        public DateTime f_createTime { get; set; }
        public DateTime f_updateTime { get; set; }
        public int ID { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string CreateTime { get; set; }
        public string UpdateTime { get; set; }
        public List<UserPermission> UserPermissions { get; set; }
    }

    public class UserManageViewModels
    {
        public int ID { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }

        public string CreateTime { get; set; }
        public string UpdateTime { get; set; }
    }

    public class UserPermission
    {
        public int MenuId { get; set; }
        public UserPermissionDetail PermissionDetail { get; set; }
    }

    public class UserPermissionDetail
    {
        public string MenuName { get; set; }
        public int PermissionCode { get; set; }
    }
}
