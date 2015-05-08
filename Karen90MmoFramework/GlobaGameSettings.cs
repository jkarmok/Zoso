using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework
{
	public static class GlobalGameSettings
	{
		public const byte MAX_PLAYER_LEVEL = 40;
		public const byte MAX_NPC_LEVEL = 45;

		public const int MAX_PLAYER_GOLD = int.MaxValue;
		public const int MAX_GROUP_MEMBERS = 5;
		
		public const float MIN_INTERACTION_RANGE = 5.0f;

		public const float PLAYER_CONTROLLER_RADIUS = 0.35f;
		public const float PLAYER_CONTROLLER_HEIGHT = 2.0f;
		public const float PLAYER_CONTROLLER_SLOPE_LIMIT = 45.0f;
		public const float PLAYER_CONTROLLER_STEP_OFFSET = 0.3f;
		public const float PLAYER_CONTROLLER_SKIN_WIDTH = 0.08f;

		public const int PLAYER_FORWARD_SPEED_MAX = 5;
		public const int PLAYER_BACKWARD_SPEED_MAX = 4;
		public const int PLAYER_SIDE_SPEED_MAX = 3;

		public const int NPC_FORWARD_SPEED_MAX = 5;
		public const int NPC_BACKWARD_SPEED_MAX = 4;
		public const int NPC_SIDE_SPEED_MAX = 3;

		public const int PLAYER_ACTION_BAR_SIZE = 10;
		public const int PLAYER_DEFAULT_INVENTORY_SIZE = 30;

		// TODO: Temporary
		/// <summary>
		/// Converts a world id to world name
		/// </summary>
		/// <param name="worldId"></param>
		/// <returns></returns>
		public static string WorldIdToWorldName(int worldId)
		{
			switch (worldId)
			{
				case 0:
					return string.Empty;
				case 1:
					return "Western Woodlands";
				case 2:
					return "Raz'Jin Sands";
				case 3:
					return "Evernight Forest";
				case 4:
					return "Devil's Point";
				default:
					return string.Empty;
			}
		}
		
		/// <summary>
		/// Converts a(n) <see cref="CastResults"/> to <see cref="ResultCode"/>
		/// </summary>
		/// <param name="castResult"></param>
		public static ResultCode ToResultCode(this CastResults castResult)
		{
			switch (castResult)
			{
				case CastResults.Ok:
					return ResultCode.Ok;
				case CastResults.OutOfRange:
					return ResultCode.ObjectIsOutOfRange;
				case CastResults.TargetTooClose:
					return ResultCode.ObjectIsTooClose;
				case CastResults.NotEnoughPower:
					return ResultCode.NotEnoughPower;
				case CastResults.TargetIsDead:
					return ResultCode.TargetIsDead;
				case CastResults.CasterIsDead:
					return ResultCode.CasterIsDead;
				case CastResults.TargetRequired:
					return ResultCode.TargetRequired;
				case CastResults.InvalidTarget:
					return ResultCode.InvalidTarget;
				case CastResults.SpellNotReady:
					return ResultCode.SpellNotReady;
				case CastResults.SpellNotFound:
					return ResultCode.SpellNotFound;
				case CastResults.InCooldown:
				case CastResults.InGcd:
					return ResultCode.SpellInCooldown;
				case CastResults.AlreadyCasting:
					return ResultCode.AlreadyCasting;
				default:
					return ResultCode.Fail;
			}
		}
	}
}
