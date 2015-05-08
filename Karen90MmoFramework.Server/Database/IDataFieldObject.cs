namespace Karen90MmoFramework.Database
{
	public interface IDataFieldObject<T> where T : IDataField
	{
		/// <summary>
		/// Converts an object to a savable data <see cref="Karen90MmoFramework.Database.IDataField"/>.
		/// </summary>
		/// <returns></returns>
		T ToDataField();
	}
}