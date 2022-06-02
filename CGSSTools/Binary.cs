using System;
using System.Collections.Generic;
using System.Text;

namespace CGSSTools
{
    public class Binary
    {
        public static byte[] StringToBytes(string str)
        {
            str = str.Replace("-", string.Empty);

            List<byte> bs = new List<byte>();
            for (int i = 0; i < str.Length / 2; i++)
            {
                bs.Add(Convert.ToByte(str.Substring(i * 2, 2), 16));
            }

            return bs.ToArray();
        }

        public static string BytesToString(byte[] bs)
        {
            string str = BitConverter.ToString(bs);
            str = str.Replace("-", string.Empty);

            return str;
        }

        public static string md5(string str)
        {

            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] srcBytes = Encoding.UTF8.GetBytes(str);
            byte[] destBytes = md5.ComputeHash(srcBytes);

            StringBuilder destStrBuilder;
            destStrBuilder = new StringBuilder();
            foreach (byte curByte in destBytes)
            {
                destStrBuilder.Append(curByte.ToString("x2"));
            }

            return destStrBuilder.ToString();
        }

        public static string sha1(string str)
        {

            System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();

            byte[] srcBytes = Encoding.UTF8.GetBytes(str);
            byte[] destBytes = sha1.ComputeHash(srcBytes);

            StringBuilder destStrBuilder;
            destStrBuilder = new StringBuilder();
            foreach (byte curByte in destBytes)
            {
                destStrBuilder.Append(curByte.ToString("x2"));
            }

            return destStrBuilder.ToString();
        }
    }
}