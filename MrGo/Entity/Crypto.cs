using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Profit.Server
{
    public class Crypto
    {
        private static string DEFAULT_HASH_ALGORITHM = "SHA1";
        private static int DEFAULT_KEY_SIZE = 256;
        private static int MAX_ALLOWED_SALT_LEN = 255;
        private static int MIN_ALLOWED_SALT_LEN = 4;
        private static int DEFAULT_MIN_SALT_LEN = MIN_ALLOWED_SALT_LEN;
        private static int DEFAULT_MAX_SALT_LEN = 8;

        private int minSaltLen = -1;
        private int maxSaltLen = -1;

        private ICryptoTransform encryptor = null;
        private ICryptoTransform decryptor = null;

        public Crypto()
            : this("Pas5pr@se", "@1B2c3D4e5F6g7H8")
        {

        }
        public Crypto(string passPhrase)
            : this(passPhrase, null)
        {
        }

        public Crypto(string passPhrase, string initVector)
            : this(passPhrase, initVector, -1)
        {
        }
        public Crypto(string passPhrase, string initVector, int minSaltLen)
            : this(passPhrase, initVector, minSaltLen, -1)
        {
        }
        public Crypto(string passPhrase, string initVector, int minSaltLen, int maxSaltLen)
            :
            this(passPhrase, initVector, minSaltLen, maxSaltLen, -1)
        {
        }
        public Crypto(string passPhrase, string initVector, int minSaltLen, int maxSaltLen, int keySize)
            :
            this(passPhrase, initVector, minSaltLen, maxSaltLen, keySize, null)
        {
        }
        public Crypto(string passPhrase, string initVector, int minSaltLen, int maxSaltLen, int keySize,
            string hashAlgorithm)
            : this(passPhrase, initVector, minSaltLen, maxSaltLen, keySize, hashAlgorithm, null)
        {
        }
        public Crypto(string passPhrase, string initVector, int minSaltLen, int maxSaltLen, int keySize, string hashAlgorithm, string saltValue)
            :
            this(passPhrase, initVector, minSaltLen, maxSaltLen, keySize, hashAlgorithm, saltValue, 1)
        {
        }

        public Crypto(string passPhrase,
                                string initVector,
                                int minSaltLen,
                                int maxSaltLen,
                                int keySize,
                                string hashAlgorithm,
                                string saltValue,
                                int passwordIterations)
        {
            // Save min salt length; set it to default if invalid value is passed.
            if (minSaltLen < MIN_ALLOWED_SALT_LEN)
                this.minSaltLen = DEFAULT_MIN_SALT_LEN;
            else
                this.minSaltLen = minSaltLen;

            // Save max salt length; set it to default if invalid value is passed.
            if (maxSaltLen < 0 || maxSaltLen > MAX_ALLOWED_SALT_LEN)
                this.maxSaltLen = DEFAULT_MAX_SALT_LEN;
            else
                this.maxSaltLen = maxSaltLen;

            // Set the size of cryptographic key.
            if (keySize <= 0)
                keySize = DEFAULT_KEY_SIZE;

            // Set the name of algorithm. Make sure it is in UPPER CASE and does
            // not use dashes, e.g. change "sha-1" to "SHA1".
            if (hashAlgorithm == null)
                hashAlgorithm = DEFAULT_HASH_ALGORITHM;
            else
                hashAlgorithm = hashAlgorithm.ToUpper().Replace("-", "");

            // Initialization vector converted to a byte array.
            byte[] initVectorBytes = null;

            // Salt used for password hashing (to generate the key, not during
            // encryption) converted to a byte array.
            byte[] saltValueBytes = null;

            if (initVector == null)
                initVectorBytes = new byte[0];
            else
                initVectorBytes = Encoding.ASCII.GetBytes(initVector);

            if (saltValue == null)
                saltValueBytes = new byte[0];
            else
                saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Generate password, which will be used to derive the key.
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

            // Convert key to a byte array adjusting the size from bits to bytes.
            byte[] keyBytes = password.GetBytes(keySize / 8);

            // Initialize Rijndael key object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // If we do not have initialization vector, we cannot use the CBC mode.
            // The only alternative is the ECB mode (which is not as good).
            if (initVectorBytes.Length == 0)
                symmetricKey.Mode = CipherMode.ECB;
            else
                symmetricKey.Mode = CipherMode.CBC;

            // Create encryptor and decryptor, which we will use for cryptographic
            // operations.
            encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
        }

        #region Encryption routines

        public string Encrypt(string plainText)
        {
            return Encrypt(Encoding.UTF8.GetBytes(plainText));
        }

        public string Encrypt(byte[] plainTextBytes)
        {
            return Convert.ToBase64String(EncryptToBytes(plainTextBytes));
        }

        public byte[] EncryptToBytes(string plainText)
        {
            return EncryptToBytes(Encoding.UTF8.GetBytes(plainText));
        }

        public byte[] EncryptToBytes(byte[] plainTextBytes)
        {
            // Add salt at the beginning of the plain text bytes (if needed).
            byte[] plainTextBytesWithSalt = AddSalt(plainTextBytes);

            // Encryption will be performed using memory stream.
            MemoryStream memoryStream = new MemoryStream();

            // Let's make cryptographic operations thread-safe.
            lock (this)
            {
                // To perform encryption, we must use the Write mode.
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                // Start encrypting data.
                cryptoStream.Write(plainTextBytesWithSalt, 0, plainTextBytesWithSalt.Length);

                // Finish the encryption operation.
                cryptoStream.FlushFinalBlock();

                // Move encrypted data from memory into a byte array.
                byte[] cipherTextBytes = memoryStream.ToArray();

                // Close memory streams.
                memoryStream.Close();
                cryptoStream.Close();

                // Return encrypted data.
                return cipherTextBytes;
            }
        }
        #endregion

        #region Decryption routines

        public string Decrypt(string cipherText)
        {
            return Decrypt(Convert.FromBase64String(cipherText));
        }

        public string Decrypt(byte[] cipherTextBytes)
        {
            return Encoding.UTF8.GetString(DecryptToBytes(cipherTextBytes));
        }

        public byte[] DecryptToBytes(string cipherText)
        {
            return DecryptToBytes(Convert.FromBase64String(cipherText));
        }

        public byte[] DecryptToBytes(byte[] cipherTextBytes)
        {
            byte[] decryptedBytes = null;
            byte[] plainTextBytes = null;
            int decryptedByteCount = 0;
            int saltLen = 0;

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Since we do not know how big decrypted value will be, use the same
            // size as cipher text. Cipher text is always longer than plain text
            // (in block cipher encryption), so we will just use the number of
            // decrypted data byte after we know how big it is.
            decryptedBytes = new byte[cipherTextBytes.Length];

            // Let's make cryptographic operations thread-safe.
            lock (this)
            {
                // To perform decryption, we must use the Read mode.
                CryptoStream cryptoStream = new CryptoStream(
                                                   memoryStream,
                                                   decryptor,
                                                   CryptoStreamMode.Read);

                // Decrypting data and get the count of plain text bytes.
                decryptedByteCount = cryptoStream.Read(decryptedBytes,
                                                        0,
                                                        decryptedBytes.Length);
                // Release memory.
                memoryStream.Close();
                cryptoStream.Close();
            }

            // If we are using salt, get its length from the first 4 bytes of plain
            // text data.
            if (maxSaltLen > 0 && maxSaltLen >= minSaltLen)
            {
                saltLen = (decryptedBytes[0] & 0x03) |
                            (decryptedBytes[1] & 0x0c) |
                            (decryptedBytes[2] & 0x30) |
                            (decryptedBytes[3] & 0xc0);
            }

            // Allocate the byte array to hold the original plain text (without salt).
            plainTextBytes = new byte[decryptedByteCount - saltLen];

            // Copy original plain text discarding the salt value if needed.
            Array.Copy(decryptedBytes, saltLen, plainTextBytes,
                        0, decryptedByteCount - saltLen);

            // Return original plain text value.
            return plainTextBytes;
        }
        #endregion

        private byte[] AddSalt(byte[] plainTextBytes)
        {
            // The max salt value of 0 (zero) indicates that we should not use 
            // salt. Also do not use salt if the max salt value is smaller than
            // the min value.
            if (maxSaltLen == 0 || maxSaltLen < minSaltLen)
                return plainTextBytes;

            // Generate the salt.
            byte[] saltBytes = GenerateSalt();

            // Allocate array which will hold salt and plain text bytes.
            byte[] plainTextBytesWithSalt = new byte[plainTextBytes.Length +
                                                     saltBytes.Length];
            // First, copy salt bytes.
            Array.Copy(saltBytes, plainTextBytesWithSalt, saltBytes.Length);

            // Append plain text bytes to the salt value.
            Array.Copy(plainTextBytes, 0,
                        plainTextBytesWithSalt, saltBytes.Length,
                        plainTextBytes.Length);

            return plainTextBytesWithSalt;
        }

        private byte[] GenerateSalt()
        {
            // We don't have the length, yet.
            int saltLen = 0;

            // If min and max salt values are the same, it should not be random.
            if (minSaltLen == maxSaltLen)
                saltLen = minSaltLen;
            // Use random number generator to calculate salt length.
            else
                saltLen = GenerateRandomNumber(minSaltLen, maxSaltLen);

            // Allocate byte array to hold our salt.
            byte[] salt = new byte[saltLen];

            // Populate salt with cryptographically strong bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            rng.GetNonZeroBytes(salt);

            // Split salt length (always one byte) into four two-bit pieces and
            // store these pieces in the first four bytes of the salt array.
            salt[0] = (byte)((salt[0] & 0xfc) | (saltLen & 0x03));
            salt[1] = (byte)((salt[1] & 0xf3) | (saltLen & 0x0c));
            salt[2] = (byte)((salt[2] & 0xcf) | (saltLen & 0x30));
            salt[3] = (byte)((salt[3] & 0x3f) | (saltLen & 0xc0));

            return salt;
        }

        private int GenerateRandomNumber(int minValue, int maxValue)
        {
            // We will make up an integer seed from 4 bytes of this array.
            byte[] randomBytes = new byte[4];

            // Generate 4 random bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert four random bytes into a positive integer value.
            int seed = ((randomBytes[0] & 0x7f) << 24) |
                        (randomBytes[1] << 16) |
                        (randomBytes[2] << 8) |
                        (randomBytes[3]);

            // Now, this looks more like real randomization.
            Random random = new Random(seed);

            // Calculate a random number.
            return random.Next(minValue, maxValue + 1);
        }
    }
}
