using System.Collections;

namespace Karen90MmoFramework
{
	public interface IReadOnlyHashSet<in T> : IEnumerable
	{
		int Count { get; }

		bool Contains(T item);
	}
}
