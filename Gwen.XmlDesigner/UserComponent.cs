using System;
using System.IO;
using Gwen.Control;
using Gwen.Xml;

namespace Gwen.XmlDesigner
{
	public class UserComponent : Component
	{
		public static void Register(string fileName)
		{
			Component.Register<UserComponent>(Path.GetFileNameWithoutExtension(fileName), fileName);
		}

		public static void Unregister(string fileName)
		{
			Component.Unregister<UserComponent>(Path.GetFileNameWithoutExtension(fileName));
		}

		public UserComponent(ControlBase parent, object fileName)
			: base(parent, new XmlFileSource((string)fileName))
		{
		}
	}
}
