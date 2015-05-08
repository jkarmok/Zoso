using System.Collections.Generic;

namespace Karen90MmoFramework.Server.Game.Systems
{
	public interface IMerchant
	{
		/// <summary>
		/// Gets the merchant inventory
		/// </summary>
		IEnumerable<short> Inventory { get; }
	}
}
