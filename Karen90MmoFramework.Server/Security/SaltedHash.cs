using System;
using System.Text;
using System.Security.Cryptography;

namespace Karen90MmoFramework.Security
{
	public sealed class SaltedHash
	{
		#region Constants and Fields

		private const int SaltLength = 32;

		private readonly string salt;
		private readonly string hash;

		#endregion

		#region Constructors and Destructors

		private SaltedHash(string salt, string hash)
		{
			this.salt = salt;
			this.hash = hash;
		}

		/// <summary>
		/// Creates a new <see cref="SaltedHash"/> instance using a password.
		/// </summary>
		public static SaltedHash Create(string password)
		{
			var salt = CreateSalt();
			var hash = CalculateHash(salt, password);

			return new SaltedHash(salt, hash);
		}

		/// <summary>
		/// Creates a new <see cref="SaltedHash"/> instance using a salt and a hash.
		/// </summary>
		public static SaltedHash Create(string salt, string hash)
		{
			return new SaltedHash(salt, hash);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the salt
		/// </summary>
		public string Salt
		{
			get { return this.salt; }
		}

		/// <summary>
		/// Gets the hash
		/// </summary>
		public string Hash
		{
			get { return this.hash; }
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Verifies the password with the hash
		/// </summary>
		public bool Verify(string password)
		{
			var h = CalculateHash(salt, password);
			return h.Equals(hash);
		}

		private static string CreateSalt()
		{
			var bytes = new byte[SaltLength];
			new RNGCryptoServiceProvider().GetBytes(bytes);

			return Convert.ToBase64String(bytes);
		}

		private static string CalculateHash(string salt, string password)
		{
			var buffer = Encoding.UTF8.GetBytes(salt + password);
			var hash = new SHA256CryptoServiceProvider().ComputeHash(buffer);

			return Convert.ToBase64String(hash);
		}

		#endregion
	}
}
