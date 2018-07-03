using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace ZarknorthClient
{
    public static class Encryption
    {
        static byte[] entropy = System.Text.Encoding.Unicode.GetBytes("a​0​0​8​d​9​c​c​a​a​4​d​7​5​6​e​0​f​1​9​d​a​c​8​f​d​e​1​8​c​f​3​3​b​3​a​5​b​b​3​b​3​d​f​4​b​d​2​5​8​0​0​b​b​d​d​a​4​0​9​1​b​9​6​f​4​d​6​1​a​e​8​1​c​4​7​1​7​d​2​8​3​0​9​9​b​5​c​b​d​2​6​8​1​a​5​e​2​d​c​b​1​8​e​b​d​2​1​4​a​9​3​c​9​6​0​b​f​9​5​3​5​7​1​a​7​f​2");

        public static string EncryptString(System.Security.SecureString input)
        {
            byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                System.Text.Encoding.Unicode.GetBytes(ToInsecureString(input)),
                entropy,
                System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        public static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    entropy,
                    System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return ToSecureString(System.Text.Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return new SecureString();
            }
        }

        public static SecureString ToSecureString(string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        public static string ToInsecureString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }
    }
}
