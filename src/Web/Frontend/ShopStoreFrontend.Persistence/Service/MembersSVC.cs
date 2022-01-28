#region 功能與歷史修改描述

/*
    描述:前台會員資料庫處理
    日期:2021-11-24

    描述:程式碼風格調整
    日期:2022-01-07

 */

#endregion

using Dapper;
using NLog;
using ShopStoreFrontend.Domain.Models.Interface;
using ShopStoreFrontend.Domain.ViewModels;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ShopStoreFrontend.Persistence.Models.Service
{
    public class MembersSVC : IMembers
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        private readonly SqlConnection CONNECTION;

        public MembersSVC(SqlConnection connection)
        {
            CONNECTION = connection;
        }

        /// <summary>
        /// 確認重複Email
        /// </summary>
        /// <param name="f_mail"></param>
        /// <returns></returns>
        public async Task<bool> VerifyEmailAsync(string f_mail)
        {
            using var conn = CONNECTION;
            string strSql = @"select top 1 1 from t_members where f_mail = @f_mail";
            return await conn.ExecuteScalarAsync<bool>(strSql, new { f_mail });
        }

        /// <summary>
        /// 確認重複帳號
        /// </summary>
        /// <param name="f_account"></param>
        /// <returns></returns>
        public async Task<bool> VerifyAccountAsync(string f_account)
        {
            using var conn = CONNECTION;
            string strSql = @"select top 1 1 from t_members where f_account = @f_account";
            return await conn.ExecuteScalarAsync<bool>(strSql, new { f_account });
        }

        /// <summary>
        /// 登入查詢有無該User
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pcode"></param>
        /// <returns></returns>
        public MemberViewModel FindUser(string account, string pcode)
        {
            try
            {
                using var conn = CONNECTION;
                return conn.QueryFirstOrDefault<MemberViewModel>(@"pro_fr_getMember",
                                                                new { account, pcode, date = DateTime.Now },
                                                                commandType: System.Data.CommandType.StoredProcedure);                
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return null;
            }
        }

        /// <summary>
        /// 取得該Member的資料
        /// </summary>
        /// <param name="memberId"></param>        
        /// <returns></returns>
        public UserProfileViewModel GetMemberProfile(int memberId)
        {
            try
            {
                using var conn = CONNECTION;
                return conn.QueryFirstOrDefault<UserProfileViewModel>(@"pro_fr_getMemberProfile",
                                                                          new { memberId },
                                                                          commandType: System.Data.CommandType.StoredProcedure);                
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return null;
            }
        }

        /// <summary>
        /// 新增新的會員
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddNewMember(MemberViewModel model)
        {
            try
            {
                //SqlMapper.SetTypeMap(typeof(MemberViewModel), new ColumnAttributeTypeMapper<MemberViewModel>());                
                MemberModelDto memberModelDto = new MemberModelDto()
                {
                    Name = model.f_name,
                    NickName = model.f_nickname,
                    Account = model.f_account,
                    Pwd = model.f_pcode,
                    Phone = model.f_phone,
                    Mail = model.f_mail,
                    CreateTime = model.f_createTime,
                    Address = model.f_address,
                    GroupId = model.f_groupid,
                    Cash = model.f_cash,
                    IsSuspend = model.f_isSuspend,                    
                };

                using var conn = CONNECTION;
                conn.Execute("pro_fr_addMember", memberModelDto, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return false;
            }
            return true;
        }

        public class MemberModelDto
        {
            public string Name { get; set; }
            public string NickName { get; set; }
            public string Account { get; set; }
            public string Pwd { get; set; }
            public string Phone { get; set; }
            public string Mail { get; set; }
            public DateTime CreateTime { get; set; }
            public string Address { get; set; }
            public string GroupId { get; set; }
            public int Cash { get; set; }
            public int IsSuspend { get; set; }
        }

        /// <summary>
        /// 重置會員密碼
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mail"></param>
        /// <returns></returns>
        public bool ResetMemberPcode(string pwd, string mail)
        {
            try
            {
                using var conn = CONNECTION;
                return conn.Execute("pro_bg_editMemberPwd", new { pwd, mail }, commandType: System.Data.CommandType.StoredProcedure) > 0;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return false;
            }
        }
    }
}
