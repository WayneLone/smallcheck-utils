using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Smallcheck.Utils.Encryption
{
    /// <summary>
    /// RSA 加解密
    /// </summary>
    public class RSAUtil
    {
        #region 获取公钥和私钥 +static void GetKeyPairXMLText(out string publicKey, out string privateKey)
        /// <summary>
        /// 获取公钥和私钥
        /// </summary>
        /// <param name="publicKey">公钥 XML字符串</param>
        /// <param name="privateKey">私钥 XML字符串</param>
        public static void GetKeyPairXMLText(out string publicKey, out string privateKey)
        {
            try
            {
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    privateKey = RSA.ToXmlString(true);
                    publicKey = RSA.ToXmlString(false);
                }
            }
            catch (CryptographicException ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 使用RSA实现加密 +static string EncryptByXMLKey(string data, string publicKey)
        /// <summary>
        /// 使用RSA实现加密 XML字符串KEY
        /// </summary>
        /// <param name="data">待加密数据</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public static string EncryptByXMLKey(string data, string publicKey)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (string.IsNullOrWhiteSpace(publicKey))
            {
                throw new ArgumentNullException(nameof(publicKey));
            }
            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                var inputBytes = Encoding.UTF8.GetBytes(data); // 有含义的字符串转化为字节流 
                rsaProvider.FromXmlString(publicKey); // 载入公钥
                int bufferSize = (rsaProvider.KeySize / 8) - 11; // 单块最大长度
                var buffer = new byte[bufferSize];
                using (MemoryStream inputStream = new MemoryStream(inputBytes), outputStream = new MemoryStream())
                {
                    // 分段加密
                    while (true)
                    {
                        int readSize = inputStream.Read(buffer, 0, bufferSize);
                        if (readSize <= 0)
                        {
                            break;
                        }
                        var temp = new byte[readSize];
                        Array.Copy(buffer, 0, temp, 0, readSize);
                        var encryptedBytes = rsaProvider.Encrypt(temp, false);
                        outputStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                    }
                    return Convert.ToBase64String(outputStream.ToArray());
                }
            }
        }
        #endregion

        #region 使用RSA实现解密 +static string DecryptByXMLKey(string data, string privateKey)
        /// <summary>
        /// 使用RSA实现解密 XML字符串KEY
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static string DecryptByXMLKey(string data, string privateKey)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (string.IsNullOrWhiteSpace(privateKey))
            {
                throw new ArgumentNullException(nameof(privateKey));
            }
            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                var inputBytes = Convert.FromBase64String(data);
                rsaProvider.FromXmlString(privateKey);
                int bufferSize = rsaProvider.KeySize / 8;
                var buffer = new byte[bufferSize];
                using (MemoryStream inputStream = new MemoryStream(inputBytes), outputStream = new MemoryStream())
                {
                    while (true)
                    {
                        int readSize = inputStream.Read(buffer, 0, bufferSize);
                        if (readSize <= 0) { break; }
                        var temp = new byte[readSize];
                        Array.Copy(buffer, 0, temp, 0, readSize);
                        var rawBytes = rsaProvider.Decrypt(temp, false);
                        outputStream.Write(rawBytes, 0, rawBytes.Length);
                    }
                    return Encoding.UTF8.GetString(outputStream.ToArray());
                }
            }
        }
        #endregion

        // 西北白杨树 https://blog.csdn.net/yysyangyangyangshan/article/details/80411134

        #region 签名 +static string SignWithPEM(string content, string privateKey, string encoding = "UTF-8")
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="content">待签名字符串</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>签名后字符串</returns>
        public static string SignWithPEM(string content, string privateKey, string encoding = "UTF-8")
        {
            byte[] contentBuffer = Encoding.GetEncoding(encoding).GetBytes(content);
            using (RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey))
            {
                SHA1 sh = new SHA1CryptoServiceProvider();
                byte[] signData = rsa.SignData(contentBuffer, sh);
                return Convert.ToBase64String(signData);
            }
        }
        #endregion

        #region 验签 +static bool VerifyWithPEM(string content, string signedString, string publicKey, string encoding = "UTF-8")
        /// <summary>
        /// 验签
        /// </summary>
        /// <param name="content">待验签字符串</param>
        /// <param name="signedString">签名</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>true(通过)，false(不通过)</returns>
        public static bool VerifyWithPEM(string content, string signedString, string publicKey, string encoding = "UTF-8")
        {
            byte[] contentBuffer = Encoding.GetEncoding(encoding).GetBytes(content);
            byte[] signature = Convert.FromBase64String(signedString);
            RSAParameters paraPub = ConvertFromPublicKey(publicKey);
            RSACryptoServiceProvider rsaPub = new RSACryptoServiceProvider();
            rsaPub.ImportParameters(paraPub);
            SHA1 sh = new SHA1CryptoServiceProvider();
            return rsaPub.VerifyData(contentBuffer, sh, signature);
        }
        #endregion

        #region 加密 +static string EncryptWithPEM(string plainText, string publicKey, string encoding = "UTF-8")
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plainText">需要加密的字符串</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>加密后的字符串</returns>
        public static string EncryptWithPEM(string plainText, string publicKey, string encoding = "UTF-8")
        {
            byte[] plainTextBuffer = Encoding.GetEncoding(encoding).GetBytes(plainText);
            return Encrypt(plainTextBuffer, publicKey);
        }
        #endregion

        #region 解密 +static string DecryptWithPEM(string encryptedText, string privateKey, string encoding = "UTF-8")
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encryptedText">加密字符串</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>明文</returns>
        public static string DecryptWithPEM(string encryptedText, string privateKey, string encoding = "UTF-8")
        {
            byte[] DataToDecrypt = Convert.FromBase64String(encryptedText);
            string result = string.Empty;
            for (int j = 0; j < DataToDecrypt.Length / 128; j++)
            {
                byte[] buf = new byte[128];
                for (int i = 0; i < 128; i++)
                {
                    buf[i] = DataToDecrypt[i + 128 * j];
                }
                result += Decrypt(buf, privateKey, encoding);
            }
            return result;
        }
        #endregion

        #region 加密 -static string Encrypt(byte[] data, string publicKey)
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">待加密的数据</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        private static string Encrypt(byte[] data, string publicKey)
        {
            using (RSACryptoServiceProvider rsa = DecodePEMPublicKey(publicKey))
            {
                byte[] result = rsa.Encrypt(data, false);
                return Convert.ToBase64String(result);
            }
        }
        #endregion

        #region 解密 -static string Decrypt(byte[] data, string privateKey, string encoding)
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">加密数据</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="encoding">编码格式</param>
        /// <returns></returns>
        private static string Decrypt(byte[] data, string privateKey, string encoding)
        {
            using (RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey))
            {
                byte[] source = rsa.Decrypt(data, false);
                char[] asciiChars = new char[Encoding.GetEncoding(encoding).GetCharCount(source, 0, source.Length)];
                Encoding.GetEncoding(encoding).GetChars(source, 0, source.Length, asciiChars, 0);
                return new string(asciiChars);
            }
        }
        #endregion

        #region 解码PEM公钥 -static RSACryptoServiceProvider DecodePEMPublicKey(string pemPublicKey)
        /// <summary>
        /// 解码PEM公钥
        /// </summary>
        /// <param name="pemPublicKey">PEM格式公钥</param>
        /// <returns></returns>
        private static RSACryptoServiceProvider DecodePEMPublicKey(string pemPublicKey)
        {
            byte[] pkcs8publickkey;
            pkcs8publickkey = Convert.FromBase64String(pemPublicKey);
            if (pkcs8publickkey == null)
            {
                throw new Exception("Decode PEM public key error");
            }
            return DecodeRSAPublicKey(pkcs8publickkey);
        }
        #endregion

        #region 解码PEM私钥 -static RSACryptoServiceProvider DecodePemPrivateKey(string pemPrivateKey)
        /// <summary>
        /// 解码PEM私钥
        /// </summary>
        /// <param name="pemPrivateKey">PEM 私钥</param>
        /// <returns></returns>
        private static RSACryptoServiceProvider DecodePemPrivateKey(string pemPrivateKey)
        {
            byte[] pkcs8privatekey;
            pkcs8privatekey = Convert.FromBase64String(pemPrivateKey);
            if (pkcs8privatekey == null)
            {
                throw new Exception("Decode PEM private key error");
            }
            return DecodePrivateKeyInfo(pkcs8privatekey);
        }
        #endregion

        #region 解码私钥 -static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
        /// <summary>
        /// 解码私钥
        /// </summary>
        /// <param name="pkcs8">PCKS8格式私钥</param>
        /// <returns></returns>
        private static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
        {
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            MemoryStream mem = new MemoryStream(pkcs8);
            int lenstream = (int)mem.Length;
            BinaryReader binr = new BinaryReader(mem);
            try
            {
                ushort twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    return null;

                byte bt = binr.ReadByte();
                if (bt != 0x02)
                    return null;

                twobytes = binr.ReadUInt16();

                if (twobytes != 0x0001)
                    return null;

                byte[] seq = binr.ReadBytes(15);
                if (!CompareByteArrays(seq, SeqOID))
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x04)
                    return null;

                bt = binr.ReadByte();
                if (bt == 0x81)
                    binr.ReadByte();
                else
                    if (bt == 0x82)
                    binr.ReadUInt16();

                byte[] rsaprivkey = binr.ReadBytes((int)(lenstream - mem.Position));
                RSACryptoServiceProvider rsacsp = DecodeRSAPrivateKey(rsaprivkey);
                return rsacsp;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }
        #endregion

        #region 比较字节数组 -static bool CompareByteArrays(byte[] a, byte[] b)
        /// <summary>
        /// 比较字节数组
        /// </summary>
        /// <param name="a">字节数组A</param>
        /// <param name="b">字节数组B</param>
        /// <returns></returns>
        private static bool CompareByteArrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }
        #endregion

        #region 解码公钥 -static RSACryptoServiceProvider DecodeRSAPublicKey(byte[] publicKey)
        /// <summary>
        /// 解码公钥
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        private static RSACryptoServiceProvider DecodeRSAPublicKey(byte[] publicKey)
        {
            byte[] seqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            MemoryStream mem = new MemoryStream(publicKey);
            BinaryReader binr = new BinaryReader(mem);
            try
            {
                ushort twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    return null;

                byte[] seq = binr.ReadBytes(15);
                if (!CompareByteArrays(seq, seqOID))
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8103)
                    binr.ReadByte();
                else if (twobytes == 0x8203)
                    binr.ReadInt16();
                else
                    return null;

                byte bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    return null;

                twobytes = binr.ReadUInt16();
                byte lowbyte = 0x00;
                byte highbyte = 0x00;

                if (twobytes == 0x8102)
                    lowbyte = binr.ReadByte();
                else if (twobytes == 0x8202)
                {
                    highbyte = binr.ReadByte();
                    lowbyte = binr.ReadByte();
                }
                else
                    return null;
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                int modsize = BitConverter.ToInt32(modint, 0);

                byte firstbyte = binr.ReadByte();
                binr.BaseStream.Seek(-1, SeekOrigin.Current);

                if (firstbyte == 0x00)
                {
                    binr.ReadByte();
                    modsize -= 1;
                }

                byte[] modulus = binr.ReadBytes(modsize);

                if (binr.ReadByte() != 0x02)
                    return null;
                int expbytes = (int)binr.ReadByte();
                byte[] exponent = binr.ReadBytes(expbytes);

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAKeyInfo = new RSAParameters
                {
                    Modulus = modulus,
                    Exponent = exponent
                };
                RSA.ImportParameters(RSAKeyInfo);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }
        #endregion

        #region 解码私钥 -static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privateKey)
        /// <summary>
        /// 解码私钥
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privateKey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            MemoryStream mem = new MemoryStream(privateKey);
            BinaryReader binr = new BinaryReader(mem);
            try
            {
                ushort twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    return null;
                byte bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;

                int elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAparams = new RSAParameters
                {
                    Modulus = MODULUS,
                    Exponent = E,
                    D = D,
                    P = P,
                    Q = Q,
                    DP = DP,
                    DQ = DQ,
                    InverseQ = IQ
                };
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }
            finally { binr.Close(); }
        }
        #endregion

        #region 获得一个数字大小 -static int GetIntegerSize(BinaryReader binr)
        /// <summary>
        /// 获得一个数字大小
        /// </summary>
        /// <param name="binr"></param>
        /// <returns></returns>
        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            int count;
            if (bt == 0x81)
            {
                count = binr.ReadByte();
            }
            else if (bt == 0x82)
            {
                byte highbyte = binr.ReadByte();
                byte lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }
            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }
        #endregion

        #region 转换公钥 -static RSAParameters ConvertFromPublicKey(string pemFileConent)
        /// <summary>
        /// 转换公钥
        /// </summary>
        /// <param name="pemFileConent">PEM公钥</param>
        /// <returns></returns>
        private static RSAParameters ConvertFromPublicKey(string pemFileConent)
        {
            byte[] keyData = Convert.FromBase64String(pemFileConent);
            if (keyData.Length < 162)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }
            byte[] pemModulus = new byte[128];
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, 29, pemModulus, 0, 128);
            Array.Copy(keyData, 159, pemPublicExponent, 0, 3);
            RSAParameters para = new RSAParameters
            {
                Modulus = pemModulus,
                Exponent = pemPublicExponent
            };
            return para;
        }
        #endregion

        #region 转换私钥 -static RSAParameters ConvertFromPrivateKey(string pemFileConent)
        /// <summary>
        /// 转换私钥
        /// </summary>
        /// <param name="pemFileConent">PEM 私钥</param>
        /// <returns></returns>
        private static RSAParameters ConvertFromPrivateKey(string pemFileConent)
        {
            byte[] keyData = Convert.FromBase64String(pemFileConent);
            if (keyData.Length < 609)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }

            int index = 11;
            byte[] pemModulus = new byte[128];
            Array.Copy(keyData, index, pemModulus, 0, 128);

            index += 128;
            index += 2;
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, index, pemPublicExponent, 0, 3);

            index += 3;
            index += 4;
            byte[] pemPrivateExponent = new byte[128];
            Array.Copy(keyData, index, pemPrivateExponent, 0, 128);

            index += 128;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);
            byte[] pemPrime1 = new byte[64];
            Array.Copy(keyData, index, pemPrime1, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//346  
            byte[] pemPrime2 = new byte[64];
            Array.Copy(keyData, index, pemPrime2, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);
            byte[] pemExponent1 = new byte[64];
            Array.Copy(keyData, index, pemExponent1, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);
            byte[] pemExponent2 = new byte[64];
            Array.Copy(keyData, index, pemExponent2, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);
            byte[] pemCoefficient = new byte[64];
            Array.Copy(keyData, index, pemCoefficient, 0, 64);

            RSAParameters para = new RSAParameters
            {
                Modulus = pemModulus,
                Exponent = pemPublicExponent,
                D = pemPrivateExponent,
                P = pemPrime1,
                Q = pemPrime2,
                DP = pemExponent1,
                DQ = pemExponent2,
                InverseQ = pemCoefficient
            };
            return para;
        }
        #endregion
    }
}
