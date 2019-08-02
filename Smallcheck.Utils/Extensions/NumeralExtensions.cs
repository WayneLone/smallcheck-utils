namespace Smallcheck.Utils.Extensions
{
    /// <summary>
    /// 数字类扩展
    /// </summary>
    public static class NumeralExtensions
    {
        #region 通过秒数格式化时间 +static string FormatTime(this int seconds)
        /// <summary>
        /// 通过秒数格式化时间
        /// e.g. in: 3720 out: 0.01:02:00
        /// </summary>
        /// <param name="seconds">秒数</param>
        /// <returns>返回格式化后的日期</returns>
        public static string FormatTime(this int seconds)
        {
            if (seconds <= 0)
            {
                return "0.00:00:00";
            }
            int days = 0, hours = 0;
            int minutes = seconds / 60;
            if (minutes > 59)
            {
                hours = minutes / 60;
                minutes %= 60;
                if (hours > 23)
                {
                    hours %= 24;
                    days = hours / 24;
                }
            }
            seconds %= 60;
            return string.Format("{0}.{1:00}:{2:00}:{3:00}", days, hours, minutes, seconds);
        }
        #endregion

        #region 通过秒数格式化时间 生成中文 +static string FormatTimeChinese(this int seconds)
        /// <summary>
        /// 通过秒数格式化时间 生成中文
        /// e.g. in: 3720 out: 1小时2分钟0秒
        /// </summary>
        /// <param name="seconds">秒数</param>
        /// <returns>返回格式化后的日期</returns>
        public static string FormatTimeChinese(this int seconds)
        {
            if (seconds <= 0)
            {
                return "0秒";
            }
            if (seconds > 59)
            {
                int mintues = seconds / 60;
                if (mintues > 59)
                {
                    int hours = mintues / 60;
                    if (hours > 23)
                    {
                        int days = hours / 24;
                        seconds %= 60;
                        mintues %= 60;
                        hours %= 24;
                        return $"{days}天{hours}小时{mintues}分钟{seconds}秒";
                    }
                    else
                    {
                        seconds %= 60;
                        mintues %= 60;
                        return $"{hours}小时{mintues}分钟{seconds}秒";
                    }
                }
                else
                {
                    seconds %= 60;
                    return $"{mintues}分钟{seconds} 秒";
                }
            }
            else
            {
                return $"{seconds}秒";
            }
        }
        #endregion
    }
}
