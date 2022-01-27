#region 功能與歷史修改描述

/*
    描述:儲存所有連接Hub的列表
    建立日期:2022-01-13

    描述:程式碼風格調整
    修改日期:2022-01-20

 */

#endregion


using MessageService.Hubs.Models;
using System.Collections.Generic;

namespace MessageService.Hubs.Models.Services
{
    public class ConUserService
    {
        /// <summary>
        /// 儲存後台 User 清單
        /// </summary>
        public List<ConUserModel> LIST;
        /// <summary>
        /// 儲存前台 User 清單
        /// </summary>
        public List<ConUserModel> ServerList;
        /// <summary>
        /// 已建立的群組
        /// </summary>
        public Dictionary<string, GroupUser> connectedGroup;


        public ConUserService()
        {
            LIST = new List<ConUserModel>();
            ServerList = new List<ConUserModel>();
            connectedGroup = new Dictionary<string, GroupUser>();
        }

        /// <summary>
        /// 加入User至清單
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<ConUserModel> AddList(ConUserModel user)
        {
            LIST.Add(user);
            return LIST;
        }
        /// <summary>
        /// 從清單移除User
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public List<ConUserModel> RemoveList(string connectionId)
        {
            var index = LIST.FindIndex(x => x.ConnectionID == connectionId);
            LIST.RemoveAt(index);
            return LIST;
        }
        /// <summary>
        /// 加入User至清單
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<ConUserModel> AddToServerList(ConUserModel user)
        {
            ServerList.Add(user);
            return ServerList;
        }
        /// <summary>
        /// 從清單移除User
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public List<ConUserModel> RemoveFromServerList(string connectionId)
        {
            var index = ServerList.FindIndex(x => x.ConnectionID == connectionId);
            ServerList.RemoveAt(index);
            return ServerList;
        }
        /// <summary>
        /// 群組
        /// </summary>
        public class GroupUser
        {
            public GroupUser()
            {
                Group = new List<ConnUser>();
            }

            public List<ConnUser> Group { get; set; }

            public class ConnUser
            {
                /// <summary>
                /// 群組ID
                /// </summary>
                /// <value></value>
                public string RoomID { get; set; }

            }
        }
    }
}
