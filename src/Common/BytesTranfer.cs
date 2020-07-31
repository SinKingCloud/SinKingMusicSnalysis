using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SinKingMusicSnalysis.Common
{
    class BytesTranfer
    {
        public static string StringToHexArray(string input)
        {
            char[] values = input.ToCharArray();
            StringBuilder sb = new StringBuilder(input.Length * 3);
            foreach (char letter in values)
            {
                int value = Convert.ToInt32(letter);
                string hexOutput = string.Format("{0:X}", value);
                sb.Append(hexOutput);
            }
            return sb.ToString().ToUpper();
        }
        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            }
            return sb.ToString().ToUpper();
        }
        public static string UnicodeDencode(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            return Regex.Unescape(str);
        }
        public static string UnicodeEncode(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            StringBuilder strResult = new StringBuilder();
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    strResult.Append("\\u");
                    strResult.Append(((int)str[i]).ToString("x4"));
                }
            }
            return strResult.ToString();
        }
        public static string HexStringToASCII(string hexstring)
        {
            byte[] bt = HexStringToBinary(hexstring);
            string lin = "";
            for (int i = 0; i < bt.Length; i++)
            {
                lin = lin + bt[i] + " ";
            }
            string[] ss = lin.Trim().Split(new char[] { ' ' });
            char[] c = new char[ss.Length];
            int a;
            for (int i = 0; i < c.Length; i++)
            {
                a = Convert.ToInt32(ss[i]);
                c[i] = Convert.ToChar(a);
            }
            string b = new string(c);
            return b;
        }
        public static byte[] HexStringToBinary(string hexstring)
        {

            string[] tmpary = hexstring.Trim().Split(' ');
            byte[] buff = new byte[tmpary.Length];
            for (int i = 0; i < buff.Length; i++)
            {
                buff[i] = Convert.ToByte(tmpary[i], 16);
            }
            return buff;
        }
        public static byte[] HexStringToBytes(string hexStr)
        {
            if (string.IsNullOrEmpty(hexStr))
            {
                return new byte[0];
            }

            if (hexStr.StartsWith("0x"))
            {
                hexStr = hexStr.Remove(0, 2);
            }

            var count = hexStr.Length;

            if (count % 2 == 1)
            {
                throw new ArgumentException("Invalid length of bytes:" + count);
            }

            var byteCount = count / 2;
            var result = new byte[byteCount];
            for (int ii = 0; ii < byteCount; ++ii)
            {
                var tempBytes = Byte.Parse(hexStr.Substring(2 * ii, 2), System.Globalization.NumberStyles.HexNumber);
                result[ii] = tempBytes;
            }

            return result;
        }
        ///编码
        public static string EncodeBase64(string code_type, string code)
        {
            string encode = "";
            byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }
        ///解码
        public static string DecodeBase64(string code_type, string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.GetEncoding(code_type).GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }
    }
}
