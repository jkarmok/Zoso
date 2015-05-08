using System;
using System.Collections.Generic;

namespace Karen90MmoFramework
{
	public class ObjectPool<TKey, TValue> where TValue : IPoolObject<TKey>
	{
		#region Constants and Fields

		/// <summary>
		/// pool resources
		/// </summary>
		private readonly Dictionary<TKey, Queue<TValue>> resources;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="ObjectPool{TKey, TValue}"/> class.
		/// </summary>
		public ObjectPool()
		{
			this.resources = new Dictionary<TKey, Queue<TValue>>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets a(n) <see cref="TValue"/> from the pool
		/// </summary>
		public TValue Take(TKey key, Func<TValue> objectCreator)
		{
			lock (resources)
			{
				Queue<TValue> queue;
				if (false == this.resources.TryGetValue(key, out queue))
				{
					queue = new Queue<TValue>();
					this.resources.Add(key, queue);
				}

				return queue.Count == 0
					       ? objectCreator()
					       : queue.Dequeue();
			}
		}

		/// <summary>
		/// Takes a(n) existing <see cref="TValue"/> from the pool
		/// </summary>
		public bool Take(TKey key, out TValue pObject)
		{
			pObject = default(TValue);
			lock (resources)
			{
				Queue<TValue> queue;
				if (!this.resources.TryGetValue(key, out queue))
					return false;

				if (this.resources.Count != 0)
				{
					pObject = queue.Dequeue();
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Returns a(n) <see cref="TValue"/> back to the pool
		/// </summary>
		public void GiveBack(TValue pObject)
		{
			lock (resources)
			{
				var key = ((IPoolObject<TKey>)pObject).GetKey();

				Queue<TValue> queue;
				if (false == this.resources.TryGetValue(key, out queue))
				{
					queue = new Queue<TValue>();
					this.resources.Add(key, queue);
				}

				queue.Enqueue(pObject);
			}
		}

		#endregion
	}

	public class ObjectPool<TValue> where TValue : IPoolObject
	{
		#region Constants and Fields

		/// <summary>
		/// pool resources
		/// </summary>
		private readonly Queue<TValue> resources;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="ObjectPool{TValue}"/> class.
		/// </summary>
		public ObjectPool()
		{
			this.resources = new Queue<TValue>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Takes a(n) existing <see cref="TValue"/> from the pool or creates a new one using the <see cref="objectCreator"/>.
		/// </summary>
		public TValue Take(Func<TValue> objectCreator)
		{
			lock (resources)
			{
				return this.resources.Count == 0
					       ? objectCreator()
					       : this.resources.Dequeue();
			}
		}

		/// <summary>
		/// Takes a(n) existing <see cref="TValue"/> from the pool.
		/// </summary>
		public bool Take(out TValue pObject)
		{
			lock (resources)
			{
				var result = this.resources.Count != 0;
				pObject = result ? this.resources.Dequeue() : default(TValue);
				return result;
			}
		}

		/// <summary>
		/// Returns a(n) <see cref="TValue"/> back to the pool
		/// </summary>
		public void GiveBack(TValue pObject)
		{
			lock (resources)
				this.resources.Enqueue(pObject);
		}

		#endregion
	}
}
