using System;
using System.Threading.Tasks;
using Raven.Client.Document;
using Raven.Client;
using Raven.Client.Linq;
using Raven.Abstractions.Data;

namespace Karen90MmoFramework.Database
{
	public sealed class DatabaseFactory : IDatabase, IDisposable
	{
		#region Constants and Fields

		private readonly DocumentStore store;

		private readonly string databaseName;

		#endregion

		#region Properties

		/// <summary>
		/// Returns the default database name
		/// </summary>
		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new <see cref="DatabaseFactory"/>.
		/// </summary>
		public DatabaseFactory(string databaseName)
		{			
			this.databaseName = databaseName;
			this.store = new DocumentStore
				{
					Url = "http://localhost:8080",
					DefaultDatabase = this.databaseName,
					Conventions = {FailoverBehavior = FailoverBehavior.FailImmediately}
				};
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Initializes the DatabaseFactory
		/// </summary>
		public bool Initialize()
		{
			try
			{
				store.Initialize();
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Stores a single object. The class must have an Id property otherwise default indexing is used(numeric)
		/// </summary>
		public void Store(IDataObject entity)
		{
			using (var session = store.OpenSession())
			{
				if (string.IsNullOrEmpty(entity.Id))
					throw new NullReferenceException("IDataObject does not have an Id set");

				session.Store(entity);
				session.SaveChanges();
			}
		}

		/// <summary>
		/// Stores a single object. The class must have an Id property otherwise default indexing is used(numeric)
		/// </summary>
		public async Task StoreAsync(IDataObject entity)
		{
			using (var session = store.OpenAsyncSession())
			{
				if (string.IsNullOrEmpty(entity.Id))
					throw new NullReferenceException("IDataObject does not have an Id set");
				
				await session.StoreAsync(entity);
				await session.SaveChangesAsync();
			}
		}

		/// <summary>
		/// Loads an object of a specific type with the specified id
		/// </summary>
		public T Load<T>(string id) where T : IDataObject
		{
			using (var session = store.OpenSession())
			{
				return session.Load<T>(id);
			}
		}

		/// <summary>
		/// Loads an object of a specific type with the specified id
		/// </summary>
		public async Task<T> LoadAsync<T>(string id) where T : IDataObject
		{
			using (var session = store.OpenAsyncSession())
			{
				return await session.LoadAsync<T>(id);
			}
		}

		/// <summary>
		/// Deletes an entity with a specific id from the database
		/// </summary>
		public void Delete<T>(string id) where T : IDataObject
		{
			using (var session = store.OpenSession())
			{
				var item = session.Load<T>(id);
				if (item != null)
				{
					session.Delete(item);
					session.SaveChanges();
				}
			}
		}

		/// <summary>
		/// Deletes any entity with the specified Id
		/// </summary>
		public void Delete(string id)
		{
			store.DatabaseCommands.Delete(id, null);
		}

		/// <summary>
		/// Deletes any entity with the specified Id
		/// </summary>
		public async Task DeleteAsync(string id)
		{
			await store.AsyncDatabaseCommands.DeleteDocumentAsync(id);
		}

		/// <summary>
		/// Returns the query for a specific type
		/// </summary>
		public IRavenQueryable<T> Query<T>() where T : IDataObject
		{
			using (var session = store.OpenSession())
			{
				return session.Query<T>();
			}
		}

		/// <summary>
		/// Returns the query for a specific type
		/// </summary>
		public IRavenQueryable<T> QueryAsync<T>() where T : IDataObject
		{
			using (var session = store.OpenAsyncSession())
			{
				return session.Query<T>();
			}
		}

		/// <summary>
		/// Returns the query for a specific type
		/// </summary>
		public IRavenQueryable<T> Query<T>(string indexName) where T : IDataObject
		{
			using (var session = store.OpenSession())
			{
				return session.Query<T>(indexName);
			}
		}

		/// <summary>
		/// Returns the query for a specific type
		/// </summary>
		public IRavenQueryable<T> QueryAsync<T>(string indexName) where T : IDataObject
		{
			using (var session = store.OpenAsyncSession())
			{
				return session.Query<T>(indexName);
			}
		}

		/// <summary>
		/// Opens a session
		/// </summary>
		public IDocumentSession OpenSession()
		{
			return this.store.OpenSession();
		}

		/// <summary>
		/// Opens an async session
		/// </summary>
		public IAsyncDocumentSession OpenAsyncSession()
		{
			return this.store.OpenAsyncSession();
		}

		/// <summary>
		/// Sends a patch request
		/// </summary>
		public void Patch(string id, PatchRequest patchRequest)
		{
			store.DatabaseCommands.Patch(
				id,
				new[]
				{
					patchRequest
				});
		}

		/// <summary>
		/// Sends a patch request
		/// </summary>
		public async Task PatchAsync(string id, PatchRequest patchRequest)
		{
			await store.AsyncDatabaseCommands.PatchAsync(id, new[] {patchRequest}, true);
		}

		/// <summary>
		/// Sends multiple patch requests
		/// </summary>
		public void Patch(string id, PatchRequest[] patchRequests)
		{
			store.DatabaseCommands.Patch(id, patchRequests);
		}

		/// <summary>
		/// Sends multiple patch requests
		/// </summary>
		public async Task PatchAsync(string id, PatchRequest[] patchRequests)
		{
			await store.AsyncDatabaseCommands.PatchAsync(id, patchRequests, true);
		}

		/// <summary>
		/// Disposes the Database
		/// </summary>
		public void Dispose()
		{
			store.Dispose();
		}

		///// <summary>
		///// Creates an index from a map
		///// </summary>
		//public void CreateIndex<T>() where T : AbstractMultiMapIndexCreationTask
		//{
		//	IndexCreation.CreateIndexes(typeof(T).Assembly, this.store);
		//}

		#endregion
	}
}
