
using System;
using System.Security.Cryptography;
using System.Text;

public class PasswordEncryptor
{
    public static int Iterations = 1000; //Mudar-Aqui---
    public static string Key = "Mudar-Aqui---asdfçlkj"; //Mudar-Aqui---
    public static string SuperUserSalt = "5wcCZP3vVmU="; //Mudar-Aqui---
    public static string SuperUserPasswordEncrypted = "Iy1/Q6GFBO6Pfg=="; //Mudar-Aqui---
    private string salt;

    public PasswordEncryptor(string salt)
    {
        this.salt = salt;
    }

    public string EncryptPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return "";
        int keyLength = password.Length * 2;

        byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        using (var hmac = new HMACSHA256(passwordBytes))
        {
            byte[] derived = PBKDF2(hmac, saltBytes, Iterations, keyLength);
            return Convert.ToBase64String(derived);
        }
    }

    private static byte[] PBKDF2(HMAC hmac, byte[] salt, int iterations, int outputBytes)
    {
        int hashLength = hmac.HashSize / 8;
        int keyBlocks = (int)Math.Ceiling((double)outputBytes / hashLength);
        byte[] output = new byte[outputBytes];
        byte[] buffer = new byte[hashLength];
        byte[] block = new byte[salt.Length + 4];

        Buffer.BlockCopy(salt, 0, block, 0, salt.Length);

        for (int i = 1; i <= keyBlocks; i++)
        {
            block[salt.Length] = (byte)(i >> 24);
            block[salt.Length + 1] = (byte)(i >> 16);
            block[salt.Length + 2] = (byte)(i >> 8);
            block[salt.Length + 3] = (byte)(i);

            byte[] temp = hmac.ComputeHash(block);
            Array.Copy(temp, 0, buffer, 0, hashLength);

            for (int j = 1; j < iterations; j++)
            {
                temp = hmac.ComputeHash(temp);
                for (int k = 0; k < hashLength; k++)
                {
                    buffer[k] ^= temp[k];
                }
            }

            int offset = (i - 1) * hashLength;
            int length = Math.Min(hashLength, outputBytes - offset);
            Array.Copy(buffer, 0, output, offset, length);
        }

        return output;
    }

    public static string GenerateRandomKey()
    {
        byte[] randomBytes = new byte[8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }


    public static string GenerateActivationKey(string padrao)
    {
        string randomKey = GenerateRandomKey();
        string combined = padrao + randomKey;
        string activationKey = combined;
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(combined);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            activationKey = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
        return activationKey;
    }
}
