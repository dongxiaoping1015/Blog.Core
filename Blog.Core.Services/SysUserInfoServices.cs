using System;
using System.Threading.Tasks;
using Blog.Core.IServices;
using Blog.Core.Model.Models;
using Blog.Core.Services.Base;

namespace Blog.Core.Services
{
    public class SysUserInfoServices : BaseServices<SysUserInfo>, ISysUserInfoServices
    {

        public async Task<string> GetUserRoleNameStr(string loginName, string loginPwd)
        {
            string result = "Admin";
            return result;
        }

        public Task<SysUserInfo> SaveUserInfo(string loginName, string loginPwd)
        {
            throw new NotImplementedException();
        }
    }
}
