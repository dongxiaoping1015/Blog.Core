using System;
using SqlSugar;

namespace Blog.Core.Model.Models
{
    public class BlogArticle
    {
        /// <summary>
        /// 主键
        /// </summary>
        /// 这里之所以没用RootEntity，是想保持和之前的数据库一致，主键是bID，不是Id
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int authorID { get; set; }

        /// <summary>
        /// 标题blog
        /// </summary>
        [SugarColumn(Length = 256, IsNullable = true)]
        public string title { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        [SugarColumn(Length = 256, IsNullable = true)]
        public string category { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "text")]
        public string content { get; set; }

        /// <summary>
        /// 访问量
        /// </summary>
        public int traffic { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int commentNum { get; set; }

        /// <summary> 
        /// 修改时间
        /// </summary>
        public DateTime updateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(Length = int.MaxValue, IsNullable = true)]
        public string remark { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool? IsDeleted { get; set; }

    }
}
