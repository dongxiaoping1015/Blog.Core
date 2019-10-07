using System;
using System.Threading.Tasks;
using Blog.Core.IServices.Base;
using Blog.Core.Model.Models;

namespace Blog.Core.IServices
{
    public interface ISysUserInfoServices: IBaseServices<SysUserInfo>
    {
        Task<SysUserInfo> SaveUserInfo(string loginName, string loginPwd);
        Task<string> GetUserRoleNameStr(string loginName, string loginPwd);
    }
}
