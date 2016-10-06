using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Libs
{
    public static class Utils
    {
        public static string ConvertBytes(double sizeInBytes)
        {
            string functionReturnValue;
            if (sizeInBytes >= 1048576)
            {
                functionReturnValue = string.Format("{0:0} MB", sizeInBytes / 1024 / 1024);
            }
            else if (sizeInBytes >= 1024)
            {
                functionReturnValue = string.Format("{0:0} KB", sizeInBytes / 1024);
            }
            else if (sizeInBytes < 1024 && sizeInBytes > 0)
            {
                functionReturnValue = string.Format("1 KB");
            }
            else
            {
                functionReturnValue = string.Format("0 KB");
            }
            return functionReturnValue;
        }
        public static long CopyTo(Stream source, Stream destination)
        {
            var buffer = new byte[2048];
            int bytesRead;
            long totalBytes = 0;
            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, bytesRead);
                totalBytes += bytesRead;
            }
            return totalBytes;
        }

        public static async Task<long> CopyToAsync(Stream source, Stream destination)
        {
            return await Task.Factory.StartNew(() => CopyTo(source, destination));
            //var buffer = new byte[2048];
            //int bytesRead;
            //long totalBytes = 0;
            //while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
            //{
            //    await destination.WriteAsync(buffer, 0, bytesRead);
            //    totalBytes += bytesRead;
            //}
            //return totalBytes;
        }

        /// <summary>
        /// test if file is readonly
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>true if readonly</returns>
        public static bool IsFileReadOnly(string fileName)
        {
            var fInfo = new FileInfo(fileName);
            return fInfo.IsReadOnly;
        }

        #region "Password Encrypt / Decrypt"
        public const string EncKey = "PSVJQRk4QTEpNVU1D1UZCRVFGV1VVT0ZOV1RRU1NaWQ=";
        public const string EncAesIv = "YWlFLV4ZZUFNaWlh1Q01ZT0lLWU5HTFJQVFNCRUJZVA=";
        public static string EncryptString(string plainText, string sharedSecret, string aesiv)
        {
            var aes = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 256,
                Padding = PaddingMode.PKCS7,
                Key = Convert.FromBase64String(sharedSecret),
                IV = Convert.FromBase64String(aesiv)
            };
            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    var xXml = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(xXml, 0, xXml.Length);
                }
                xBuff = ms.ToArray();
            }
            var output = Convert.ToBase64String(xBuff);
            return output;
        }
        public static string DecryptStrings(string cipherText, string sharedSecret, string aesiv)
        {
            var aes = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 256,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                Key = Convert.FromBase64String(sharedSecret),
                IV = Convert.FromBase64String(aesiv)
            };
            var decrypt = aes.CreateDecryptor();
            byte[] xBuff;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    var xXml = Convert.FromBase64String(cipherText);
                    cs.Write(xXml, 0, xXml.Length);
                }
                xBuff = ms.ToArray();
            }
            var output = Encoding.UTF8.GetString(xBuff);
            return output;
        }
        #endregion

        public static string RemoveAllSlashAtFinish(string url)
        {
            if (!url.EndsWith("/")) return url;
            url = url.Substring(0, url.LastIndexOf('/'));
            url = RemoveAllSlashAtFinish(url);
            return url;
        }

        //public static string CombineUrl(this string url1, string url2)
        //{
        //    url1 = url1.Trim('/');
        //    url2 = url2.Trim('/');
        //    return string.Format("{0}/{1}", url1, url2);
        //}

        public static string CombineUrl(this string url1, params string[] agrs)
        {
            if (agrs.Length == 1)
            {
                url1 = url1.TrimEnd('/');
                var url2 = agrs[0].TrimStart('/');
                return string.Format("{0}/{1}", url1, url2);
            }

            var url11 = agrs[0];
            //var agrs11 = agrs.to
            var a = agrs.ToList();
            a.RemoveAt(0);
            agrs = a.ToArray();
            return url1.CombineUrl(url11.CombineUrl(agrs));
        }

        /// <summary>
        /// Get a root url ensure doesnt ended with "/"
        /// </summary>
        /// <param name="url"></param>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public static string GetRootUrl(string url, string relativeUrl)
        {
            var url1 = RemoveAllSlashAtFinish(url.Trim()).ToLower();
            var relativeUrl1 = RemoveAllSlashAtFinish(relativeUrl.Trim()).ToLower();
            var index = url1.LastIndexOf(relativeUrl1, StringComparison.Ordinal);
            var result = index < 0
                ? url1
                : url1.Remove(index, relativeUrl1.Length);

            result = RemoveAllSlashAtFinish(result);
            return result;
        }

        //public static string CombineUrl(string url, string relativeUrl)
        //{
            
        //}
    }
}
