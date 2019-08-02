using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Smallcheck.Utils.Extensions
{
    /// <summary>
    /// String扩展
    /// </summary>
    public static class StringExtensions
    {
        #region 获得SQL语句where部分 +static string GetSQLWhere(this string str)
        /// <summary>
        /// 获得SQL语句where部分
        /// </summary>
        /// <param name="str">查询语句</param>
        /// <example>
        /// <![CDATA[
        /// 输入：and field1 = 'abc';
        /// 输出：where field1 = 'abc';
        /// ]]>
        /// </example>
        /// <returns></returns>
        public static string GetSQLWhere(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }
            str = str.Trim();
            if (!str.StartsWith("where ") && !str.StartsWith("WHERE "))
            {
                if (str.StartsWith("and "))
                {
                    str = str.Substring(0, 4);
                }
                str = "where " + str;
            }
            return str;
        }
        #endregion

        #region 是否为有效的身份证号 +static bool IsValidIDCardNo(this string idCardNo)
        /// <summary>
        /// 是否为有效的身份证号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsValidIDCardNo(this string idCardNo)
        {
            if (string.IsNullOrWhiteSpace(idCardNo))
            {
                return false;
            }
            if (idCardNo.Length == 18)
            {
                long n = 0;
                if (long.TryParse(idCardNo.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(idCardNo.Replace('x', '0').Replace('X', '0'), out n) == false)
                {
                    return false;
                }
                string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
                if (address.IndexOf(idCardNo.Remove(2)) == -1)
                {
                    return false;
                }
                string birth = idCardNo.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                DateTime time = new DateTime();
                if (DateTime.TryParse(birth, out time) == false)
                {
                    return false;
                }
                string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
                string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
                char[] Ai = idCardNo.Remove(17).ToCharArray();
                int sum = 0;
                for (int i = 0; i < 17; i++)
                {
                    sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
                }
                int y = -1;
                Math.DivRem(sum, 11, out y);
                if (arrVarifyCode[y] != idCardNo.Substring(17, 1).ToLower())
                {
                    return false;
                }
                return true;//正确
            }
            else if (idCardNo.Length == 15)
            {
                long n = 0;
                if (long.TryParse(idCardNo, out n) == false || n < Math.Pow(10, 14))
                {
                    return false;
                }
                string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
                if (address.IndexOf(idCardNo.Remove(2)) == -1)
                {
                    return false;
                }
                string birth = idCardNo.Substring(6, 6).Insert(4, "-").Insert(2, "-");
                DateTime time = new DateTime();
                if (DateTime.TryParse(birth, out time) == false)
                {
                    return false;
                }
                return true;//正确
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 过滤掉标识符号 +static string FilterPunctuation(this string str)
        /// <summary>
        /// 过滤掉标识符号及回车换行符
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string FilterPunctuation(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }
            str = str.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Trim();
            return Regex.Replace(str, "[ \\[ \\] \\^ \\-_*×――(^)$%~!@#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;\"‘’“”-]", "");
        }
        #endregion

        #region 去掉空白行 +static string TrimBlankLine(this string str)
        /// <summary>
        /// 去掉空白行
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string TrimBlankLine(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }
            // return Regex.Replace(str, @"\s+", "\n");
            List<string> resultList = new List<string>();
            foreach (var item in str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                resultList.Add(item.Trim());
            }
            return string.Join("\n", resultList);
        }
        #endregion
    }
}
