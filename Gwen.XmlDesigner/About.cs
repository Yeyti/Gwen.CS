using System;
using System.Reflection;
using Gwen.Control;
using Gwen.Xml;
using Gwen.RichText;

namespace Gwen.XmlDesigner
{
	public class About : Component
	{
		private Window m_Window;
		private RichLabel m_AboutText;

		public About(ControlBase parent)
			: base(parent, new XmlStringSource(Xml))
		{
		}

		protected override void OnCreated()
		{
			m_Window = View as Window;
			m_AboutText = GetControl<RichLabel>("AboutText");

			string versionStr = String.Empty;
			Version version = Assembly.GetExecutingAssembly().GetName().Version;
			if (version != null)
				versionStr = String.Format("v{0}", version.ToString(2));

			Document document = new Document();
			document.Paragraph(new Margin(15, 15, 15, 5)).Text("XML Designer for GWEN.Net Extended Layout ").Text(versionStr);
			document.Paragraph(new Margin(15, 5, 15, 10)).Text("Copyright © 2016 Kimmo Palosaari");
			document.Paragraph(new Margin(15, 35, 15, 5)).Text("GWEN.Net Extended Layout");
			document.Paragraph(new Margin(15, 5, 15, 10)).Text("Copyleft © 2016 Kimmo Palosaari");
			document.Paragraph(new Margin(15, 10, 15, 5)).Text("GWEN.Net");
			document.Paragraph(new Margin(15, 5, 15, 10)).Text("Copyleft © 2011 Omega Red");
			document.Paragraph(new Margin(15, 10, 15, 5)).Text("GWEN");
			document.Paragraph(new Margin(15, 5, 15, 10)).Text("Copyright © 2012 Garry Newman");
			document.Paragraph(new Margin(15, 10, 15, 5)).Text("MIT License");
			document.Paragraph(new Margin(15, 5, 15, 10)).Text("Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:");
			document.Paragraph(new Margin(15, 5, 15, 10)).Text("The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.");
			document.Paragraph(new Margin(15, 5, 15, 10)).Text("THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.");

			m_AboutText.Document = document;
		}

		private void OnOkClicked(ControlBase sender, ClickedEventArgs args)
		{
			m_Window.Close();
		}

		private static readonly string Xml = @"<?xml version='1.0' encoding='UTF-8'?>
			<Window Size='400,150' Title='About XML Designer' StartPosition='CenterCanvas'>
				<DockLayout Margin='2'>
					<ScrollControl Margin='2' Dock='Fill' CanScrollH='False'>
						<RichLabel Name='AboutText' Dock='Fill' />
					</ScrollControl>
					<Button Name='Ok' Margin='2' Dock='Bottom' HorizontalAlignment='Center' Width='100' Text='Ok' Clicked='OnOkClicked' />
				</DockLayout>
			</Window>
			";
	}
}
