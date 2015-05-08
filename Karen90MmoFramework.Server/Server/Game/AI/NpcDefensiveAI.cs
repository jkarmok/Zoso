using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.AI
{
	public class NpcDefensiveAI : NpcAI
	{
		#region Constants and Fields


		#endregion

		#region Properties


		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="NpcDefensiveAI"/> class.
		/// </summary>
		/// <param name="owner"></param>
		public NpcDefensiveAI(Npc owner)
			: base(owner)
		{
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Called when a <see cref="Character"/> enters owner's range
		/// </summary>
		/// <param name="target"></param>
		public override void OnCharacterEnterRange(Character target)
		{
		}

		/// <summary>
		/// Called when a <see cref="Character"/> exits owner's range
		/// </summary>
		/// <param name="target"></param>
		public override void OnCharacterExitRange(Character target)
		{
		}

		/// <summary>
		/// Updates the AI
		/// </summary>
		/// <param name="deltaTime"></param>
		public override void Update(float deltaTime)
		{
		}

		#endregion
	}
}
