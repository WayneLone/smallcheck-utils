using System;
using System.Security.Cryptography;
using System.Text;

namespace Smallcheck.Utils.Encryption
{
    /// <summary>
    /// AES加解密
    /// </summary>
    public class AESUtil
    {
        #region AES加密 +static string AESEncrypt(string data, string key)
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="data">待加密数据</param>
        /// <param name="key">密钥</param>
        /// <returns>返回base64编码字符串</returns>
        public static string AESEncrypt(string data, string key)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            if ((key.Length % 16) != 0)
            {
                throw new ArgumentException($"{nameof(key)}必须为16的倍数！");
            }
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(data);
            using (RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            })
            {
                ICryptoTransform cTransform = rm.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
                // return BitConverter.ToString(resultArray).Replace("-", "").ToLower();
            }
        }
        #endregion

        #region AES解密 +static string AESDecrypt(string data, string key)
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="data">待解密数据</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string AESDecrypt(string data, string key)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            if ((key.Length % 16) != 0)
            {
                throw new ArgumentException($"{nameof(key)}必须为16的倍数！");
            }
            var toEncryptArray = Convert.FromBase64String(data);
            using (var rDel = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            })
            {
                var cTransform = rDel.CreateDecryptor();
                var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Encoding.UTF8.GetString(resultArray);
            }
        }
        #endregion
    }
}
