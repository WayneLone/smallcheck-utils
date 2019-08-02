using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smallcheck.Utils.Encryption;
using System;
using System.IO;

namespace Smallcheck.Utils.Test
{
    [TestClass]
    public class EncryptionTest
    {
        [TestMethod]
        public void TestMD5()
        {
            string result = HashUtils.MD5("hello");
            Assert.AreEqual("5d41402abc4b2a76b9719d911017c592", result);
        }

        [TestMethod]
        public void TestMD5_16()
        {
            string result = HashUtils.MD5_16("hello");
            Assert.AreEqual("bc4b2a76b9719d91", result);
        }

        [TestMethod]
        public void TestHMACSHA1()
        {
            string data = "hello";
            string key = "privateKey";
            string result = HashUtils.HMACSHA1(data, key);
            Assert.AreEqual("5b6ef0a9dbebe87637bbe2971d9b0eb0cf878b16", result);
        }

        [TestMethod]
        public void TestHMACSHA256()
        {
            string data = "hello";
            string key = "privateKey";
            string result = HashUtils.HMACSHA256(data, key);
            Assert.AreEqual("369aa70ad2d02d3790dcf6b507a1a61b836a9eeabc78037991bc0aff538ae2d7", result);
        }

        [TestMethod]
        public void TestAESEncrypt()
        {
            string key = "706a344a66ff2e45";
            string data = "hello";
            string result = AESUtil.AESEncrypt(data, key);
            Assert.AreEqual("Z6sZ75oV3Ekuy0XIB9ks5Q==", result);
        }

        [TestMethod]
        public void TestAESDecrypt()
        {
            string key = "706a344a66ff2e45";
            string data = "Z6sZ75oV3Ekuy0XIB9ks5Q==";
            string result = AESUtil.AESDecrypt(data, key);
            Assert.AreEqual("hello", result);
        }

        [TestMethod]
        public void TestRSA()
        {
            string data = "hello";
            RSAUtil.GetKeyPairXMLText(out string publicKey, out string privateKey);

            // 加解密
            string encryptedText = RSAUtil.EncryptByXMLKey(data, publicKey);
            string result = RSAUtil.DecryptByXMLKey(encryptedText, privateKey);
            Assert.AreEqual(data, result);

            // 签名验证
            string signature = RSAUtil.SignWithXML(data, privateKey);
            Console.WriteLine(signature);
            bool isValidSignature = RSAUtil.VerifyWithXML(data, signature, publicKey);
            Assert.IsTrue(isValidSignature);
        }

        [TestMethod]
        public void TestRSAWithPEM()
        {
            /**
             * RSA加密测试,RSA中的密钥对通过SSL工具生成，生成命令如下
             * 1 生成RSA私钥
             * > openssl genrsa -out rsa_private_key.pem 1024
             * 2.生成RSA公钥
             * > openssl rsa -in rsa_private_key.pem -pubout -out rsa_public_key.pem
             * 3. 将RSA私钥转换成PKCS8格式
             * > openssl pkcs8 -topk8 -inform PEM -in rsa_private_key.pem -outform PEM -nocrypt -out rsa_pub_pk8.pem
             */

            // rsa_pub_pk8.pem内容
            string privateKey = File.ReadAllText("PEM/rsa_pub_pk8.pem")
                .Replace("-----BEGIN PRIVATE KEY-----", "")
                .Replace("-----END PRIVATE KEY-----", "")
                .Replace("\n", "");
            // rsa_public_key.pem内容
            string publicKey = File.ReadAllText("PEM/rsa_public_key.pem")
                .Replace("-----BEGIN PUBLIC KEY-----", "")
                .Replace("-----END PUBLIC KEY-----", "")
                .Replace("\n", "");
            string plainText = "hello, world!";

            // 加解密
            string encryptedData = RSAUtil.EncryptWithPEM(plainText, publicKey);
            Console.WriteLine(encryptedData);
            string result = RSAUtil.DecryptWithPEM(encryptedData, privateKey);
            Assert.AreEqual(result, plainText);

            // 签名验签
            string signature = RSAUtil.SignWithPEM(plainText, privateKey);
            Console.WriteLine(signature);
            bool isValidSignature = RSAUtil.VerifyWithPEM(plainText, signature, publicKey);
            Assert.AreEqual(true, isValidSignature);
        }

        [TestMethod]
        public void TestXXTEA()
        {
            string plainText = "hello";
            string privateKey = "abcd";
            string encryptedText = plainText.XXTeaEncrypt(privateKey);
            Assert.AreEqual("c9706158ba7fa38a55ac581c9004039d9f7e701489c5ee36bb5c2502a5b32a3a", encryptedText);
            Assert.AreEqual(plainText, "c9706158ba7fa38a55ac581c9004039d9f7e701489c5ee36bb5c2502a5b32a3a".XXTeaDecrypt(privateKey));
        }
    }
}
