using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Core.AuthHelper.OverWrite;
using Blog.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Blog.Core.Controllers
{
    [Route("api/Login")]
    [AllowAnonymous]
    public class LoginController : Controller
    {
        readonly ISysUserInfoServices _sysUserInfoServices;

        public LoginController(ISysUserInfoServices sysUserInfoServices)
        {
            this._sysUserInfoServices = sysUserInfoServices;
        }

        [HttpGet]
        [Route("Token")]
        public async Task<object> GetJwtStr(string name, string pass)
        {
            string jwtStr = string.Empty;
            bool suc = false;

            var userRole = await _sysUserInfoServices.GetUserRoleNameStr(name, pass);
            if (userRole != null)
            {
                JwtTokenModel tokenModel = new JwtTokenModel { Uid = 1, Role = userRole };
                jwtStr = JwtHelper.IssueJwt(tokenModel);
                suc = true;
            }
            else
            {
                jwtStr = "login fail";
            }

            return Ok(new
            {
                success = suc,
                token = jwtStr
            });
        }
    }
}
