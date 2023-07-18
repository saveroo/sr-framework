using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DevExpress.XtraEditors;
using Flurl.Http;
using Newtonsoft.Json;
using SmartAssembly.Attributes;

namespace SRUL
{
    
    [Serializable]
    [DoNotPruneType]
    [DoNotObfuscateType]
    [DoNotPrune]
    public struct TSteamResponse
    {
        public TSteamPlayerResponse response;
    }
    
    [Serializable]
    [DoNotPruneType]
    [DoNotObfuscateType]
    [DoNotPrune]
    public struct TSteamPlayerResponse
    {
        public int player_count;
        public int result;
    }

    public class APICompare
    {
        public string query { get; set; }
        public string value { get; set; }
        public bool result { get; set; }
        public int status { get; set; }
    }

    public class APIData
    {
        public int status { get; set; }
        public Root body { get; set; }
    }

    public class APIEncryptedBody
    {
        public int status { get; set; }
        public string author { get; set; }
        public string body { get; set; }
    }

    public class APIRegisterClient
    {
        // [JsonProperty("ref")]
        public string refId { get; set; }
        public string playerSteamID { get; set; }
        public string playerSteamPersona { get; set; }
        public string deviceId { get; set; }
        public string data { get; set; }
    }

    public class APIOnlineStatusRequest
    {
        public string refId { get; set; }
        public bool IsOnline { get; set; }
    }

    public class APISRPlayers
    {
        public List<SRClient> Players { get; set; }
    }
    public enum EnumPath
    {
        Register,
        Update,
        Data,
        Steam,
        MandatoryUpdate,
        OnlineStatus,
        RetrievePlayers,
        Donators,
        Whitelist
    }
    public static class EndpointPath
    {
        public const string AbsolutePath = "api";
        public const string Register = "client";
        public const string PostSteamId = "client";
        public const string Update = "check";
        public const string Data = "data";
        public const string OnlineStatus = "client";
        public const string RetrievePlayers = "client";
        public const string Donators = "data";
        public const string Whitelist = "check";
    }

    public interface ICrypto
    {
        string Decrypt(string base64String, byte[] secretKey, byte[] secretSalt);
        string ByteDecrypt(string base64String, byte[] secretKey, byte[] secretSalt);
        T DeserializeAs<T>(string encryptedString);
        string Encrypt(string strDataToEncrypt, byte[] secretKey, byte[] secretSalt);
        void OverwriteFileWith(byte[] strDataToEncrypt);
        string SrEncryptor(string data);
        string SrDecryptor(string data);
        string BytesSrDecryptor(byte[] data);
        string HashMessage(string key, string message);
    }

    public class SrCrypto : ICrypto
    {
        public string Decrypt(string base64String, byte[] secretKey, byte[] secretSalt)
        {
            byte[] encryptedBytes;
            try
            {
                encryptedBytes = Convert.FromBase64String(base64String);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException($"Data is not valid~!", e);
            }
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            
            byte[] passwordBytes = secretKey;
            byte[] salt = secretSalt;
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);    
            aes.KeySize = 256; //Not Required
            aes.BlockSize = 128; //Not Required
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8); //2314345645678765
            ICryptoTransform crypto = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] secret = crypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            crypto.Dispose();
            var result = ASCIIEncoding.ASCII.GetString(secret);
            return result;
        }
        
        public string ByteDecrypt(string bytesString, byte[] secretKey, byte[] secretSalt)
        {
            byte[] encryptedBytes = Convert.FromBase64String(bytesString);
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            
            byte[] passwordBytes = secretKey;
            byte[] salt = secretSalt;
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);    
            aes.KeySize = 256; //Not Required
            aes.BlockSize = 128; //Not Required
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8); //2314345645678765
            ICryptoTransform crypto = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] secret = crypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            crypto.Dispose();
            var result = Convert.ToBase64String(secret);
            return result;
        }

        public string Encrypt(string strDataToEncrypt, byte[] secretKey, byte[] secretSalt)
        {
            byte[] encryptedBytes = Encoding.UTF8.GetBytes(strDataToEncrypt);
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            
            byte[] passwordBytes = secretKey;
            byte[] saltBytes = secretSalt;
            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);    
            aes.KeySize = 256; //Not Required
            aes.BlockSize = 128; //Not Required
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8); //2314345645678765
            ICryptoTransform crypto = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] secret = crypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            crypto.Dispose();
            var result = Convert.ToBase64String(secret);
            return result;
        }

        public void OverwriteFileWith(byte[] encryptedString)
        {
            var str = Convert.ToBase64String(encryptedString);
            var str2 = SrEncryptor(str);
            try
            {
                using (var fs = File.Create(SrConfig.FileGetPathName()))
                {
                    fs.Write(Encoding.UTF8.GetBytes(str2), 0, str2.Length);
                }
                    // File.CreateText(SrConfig.FileGetPathName()).Write(encryptedString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public T DeserializeAs<T>(string encryptedString)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(encryptedString);
            }
            catch (Exception e)
            {
                XtraMessageBox.Show("Error: " + e.Message, "Error");
                throw;
            }
        }

        public string SrEncryptor(string data)
        {
            var key = SrConfig.CryptKey;
            var salt = SrConfig.CryptSalt;
            var encrypted = Encrypt(data, key, salt);
            return encrypted;
        }
        
        public string SrDecryptor(string data)
        {
            var key = SrConfig.CryptKey;
            var salt = SrConfig.CryptSalt;
            var decrypted = Decrypt(data, key, salt);
            return decrypted;
        }
        public string BytesSrDecryptor(byte[] data)
        {
            var key = SrConfig.CryptKey;
            var salt = SrConfig.CryptSalt;
            var decrypted = ByteDecrypt(Encoding.UTF8.GetString(data), key, salt);
            return decrypted;
        }
            
            public string HashMessage(string secretKey, string message)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var msgBytes = Encoding.ASCII.GetBytes(message);
            using (var h = new HMACSHA512(keyBytes))
            {
                var hashMessage = h.ComputeHash(msgBytes);
                var r = BitConverter.ToString(hashMessage).Replace("-", "").ToLower();
                return r;
            }
        }
    }

    public static class SrConfig
    {
        public static readonly byte[] CryptKey = 
            { 83, 82, 70, 114, 97, 109, 101, 119, 111, 114, 107 };
        public static readonly byte[] CryptSalt = 
            { 77, 117, 104, 97, 109, 109, 97, 100, 83, 117, 114, 103, 97, 83, 97, 118, 101, 114, 111 };
        public static bool OfflineMode = false;
        public static string OfflineDataLink = "";
        public static Root? DataMain { get; set; }
        public static Root? DataServer { get; set; }
        public static string DataFilePath { get; set; } = "./";
        public static string DataFileName { get; set; } = "meta.sr";
        public static string CacheFilePath { get; }

        public static string FileGetPathName()
        {
            try
            {
                var filePath = $"{DataFilePath}{DataFileName}";
                if (File.Exists(filePath))
                    return filePath;
            }
            catch (Exception e)
            { 
                throw new Exception("Data file is missing");
            }
            
            return "";
        }

        public static bool IsDataFileExist()
        {
            return File.Exists(DataFilePath + DataFileName);
        }

        public static void FileDeleteFromPath()
        {
            var filePath = DataFilePath + DataFileName;
            if (File.Exists(filePath))
                File.Delete(filePath);
            // throw new Exception("Data file is missing");
        }
    }
}