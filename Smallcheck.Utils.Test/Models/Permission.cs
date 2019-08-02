using System;
using System.ComponentModel;

namespace Smallcheck.Utils.Test.Models
{
    /// <summary>
    /// 权限
    /// </summary>
    [Flags]
    public enum Permission
    {
        /// <summary>
        /// 查询
        /// </summary>
        [Description("查询")]
        Query = 1,

        /// <summary>
        /// 创建
        /// </summary>
        [Description("创建")]
        Create = Query << 1,

        /// <summary>
        /// 修改
        /// </summary>
        [Description("修改")]
        Update = Create << 1,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Delete = Update << 1,
    }
}
