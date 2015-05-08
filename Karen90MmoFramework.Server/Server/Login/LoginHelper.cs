using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.Login
{
	public static class LoginHelper
	{
		private const int MinUsernameLength = 1;
		private const int MinPasswordLength = 1;

		public static bool IsValidUsername(string username)
		{
			ResultCode errorCode;
			return IsValidUsername(username, out errorCode);
		}

		public static bool IsValidUsername(string username, out ResultCode resultCode)
		{
			resultCode = ResultCode.Ok;
			if (string.IsNullOrEmpty(username))
			{
				resultCode = ResultCode.UsernameIsEmpty;
				return false;
			}

			if (username.Length < MinUsernameLength)
			{
				resultCode = ResultCode.UsernameIsTooShort;
				return false;
			}

			// TODO: Check for invalid characters
			return true;
		}

		public static bool IsValidPassword(string password)
		{
			ResultCode errorCode;
			return IsValidPassword(password, out errorCode);
		}

		public static bool IsValidPassword(string password, out ResultCode resultCode)
		{
			resultCode = ResultCode.Ok;
			if (string.IsNullOrEmpty(password))
			{
				resultCode = ResultCode.PasswordIsEmpty;
				return false;
			}

			if (password.Length < MinPasswordLength)
			{
				resultCode = ResultCode.PasswordIsTooShort;
				return false;
			}

			// TODO: Check for invalid characters
			return true;
		}
	}
}
