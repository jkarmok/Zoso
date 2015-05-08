using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using Karen90MmoFramework.Game;

namespace Karen90MmoTests
{
	class QuestCollectionTest : IDemo
	{
		private const int MAX_SIZE = 20;
		private const int ITERATION = 100000;

		#region Implementation of IDemo

		public void Run()
		{
			Console.WriteLine("========== Iterator ==========");
			RunIteratorFilled();
			Console.WriteLine("========== Iterator ==========\n");

			Console.WriteLine("========== Locator ==========");
			RunLocatorFilled();
			Console.WriteLine("========== Locator ==========\n");
		}

		static void RunLocatorFilled()
		{
			var dictCollection = new QuestCollectionDictionary(MAX_SIZE);
			var indexCollection = new QuestCollectionIndexer(MAX_SIZE);
			var arrayCollection = new QuestCollectionArray(MAX_SIZE);
			var linkCollection = new QuestCollectionLink(MAX_SIZE);

			for (short i = 1; i <= MAX_SIZE; i++)
			{
				dictCollection.Add(i, new QuestProgression());
				indexCollection.Add(i, new QuestProgression());
				arrayCollection.Add(i, new QuestProgression());
				linkCollection.Add(i, new QuestProgression());
			}

			var sw1 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				for (short j = 1; j <= MAX_SIZE; j++)
				{
					QuestProgression progression;
					dictCollection.TryGetValue(j, out progression);
				}
			}
			sw1.Stop();
			Console.WriteLine("QuestCollectionDictionary " + sw1.ElapsedMilliseconds);

			var sw2 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				for (short j = 1; j <= MAX_SIZE; j++)
				{
					QuestProgression progression;
					indexCollection.TryGetValue(j, out progression);
				}
			}
			sw2.Stop();
			Console.WriteLine("QuestCollectionIndexer " + sw2.ElapsedMilliseconds);

