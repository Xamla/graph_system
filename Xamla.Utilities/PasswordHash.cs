using System;
using System.Security.Cryptography;
using System.Text;

namespace Xamla.Utilities
{
    /// <summary>
    /// Salted password hashing with PBKDF2-SHA1.
    /// based on: http://crackstation.net/hashing-security.htm by havoc AT defuse.ca (Taylor Hornby)
    /// </summary>
    public class PasswordHash
    {
        static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();
        public const string ALPHANUMERIC_CHARSET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        // The following constants may be changed without breaking existing hashes.
        public const int DEFAULT_SALT_BYTE_SIZE = 24;       // 196 bit salt
        public const int DEFAULT_HASH_BYTE_SIZE = 20;
        public const int DEFAULT_ITERATIONS = 2048;

        public const int ITERATION_INDEX = 0;
        public const int SALT_INDEX = 1;
        public const int PBKDF2_INDEX = 2;

        int iterations;
        byte[] salt;
        byte[] hash;

        public static byte[] GenerateSalt(int count)
        {
            var salt = new byte[count];
            rng.GetBytes(salt);
            return salt;
        }

        public static byte[] Sha1(byte[] data)
        {
            return SHA1.Create().ComputeHash(data);
        }

        public static byte[] Sha256(byte[] data)
        {
            return SHA256.Create().ComputeHash(data);
        }

        public static byte[] Sha512(byte[] data)
        {
            return SHA512.Create().ComputeHash(data);
        }

        public static byte[] Hmac(byte[] key, byte[] buffer)
        {
            return new HMACSHA1(key).ComputeHash(buffer);
        }

        public static byte[] Hmac(byte[] key, string text)
        {
            return Hmac(key, Encoding.UTF8.GetBytes(text));
        }

        public static byte[] HmacSha1(byte[] key, byte[] data)
        {
            return new HMACSHA1(key).ComputeHash(data);
        }

        public static byte[] HmacSha1(byte[] key, string text)
        {
            return HmacSha1(key, Encoding.UTF8.GetBytes(text));
        }

        public static byte[] HmacSha256(byte[] key, byte[] data)
        {
            return new HMACSHA256(key).ComputeHash(data);
        }

        public static byte[] HmacSha256(byte[] key, string text)
        {
            return HmacSha256(key, Encoding.UTF8.GetBytes(text));
        }

        public static string GenerateRandomPassword(int length, string charset = ALPHANUMERIC_CHARSET)
        {
            if (string.IsNullOrEmpty(charset))
                throw new ArgumentException("Invalid charset specified.", "charset");

            var buffer = new char[length];
            var noise = GenerateSalt(length);
            for (int i = 0; i < length; ++i)
                buffer[i] = charset[noise[i] % charset.Length];
            return new string(buffer);
        }

        public PasswordHash(int iterations, byte[] salt, byte[] hash)
        {
            this.iterations = iterations;
            this.salt = salt;
            this.hash = hash;
        }

        public int Iterations
        {
            get { return iterations; }
        }

        public byte[] Salt
        {
            get { return salt; }
        }

        public byte[] Hash
        {
            get { return hash; }
        }

        public bool ValidatePassword(string password)
        {
            byte[] testHash = PBKDF2(Encoding.UTF8.GetBytes(password), salt, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }

        public override string ToString()
        {
            return iterations + ":" + Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        public static PasswordHash CreateHash(string password, int iterations = DEFAULT_ITERATIONS, int saltByteSize = DEFAULT_SALT_BYTE_SIZE, int hashByteSize = DEFAULT_HASH_BYTE_SIZE)
        {
            return CreateHash(password, iterations, GenerateSalt(saltByteSize), hashByteSize);
        }

        public static PasswordHash CreateHash(string password, int iterations, byte[] salt, int hashByteSize = DEFAULT_HASH_BYTE_SIZE)
        {
            var hash = PBKDF2(Encoding.UTF8.GetBytes(password), salt, iterations, hashByteSize);
            return new PasswordHash(iterations, salt, hash);
        }

        public static PasswordHash Parse(string passwordHashString)
        {
            string[] split = passwordHashString.Split(':');
            int iterations = Int32.Parse(split[ITERATION_INDEX]);
            byte[] salt = Convert.FromBase64String(split[SALT_INDEX]);
            byte[] hash = Convert.FromBase64String(split[PBKDF2_INDEX]);
            return new PasswordHash(iterations, salt, hash);
        }

        public static bool ValidatePassword(string password, string passwordHashString)
        {
            return Parse(passwordHashString).ValidatePassword(password);
        }

        /// <summary>
        /// Compares two byte arrays in length-constant time. This comparison
        /// method is used so that password hashes cannot be extracted from
        /// on-line systems using a timing attack and then attacked off-line.
        /// </summary>
        /// <param name="a">The first byte array.</param>
        /// <param name="b">The second byte array.</param>
        /// <returns>True if both byte arrays are equal. False otherwise.</returns>
        public static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }

        /// <summary>
        /// Computes the PBKDF2-SHA1 hash of a password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="iterations">The PBKDF2 iteration count.</param>
        /// <param name="outputBytes">The length of the hash to generate, in bytes.</param>
        /// <returns>A hash of the password.</returns>
        private static byte[] PBKDF2(byte[] password, byte[] salt, int iterations, int outputBytes)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            return pbkdf2.GetBytes(outputBytes);
        }
    }
}