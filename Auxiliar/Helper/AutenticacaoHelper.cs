using System;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Auxiliar.Helper
{
    public static class AutenticacaoHelper
    {

        public static bool ValidaSenha(string senha)
        {
            return (
                    senha.Length > 6
                    && Regex.IsMatch(senha, "[A-Z]")
                    && Regex.IsMatch(senha, "[a-z]")
                    && Regex.IsMatch(senha, "[0-9]")
                );
        }

        public static int getIdByToken(ClaimsPrincipal user)
        {
            var id = 0;
            int.TryParse(user?.FindFirst(ClaimTypes.NameIdentifier).Value, out id);
            return id;
        }

        // Criptografa o texto usando AES e uma chave
        public static string Encrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            return EnAndDecrypt(value);
        }

        // Descriptografa o texto usando AES e a mesma chave
        public static string Decrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            return EnAndDecrypt(value, false);
        }

        private static string EnAndDecrypt(string value, bool isEncrypt = true)
        {
            try
            {
                byte[] keyBytes = GetKeyBytes(PasswordEncryptor.Key);
                byte[] ivBytes = new byte[16]; // Mesmo IV usado na criptografia

                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = ivBytes;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (MemoryStream ms = new MemoryStream())
                    using (ICryptoTransform subject = (isEncrypt) ? aes.CreateEncryptor() : aes.CreateDecryptor())
                    using (CryptoStream cs = new CryptoStream(ms, subject, CryptoStreamMode.Write))
                    {
                        byte[] subjectBytes = (isEncrypt)
                            ? Encoding.UTF8.GetBytes(value)
                            : Convert.FromBase64String(value);
                        cs.Write(subjectBytes, 0, subjectBytes.Length);
                        cs.FlushFinalBlock();
                        return (isEncrypt)
                            ? Convert.ToBase64String(ms.ToArray())
                            : Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
            catch { }
            return "error - invalido";
        }

        // Gera uma chave de 256 bits a partir da string fornecida
        private static byte[] GetKeyBytes(string key)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(key));
            }
        }

        public static string GeraSenha(string usuario, string senha)
        {
            return ComputeSha256Hash(senha + usuario.ToLower());
        }

        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string GeraCodigo(string original)
        {
            original = original.ToLower();
            var meio = original.Length / 2;
            return ComputeSha256Hash(
                        original.Substring(meio) 
                        + PasswordEncryptor.Key.Substring(0,6) 
                        + original.Substring(0, meio)
                    );
        }

        public static bool ValidaCodigo(string original, string codigo)
        {
            return codigo.Equals(GeraCodigo(original));
        }

        public static string EncryptCodigoURL(string original)
        {
            return WebUtility.UrlEncode((PasswordEncryptor.Key.Substring(0, 6) + original));
        }

        public static string DecryptCodigoURL(string codigo)
        {
            return codigo.Replace(PasswordEncryptor.Key.Substring(0, 6), "");
        }

        public static string GeraCaracter(int numero)
        {
            if (numero == 0 || numero > PasswordEncryptor.Key.Length)
            {
                var random = new Random();
                return PasswordEncryptor.Key.Substring(random.Next(0, PasswordEncryptor.Key.Length), 1);
            }
            return PasswordEncryptor.Key.Substring(numero - 1, 1);
        }

        public static string GeraHash(int inicial, int comprimento)
        {
            inicial = inicial % (PasswordEncryptor.Key.Length + 1);
            string id = GeraCaracter(inicial);
            for (int i = 1; i < comprimento; i++)
                id += GeraCaracter(0);
            return id;
        }
    }
}
