using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;

namespace Karen90MmoFramework.Database
{
	public interface IDatabase
	{
		/// <summary>
		/// Stores a single object. The class must have an Id property otherwise default indexing is used(numeric)
		/// </summary>
		void Store(IDataObject entity);

		/// <summary>
		/// Loads an object of a specific type with the specified id
		/// </summary>
		T Load<T>(string id) where T : IDataObject;

		/// <summary>
		/// Deletes an entity with a specific id from the database
		/// </summary>
		void Delete<T>(string id) where T : IDataObject;

		/// <summary>
		/// Deletes any entity with the specified Id
		/// </summary>
		void Delete(string id);

		/// <summary>
		/// Returns the query for a specific type
		/// </summary>
		IRavenQueryable<T> Query<T>() where T : IDataObject;

		/// <summary>
		/// Returns the query for a specific type using an index
		/// </summary>
		IRavenQueryable<T> Query<T>(string indexName) where T : IDataObject;

		/// <summary>
		/// Opens a database session
		/// </summary>
		IDocumentSession OpenSession();

		/// <summary>
		/// Opens an async database session
		/// </summary>
		IAsyncDocumentSession OpenAsyncSession();

		/// <summary>
		/// Sends a patch request
		/// </summary>
		void Patch(string id, PatchRequest patchRequest);

		/// <summary>
		/// Sends multiple patch requests
		/// </summary>
		void Patch(string id, PatchRequest[] patchRequests);
	}
}
