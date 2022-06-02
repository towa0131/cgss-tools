using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace CGSSTools
{
    public class Rijndael
    {
        public static string Encrypt256(byte[] data, byte[] key, byte[] iv, int keySize = 128)
        {
            byte[] encrypted;
            AesManaged rijndael = Rijndael.GetAES128(key, iv, keySize);
            using (ICryptoTransform encryptor = rijndael.CreateEncryptor(key, iv))
            {
                encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);
            }

            string result = System.Convert.ToBase64String(encrypted);

            return result;
        }

        public static string Decrypt256(byte[] data, byte[] key, byte[] iv, int keySize = 128)
        {
            AesManaged rijndael = Rijndael.GetAES128(key, iv, keySize);
            ICryptoTransform decryptor = rijndael.CreateDecryptor(key, iv);

            byte[] plain = new byte[data.Length];
            using (MemoryStream mStream = new MemoryStream(data))
            {
                using (CryptoStream ctStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
                {
                    ctStream.Read(plain, 0, plain.Length);
                }
            }

            return Encoding.UTF8.GetString(plain).TrimEnd(new char[1]);
        }

        public static AesManaged GetAES128(byte[] key, byte[] iv, int keySize)
        {
            AesManaged aes = new AesManaged();
            aes.BlockSize = 128;
            aes.KeySize = keySize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            aes.IV = iv;
            aes.Key = key;

            return aes;
        }

        public static AesManaged GetAES256(byte[] key, byte[] iv)
        {
            AesManaged aes = new AesManaged();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            aes.IV = iv;
            aes.Key = key;

            return aes;
        }
    }
}
