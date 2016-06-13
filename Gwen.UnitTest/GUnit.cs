using System;
using Gwen.Control;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
    public class GUnit : ControlBase
    {
        public UnitTest UnitTest;

        public GUnit(ControlBase parent) : base(parent)
        {
			this.IsVirtualControl = true;
        }

        public void UnitPrint(string str)
        {
            if (UnitTest != null)
                UnitTest.PrintText(str);
        }
	}
}
