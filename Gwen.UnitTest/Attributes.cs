using System;

namespace Gwen.UnitTest
{
	public class UnitTestAttribute : Attribute
	{
		public string Category { get; set; }
		public int Order { get; set; }
		public string Name { get; set; }

		public UnitTestAttribute()
		{
			Order = Int32.MaxValue;
		}
	}
}
