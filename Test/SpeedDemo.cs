using System;
using System.Threading;

namespace Karen90MmoTests
{
	class SpeedDemo : IDemo
	{
		#region Implementation of IDemo

		public void Run()
		{
			for (int i = 0; i < 5; i++)
			{
				new Thread(() => BaseClass.Instantiate()).Start();
			}
		}
		
		#endregion
	}

	class BaseClass
	{
		private static BaseClass _instance;
		public static BaseClass Instance
		{
			get
			{
				return _instance;
			}
		}

		private BaseClass()
		{
			Console.WriteLine("Created");
		}

		public static BaseClass Instantiate()
		{
			lock (typeof (BaseClass))
				return _instance ?? (_instance = new BaseClass());
		}
	}
}
