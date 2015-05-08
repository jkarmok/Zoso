namespace Karen90MmoFramework
{
	public interface IStringConverter<T>
	{
		/// <summary>
		/// Gets the string representation of <see cref="T"/>
		/// </summary>
		/// <returns></returns>
		string ToString(T value);
	}

	/// <summary>
	/// This class invokes the <see cref="ToString"/> on the <see cref="T"/> when the <see cref="Value"/> is set.
	/// This class is useful when the frequency of calling <see cref="ToString"/> is much greater than the change in <see cref="Value"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StringCacher<T>
	{
		private T value;
		private string toString;
		private readonly IStringConverter<T> stringConverter;

		public StringCacher(T value)
		{
			this.Value = value;
			this.stringConverter = new DefaultStringConverter<T>();
		}

		public StringCacher()
		{
			this.Value = default(T);
			this.stringConverter = new DefaultStringConverter<T>();
		}

		public StringCacher(T value, IStringConverter<T> stringConverter)
		{
			this.Value = value;
			this.stringConverter = stringConverter;
		}

		public StringCacher(IStringConverter<T> stringConverter)
		{
			this.Value = default(T);
			this.stringConverter = stringConverter;
		}

		public T Value
		{
			get
			{
				return this.value;
			}

			set
			{
				this.value = value;
				this.toString = this.stringConverter.ToString(value);
			}
		}

		public override sealed string ToString()
		{
			return toString;
		}
	}

	public class DefaultStringConverter<T> : IStringConverter<T>
	{
		/// <summary>
		/// Gets the string representation of <see cref="T"/>
		/// </summary>
		/// <returns></returns>
		public string ToString(T value)
		{
			return value.ToString();
		}
	}
}
