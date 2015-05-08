using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.AI
{
	public abstract class NpcAI : AI
	{
		#region Constants and Fields

		private readonly Npc owner;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the owner
		/// </summary>
		protected Npc Owner
		{
			get
			{
				return this.owner;
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="NpcAI"/> class.
		/// </summary>
		/// <param name="owner"></param>
		protected NpcAI(Npc owner)
		{
			this.owner = owner;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Called when a <see cref="Character"/> enters owner's range
		/// </summary>
		/// <param name="target"></param>
		public virtual void OnCharacterEnterRange(Character target)
		{
		}

		/// <summary>
		/// Called when a <see cref="Character"/> exits owner's range
		/// </summary>
		/// <param name="target"></param>
		public virtual void OnCharacterExitRange(Character target)
		{
		}

		#endregion
	}
}
