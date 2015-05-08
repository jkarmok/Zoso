using System;
using System.Collections;
using System.Collections.Generic;

namespace Karen90MmoFramework
{
	public class FixedList<T> : IEnumerable<T>
	{
		#region Constants and Fields

		private int length;
		private readonly T[] collection;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the length
		/// </summary>
		public int Length
		{
			get
			{
				return this.length;
			}

			set
			{
				if(length < 0 || length > Capacity)
					throw new IndexOutOfRangeException("Length");

				this.length = value;
			}
		}

		/// <summary>
		/// Gets the capacity
		/// </summary>
		public int Capacity
		{
			get
			{
				return this.collection.Length;
			}
		}

		/// <summary>
		/// Gets the item at the index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= Length)
					throw new IndexOutOfRangeException("Index");

				return this.collection[index];
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="FixedList{T}"/> class.
		/// </summary>
		/// <param name="size"></param>
		public FixedList(int size)
		{
			this.collection = new T[size];
			this.Length = 0;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns whether the list is full or not
		/// </summary>
		/// <returns></returns>
		public bool IsFull()
		{
			return this.Length >= this.Capacity;
		}

		/// <summary>
		/// Sets the item at the next index
		/// </summary>
		/// <param name="item"></param>
		public void SetNext(T item)
		{
			if(IsFull())
				return;

			this.collection[Length++] = item;
		}

		/// <summary>
		/// Converts the list to an array
		/// </summary>
		/// <returns></returns>
		public T[] ToArray()
		{
			var array = new T[Length];
			Array.Copy(collection, array, Length);
			return array;
		}
		
		#endregion

		#region Implementation of IEnumerable

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		#endregion

		struct Enumerator : IEnumerator<T>
		{
			#region Constructors, Destructors, Constants and Fields

			private readonly FixedList<T> list;
			private int index;
			private T current;

			internal Enumerator(FixedList<T> list)
			{
				this.list = list;
				this.index = 0;
				this.current = default(T);
			}

			#endregion

			#region Implementation of IDisposable

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			/// <filterpriority>2</filterpriority>
			void IDisposable.Dispose()
			{
			}

			#endregion

			#region Implementation of IEnumerator

			/// <summary>
			/// Advances the enumerator to the next element of the collection.
			/// </summary>
			/// <returns>
			/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
			/// </returns>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. 
			///                 </exception><filterpriority>2</filterpriority>
			bool IEnumerator.MoveNext()
			{
				if(index >= list.length)
				{
					this.index = list.length + 1;
					this.current = default(T);
					return false;
				}

				this.current = list.collection[index++];
				return true;
			}

			/// <summary>
			/// Sets the enumerator to its initial position, which is before the first element in the collection.
			/// </summary>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. 
			///                 </exception><filterpriority>2</filterpriority>
			void IEnumerator.Reset()
			{
				this.index = 0;
				this.current = default(T);
			}

			/// <summary>
			/// Gets the element in the collection at the current position of the enumerator.
			/// </summary>
			/// <returns>
			/// The element in the collection at the current position of the enumerator.
			/// </returns>
			public T Current
			{
				get
				{
					return this.current;
				}
			}

			/// <summary>
			/// Gets the current element in the collection.
			/// </summary>
			/// <returns>
			/// The current element in the collection.
			/// </returns>
			/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.
			///                 </exception><filterpriority>2</filterpriority>
			object IEnumerator.Current
			{
				get
				{
					return this.current;
				}
			}

			#endregion
		}
	}
}
