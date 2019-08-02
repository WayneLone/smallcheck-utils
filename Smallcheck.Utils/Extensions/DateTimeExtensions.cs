using System;

namespace Smallcheck.Utils.Extensions
{
    /// <summary>
    /// 日期时间扩展
    /// </summary>
    public static class DateTimeExtensions
    {
        #region 日期为合法的 Sql Server的日期 +static bool IsValidSQLServerDateTime(this DateTime dt)
        /// <summary>
        /// 日期为合法的 Sql Server的日期
        /// </summary>
        /// <param name="dt">日期值</param>
        /// <returns></returns>
        public static bool IsValidSQLServerDateTime(this DateTime dt) =>
            DateTime.Parse("1753-01-01") <= dt && dt <= DateTime.Parse("9999-12-31  23:59:59.997");
        #endregion

        #region 该时间与当前时间差 +static string GetHumanBasedDateStringFromNow(this DateTime dt)
        /// <summary>
        /// 该时间与当前时间差
        /// 获得人性化的时间
        /// </summary>
        /// <param name="passedTime">过去的时间</param>
        /// <returns></returns>
        public static string GetHumanBasedDateStringFromNow(this DateTime passedTime)
        {
            TimeSpan span = DateTime.Now - passedTime;
            if (span.TotalDays > 60)
            {
                return passedTime.ToShortDateString();
            }
            else
            {
                if (span.TotalDays > 30)
                {
                    return "1个月前";
                }
                else
                {
                    if (span.TotalDays > 14)
                    {
                        return "2周前";
                    }
                    else
                    {
                        if (span.TotalDays > 7)
                        {
                            return "1周前";
                        }
                        else
                        {
                            if (span.TotalDays > 1)
                            {
                                return string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
                            }
                            else
                            {
                                if (span.TotalHours > 1)
                                {
                                    return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
                                }
                                else
                                {
                                    if (span.TotalMinutes > 1)
                                    {
                                        return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
                                    }
                                    else
                                    {
                                        if (span.TotalSeconds >= 1)
                                        {
                                            return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
                                        }
                                        else
                                        {
                                            return "1秒前";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region 传入时间，转换为时间戳(精确到毫秒) +static long GetUnixTimestampMilliseconds(this DateTime time)
        /// <summary>
        /// 传入时间，转换为时间戳(精确到毫秒)
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static long GetUnixTimestampMilliseconds(this DateTime time)
        {
            return (long)(time.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
        #endregion

        #region 传入时间，转换为时间戳(精确到秒) +static long GetUnixTimestampSeconds(this DateTime time)
        /// <summary>
        /// 传入时间，转换为时间戳(精确到秒)
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static long GetUnixTimestampSeconds(this DateTime time)
        {
            return (long)(time.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }
        #endregion
    }
}
