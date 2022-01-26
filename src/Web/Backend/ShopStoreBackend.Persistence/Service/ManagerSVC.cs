#region 功能與歷史修改描述

/*
    描述:後台網頁資料庫處理
    建立日期:2021-12-03

    描述:程式碼風格調整
    修改日期:2022-01-07

    描述:新增庫存量檢查
    修改日期:2022-01-11

 */

#endregion

using Dapper;
using NLog;
using ShopStoreBackend.Domain.Models;
using ShopStoreBackend.Domain.Models.Interface;
using ShopStoreBackend.Domain.Models.Manager;
using ShopStoreBackend.Domain.Models.Manager.ViewModels;
using ShopStoreBackend.Domain.Models.Manager.ViewModels.Product;
using ShopStoreBackend.Domain.Models.Manager.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static ShopStoreBackend.Domain.Models.Manager.OrderStatusModel;
using static ShopStoreBackend.Domain.Models.Manager.PermissionDataModel;

namespace ShopStore.Models.Service
{
    public class ManagerSVC : IManager
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        private readonly SqlConnection CONNECTION;
        public ManagerSVC(SqlConnection connection)
        {
            CONNECTION = connection;
        }

        /// <summary>
        /// 取得菜單
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MenuModel>> GetMenu(int userId)
        {
            try
            {
                using var conn = CONNECTION;
                SqlMapper.GridReader result = await conn.QueryMultipleAsync("pro_bg_getMenu", new { userId }, commandType: System.Data.CommandType.StoredProcedure);
                List<MenuModel> menuModels = result.Read<MenuModel>().ToList();
                List<MenuSubModel> menuSubModels = result.Read<MenuSubModel>().ToList();
                List<MenuModel> menu = (from a in menuModels
                                        select new MenuModel
                                        {
                                            f_id = a.f_id,
                                            f_icon = a.f_icon,
                                            f_name = a.f_name,
                                            f_level = a.f_level,
                                            f_isdel = a.f_isdel,
                                            f_issys = a.f_issys,
                                            MenuSubModels = menuSubModels.Where(x => x.f_menuid == a.f_id).ToList()

                                        }).ToList();

                return menu;
            }
            catch (Exception ex)
            {
                LOGGER.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// 新增菜單
        /// </summary>
        /// <param name="menuModel"></param>
        /// <returns></returns>
        public bool AddMenu(MenuModel menuModel)
        {
            try
            {
                using var conn = CONNECTION;
                var result = conn.Execute("pro_bg_addMenu", menuModel, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return false;
            }
        }


        /// <summary>
        /// 新增子菜單
        /// </summary>
        /// <param name="menuModel"></param>
        /// <returns></returns>
        public async Task<bool> AddSubMenu(MenuViewModel model)
        {
            try
            {
                using var conn = CONNECTION;
                if (model.MainMenuItems != null && model.MainMenuItems.Count > 0)
                {
                    var result = await conn.ExecuteAsync("pro_bg_addManagerMainMenu", model.MainMenuItems, commandType: System.Data.CommandType.StoredProcedure);
                }

                if (model.SubItems != null && model.SubItems.Count > 0)
                {
                    var result = await conn.ExecuteAsync("pro_bg_addManagerSubMenu", model.SubItems, commandType: System.Data.CommandType.StoredProcedure);
                }

                if (model.MenuSubModels != null && model.MenuSubModels.Count > 0)
                {
                    var result = await conn.ExecuteAsync("pro_bg_editManagerSubMenu", model.MenuSubModels, commandType: System.Data.CommandType.StoredProcedure);
                }

            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 取訂單資料
        /// </summary>
        /// <returns></returns>
        public List<OrderManageViewModel> GetOrderList()
        {
            try
            {
                using var conn = CONNECTION;
                string sqlStr = "pro_bg_getOrders";
                var result = conn.Query(sqlStr);
                List<OrderManageViewModel> model = (List<OrderManageViewModel>)conn.Query<OrderManageViewModel>(sqlStr);

                return model;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return null;
            }
        }

        /// <summary>
        /// 取得訂單的設定
        /// </summary>
        /// <returns></returns>
        public OrderStatusModel GetOrderStatus()
        {
            try
            {
                using var conn = CONNECTION;
                var result = conn.QueryMultiple("pro_bg_getOrderSetting", commandType: System.Data.CommandType.StoredProcedure);

                OrderStatusModel model = new OrderStatusModel()
                {
                    CartgoState = result.Read<StatusProp>().ToDictionary(x => x.Code, x => x),
                    SipState = result.Read<StatusProp>().ToDictionary(x => x.Code, x => x)
                };

                return model;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "GetOrderStatus");
                return null;
            }
        }

        /// <summary>
        /// 刪除Order單
        /// isdel => 1, status => 4(已退貨)
        /// </summary>
        /// <returns></returns>
        public bool RemoveOrder(string ordernum)
        {
            try
            {
                using var conn = CONNECTION;
                bool result = conn.Query<bool>("pro_bg_delOrder", new { ordernum }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return true;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return false;
            }
        }

        /// <summary>
        /// 更新訂單
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public bool editOrder(List<Order> orders)
        {
            try
            {
                using var conn = CONNECTION;
                return conn.Execute("pro_bg_editOrder", orders, commandType: System.Data.CommandType.StoredProcedure) > 0;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return false;
            }
        }

        /// <summary>
        /// 新增後台帳號
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddUser(UserManageModel model)
        {
            try
            {
                using var conn = CONNECTION;
                return conn.Execute(@"pro_bg_addUser", model, commandType: System.Data.CommandType.StoredProcedure) == 1;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return false;
            }
        }

        /// <summary>
        /// 取得使用者ById
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserManageViewModels GetUser(UserLoginViewModel userLogin)
        {
            try
            {
                using var conn = CONNECTION;
                return (UserManageViewModels)conn.QueryFirstOrDefault<UserManageViewModels>("pro_bg_getUserById", new { userLogin.Account, userLogin.Pcode }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return null;
            }
        }


        /// <summary>
        /// 取得後台使用者
        /// </summary>
        /// <returns></returns>
        public List<UserManageViewModels> GetUsers()
        {
            try
            {
                using var conn = CONNECTION;
                var result = conn.QueryMultiple("pro_bg_getUsers", commandType: System.Data.CommandType.StoredProcedure);

                List<UserManageViewModel> userManageViewModels = result.Read<UserManageViewModel>().ToList();
                List<UserManageViewModels> model = userManageViewModels.Select(x => new UserManageViewModels
                {
                    ID = x.ID,
                    Account = x.Account,
                    Name = x.Name,
                    GroupId = x.GroupId,
                    GroupName = x.GroupName,
                    CreateTime = x.f_createTime.ToString("yyyy/MM/dd HH:mm:ss"),
                    UpdateTime = x.f_updateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                }).ToList();

                return model;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return null;
            }
        }

        /// <summary>
        /// 取得用戶權限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserPermission> GetUserPermissionsByID(int userId)
        {
            try
            {
                using var conn = CONNECTION;
                var userPermission = conn.Query<UserPermission, UserPermissionDetail, UserPermission>
                    ("pro_bg_getUsersAuth",
                    (o, c) =>
                    {
                        o.PermissionDetail = c;
                        return o;
                    },
                    new { userId }, splitOn: "MenuName", commandType: System.Data.CommandType.StoredProcedure).ToList();

                return userPermission;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return null;
            }
        }

        /// <summary>
        /// 更新使用者權限
        /// </summary>
        /// <param name="permissionData"></param>
        /// <returns></returns>
        public bool UpdatePermissionsByID(PermissionData permissionData)
        {
            try
            {
                List<PermissionModel> permissionModels = permissionData.PermissionDetails.Select(x => new PermissionModel
                {
                    f_userId = permissionData.UserId,
                    f_menuSubId = x.MenuId,
                    f_permissionCode = x.PermissionsCode,
                    f_updateTime = DateTime.Now,
                    f_groupId = permissionData.GroupId,
                    UpdateType = 1
                }).ToList();


                if (permissionModels.Count == 0)
                {
                    permissionModels.Add(new PermissionModel()
                    {
                        f_userId = permissionData.UserId,
                        f_menuSubId = 0,
                        f_permissionCode = 0,
                        f_updateTime = DateTime.Now,
                        f_groupId = permissionData.GroupId,
                        UpdateType = 0
                    });
                }

                using var conn = CONNECTION;
                return conn.Execute("pro_bg_editUserAuth", permissionModels, commandType: System.Data.CommandType.StoredProcedure) > 0;                
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return false;
            }
        }

        /// <summary>
        /// 刪除使用者
        /// </summary>
        /// <returns></returns>
        public bool RemoveUserByID(string userId)
        {
            try
            {
                using var conn = CONNECTION;
                return conn.Execute("pro_bg_delUser",
                                    new { userId },
                                    commandType: System.Data.CommandType.StoredProcedure) == 1;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "RemoveUserByID");
                return false;
            }
        }

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <returns></returns>
        public List<MemberManagerViewModel> GetMemberList()
        {
            try
            {
                using var conn = CONNECTION;
                return (List<MemberManagerViewModel>)conn.Query<MemberManagerViewModel>("pro_bg_getMembers",
                                    commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "GetMemberList");
                return null;
            }
        }

        /// <summary>
        /// 會員停權
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public bool SuspendByMemberId(int memberId, int isSuspend)
        {
            try
            {
                using var conn = CONNECTION;
                return conn.Execute("pro_bg_editMemberAuth",
                    new { memberId, isSuspend },
                    commandType: System.Data.CommandType.StoredProcedure) == 1;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, $"SuspendByID={memberId}");
                return false;
            }
        }

        /// <summary>
        /// 修改會員等級
        /// </summary>
        /// <param name="memberManageModel"></param>
        /// <returns></returns>
        public bool UpdateByMemberId(MemberManageModel memberManageModel)
        {
            try
            {
                using var conn = CONNECTION;
                return conn.Execute("pro_bg_editMemberLevel",
                    memberManageModel.MemberModel,
                    commandType: System.Data.CommandType.StoredProcedure) > 0;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, $"UpdateByMemberId={memberManageModel}");
                return false;
            }
        }

        /// <summary>
        /// 庫存量檢查
        /// </summary>
        /// <returns>回傳庫存量低的產品</returns>
        public Dictionary<int, InventoryViewModel> InventoryCheck()
        {
            try
            {
                using var conn = CONNECTION;
                var result = conn.QueryMultiple("pro_bg_checkProducts",
                    commandType: System.Data.CommandType.StoredProcedure);

                return result.Read<InventoryViewModel>().ToDictionary(x => x.Id, x => x);


                //return (Dictionary<string, InventoryViewModel>)result.Cast<IDictionary<string, object>>().Select(it => it.ToDictionary(it => it.Key, it => it.Value));
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, $"InventoryCheck Error");
                return null;
            }
        }
        
    }
}
