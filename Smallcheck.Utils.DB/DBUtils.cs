using System;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Smallcheck.Utils.DB
{
    /// <summary>
    /// 数据库工具类
    /// </summary>
    public sealed class DBUtils
    {
        #region Singleton Pattern
        private static volatile DBUtils util;
        private static object syncRoot = new object();

        private DBUtils() { }

        /// <summary>
        /// 工具实例
        /// </summary>
        public static DBUtils Util
        {
            get
            {
                if (util == null)
                {
                    lock (syncRoot)
                    {
                        if (util == null)
                        {
                            util = new DBUtils();
                        }
                    }
                }
                return util;
            }
        }
        #endregion

        #region 获得指定连接字符串的数据库连接 +IDbConnection GetDbConnection(string connectionName)
        /// <summary>
        /// 获得指定连接字符串的数据库连接 未打开
        /// </summary>
        /// <param name="connectionName">数据连接配置节名称</param>
        /// <returns></returns>
        public IDbConnection GetDbConnection(string connectionName)
        {
            ConnectionStringSettingsCollection ConfigStringCollention = ConfigurationManager.ConnectionStrings;
            if (ConfigStringCollention == null || ConfigStringCollention.Count <= 0)
            {
                throw new Exception("web.config 中无连接字符串!");
            }
            ConnectionStringSettings setting = ConfigStringCollention[connectionName];
            if (setting == null)
            {
                throw new Exception($"web.config 中无名称为{connectionName}的连接字符串!");
            }
            // 调用DbProviderFactories根据providerName属性中的值创建特定的数据库工厂
            DbProviderFactory dbProviderFactory = DbProviderFactories.GetFactory(setting.ProviderName);
            DbConnection dbConnection = dbProviderFactory.CreateConnection();
            dbConnection.ConnectionString = setting.ConnectionString;   // 指定配置节中的连接字符串
            return dbConnection;
        }
        #endregion
    }
}
