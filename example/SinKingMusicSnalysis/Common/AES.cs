using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SinKingMusicSnalysis.Common
{
    class AES
    {
        /// <summary>
        /// 有密码的AES加密 128位
        /// </summary>
        /// <param name="text">加密字符</param>
        /// <param name="password">加密的密码</param>
        /// <param name="iv">密钥</param>
        /// <returns></returns>
        public static string Encrypt128(string toEncrypt, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// AES解密 128位
        /// </summary>
        /// <param name="text"></param>
        /// <param name="password"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string Decrypt128(string toDecrypt, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>  
        /// AES加密256
        /// </summary>  
        /// <param name="encryptStr">明文</param>  
        /// <param name="key">密钥</param>  
        /// <returns></returns>
        public static string Encrypt256(string encryptStr, byte[] key,byte[] iv)
        {
            var _aes = new AesCryptoServiceProvider();
            _aes.BlockSize = 128;
            _aes.KeySize = 256;
            _aes.Key = key;
            _aes.IV = iv;
            _aes.Padding = PaddingMode.PKCS7;
            _aes.Mode = CipherMode.CBC;
            var _crypto = _aes.CreateEncryptor(_aes.Key, _aes.IV);
            byte[] encrypted = _crypto.TransformFinalBlock(Encoding.UTF8.GetBytes(encryptStr), 0, Encoding.UTF8.GetBytes(encryptStr).Length);

            _crypto.Dispose();

            return System.Convert.ToBase64String(encrypted);
        }
        /// <summary>  
        /// AES解密256
        /// </summary>  
        /// <param name="decryptStr">密文</param>  
        /// <param name="key">密钥</param>  
        /// <returns></returns>  
        public static string Decrypt256(string decryptStr, string key)
        {
            var _aes = new AesCryptoServiceProvider();
            _aes.BlockSize = 128;
            _aes.KeySize = 256;
            _aes.Key = Encoding.UTF8.GetBytes(key);
            _aes.IV = (byte[])(object)new sbyte[16];//Encoding.UTF8.GetBytes(IV);
            _aes.Padding = PaddingMode.PKCS7;
            _aes.Mode = CipherMode.CBC;

            var _crypto = _aes.CreateDecryptor(_aes.Key, _aes.IV);
            byte[] decrypted = _crypto.TransformFinalBlock(
                System.Convert.FromBase64String(decryptStr), 0, System.Convert.FromBase64String(decryptStr).Length);
            _crypto.Dispose();
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
