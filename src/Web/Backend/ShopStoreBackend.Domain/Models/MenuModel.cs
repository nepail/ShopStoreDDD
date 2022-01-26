using System.Collections.Generic;

namespace ShopStoreBackend.Domain.Models
{
    public class MenuModel
    {
        public MenuModel()
        {
            MenuSubModels = new List<MenuSubModel>();
        }

        /// <summary>
        /// ID
        /// </summary>
        public int f_id { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string f_name { get; set; }
        /// <summary>
        /// ICON
        /// </summary>
        public string f_icon { get; set; }
        /// <summary>
        /// 菜單等級
        /// </summary>
        public int f_level { get; set; }
        /// <summary>
        /// 是否開啟
        /// </summary>
        public int f_isopen { get; set; }
        /// <summary>
        /// 是否系統
        /// </summary>
        public int f_issys { get; set; }
        /// <summary>
        /// 是否刪除
        /// </summary>
        public int f_isdel { get; set; }

        public List<MenuSubModel> MenuSubModels { get; set; }
    }

    public class MenuViewModel
    {
        /// <summary>
        /// 欲更新的子選單
        /// </summary>
        public List<MenuSubModel> MenuSubModels { get; set; }
        /// <summary>
        /// 新加入的子選單
        /// </summary>
        public List<SubItem> SubItems { get; set; }
        /// <summary>
        /// 主選單item
        /// </summary>
        public List<MainMenuItem> MainMenuItems { get; set; }
    }


    public class MenuSubModel
    {
        public int f_id { get; set; }
        public int f_menuid { get; set; }
        public string f_name { get; set; }
        public string f_controller { get; set; }
        public int f_level { get; set; }
        public int f_isopen { get; set; }
        /// <summary>
        /// 是否系統
        /// </summary>
        public int f_issys { get; set; }
        public int f_isdel { get; set; }
    }

    public class SubItem
    {
        public int f_id { get; set; }
        public int f_menuid { get; set; }
        public string f_name { get; set; }
        public string f_controller { get; set; }
        public int f_isopen { get; set; }
        public int f_level { get; set; }        
    }

    public class MainMenuItem
    {
        public int f_id { get; set; }
        public string f_name { get; set; }
        public string f_icon { get; set; }
        public int f_level { get; set; }
        public int f_isopen { get; set; }
        public int f_issys { get; set; }
        public int f_isdel { get; set; }
    }
}
