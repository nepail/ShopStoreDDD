﻿#region 功能與歷史修改描述

/*
    描述:Jwt服務
    日期:2022-01-27    

 */

#endregion

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShopStoreFrontend.Domain.Service
{
    public class JwtSVC
    {
        private readonly IConfiguration CONFIG;        
        
        public JwtSVC(IConfiguration configuration)
        {
            CONFIG = configuration;
        }

        /// <summary>
        /// 產生 JWT Token
        /// </summary>
        /// <param name="userName">使用者帳號</param>
        /// <param name="expireMinutes">過期時間</param>
        /// <returns></returns>
        public string GenerateToken(string userName,string userAccount, int expireMinutes = 30)
        {
            //var issuer = CONFIG
            //簽發者
            var issuer = "AAA";
            //對稱密鑰
            var signKey = "fkadsf;pdfddksssfq";

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //前台User
                new Claim("type", "Cookies"),
                new Claim("Account", userAccount)
            };

            var userClaimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Subject = userClaimsIdentity,
                Expires = DateTime.Now.AddMinutes(expireMinutes),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }
    }
}
