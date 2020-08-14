using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Newtonsoft.Json;

namespace SRUL
{
    public class SREncryptor
    {
        //  Call this function to remove the key from memory after use for security
        [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
        public static extern bool ZeroMemory(IntPtr Destination, int Length);

        public string filename = System.IO.Path.GetTempPath() + "SRFramework" + ".tmp";
        // private string inputFileTmp = ".tmp";
        private string _url = SRFApis.APIUrl;
        public string decrypted { get; set; }
        public Root Data { get; set; }
        public bool Done = false;
        private bool Deserialized = false;
        private bool _IsEncrypting = false;
        private bool _IsDecrypting = false;
        private bool _IsDownloading = false;
        private int _retryCount = 0;
        private int _retryMax = 10;
        private APIData _apiData { get; set; }
        private static readonly Lazy<SREncryptor> _instance = new Lazy<SREncryptor>(() => new SREncryptor());
        // private static readonly Lazy<JSONReader> JsonReader = new Lazy<JSONReader>(() => new JSONReader());

        public static SREncryptor Instance
        {
            get { return _instance.Value; }
        }

        /// <summary>
        /// Creates a random salt that will be used to encrypt your file. This method is required on FileEncrypt.
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateRandomSalt()
        {
            byte[] data = new byte[32];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < 10; i++)
                {
                    rng.GetBytes(data);
                }
            }
            return data;
        }

        /// <summary>
        /// Encrypts a file from its path and a plain password.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="password"></param>
        private void FileEncrypt(byte[] inputBuffer, string password)
        {
            _IsEncrypting = true;
            // byte[] salt = GenerateRandomSalt();
            byte[] salt = {4, 7};

            //create output file name
            FileStream fsCrypt = new FileStream(filename, FileMode.Create);
            // MemoryStream fsCrypt = new MemoryStream(inputBuffer);
            
            //convert password to byte array
            byte[] passBytes = System.Text.Encoding.UTF8.GetBytes(password);
            
            //Set Rijndael symmetric encryption algorithm
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;
            
            //http://stackoverflow.com/questions/2659214/why-do-i-need-to-use-the-rfc2898derivebytes-class-in-net-instead-of-directly
            //"What it does is repeatedly hash the user password along with the salt." High iteration counts.
            var key = new Rfc2898DeriveBytes(passBytes, salt, 1000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);

            AES.Mode = CipherMode.CFB;
            
            // write salt to the begining of the output file, so in this case can be random every time
            fsCrypt.Write(salt, 0, salt.Length);
            
            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);
            // FileStream fsIn = new FileStream(filename, FileMode.Open);
            MemoryStream fsIn = new MemoryStream(inputBuffer);
            
            // Buffer
            byte[] buffer = new byte[500000];
            int read;

