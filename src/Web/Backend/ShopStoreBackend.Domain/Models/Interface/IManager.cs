using ShopStoreBackend.Domain.Models.Manager;
using ShopStoreBackend.Domain.Models.Manager.ViewModels;
using ShopStoreBackend.Domain.Models.Manager.ViewModels.Product;
using ShopStoreBackend.Domain.Models.Manager.ViewModels.User;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ShopStoreBackend.Domain.Models.Manager.PermissionDataModel;

namespace ShopStoreBackend.Domain.Models.Interface
{
    /// <summary>
    /// 後台 Interface
    /// </summary>
    public interface IManager
    {
        /// <summary>
        /// 取得菜單
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MenuModel>> GetMenu(int userid);

        /// <summary>
        /// 新增菜單
        /// </summary>
        /// <param name="menuModel"></param>
        /// <returns></returns>
        public bool AddMenu(MenuModel menuModel);

        /// <summary>
        /// 新增子菜單 && 變更狀態
        /// </summary>
        /// <param name="menuModel"></param>
        /// <returns></returns>
        Task<bool> AddSubMenu(MenuViewModel model);

        /// <summary>
        /// 取訂單列表
        /// </summary>
        /// <returns></returns>
        public List<OrderManageViewModel> GetOrderList();

        /// <summary>
        /// 取所有狀態
        /// </summary>
        /// <returns></returns>
        public OrderStatusModel GetOrderStatus();

        /// <summary>
        /// 刪除訂單
        /// </summary>
        /// <param name="ordernum"></param>
        /// <returns></returns>
        public bool RemoveOrder(string ordernum);

        /// <summary>
        /// 更新訂單
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public bool editOrder(List<Order> orders);

        /// <summary>
        /// 新增後台帳號
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddUser(UserManageModel model);

        /// <summary>
        /// 取得使用者ById
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserManageViewModels GetUser(UserLoginViewModel userLogin);

        /// <summary>
        /// 取得所有後台使用者
        /// </summary>
        /// <returns></returns>
        public List<UserManageViewModels> GetUsers();

        /// <summary>
        /// 取得後台使用者
        /// </summary>
        /// <returns></returns>
        public List<UserPermission> GetUserPermissionsByID(int userId);

        /// <summary>
        /// 更新使用者權限
        /// </summary>
        /// <param name="permissionData"></param>
        /// <returns></returns>
        public bool UpdatePermissionsByID(PermissionData permissionData);

        /// <summary>
        /// 刪除使用者
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool RemoveUserByID(string userId);

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <returns></returns>
        public List<MemberManagerViewModel> GetMemberList();

        /// <summary>
        /// 會員停權
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public bool SuspendByMemberId(int memberId, int isSuspend);

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="memberManageModel"></param>
        /// <returns></returns>
        public bool UpdateByMemberId(MemberManageModel memberManageModel);

        /// <summary>
        /// 庫存量檢查
        /// </summary>
        /// <returns>回傳庫存量低的產品</returns>
        public Dictionary<int, InventoryViewModel> InventoryCheck();
    }
}
