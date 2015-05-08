namespace Karen90MmoFramework
{
	public interface IPoolObject<out TKey>
	{
		/// <summary>
		/// Gets the key
		/// </summary>
		/// <returns></returns>
		TKey GetKey();
	}

	public interface IPoolObject
	{
	}
}