			var sw3 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				for (short j = 1; j <= MAX_SIZE; j++)
				{
					QuestProgression progression;
					arrayCollection.TryGetValue(j, out progression);
				}
			}
			sw3.Stop();
			Console.WriteLine("QuestCollectionArray " + sw3.ElapsedMilliseconds);

			var sw7 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				for (short j = 1; j <= MAX_SIZE; j++)
				{
					QuestProgression progression;
					linkCollection.TryGetValue(j, out progression);
				}
			}
			sw7.Stop();
			Console.WriteLine("QuestCollectionLink " + sw7.ElapsedMilliseconds);

			var sw4 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				for (short j = 1; j <= MAX_SIZE; j++)
				{
					QuestProgression progression;
					arrayCollection.TryGetValue(j, out progression);
				}
			}
			sw4.Stop();
			Console.WriteLine("QuestCollectionArray " + sw4.ElapsedMilliseconds);

			var sw5 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				for (short j = 1; j <= MAX_SIZE; j++)
				{
					QuestProgression progression;
					indexCollection.TryGetValue(j, out progression);
				}
			}
			sw5.Stop();
			Console.WriteLine("QuestCollectionIndexer " + sw5.ElapsedMilliseconds);

			var sw6 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				for (short j = 1; j <= MAX_SIZE; j++)
				{
					QuestProgression progression;
					dictCollection.TryGetValue(j, out progression);
				}
			}
			sw6.Stop();
			Console.WriteLine("QuestCollectionDictionary " + sw6.ElapsedMilliseconds);

			var sw8 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				for (short j = 1; j <= MAX_SIZE; j++)
				{
					QuestProgression progression;
					linkCollection.TryGetValue(j, out progression);
				}
			}
			sw8.Stop();
			Console.WriteLine("QuestCollectionLink " + sw8.ElapsedMilliseconds);
		}

		static void RunIteratorFilled()
		{
			var dictCollection = new QuestCollectionDictionary(MAX_SIZE);
			var indexCollection = new QuestCollectionIndexer(MAX_SIZE);
			var arrayCollection = new QuestCollectionArray(MAX_SIZE);
			var linkCollection = new QuestCollectionLink(MAX_SIZE);

			for (short i = 1; i <= MAX_SIZE; i++)
			{
				dictCollection.Add(i, new QuestProgression());
				indexCollection.Add(i, new QuestProgression());
				arrayCollection.Add(i, new QuestProgression());
				linkCollection.Add(i, new QuestProgression());
			}

			var sw1 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				foreach (var entry in dictCollection)
				{
					var questId = entry.Key;
					var progression = entry.Value;
				}
			}
			sw1.Stop();
			Console.WriteLine("QuestCollectionDictionary " + sw1.ElapsedMilliseconds);

			var sw2 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				foreach (var entry in indexCollection)
				{
					var questId = entry.Key;
					var progression = entry.Value;
				}
			}
			sw2.Stop();
			Console.WriteLine("QuestCollectionIndexer " + sw2.ElapsedMilliseconds);

			var sw3 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				foreach (var entry in arrayCollection)
				{
					var questId = entry.Key;
					var progression = entry.Value;
				}
			}
			sw3.Stop();
			Console.WriteLine("QuestCollectionArray " + sw3.ElapsedMilliseconds);

			var sw7 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				foreach (var entry in linkCollection)
				{
					var questId = entry.Key;
					var progression = entry.Value;
				}
			}
			sw7.Stop();
			Console.WriteLine("QuestCollectionLink " + sw7.ElapsedMilliseconds);

			var sw4 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				foreach (var entry in arrayCollection)
				{
					var questId = entry.Key;
					var progression = entry.Value;
				}
			}
			sw4.Stop();
			Console.WriteLine("QuestCollectionArray " + sw4.ElapsedMilliseconds);

			var sw5 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				foreach (var entry in indexCollection)
				{
					var questId = entry.Key;
					var progression = entry.Value;
				}
			}
			sw5.Stop();
			Console.WriteLine("QuestCollectionIndexer " + sw5.ElapsedMilliseconds);

			var sw6 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				foreach (var entry in dictCollection)
				{
					var questId = entry.Key;
					var progression = entry.Value;
				}
			}
			sw6.Stop();
			Console.WriteLine("QuestCollectionDictionary " + sw6.ElapsedMilliseconds);

			var sw8 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				foreach (var entry in linkCollection)
				{
					var questId = entry.Key;
					var progression = entry.Value;
				}
			}
			sw8.Stop();
			Console.WriteLine("QuestCollectionLink " + sw8.ElapsedMilliseconds);
			
		}

		static void RunLocatorPartial()
		{
			var dictCollection = new QuestCollectionDictionary(MAX_SIZE);
			var indexCollection = new QuestCollectionIndexer(MAX_SIZE);
			var arrayCollection = new QuestCollectionArray(MAX_SIZE);
			var linkCollection = new QuestCollectionLink(MAX_SIZE);

			for (short i = 1; i <= MAX_SIZE; i++)
			{
				dictCollection.Add(i, new QuestProgression());
				indexCollection.Add(i, new QuestProgression());
				arrayCollection.Add(i, new QuestProgression());
				linkCollection.Add(i, new QuestProgression());
			}

			var sw1 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				for (short j = 1; j <= MAX_SIZE; j++)
				{
					QuestProgression progression;
					dictCollection.TryGetValue(j, out progression);
				}
			}
			sw1.Stop();
			Console.WriteLine("QuestCollectionDictionary " + sw1.ElapsedMilliseconds);

			var sw2 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				for (short j = 1; j <= MAX_SIZE; j++)
				{
					QuestProgression progression;
					indexCollection.TryGetValue(j, out progression);
				}
			}
			sw2.Stop();
			Console.WriteLine("QuestCollectionIndexer " + sw2.ElapsedMilliseconds);

			var sw3 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				for (short j = 1; j <= MAX_SIZE; j++)
				{
					QuestProgression progression;
					arrayCollection.TryGetValue(j, out progression);
				}
			}
			sw3.Stop();
			Console.WriteLine("QuestCollectionArray " + sw3.ElapsedMilliseconds);

			var sw7 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				for (short j = 1; j <= MAX_SIZE; j++)
				{
					QuestProgression progression;
					linkCollection.TryGetValue(j, out progression);
				}
			}
			sw7.Stop();
			Console.WriteLine("QuestCollectionLink " + sw7.ElapsedMilliseconds);

			var sw4 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				for (short j = 1; j <= MAX_SIZE; j++)
				{
					QuestProgression progression;
					arrayCollection.TryGetValue(j, out progression);
				}
			}
			sw4.Stop();
			Console.WriteLine("QuestCollectionArray " + sw4.ElapsedMilliseconds);

			var sw5 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				for (short j = 1; j <= MAX_SIZE; j++)
				{
					QuestProgression progression;
					indexCollection.TryGetValue(j, out progression);
				}
			}
			sw5.Stop();
			Console.WriteLine("QuestCollectionIndexer " + sw5.ElapsedMilliseconds);

			var sw6 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				for (short j = 1; j <= MAX_SIZE; j++)
				{
					QuestProgression progression;
					dictCollection.TryGetValue(j, out progression);
				}
			}
			sw6.Stop();
			Console.WriteLine("QuestCollectionDictionary " + sw6.ElapsedMilliseconds);

			var sw8 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				for (short j = 1; j <= MAX_SIZE; j++)
				{
					QuestProgression progression;
					linkCollection.TryGetValue(j, out progression);
				}
			}
			sw8.Stop();
			Console.WriteLine("QuestCollectionLink " + sw8.ElapsedMilliseconds);
		}

		static void RunIteratorPartial()
		{
			var dictCollection = new QuestCollectionDictionary(MAX_SIZE);
			var indexCollection = new QuestCollectionIndexer(MAX_SIZE);
			var arrayCollection = new QuestCollectionArray(MAX_SIZE);
			var linkCollection = new QuestCollectionLink(MAX_SIZE);

			for (short i = 1; i <= MAX_SIZE; i++)
			{
				dictCollection.Add(i, new QuestProgression());
				indexCollection.Add(i, new QuestProgression());
				arrayCollection.Add(i, new QuestProgression());
				linkCollection.Add(i, new QuestProgression());
			}

			var sw1 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				foreach (var entry in dictCollection)
				{
					var questId = entry.Key;
					var progression = entry.Value;
				}
			}
			sw1.Stop();
			Console.WriteLine("QuestCollectionDictionary " + sw1.ElapsedMilliseconds);

			var sw2 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				foreach (var entry in indexCollection)
				{
					var questId = entry.Key;
					var progression = entry.Value;
				}
			}
			sw2.Stop();
			Console.WriteLine("QuestCollectionIndexer " + sw2.ElapsedMilliseconds);

			var sw3 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				foreach (var entry in arrayCollection)
				{
					var questId = entry.Key;
					var progression = entry.Value;
				}
			}
			sw3.Stop();
			Console.WriteLine("QuestCollectionArray " + sw3.ElapsedMilliseconds);

			var sw7 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				foreach (var entry in linkCollection)
				{
					var questId = entry.Key;
					var progression = entry.Value;
				}
			}
			sw7.Stop();
			Console.WriteLine("QuestCollectionLink " + sw7.ElapsedMilliseconds);

			var sw4 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				foreach (var entry in arrayCollection)
				{
					var questId = entry.Key;
					var progression = entry.Value;
				}
			}
			sw4.Stop();
			Console.WriteLine("QuestCollectionArray " + sw4.ElapsedMilliseconds);

			var sw5 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				foreach (var entry in indexCollection)
				{
					var questId = entry.Key;
					var progression = entry.Value;
				}
			}
			sw5.Stop();
			Console.WriteLine("QuestCollectionIndexer " + sw5.ElapsedMilliseconds);

			var sw6 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				foreach (var entry in dictCollection)
				{
					var questId = entry.Key;
					var progression = entry.Value;
				}
			}
			sw6.Stop();
			Console.WriteLine("QuestCollectionDictionary " + sw6.ElapsedMilliseconds);

			var sw8 = Stopwatch.StartNew();
			for (var i = 0; i < ITERATION; i++)
			{
				foreach (var entry in linkCollection)
				{
					var questId = entry.Key;
					var progression = entry.Value;
				}
			}
			sw8.Stop();
			Console.WriteLine("QuestCollectionLink " + sw8.ElapsedMilliseconds);

		}

		#endregion
	}

	public class QuestCollectionDictionary : IEnumerable<KeyValuePair<short, QuestProgression>>
	{
		private readonly int size;
		private readonly Dictionary<short, QuestProgression> collection;

		public QuestCollectionDictionary(int size)
		{
			this.size = size;
			this.collection = new Dictionary<short, QuestProgression>();
		}

		public bool Add(short questId, QuestProgression progression)
		{
			if (collection.Count >= size)
				return false;

			if (collection.ContainsKey(questId))
				return false;

			this.collection.Add(questId, progression);
			return true;
		}

		/// <summary>
		/// Tries to retrieve an <see cref="QuestProgression"/>.
		/// </summary>
		public bool TryGetValue(short key, out QuestProgression value)
		{
			return this.collection.TryGetValue(key, out value);
		}

		public bool Remove(short questId)
		{
			return this.collection.Remove(questId);
		}

		#region Implementation of IEnumerable

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<KeyValuePair<short, QuestProgression>> GetEnumerator()
		{
			return this.collection.GetEnumerator();
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
			return this.collection.GetEnumerator();
		}

		#endregion
	}

	public class QuestCollectionIndexer : IEnumerable<KeyValuePair<short, QuestProgression>>
	{
		#region Constants and Fields

		private readonly Dictionary<short, QuestProgression> dictCollection;
		private readonly KeyValuePair<short, QuestProgression>[] arrayCollection;

		#endregion

		#region Constructors and Destructors

		public QuestCollectionIndexer(int size)
		{
			this.dictCollection = new Dictionary<short, QuestProgression>();
			this.arrayCollection = new KeyValuePair<short, QuestProgression>[size];
		}

		#endregion

		#region IEnumerable Implementation

		public IEnumerator<KeyValuePair<short, QuestProgression>> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds an <see cref="QuestProgression"/>.
		/// </summary>
		public bool Add(short key, QuestProgression value)
		{
			if (dictCollection.Count >= arrayCollection.Length)
				return false;

			if (dictCollection.ContainsKey(key))
				return false;

			var index = this.GetNextAvailableIndex();
			if (index == -1)
				return false;

			this.dictCollection.Add(key, value);
			this.arrayCollection[index] = new KeyValuePair<short, QuestProgression>(key, value);
			return true;
		}

		/// <summary>
		/// Tries to retrieve an <see cref="QuestProgression"/>.
		/// </summary>
		public bool TryGetValue(short key, out QuestProgression value)
		{
			return this.dictCollection.TryGetValue(key, out value);
		}

		/// <summary>
		/// Removes an <see cref="QuestProgression"/>.
		/// </summary>
		public bool Remove(short key)
		{
			if (dictCollection.Count == 0)
				return false;

			if(dictCollection.Remove(key))
			{
				for (var i = 0; i < this.arrayCollection.Length; i++)
				{
					if(arrayCollection[i].Key == key)
						arrayCollection[i] = new KeyValuePair<short, QuestProgression>(0, null);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the next available index
		/// </summary>
		private int GetNextAvailableIndex()
		{
			for (var i = 0; i < arrayCollection.Length; i++)
			{
				if (arrayCollection[i].Value == null)
					return i;
			}

			return -1;
		}

		#endregion

		class Enumerator : IEnumerator<KeyValuePair<short, QuestProgression>>
		{
			private readonly KeyValuePair<short, QuestProgression>[] collection;

			private readonly int size;
			private readonly int endIndex;

			private int count;
			private int index;

			public Enumerator(QuestCollectionIndexer collection)
			{
				this.collection = collection.arrayCollection;

				this.size = collection.dictCollection.Count;
				this.endIndex = collection.arrayCollection.Length;

				this.index = -1;
				this.count = 0;

			}

			#region Implementation of IDisposable

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			/// <filterpriority>2</filterpriority>
			public void Dispose()
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
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
			public bool MoveNext()
			{
				if (index < endIndex)
				{
					while(++index < endIndex)
					{
						if (collection[index].Value != null)
							break;
					}
					return (index < endIndex);
				}
				
				return false;
			}

			/// <summary>
			/// Sets the enumerator to its initial position, which is before the first element in the collection.
			/// </summary>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
			public void Reset()
			{
				this.count = 0;
				this.index = -1;
			}

			/// <summary>
			/// Gets the element in the collection at the current position of the enumerator.
			/// </summary>
			/// <returns>
			/// The element in the collection at the current position of the enumerator.
			/// </returns>
			public KeyValuePair<short, QuestProgression> Current
			{
				get
				{
					return this.collection[index];
				}
			}

			/// <summary>
			/// Gets the current element in the collection.
			/// </summary>
			/// <returns>
			/// The current element in the collection.
			/// </returns>
			/// <filterpriority>2</filterpriority>
			object IEnumerator.Current
			{
				get
				{
					return this.collection[index];
				}
			}

			#endregion
		}
	}

	public class QuestCollectionArray : IEnumerable<KeyValuePair<short, QuestProgression>>
	{
		private readonly KeyValuePair<short, QuestProgression>[] collection;
		private int count;

		public QuestCollectionArray(int size)
		{
			this.collection = new KeyValuePair<short, QuestProgression>[size];
		}

		public bool Add(short questId, QuestProgression progression)
		{
			if (count >= collection.Length)
				return false;

			var index = Array.FindIndex(collection, quest => quest.Key == questId);
			if (index != -1)
				return false;

			index = Array.FindIndex(collection, quest => quest.Value == null);
			if(index == -1)
				throw new IndexOutOfRangeException("Unexpected");

			this.collection[index] = new KeyValuePair<short, QuestProgression>(questId, progression);
			this.count++;
			return true;
		}

		/// <summary>
		/// Tries to retrieve an <see cref="QuestProgression"/>.
		/// </summary>
		public bool TryGetValue(short key, out QuestProgression value)
		{
			value = Array.Find(collection, pair => pair.Key == key).Value;
			return value != null;
		}

		public bool Remove(short questId)
		{
			if (count <= 0)
				return false;

			var index = Array.FindIndex(collection, quest => quest.Value == null);
			if (index == -1)
				return false;

			this.collection[index] = new KeyValuePair<short, QuestProgression>(0, null);
			this.count--;
			return true;
		}

		#region Implementation of IEnumerable

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<KeyValuePair<short, QuestProgression>> GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<short, QuestProgression>>)this.collection).GetEnumerator();
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
			return this.collection.GetEnumerator();
		}

		#endregion
	}

	public class QuestCollectionLink : IEnumerable<KeyValuePair<short, QuestProgression>>
	{
		private readonly LinkedList<KeyValuePair<short, QuestProgression>> collection;
		private readonly int size;

		public QuestCollectionLink(int size)
		{
			this.size = size;
			this.collection = new LinkedList<KeyValuePair<short, QuestProgression>>();
		}

		public bool Add(short questId, QuestProgression progression)
		{
			if (collection.Count >= size)
				return false;
			this.collection.AddLast(new KeyValuePair<short, QuestProgression>(questId, progression));
			return true;
		}

		/// <summary>
		/// Tries to retrieve an <see cref="QuestProgression"/>.
		/// </summary>
		public bool TryGetValue(short key, out QuestProgression value)
		{
			value = default(QuestProgression);

			var current = this.collection.First;
			while (current != null)
			{
				if(current.Value.Key == key)
				{
					value = current.Value.Value;
					break;
				}
				current = current.Next;
			}
			return value != null;
		}

		public bool Remove(short questId)
		{
			var current = this.collection.First;
			while (current != null)
			{
				if (current.Value.Key == questId)
				{
					this.collection.Remove(current);
					break;
				}
				current = current.Next;
			}
			return true;
		}

		#region Implementation of IEnumerable

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<KeyValuePair<short, QuestProgression>> GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<short, QuestProgression>>)this.collection).GetEnumerator();
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
			return this.collection.GetEnumerator();
		}

		#endregion
	}

	public class QuestProgression
	{
		public short QuestId;
		public QuestStatus Status;
		public byte[] ItemCount;
		public byte[] NpcCount;
		public byte PlayerCount;
	}
}
