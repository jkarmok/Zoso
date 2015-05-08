using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.CollisionTests;
using BEPUutilities;
using BEPUutilities.DataStructures;
using BEPUphysics.NarrowPhaseSystems;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Settings;

namespace Karen90MmoFramework.Server.Game.Physics.BEPU
{
	/// <summary>
	/// Helps a character identify supports by finding contacts and ray cast intersections with its immediate surroundings.
	/// </summary>
	public class QueryManager
	{
		//This QueryManager is not thread safe in any way, but it's only ever used by a single character at a time, so this isn't an issue.
		private readonly RawList<ContactData> contacts = new RawList<ContactData>();
		private readonly RawList<ContactData> supportContacts = new RawList<ContactData>();
		private readonly RawList<ContactData> tractionContacts = new RawList<ContactData>();
		private readonly RawList<ContactData> sideContacts = new RawList<ContactData>();
		private readonly RawList<ContactData> headContacts = new RawList<ContactData>();

		private readonly ConvexCollidable<CapsuleShape> queryObject;
		private readonly BEPUCharacterController character;

		public RawList<ContactData> Contacts
		{
			get { return this.contacts; }
		}

		public RawList<ContactData> SupportContacts
		{
			get { return this.supportContacts; }
		}

		public RawList<ContactData> TractionContacts
		{
			get { return this.tractionContacts; }
		}

		/// <summary>
		/// Constructs the query manager for a character.
		/// </summary>
		/// <param name="character">Character to manage queries for.</param>
		public QueryManager(BEPUCharacterController character)
		{
			this.character = character;
			//We can share the real shape with the 'current' query object.
			this.queryObject = new ConvexCollidable<CapsuleShape>(character.Body.CollisionInformation.Shape);
			//Share the collision rules between the main body and its query objects.  That way, the character's queries return valid results.
			this.queryObject.CollisionRules = character.Body.CollisionInformation.CollisionRules;

			this.supportRayFilter = this.SupportRayFilterFunction;
		}

		private readonly Func<BroadPhaseEntry, bool> supportRayFilter;

		private bool SupportRayFilterFunction(BroadPhaseEntry entry)
		{
			//Only permit an object to be used as a support if it fully collides with the character.
			return CollisionRules.CollisionRuleCalculator(entry, this.character.Body.CollisionInformation) ==
			       CollisionRule.Normal;
		}

		/// <summary>
		/// Computes the intersection, if any, between a ray and the objects in the character's bounding box.
		/// </summary>
		/// <param name="ray">Ray to test.</param>
		/// <param name="length">Length of the ray to use in units of the ray's length.</param>
		/// <param name="earliestHit">Earliest intersection location and information.</param>
		/// <returns>Whether or not the ray hit anything.</returns>
		public bool RayCast(Ray ray, float length, out RayHit earliestHit)
		{
			earliestHit = new RayHit {T = float.MaxValue};
			foreach (var collidable in this.character.Body.CollisionInformation.OverlappedCollidables)
			{
				//Check to see if the collidable is hit by the ray.
				float? t = ray.Intersects(collidable.BoundingBox);
				if (t != null && t < length)
				{
					//Is it an earlier hit than the current earliest?
					RayHit hit;
					if (collidable.RayCast(ray, length, this.supportRayFilter, out hit) && hit.T < earliestHit.T)
					{
						earliestHit = hit;
					}
				}
			}
			return earliestHit.T != float.MaxValue;
		}

		/// <summary>
		/// Computes the intersection, if any, between a ray and the objects in the character's bounding box.
		/// </summary>
		/// <param name="ray">Ray to test.</param>
		/// <param name="length">Length of the ray to use in units of the ray's length.</param>
		/// <param name="earliestHit">Earliest intersection location and information.</param>
		/// <param name="hitObject">Collidable intersected by the ray, if any.</param>
		/// <returns>Whether or not the ray hit anything.</returns>
		public bool RayCast(Ray ray, float length, out RayHit earliestHit, out Collidable hitObject)
		{
			earliestHit = new RayHit {T = float.MaxValue};
			hitObject = null;
			foreach (var collidable in this.character.Body.CollisionInformation.OverlappedCollidables)
			{
				//Check to see if the collidable is hit by the ray.
				float? t = ray.Intersects(collidable.BoundingBox);
				if (t != null && t < length)
				{
					//Is it an earlier hit than the current earliest?
					RayHit hit;
					if (collidable.RayCast(ray, length, this.supportRayFilter, out hit) && hit.T < earliestHit.T)
					{
						earliestHit = hit;
						hitObject = collidable;
					}
				}
			}
			return earliestHit.T != float.MaxValue;
		}

