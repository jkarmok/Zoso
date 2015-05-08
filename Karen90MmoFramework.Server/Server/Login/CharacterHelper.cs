using System;
using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.Login
{
	public static class CharacterHelper
	{
		private const int MinCharacterNameLength = 2;
		private const int MaxCharacterNameLength = 15;

		public static bool IsValidCharacterName(string characterName)
		{
			ResultCode errorCode;
			return IsValidCharacterName(characterName, out errorCode);
		}

		public static bool IsValidCharacterName(string characterName, out ResultCode resultCode)
		{
			resultCode = ResultCode.Ok;
			if (string.IsNullOrEmpty(characterName))
			{
				resultCode = ResultCode.CharacterNameIsEmpty;
				return false;
			}

			if (characterName.Length < MinCharacterNameLength)
			{
				resultCode = ResultCode.CharacterNameIsTooShort;
				return false;
			}

			if (characterName.Length > MaxCharacterNameLength)
			{
				resultCode = ResultCode.CharacterNameIsTooLong;
				return false;
			}

			// TODO: Check for invalid characters
			return true;
		}

		public static bool IsValidCharacterInfo(CharacterStructure characterInfo)
		{
			ResultCode resultCode;
			return IsValidCharacterInfo(characterInfo, out resultCode);
		}

		public static bool IsValidCharacterInfo(CharacterStructure characterInfo, out ResultCode resultCode)
		{
			resultCode = ResultCode.Ok;
			if (!Enum.IsDefined(typeof(Race), characterInfo.Race))
			{
				resultCode = ResultCode.InvalidRace;
				return false;
			}

			if (!Enum.IsDefined(typeof(Origin), characterInfo.Origin))
			{
				resultCode = ResultCode.InvalidOrigin;
				return false;
			}

			return true;
		}
	}
}
