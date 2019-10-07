using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Blog.Core.Common.Helper;
using Blog.Core.Model.Models;

namespace Blog.Core.Model
{
    public class DBSeed
    {
        /// <summary>
        /// 异步添加种子数据
        /// </summary>
        /// <param name="myContext"></param>
        /// <returns></returns>
        public static async Task SeedAsync(MyContext myContext)
        {
            try
            {
                // 创建表
                myContext.CreateTableByEntity(false,
                    typeof(BlogArticle),
                    typeof(SysUserInfo));

                #region BlogArticle
                if (!await myContext.Db.Queryable<BlogArticle>().AnyAsync())
                {
                    myContext.GetEntityDB<BlogArticle>().InsertRange(JsonHelper.ParseFormByJson<List<BlogArticle>>(GetDataDemo("BlogArticle")));
                    Console.WriteLine("Table:BlogArticle created success!");
                }
                else
                {
                    Console.WriteLine("Table:BlogArticle already exists...");
                }
                #endregion

                #region sysUserInfo
                if (!await myContext.Db.Queryable<SysUserInfo>().AnyAsync())
                {
                    myContext.GetEntityDB<SysUserInfo>().InsertRange(JsonHelper.ParseFormByJson<List<SysUserInfo>>(GetDataDemo("SysUserInfo")));
                    Console.WriteLine("Table:sysUserInfo created success!");
                }
                else
                {
                    Console.WriteLine("Table:sysUserInfo already exists...");
                }
                #endregion

                Console.WriteLine("Done seeding database.");
                Console.WriteLine();

            }
            catch (Exception ex)
            {
                throw new Exception("1、注意要先创建空的数据库\n2、" + ex.Message);
            }
        }

        private static string GetDataDemo(string fileName)
        {
            fileName = $"/Users/dong/Downloads/{fileName}.tsv";
            if (File.Exists(fileName))
            {
                return File.ReadAllText(fileName).Trim();
            }
            return "";
        }
    }
}
