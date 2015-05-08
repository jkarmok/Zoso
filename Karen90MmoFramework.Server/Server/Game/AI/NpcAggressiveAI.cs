using Karen90MmoFramework.Server.Game.Objects;
using Karen90MmoFramework.Server.Game.Systems;

namespace Karen90MmoFramework.Server.Game.AI
{
	public class NpcAggressiveAI : NpcAI
	{
		#region Constants and Fields


		#endregion

		#region Properties


		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="NpcAggressiveAI"/> class.
		/// </summary>
		/// <param name="owner"></param>
		public NpcAggressiveAI(Npc owner)
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
			if (Owner.IsDead())
				return;

			if (Owner.CurrentAggroState != AggroState.Idle)
				return;
			
			if (!Owner.CanAttackTarget(target))
				return;

			Owner.BeginCombat(target);
		}

		/// <summary>
		/// Called when a <see cref="Character"/> exits owner's range
		/// </summary>
		/// <param name="target"></param>
		public override void OnCharacterExitRange(Character target)
		{
			if (Owner.IsDead())
				return;

			if (Owner.GetCurrentFocus() != target)
				return;

			if (Owner.CurrentAggroState != AggroState.Chasing)
				return;

			Owner.EndCombat();
		}

		/// <summary>
		/// Updates the AI
		/// </summary>
		/// <param name="deltaTime"></param>
		public override void Update(float deltaTime)
		{
			if (Owner.IsDead())
				return;

			var victim = Owner.GetCurrentFocus();
			if (victim == null)
				return;

			if (!Owner.IsWithinAttackRange(victim))
				return;

			Owner.DoCombat();
		}

		#endregion
	}
}
