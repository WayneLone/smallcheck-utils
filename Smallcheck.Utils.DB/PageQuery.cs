namespace Smallcheck.Utils.DB
{
    /// <summary>
    /// 页面查询
    /// </summary>
    public class PageQuery
    {
        /// <summary>
        /// 页索引
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 页容量
        /// </summary>
        public int Limit { get; set; } = 10;

        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderField { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public string Order { get; set; }

        /// <summary>
        /// where查询条件
        /// </summary>
        public string Where { get; set; }

        /// <summary>
        /// 要筛选的字段
        /// </summary>
        public string Fields { get; set; }

        /// <summary>
        /// 查询到的记录数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 查询要的表
        /// </summary>
        public string Tables { get; set; }

        /// <summary>
        /// 汇总SQL
        /// </summary>
        public string SummaryPartial { get; set; }

        /// <summary>
        /// 汇兑数据
        /// </summary>
        public dynamic Summary { get; set; }
    }
}