            try
            {
                while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Application.DoEvents();
                    cs.Write(buffer, 0, read);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                cs.Close();
                fsCrypt.Close();
                _IsEncrypting = false;
                // SaveToTMP(Encoding.UTF8.GetString(buffer));
            }
        }
        
        /// <summary>
        /// Decrypts an encrypted file with the FileEncrypt method through its path and the plain password.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="password"></param>
        private void FileDecrypt(string inputFile, string password)
        {
            _IsDecrypting = true;
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[32];

            FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
            fsCrypt.Read(salt, 0, salt.Length);

            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CFB;

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);
            MemoryStream ms = new MemoryStream();
            // FileStream fsOut = new FileStream(filename+".json", FileMode.OpenOrCreate);

            int read;
            // byte[] buffer = new byte[1048576];
            byte[] buffer = new byte[500000];

            try
            {
                while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    // fsOut.Write(buffer, 0, read);
                    Application.DoEvents();
                    ms.Write(buffer, 0, read);
                }
            }
            catch (CryptographicException ex_CryptographicException)
            {
                Done = false;
                Console.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
            }
            catch (Exception ex)
            {
                Done = false;
                Debug.WriteLine("Error WHAT: " + ex.Message);
            }

            try
            {
                cs.Close();
                ms.Close();
            }
            catch (Exception ex)
            {
                Done = false;
                Debug.WriteLine("Error by closing CryptoStream: " + ex.Message);
            }
            finally
            {
                fsCrypt.Close();
                decrypted = Encoding.UTF8.GetString(buffer);
                Debug.WriteLine(decrypted);
                Data = JsonConvert.DeserializeObject<Root>(decrypted);
                if (Data.SRFAuthor == "Muhammad Surga Savero")
                    Deserialized = true;
                else
                    Deserialized = false;
                _IsDecrypting = false;
            }
        }

        public Root BufferDecryption(string encrypted,string secretKey)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encrypted);
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            
            byte[] passwordBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] salt = new byte[8];
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);    
            aes.KeySize = 256; //Not Required
            aes.BlockSize = 128; //Not Required
            aes.Mode = CipherMode.CFB;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8); //2314345645678765
            Debug.WriteLine(aes.IV);
            Debug.WriteLine(aes.Key);
            // aes.Key = Encoding.UTF8.GetBytes(secretKey);
            // aes.IV = Encoding.UTF8.GetBytes("2314345645678765"); //2314345645678765
            ICryptoTransform crypto = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] secret = crypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            crypto.Dispose();
            var result = ASCIIEncoding.ASCII.GetString(secret);
            Data = JsonConvert.DeserializeObject<Root>(result);
            Debug.WriteLine(Data);
            return Data;
        }
        public void BufferDecrypt(byte[] inputBuffer, string password)
        {
            _IsDecrypting = true;
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[32];

            // FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
            MemoryStream fsCrypt = new MemoryStream(inputBuffer);
            fsCrypt.Read(salt, 0, salt.Length);

            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CFB;

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);
            MemoryStream ms = new MemoryStream();
            // FileStream fsOut = new FileStream(filename+".json", FileMode.OpenOrCreate);

            int read;
            // byte[] buffer = new byte[1048576];
            byte[] buffer = new byte[500000];

            try
            {
                while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    // fsOut.Write(buffer, 0, read);
                    Application.DoEvents();
                    ms.Write(buffer, 0, read);
                }
            }
            catch (CryptographicException ex_CryptographicException)
            {
                Done = false;
                Console.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
            }
            catch (Exception ex)
            {
                Done = false;
                Debug.WriteLine("Error WHAT: " + ex.Message);
            }

            try
            {
                cs.Close();
                ms.Close();
            }
            catch (Exception ex)
            {
                Done = false;
                Debug.WriteLine("Error by closing CryptoStream: " + ex.Message);
            }
            finally
            {
                fsCrypt.Close();
                decrypted = Encoding.UTF8.GetString(buffer);
                Debug.WriteLine(decrypted);
                Data = JsonConvert.DeserializeObject<Root>(decrypted);
                if (Data.SRFAuthor == "Muhammad Surga Savero")
                    Deserialized = true;
                else
                    Deserialized = false;
                _IsDecrypting = false;
            }
        }
        
        // Will decrypt .tmp file into Root class
        public void JBufferDecrypt(APIEncryptedBody data, byte[] secretKey)
        {
            if (ReferenceEquals(null, data))
            {
                MessageBox.Show("Please contact sysadmin47@gmail.com", "Oh.. There's Bug!");
                Application.Exit();
            }
            _IsDecrypting = true;
            byte[] encryptedBytes = Convert.FromBase64String(data.body);
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            
            byte[] passwordBytes = secretKey;
            byte[] salt = Encoding.UTF8.GetBytes(data.author);
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);    
            aes.KeySize = 256; //Not Required
            aes.BlockSize = 128; //Not Required
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8); //2314345645678765
            // Debug.WriteLine(aes.IV);
            // Debug.WriteLine(aes.Key);
            // aes.Key = Encoding.UTF8.GetBytes(secretKey);
            // aes.IV = Encoding.UTF8.GetBytes("2314345645678765"); //2314345645678765
            ICryptoTransform crypto = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] secret = crypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            crypto.Dispose();
            var result = ASCIIEncoding.ASCII.GetString(secret);
            Data = JsonConvert.DeserializeObject<Root>(result);
            _IsDecrypting = false;
        }
        // Will save from APIs and serialize to a .tmp file
        public void JFileSave(APIEncryptedBody data, string secretKey)
        {
            using (var fs = new StreamWriter(filename))
            {
                try
                {
                    var js = new JsonSerializer();
                    js.Serialize(fs, data);
                    fs.Close();
                    if (File.Exists(filename))
                    {
                        // JBufferDecrypt(data, secretKey);
                        SrDecryptor(data, secretKey);
                    }
                }
                catch (Exception e)
                {
                    XtraMessageBox.Show( "Possible Cause:\n1. Connection unavailable\n2. Update server down\n3. Not running as admin\n\nPlease contact: sysadmin47@gmail.com", "There's bug");
                    if(File.Exists(filename)) 
                        File.Delete(filename);
                    Application.Exit();
                    Debug.WriteLine(e);
                }
            }
        }

        public string JStringEncrypt(byte[] inputBuffer, string salt, string password)
        {
            _IsEncrypting = true;
            byte[] encryptedBytes = inputBuffer;
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
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
            _IsEncrypting = false;
            return result;
        }
        public void SRFEncryptor(byte[] inputBuffer)
        {
            _IsEncrypting = true;
            string password = "test";
            GCHandle gch = GCHandle.Alloc(password, GCHandleType.Pinned);
            FileEncrypt(inputBuffer, password);
            ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
            gch.Free();
            _IsEncrypting = false;
            // Console.WriteLine("The given password is surely nothing: " + password);
        }

        public string SrEncryptor(byte[] inputBuffer, string salt, string password)
        {
            _IsEncrypting = true;
            // GCHandle gchPass = GCHandle.Alloc(password, GCHandleType.Pinned);
            // GCHandle gchSalt = GCHandle.Alloc(password, GCHandleType.Pinned);
            var str = JStringEncrypt(inputBuffer, salt, password);
            // ZeroMemory(gchPass.AddrOfPinnedObject(), password.Length * 2);
            // ZeroMemory(gchSalt.AddrOfPinnedObject(), salt.Length * 2);
            // gchPass.Free();
            // gchSalt.Free();
            _IsEncrypting = false;
            return str;
        }

        public void SrDecryptor(APIEncryptedBody data, string secretKey)
        {
            _IsDecrypting = true;
            // GCHandle gch = GCHandle.Alloc(secretKey, GCHandleType.Pinned);
            JBufferDecrypt(data, Encoding.UTF8.GetBytes(secretKey));
            // ZeroMemory(gch.AddrOfPinnedObject(), secretKey.Length * 2);
            // gch.Free();
            _IsDecrypting = false;
        }

        public void SRFDecryptor(string inputFile, string outputFile)
        {
            _IsDecrypting = true;
            string password = "test";
            // For additional security Pin the password of your files
            GCHandle gch = GCHandle.Alloc(password, GCHandleType.Pinned);
            // Decrypt the file
            FileDecrypt(inputFile, password);
            // To increase the security of the decryption, delete the used password from the memory !
            ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
            gch.Free();
            _IsDecrypting = false;
            // You can verify it by displaying its value later on the console (the password won't appear)
            // Console.WriteLine("The given password is surely nothing: " + password);
        }
        public void SRFDecryptorFromBuffer(byte[] buffer, string password)
        {
            _IsDecrypting = true;
            // string password = "test";
            // For additional security Pin the password of your files
            GCHandle gch = GCHandle.Alloc(password, GCHandleType.Pinned);
            // Decrypt the file
            BufferDecrypt(buffer, password);
            // To increase the security of the decryption, delete the used password from the memory !
            ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
            gch.Free();
            _IsDecrypting = false;
            // You can verify it by displaying its value later on the console (the password won't appear)
            // Console.WriteLine("The given password is surely nothing: " + password);
        }

        public void SaveToTMP(string inputFile)
        {
            // SRFEncryptor(Encoding.UTF8.GetBytes(data));
            FileStream fsIn = new FileStream(inputFile, FileMode.Create);
            
            // Debug.WriteLine(data);
            // // filename = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".srf";
            // try
            // { 
            //     // var json = JsonConvert.SerializeObject(data, typeof(root));
            //     using (StreamWriter file = File.CreateText(filename))
            //     {
            //         // Encoding.UTF8.GetBytes(data);
            //         // JsonSerializer jsr = new JsonSerializer();
            //         // jsr.Serialize(file, data);
            //         // var json3 = JsonConvert.SerializeObject(data, typeof(Root), Newtonsoft.Json.Formatting.None, new Newtonsoft.Json.JsonSerializerSettings { StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.EscapeHtml });
            //         file.Write(data);
            //     }
            // }
            // catch (Exception e)
            // {
            //     Debug.WriteLine(e);
            //     throw;
            // }
            // finally
            // {
            //     // SRFEncryptor(Encoding.UTF8.GetBytes(data););
            // }
        }
        
        public void DownloadJson()
        {
            _IsDownloading = true;
            if (_retryCount == _retryMax) MessageBox.Show("Restart the Application");
            if (_retryCount == _retryMax) return;
            using (var webClient = new System.Net.WebClient())
            {
                string jsonData = String.Empty;
                try
                {
                    webClient.Headers.Set("user-agent", "SRFramework");
                    jsonData = webClient.DownloadString(_url + SRFApis.URIPathSRFramework);
                    _apiData = JsonConvert.DeserializeObject<APIData>(jsonData);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    if (_apiData.status == 200 && _apiData.body.SRFAuthor == "Muhammad Surga Savero")
                    {
                        SRFEncryptor(Encoding.UTF8.GetBytes(_apiData.body.ToString()));
                    } else
                    {
                        _retryCount += 1;
                        DownloadJson();
                    }
                    _IsDownloading = false;
                }
                // webClient.Headers.Set("user-agent", "SRFramework");
                // byte[] jsonData = webClient.DownloadData("https://srframework.vercel.app/api/sr-framework?key=sr");
                // MemoryStream ms = new MemoryStream();
                // ms.SetLength(jsonData.Length);
                // try
                // {
                //     // ms.Write(jsonData, 0, (int)ms.Length);
                //     // ms.Flush();
                //     // string str = Encoding.UTF8.GetString(jsonData);
                //     // sa
                // } catch (Exception e) {
                //     Console.WriteLine(e);
                //     throw;
                // }
            };
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

        public void ClearTMPData()
        {
            try
            {
                File.Delete(filename);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                MessageBox.Show("Error: 1", "Contact sysadmin47@gmail.com");
            }
        }

        public bool IsConnected()
        {
            string host = _url;
            bool result = false;
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead(host))
                        return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("J: " + e);
                return result;
            }
        }

        public SREncryptor()
        {
            // Load();
        }

        public void Load()
        {
            if (_IsDecrypting) return;
            if (_IsDownloading) return;
            if (_IsEncrypting) return;
            if (File.Exists(filename))
            { 
                SRFDecryptor(filename, "test");
                if (Deserialized)
                {
                    if (SRFApis.Instance.CompareLocalToAPI("SRFRevision", Data.SRFRevision))
                    {
                        Done = true;
                    }
                    else
                    {
                        Done = false;
                        ClearTMPData();
                        Load();
                    }
                }
            }
            else
            {
                if (IsConnected())
                {
                    DownloadJson();
                }    
            }
        }
    }
}