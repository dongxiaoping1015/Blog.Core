using System;
using System.Data.SqlTypes;
using SqlSugar;

namespace Blog.Core.Model.Models
{
    /// <summary>
    /// 用户信息表
    /// </summary>
    public class SysUserInfo
    {
        public SysUserInfo() { }

        public SysUserInfo(string loginName, string loginPWD)
        {
            
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }
        /// <summary>
        /// 登录账号
        /// </summary>
        [SugarColumn(Length = 60, IsNullable = false)]
        public string loginName { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        [SugarColumn(Length = 60, IsNullable = false)]
        public string loginPWD { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [SugarColumn(Length = 60, IsNullable = true)]
        public string userName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(Length = int.MaxValue, IsNullable = true)]
        public string remark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime createTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 更新时间
        /// </summary>
        public System.DateTime updateTime { get; set; } = DateTime.Now;

        /// <summary>
        ///最后登录时间 
        /// </summary>
        public DateTime lastErrTime { get; set; } = DateTime.Now;

        /// <summary>
        ///错误次数 
        /// </summary>
        public int errorCount { get; set; }



        /// <summary>
        /// 真实姓名
        /// </summary>
        [SugarColumn(Length = 60, IsNullable = true)]
        public string name { get; set; }

        // 性别
        [SugarColumn(IsNullable = true)]
        public int sex { get; set; } = 0;

        // 年龄
        [SugarColumn(IsNullable = true)]
        public int age { get; set; }
        // 生日
        [SugarColumn(IsNullable = true)]
        public DateTime birth { get; set; } = DateTime.Now;
        // 地址
        [SugarColumn(Length = 200, IsNullable = true)]
        public string addr { get; set; }

        [SugarColumn(IsNullable = true)]
        public bool isDelete { get; set; }


        [SugarColumn(IsIgnore = true)]
        public int roleID { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string roleName { get; set; }
    }
}
