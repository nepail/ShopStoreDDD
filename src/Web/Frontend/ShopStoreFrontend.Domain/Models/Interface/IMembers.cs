using ShopStoreFrontend.Domain.ViewModels;
using System.Threading.Tasks;

namespace ShopStoreFrontend.Domain.Models.Interface
{
    /// <summary>
    /// 會員 Interface
    /// </summary>
    public interface IMembers
    {
        /// <summary>
        /// 新增新的會員
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddNewMember(MemberViewModel model);

        /// <summary>
        /// 確認重複Email
        /// </summary>
        /// <param name="f_mail"></param>
        /// <returns></returns>
        Task<bool> VerifyEmailAsync(string f_mail);

        /// <summary>
        /// 確認重複帳號
        /// </summary>
        /// <param name="f_account"></param>
        /// <returns></returns>
        Task<bool> VerifyAccountAsync(string f_account);

        /// <summary>
        /// 登入查詢有無該User
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pcode"></param>
        /// <returns></returns>
        public MemberViewModel FindUser(string account, string pcode);

        /// <summary>
        /// 取得該Member的資料
        /// </summary>
        /// <param name="f_account"></param>
        /// <param name="f_pcode"></param>
        /// <returns></returns>
        public UserProfileViewModel GetMemberProfile(int memberId);

        /// <summary>
        /// 重置會員密碼
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mail"></param>
        /// <returns></returns>
        public bool ResetMemberPcode(string code, string mail);
    }
}
