using Karen90MmoFramework.Quantum.Physics;

namespace Karen90MmoFramework.Server.Game.Physics
{
	public static class CollisionHelper
	{
		public const int COLLISION_GROUP_WORLD_OBJECT = 0;
		public const int COLLISION_GROUP_AVATAR_OBJECT = 1;

		public static ColliderDescription WorldObjectColliderDescription { get; private set; }
		public static ColliderDescription AvatarObjectColliderDescription { get; private set; }
		
		static CollisionHelper()
		{
			WorldObjectColliderDescription = new ColliderDescription() { CollisionGroup = COLLISION_GROUP_WORLD_OBJECT, MotionType = MotionType.Static };
			AvatarObjectColliderDescription = new ColliderDescription() { CollisionGroup = COLLISION_GROUP_AVATAR_OBJECT, MotionType = MotionType.Dynamic };
		}
	}
}