		/// <summary>
		/// Determines if a ray intersects any object in the character's bounding box.
		/// </summary>
		/// <param name="ray">Ray to test.</param>
		/// <param name="length">Length of the ray to use in units of the ray's length.</param>
		/// <returns>Whether or not the ray hit anything.</returns>
		public bool RayCastHitAnything(Ray ray, float length)
		{
			foreach (var collidable in this.character.Body.CollisionInformation.OverlappedCollidables)
			{
				//Check to see if the collidable is hit by the ray.
				float? t = ray.Intersects(collidable.BoundingBox);
				if (t != null && t < length)
				{
					RayHit hit;
					if (collidable.RayCast(ray, length, this.supportRayFilter, out hit))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void ClearContacts()
		{
			this.contacts.Clear();
			this.supportContacts.Clear();
			this.tractionContacts.Clear();
			this.sideContacts.Clear();
			this.headContacts.Clear();
		}

		/// <summary>
		/// Tests a collision object with the same shape as the current character at the given position for contacts.
		/// Output data is stored in the query manager's supporting lists.
		/// </summary>
		/// <param name="position">Position to use for the query.</param>
		public void QueryContacts(Vector3 position)
		{
			this.QueryContacts(position, this.queryObject);
		}

		private void QueryContacts(Vector3 position, EntityCollidable queryObject)
		{
			this.ClearContacts();

			//Update the position and orientation of the query object.
			RigidTransform transform;
			transform.Position = position;
			transform.Orientation = this.character.Body.Orientation;
			queryObject.UpdateBoundingBoxForTransform(ref transform, 0);

			foreach (var collidable in this.character.Body.CollisionInformation.OverlappedCollidables)
			{
				if (collidable.BoundingBox.Intersects(queryObject.BoundingBox))
				{
					var pair = new CollidablePair(collidable, queryObject);
					var pairHandler = NarrowPhaseHelper.GetPairHandler(ref pair);
					if (pairHandler.CollisionRule == CollisionRule.Normal)
					{
						pairHandler.SuppressEvents = true;
						pairHandler.UpdateCollision(0);
						pairHandler.SuppressEvents = false;

						foreach (var contact in pairHandler.Contacts)
						{
							//Must check per-contact collision rules, just in case
							//the pair was actually a 'parent pair.'
							if (contact.Pair.CollisionRule == CollisionRule.Normal)
							{
								ContactData contactData;
								contactData.Position = contact.Contact.Position;
								contactData.Normal = contact.Contact.Normal;
								contactData.Id = contact.Contact.Id;
								contactData.PenetrationDepth = contact.Contact.PenetrationDepth;
								this.contacts.Add(contactData);
							}
						}
					}
					//TODO: It would be nice if this was a bit easier.
					//Having to remember to clean up AND give it back is a bit weird, especially with the property-diving.
					//No one would ever just guess this correctly.
					//At least hide it behind a NarrowPhaseHelper function.
					pairHandler.CleanUp();
					pairHandler.Factory.GiveBack(pairHandler);
				}
			}

			this.CategorizeContacts(ref position);
		}

		private void CategorizeContacts(ref Vector3 position)
		{
			Vector3 downDirection = this.character.Down;
			for (int i = 0; i < this.contacts.Count; i++)
			{
				float dot;
				Vector3 offset;
				Vector3.Subtract(ref this.contacts.Elements[i].Position, ref position, out offset);
				Vector3.Dot(ref this.contacts.Elements[i].Normal, ref offset, out dot);
				ContactData processed = this.contacts.Elements[i];
				if (dot < 0)
				{
					//The normal should face outward.
					dot = -dot;
					Vector3.Negate(ref processed.Normal, out processed.Normal);
				}
				Vector3.Dot(ref processed.Normal, ref downDirection, out dot);
				if (dot > SupportFinder.SideContactThreshold)
				{
					//It's a support.
					this.supportContacts.Add(processed);
					if (dot > this.character.SupportFinder.MaximumSlope)
					{
						//It's a traction contact.
						this.tractionContacts.Add(processed);
					}
					else
						this.sideContacts.Add(processed); //Considering the side contacts to be supports can help with upstepping.
				}
				else if (dot < -SupportFinder.SideContactThreshold)
				{
					//It's a head contact.
					this.headContacts.Add(processed);
				}
				else
				{
					//It's a side contact.  These could obstruct the stepping.
					this.sideContacts.Add(processed);
				}
			}
		}

		internal bool HasSupports(out bool hasTraction, out PositionState state, out ContactData supportContact)
		{
			float maxDepth = -float.MaxValue;
			int deepestIndex = -1;
			if (this.tractionContacts.Count > 0)
			{
				//It has traction!
				//But is it too deep?
				//Find the deepest contact.
				for (int i = 0; i < this.tractionContacts.Count; i++)
				{
					if (this.tractionContacts.Elements[i].PenetrationDepth > maxDepth)
					{
						maxDepth = this.tractionContacts.Elements[i].PenetrationDepth;
						deepestIndex = i;
					}
				}
				hasTraction = true;
				supportContact = this.tractionContacts.Elements[deepestIndex];
			}
			else if (this.supportContacts.Count > 0)
			{
				//It has support!
				//But is it too deep?
				//Find the deepest contact.

				for (int i = 0; i < this.supportContacts.Count; i++)
				{
					if (this.supportContacts.Elements[i].PenetrationDepth > maxDepth)
					{
						maxDepth = this.supportContacts.Elements[i].PenetrationDepth;
						deepestIndex = i;
					}
				}
				hasTraction = false;
				supportContact = this.supportContacts.Elements[deepestIndex];
			}
			else
			{
				hasTraction = false;
				state = PositionState.NoHit;
				supportContact = new ContactData();
				return false;
			}
			//Check the depth.
			if (maxDepth > CollisionDetectionSettings.AllowedPenetration)
			{
				//It's too deep.
				state = PositionState.TooDeep;
			}
			else if (maxDepth < 0)
			{
				//The depth is negative, meaning it's separated.  This shouldn't happen with the initial implementation of the character controller,
				//but this case could conceivably occur in other usages of a system like this (or in a future version of the character),
				//so go ahead and handle it.
				state = PositionState.NoHit;
			}
			else
			{
				//The deepest contact appears to be very nicely aligned with the ground!
				//It's fully supported.
				state = PositionState.Accepted;
			}
			return true;
		}
	}

	internal enum PositionState
	{
		Accepted,
		Rejected,
		TooDeep,
		Obstructed,
		HeadObstructed,
		NoHit
	}
}
