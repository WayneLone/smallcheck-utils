using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Smallcheck.Utils.DB
{
    /// <summary>
    /// SQL Helper
    /// </summary>
    public partial class DBHelper
    {
        #region 处理实体默认值 +static void ResetEntityDefaultValueForDb<T>(T entity) where T : class, new()
        /// <summary>
        /// 处理实体默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体实例</param>
        public static void ResetEntityDefaultValueForDb<T>(T entity) where T : class, new()
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            foreach (var property in propertyInfos)
            {
                if (property.PropertyType == typeof(string) && property.GetValue(entity, null) == null)
                {
                    property.SetValue(entity, "", null);
                }
            }
        }
        #endregion

        #region 获得查询和统计查询记录SQL语句 -static string GetPageAndCountSql(PageQuery page)
        /// <summary>
        /// 获得查询和统计查询记录SQL语句
        /// </summary>
        /// <param name="page">分页查询实体</param>
        /// <returns></returns>
        private static string GetPageAndCountSql(PageQuery page)
        {
            if (string.IsNullOrWhiteSpace(page.Fields))
            {
                throw new System.ArgumentNullException(nameof(page.Fields));
            }
            if (string.IsNullOrWhiteSpace(page.Tables))
            {
                throw new System.ArgumentNullException(nameof(page.Tables));
            }
            string where = string.IsNullOrWhiteSpace(page.Where) ? "" : "where " + page.Where;
            return $@"
                select
                    T.*
                from (
                    select
                        row_number() over(order by {page.OrderField} {page.Order}) row__no,
                        {page.Fields}
                    from {page.Tables}
                    {where}
                ) T
                where
                    T.row__no between {((page.Page - 1) * page.Limit + 1)} and {(page.Page * page.Limit)}
                order by T.row__no;
                select
                    {(string.IsNullOrWhiteSpace(page.SummaryPartial) ? "count(1)" : page.SummaryPartial)}
                from {page.Tables}
                {where}";
        }
        #endregion

        #region 查询分页数据 +static List<T> Page<T>(string connectionName, PageQuery page) where T : class, new()
        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <typeparam name="T">目标返回类型</typeparam>
        /// <param name="connectionName">数据库连接字符串名称名称</param>
        /// <param name="page">分页查询实体</param>
        /// <returns></returns>
        public static List<T> Page<T>(string connectionName, PageQuery page) where T : class, new()
        {
            string sql = GetPageAndCountSql(page);
            using (IDbConnection conn = DBUtils.Util.GetDbConnection(connectionName))
            {
                using (var multi = conn.QueryMultiple(sql, page))
                {
                    var pageList = multi.Read<T>().ToList();
                    if (!string.IsNullOrWhiteSpace(page.SummaryPartial))
                    {
                        page.Summary = multi.Read().FirstOrDefault();
                    }
                    else
                    {
                        page.Total = multi.Read<int>().SingleOrDefault();
                    }
                    return pageList ?? new List<T>();
                }
            }
        }
        #endregion

        #region 执行SQL +static int Execute(string dbConnName, string sql, object paras = null)
        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="dbConnName">数据连接字符串名称</param>
        /// <param name="sql">sql</param>
        /// <param name="paras">查询参数</param>
        /// <returns></returns>
        public static int Execute(string dbConnName, string sql, object paras = null)
        {
            using (IDbConnection dbConn = DBUtils.Util.GetDbConnection(dbConnName))
            {
                return dbConn.Execute(sql, paras);
            }
        }
        #endregion

        #region 执行SQL 获取执行后的结果 +static T ExecuteScalar<T>(string dbConnName, string sql, object paras = null)
        /// <summary>
        /// 执行SQL 获取执行后的结果
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="dbConnName">数据库连接字符串名称名称</param>
        /// <param name="sql">SQL</param>
        /// <param name="paras">查询参数</param>
        /// <returns></returns>
        public static T ExecuteScalar<T>(string dbConnName, string sql, object paras = null)
        {
            using (IDbConnection dbConn = DBUtils.Util.GetDbConnection(dbConnName))
            {
                return dbConn.ExecuteScalar<T>(sql, paras);
            }
        }
        #endregion

        #region 查询第一条数据 +static T QueryFirstOrDefault<T>(string dbConnName, string sql, object paras = null) where T : class, new()
        /// <summary>
        /// 查询第一条数据
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="dbConnName">数据库连接字符串名称</param>
        /// <param name="sql">SQL</param>
        /// <param name="paras">查询参数</param>
        /// <returns></returns>
        public static T QueryFirstOrDefault<T>(string dbConnName, string sql, object paras = null) where T : class, new()
        {
            using (IDbConnection dbConn = DBUtils.Util.GetDbConnection(dbConnName))
            {
                return dbConn.QueryFirstOrDefault<T>(sql, paras);
            }
        }
        #endregion

        #region 查询一条数据 +static T QuerySingleOrDefault<T>(string dbConnName, string sql, object paras = null) where T : class, new()
        /// <summary>
        /// 查询一条数据 如果存在多条将引发异常
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="dbConnName">数据库连接字符串名称</param>
        /// <param name="sql">SQL</param>
        /// <param name="paras">查询参数</param>
        /// <returns></returns>
        public static T QuerySingleOrDefault<T>(string dbConnName, string sql, object paras = null) where T : class, new()
        {
            using (IDbConnection dbConn = DBUtils.Util.GetDbConnection(dbConnName))
            {
                return dbConn.QuerySingleOrDefault<T>(sql, paras);
            }
        }
        #endregion

        #region 获取数据列表 数据为空则返回空集合 +static List<T> Query<T>(string dbConnName, string sql, object paras = null) where T : class, new()
        /// <summary>
        /// 获取数据列表 数据为空则返回空集合
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="dbConnName">数据库连接字符串名称</param>
        /// <param name="sql">SQL</param>
        /// <param name="paras">查询参数</param>
        /// <returns></returns>
        public static List<T> Query<T>(string dbConnName, string sql, object paras = null) where T : class, new()
        {
            using (IDbConnection dbConn = DBUtils.Util.GetDbConnection(dbConnName))
            {
                List<T> result = dbConn.Query<T>(sql, paras).ToList();
                return result ?? new List<T>();
            }
        }
        #endregion

        #region 执行事务 -static bool ExecuteTransaction(string dbConnName, Func<DBUtils, bool> execBusiness)
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="dbConnName">数据库连接字符串名称</param>
        /// <param name="execBusiness">执行业务逻辑 执行工具实例 返回是否成功 成功提交事务 失败回滚事务</param>
        /// <returns></returns>
        public static bool ExecuteTransaction(string dbConnName, Func<DBContext, bool> execBusiness)
        {
            using (IDbConnection dbConn = DBUtils.Util.GetDbConnection(dbConnName))
            {
                dbConn.Open();
                using (IDbTransaction dbTrans = dbConn.BeginTransaction())
                {
                    DBContext ctx = new DBContext(dbConn, dbTrans);
                    if (execBusiness(ctx))
                    {
                        dbTrans.Commit();
                        return true;
                    }
                    else
                    {
                        dbTrans.Rollback();
                        return false;
                    }
                }
            }
        }
        #endregion

        #region 获取复合实体 +static T QueryMultiple<T>(string dbConnName, string sql, Func<SqlMapper.GridReader, T> func, object paras = null) where T : class, new()
        /// <summary>
        /// 获取复合实体
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="dbConnName">数据库连接名称</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="func">获取数据方法</param>
        /// <param name="paras">查询参数</param>
        /// <returns></returns>
        public static T QueryMultiple<T>(string dbConnName, string sql, Func<SqlMapper.GridReader, T> func, object paras = null) where T : class, new()
        {
            using (IDbConnection dbConn = DBUtils.Util.GetDbConnection(dbConnName))
            using (SqlMapper.GridReader multi = dbConn.QueryMultiple(sql, paras))
            {
                return func(multi);
            }
        }
        #endregion
    }
}
