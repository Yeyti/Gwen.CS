using System;
using Gwen.Control;
using Gwen.Xml;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Xml", Order = 601)]
	public class XmlComponent : GUnit
	{
		public XmlComponent(ControlBase parent)
			: base(parent)
		{
			Component.Register<ValueControl>();

			new Container(this);
        }

		private class Container : Component
		{
			public Container(GUnit unit)
				: base(unit, new XmlStringSource(m_Xml))
			{
				m_unit = unit;
			}

			public void OnValueChanged(Gwen.Control.ControlBase sender, ValueChangedEventArgs args)
			{
				m_unit.UnitPrint(sender.Name + ": ValueChanged " + args.Value);
			}

			private GUnit m_unit;

			private static readonly string m_Xml = @"<?xml version='1.0' encoding='UTF-8'?>
			<GridLayout Width='400' ColumnWidths='Auto, 100%'>
				<Label AutoSizeToContents='False' Alignment='CenterV' Text='Value 1:' />
				<ValueControl Name='Value1' Step='2.0' ValueChanged='OnValueChanged' />
				<Label AutoSizeToContents='False' Alignment='CenterV' Text='Value 2:' />
				<ValueControl Name='Value2' Step='5.0' ValueChanged='OnValueChanged' />
			</GridLayout>
			";
		}

		public class ValueChangedEventArgs : EventArgs
		{
			public int Value;
		}

		private class ValueControl : Component
		{
			static ValueControl()
			{
				Parser.RegisterEventHandlerConverter(typeof(ValueChangedEventArgs), (attribute, value) =>
				{
					return new Control.ControlBase.GwenEventHandler<ValueChangedEventArgs>(new XmlEventHandler<ValueChangedEventArgs>(value, attribute).OnEvent);
				});
			}

			[Xml.XmlEvent]
			public event GwenEventHandler<ValueChangedEventArgs> ValueChanged;

			public ValueControl(Control.ControlBase parent)
				: base(parent, new XmlStringSource(m_Xml))
			{

			}

			public void OnButtonClicked(ControlBase sender, ClickedEventArgs args)
			{
				TextBoxNumeric value = GetControl("Value") as TextBoxNumeric;

                int buttonId = (int)sender.UserData;
				if (buttonId == 1)
					value.Value -= Step;
				else if (buttonId == 2)
					value.Value += Step;
				
				if (ValueChanged != null)
					ValueChanged(this.View, new ValueChangedEventArgs() { Value = (int)value.Value });
			}

			public void OnSubmitPressed(ControlBase sender, EventArgs args)
			{
				TextBoxNumeric value = GetControl("Value") as TextBoxNumeric;

				if (ValueChanged != null)
					ValueChanged(this.View, new ValueChangedEventArgs() { Value = (int)value.Value });
			}

			[Xml.XmlProperty]
			public float Step { get; set; }

			private static readonly string m_Xml = @"<?xml version='1.0' encoding='UTF-8'?>
				<GridLayout ColumnWidths='25%, 50%, 25%'>
					<Button Name='DecButton' Text='Dec' UserData='1' Clicked='OnButtonClicked' />
					<TextBoxNumeric Name='Value' Dock='Fill' SubmitPressed='OnSubmitPressed' />
					<Button Name='IndButton' Text='Inc' UserData='2' Clicked='OnButtonClicked' />
				</GridLayout>
				";
        }
	}
}
