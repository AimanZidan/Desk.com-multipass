using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Desk.com
{
	public class Multipass
	{
		protected const string AccountKey = "your sub domain name";
		protected const string ApiKey = "Your Key here";
		protected const string InitVector = "OpenSSL for Ruby"; // DO NOT CHANGE

		public class UserData
		{
			public string uid;  // must be at least 4 Chars
			public string customer_email;
			public string customer_name;
			public string expires; // must be in this format yyyy-MM-ddTHH:mm:sszzz
			public string to;
		}

		public static string BuildLink(UserData user_data)
		{
			var userDetails = JsonConvert.SerializeObject(user_data, new KeyValuePairConverter());

			var pad = 16 - (userDetails.Length%16);
			userDetails = userDetails + new String((char) pad, pad);

			var initVectorBytes = Encoding.UTF8.GetBytes(InitVector);
			byte[] keyBytesLong;
			using (var sha = new SHA1CryptoServiceProvider())
			{
				keyBytesLong = sha.ComputeHash(Encoding.UTF8.GetBytes(ApiKey + AccountKey));
			}

			var keyBytes = new byte[16];
			Array.Copy(keyBytesLong, keyBytes, 16);

			byte[] textBytes = Encoding.UTF8.GetBytes(userDetails);

			byte[] encrypted = encryptStringToBytes_AES(textBytes, keyBytes, initVectorBytes);

			var multipass = new byte[encrypted.Length + initVectorBytes.Length];
			Array.Copy(initVectorBytes, multipass, InitVector.Length);
			Array.Copy(encrypted, 0, multipass, InitVector.Length, encrypted.Length);

			string encoded = Convert.ToBase64String(multipass);

			var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(ApiKey));
			byte[] signature = hmac.ComputeHash(Encoding.UTF8.GetBytes(encoded));


			return string.Format("http://{0}.desk.com/customer/authentication/multipass/callback?multipass={1}&signature={2}",
			                     AccountKey,
			                     HttpUtility.UrlEncode(encoded), HttpUtility.UrlEncode(Convert.ToBase64String(signature)));
		}

		protected static byte[] encryptStringToBytes_AES(byte[] textBytes, byte[] Key, byte[] IV)
		{
			// Declare the stream used to encrypt to an in memory
			// array of bytes and the RijndaelManaged object
			// used to encrypt the data.
			using (var msEncrypt = new MemoryStream())
			using (var aesAlg = new RijndaelManaged())
			{
				// Provide the RijndaelManaged object with the specified key and IV.
				aesAlg.Mode = CipherMode.CBC;
				aesAlg.Padding = PaddingMode.PKCS7;
				aesAlg.KeySize = 128;
				aesAlg.BlockSize = 128;
				aesAlg.Key = Key;
				aesAlg.IV = IV;
				// Create an encryptor to perform the stream transform.
				var encryptor = aesAlg.CreateEncryptor();

				// Create the streams used for encryption.
				using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
				{
					csEncrypt.Write(textBytes, 0, textBytes.Length);
					csEncrypt.FlushFinalBlock();
				}
				var encrypted = msEncrypt.ToArray();

				// Return the encrypted bytes from the memory stream.
				return encrypted;
			}
		}
	}
}
