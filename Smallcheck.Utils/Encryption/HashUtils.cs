using System;
using System.Security.Cryptography;
using System.Text;

namespace Smallcheck.Utils.Encryption
{
    /// <summary>
    /// 哈希算法工具类
    /// </summary>
    public class HashUtils
    {
        #region 计算字符串MD5值 +static string MD5(string str)
        /// <summary>
        /// 计算字符串MD5值
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>32位MD5小写字符串</returns>
        public static string MD5(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            return MD5(UTF8Encoding.Default.GetBytes(str));
        }
        #endregion

        #region 计算文件MD5值 +static string MD5(byte[] fileStream)
        /// <summary>
        /// 计算文件MD5值
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <returns></returns>
        public static string MD5(byte[] fileStream)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                string result = BitConverter.ToString(md5.ComputeHash(fileStream));
                return result.Replace("-", "").ToLower();
            }
        }
        #endregion

        #region 获得字符串16位的MD5值 +static string MD5_16(string str)
        /// <summary>
        /// 获得字符串16位的MD5值
        /// </summary>
        /// <param name="str">计算的字符串</param>
        /// <returns></returns>
        public static string MD5_16(string str)
        {
            return MD5_16(UTF8Encoding.Default.GetBytes(str));
        }
        #endregion

        #region 获得文件16位的MD5值 +static string MD5_16(byte[] fileStream)
        /// <summary>
        /// 获得文件16位的MD5值
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <returns></returns>
        public static string MD5_16(byte[] fileStream)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                string result = BitConverter.ToString(md5.ComputeHash(fileStream), 4, 8);
                return result.Replace("-", "").ToLower();
            }
        }
        #endregion

        #region SHA256加密 +static string SHA256(string data)
        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static string SHA256(string data)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            using (var sha256 = new SHA256Managed())
            {
                byte[] result = sha256.ComputeHash(byteData);
                return BitConverter.ToString(result).Replace("-", "").ToLower();
            }
        }
        #endregion

        #region HMACSHA1加密 +static string HMACSHA1(string data, string key)
        /// <summary>
        /// HMACSHA1加密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string HMACSHA1(string data, string key)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            byte[] byteKey = Encoding.UTF8.GetBytes(key);
            using (var hmacsha1 = new HMACSHA1(byteKey))
            {
                byte[] result = hmacsha1.ComputeHash(byteData);
                return BitConverter.ToString(result).Replace("-", "").ToLower();
            }
        }
        #endregion

        #region HMACSHA256加密 +static string HMACSHA256(string data, string key)
        /// <summary>
        /// HMACSHA256加密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string HMACSHA256(string data, string key)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            byte[] byteKey = Encoding.UTF8.GetBytes(key);
            using (var hmacsha256 = new HMACSHA256(byteKey))
            {
                byte[] result = hmacsha256.ComputeHash(byteData);
                return BitConverter.ToString(result).Replace("-", "").ToLower();
            }
        }
        #endregion
    }
}
