using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Blog.Core.Common;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Core.AuthHelper.OverWrite
{
    public class JwtHelper
    {
        /// <summary>
        /// 颁发JWT字符串
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        public static string IssueJwt(JwtTokenModel tokenModel)
        {
            string iss = Appsettings.app(new string[] { "Audience", "Issuer" });
            string aud = Appsettings.app(new string[] { "Audience", "Audience" });
            string secret = Appsettings.app(new string[] { "Audience", "Secret" });

            var claims = new List<Claim>
            {
                // 这里将用户的部分信息,如uid存到了Claim中
                new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Uid.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(DateTime.Now.AddSeconds(1000)).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Iss, iss),
                new Claim(JwtRegisteredClaimNames.Aud, aud),
            };

            // 赋予用户角色
            claims.AddRange(tokenModel.Role.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));
            // 秘钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                issuer: iss,
                claims: claims,
                signingCredentials: creds
                );
            var jwtHandler = new JwtSecurityTokenHandler();
            var encodedJwt = jwtHandler.WriteToken(jwt);

            return encodedJwt;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static JwtTokenModel SerializeJwt(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);
            object role = null;
            try
            {
                jwtToken.Payload.TryGetValue(ClaimTypes.Role, out role);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            var tm = new JwtTokenModel
            {
                Uid = int.Parse(jwtToken.Id),
                Role = role != null ? role.ToString() : "",
            };

            return tm;
        }
    }

    /// <summary>
    /// 令牌
    /// </summary>
    public class JwtTokenModel
    {
        private long _uid;
        private string _role;
        private string _work;

        /// <summary>
        /// Id
        /// </summary>
        public long Uid { get => _uid; set => _uid = value; }
        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get => _role; set => _role = value; }
        /// <summary>
        /// 职能
        /// </summary>
        public string Work { get => _work; set => _work = value; }
    }
}
