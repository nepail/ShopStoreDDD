using DAL.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace ShopStore.ViewModels
{
    /// <summary>
    /// 會員 ViewModel
    /// </summary>
    public class MemberViewModel
    {
        private string _f_level;

        /// <summary>
        /// 流水號
        /// </summary>
        public string f_id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名", Prompt = "真實姓名"), Required(ErrorMessage = "請輸入姓名")]
        [MinLength(1, ErrorMessage = "{0}不得小於{1}個字元")]
        [Column(Name = "name")]
        public string f_name { get; set; }

        /// <summary>
        /// 暱稱
        /// </summary>
        [Display(Name = "暱稱", Prompt = "暱稱"), Required(ErrorMessage = "請輸入暱稱")]
        [MinLength(1, ErrorMessage = "{0}不得小於{1}個字元")]
        [Column(Name = "nickName")]
        public string f_nickname { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        [Display(Name = "帳號", Prompt = "至少 6 個字元"), Required(ErrorMessage = "請輸入帳號")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "帳號長度6~30碼")]
        [Remote(action: "VerifyAccount", controller: "Verify")]
        [Column(Name = "account")]
        public string f_account { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Display(Name = "密碼", Prompt = "至少 6 個字元"), Required(ErrorMessage = "請輸入密碼")]
        [StringLength(12, MinimumLength = 6, ErrorMessage = "密碼長度6~12碼")]
        [RegularExpression(@"[a-zA-Z]+[a-zA-Z0-9]*$", ErrorMessage = "密碼僅能有英文或數字，且開頭需為英文字母！")]
        [DataType(DataType.Password)]
        [Column(Name = "pwd")]
        public string f_pcode { get; set; }

        [Display(Name = "確認會員密碼："), Required(ErrorMessage = "請您再次輸入密碼！")]                
        [Compare("f_pcode", ErrorMessage = "兩次輸入的密碼必須相同")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// 電話
        /// </summary>
        [Display(Name = "電話", Prompt = "電話號碼"), Required(ErrorMessage = "請輸入電話")]
        [MinLength(10, ErrorMessage = "電話號碼小於{1}碼，請重新確認")]
        [Phone]
        [Column(Name = "phone")]
        public string f_phone { get; set; }

        /// <summary>
        /// 信箱
        /// </summary>
        [Display(Name = "電子信箱", Prompt = "電子信箱"), Required(ErrorMessage = "請輸入電子信箱")]
        [EmailAddress(ErrorMessage = "請輸入正確的電子信箱格式")]
        [Remote(action: "VerifyEmail", controller: "Verify")]
        [Column(Name = "mail")]
        public string f_mail { get; set; }

        /// <summary>
        /// 註冊時間
        /// </summary>
        [Column(Name = "createTime")]
        public DateTime f_createTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 通訊地址
        /// </summary>
        [Display(Name = "通訊地址", Prompt = "ex: 台中市西屯區市政路400號"), Required(ErrorMessage = "請輸入通訊地址")]
        [Column(Name = "address")]
        public string f_address { get; set; }

        [Column(Name = "groupId")]
        public string f_groupid
        {
            get
            {
                return _f_level ?? "0";
            }
            set
            {
                _f_level = value switch
                {
                    "1" => "admin",
                    _ => "basic"
                };
            }
        }

        /// <summary>
        /// 是否停權
        /// </summary>
        [Column(Name = "isSuspend")]
        public int f_isSuspend { get; set; } = 0;

        /// <summary>
        /// 購物金
        /// </summary>
        [Column(Name = "cash")]
        public int f_cash { get; set; } = 0;
    }
}
