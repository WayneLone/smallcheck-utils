using Dapper;
using System;
using System.Data;

namespace Smallcheck.Utils.DB
{
    /// <summary>
    /// DBContext
    /// </summary>
    public class DBContext
    {
        #region 数据库连接 -readonly IDbConnection dbConnection
        /// <summary>
        /// 数据库连接
        /// </summary>
        private readonly IDbConnection dbConnection;
        #endregion

        #region 数据库事务 -readonly IDbTransaction dbTransaction
        /// <summary>
        /// 数据库事务
        /// </summary>
        private readonly IDbTransaction dbTransaction;
        #endregion

        #region 构造函数 初始连接和事务 +DBContext(IDbConnection dbConn, IDbTransaction dbTrans)
        /// <summary>
        /// 构造函数 初始连接和事务
        /// </summary>
        /// <param name="dbConn">数据库连接</param>
        /// <param name="dbTrans">数据库事务</param>
        public DBContext(IDbConnection dbConn, IDbTransaction dbTrans)
        {
            dbConnection = dbConn ?? throw new ArgumentNullException(nameof(dbConn));
            dbTrans = dbTrans ?? throw new ArgumentNullException(nameof(dbTrans));
            if (dbConnection.State == ConnectionState.Closed)
            {
                dbConnection.Open();
            }
            dbTransaction = dbTrans;
        }
        #endregion

        #region 执行SQL语句 +int Execute(string sql, object paras = null)
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="paras">查询参数</param>
        /// <returns></returns>
        public int Execute(string sql, object paras = null)
        {
            return dbConnection.Execute(sql, paras, dbTransaction);
        }
        #endregion

        #region 执行SQL返回第一行第一列数据 +T ExecuteScalar<T>(string sql, object paras = null)
        /// <summary>
        /// 执行SQL返回第一行第一列数据
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="paras">查询参数</param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, object paras = null)
        {
            return dbConnection.ExecuteScalar<T>(sql, paras, dbTransaction);
        }
        #endregion
    }
}
