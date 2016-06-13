using System;

namespace Gwen.Control
{
	/// <summary>
	/// Single table row.
	/// </summary>
	public class TableRow : ControlBase
	{
		// [omeg] todo: get rid of this
		public const int MaxColumns = 5;

		private int m_ColumnCount;
		private bool m_EvenRow;
		private readonly Label[] m_Columns;

		internal Label GetColumn(int index)
		{
			return m_Columns[index];
		}

		/// <summary>
		/// Invoked when the row has been selected.
		/// </summary>
		public event GwenEventHandler<ItemSelectedEventArgs> Selected;

		/// <summary>
		/// Column count.
		/// </summary>
		public int ColumnCount { get { return m_ColumnCount; } set { SetColumnCount(value); } }

		/// <summary>
		/// Indicates whether the row is even or odd (used for alternate coloring).
		/// </summary>
		public bool EvenRow { get { return m_EvenRow; } set { m_EvenRow = value; } }

		/// <summary>
		/// Text of the first column.
		/// </summary>
		[Xml.XmlProperty]
		public string Text { get { return GetText(0); } set { SetCellText(0, value); } }

		/// <summary>
		/// Initializes a new instance of the <see cref="TableRow"/> class.
		/// </summary>
		/// <param name="parent">Parent control.</param>
		public TableRow(ControlBase parent)
			: base(parent)
		{
			m_Columns = new Label[MaxColumns];
			if (parent is ListBox)
				m_ColumnCount = ((ListBox)parent).ColumnCount;
			else if (parent is Table)
				m_ColumnCount = ((Table)parent).ColumnCount;
			KeyboardInputEnabled = true;
		}

		/// <summary>
		/// Sets the number of columns.
		/// </summary>
		/// <param name="columnCount">Number of columns.</param>
		protected void SetColumnCount(int columnCount)
		{
			if (columnCount == m_ColumnCount) return;

			if (columnCount >= MaxColumns)
				throw new ArgumentException("Invalid column count", "columnCount");

			for (int i = 0; i < MaxColumns; i++)
			{
				if (i < columnCount)
				{
					if (null == m_Columns[i])
					{
						m_Columns[i] = new Label(this);
						m_Columns[i].Padding = Padding.Three;
						m_Columns[i].Margin = new Margin(0, 0, 2, 0); // to separate them slightly
						m_Columns[i].TextColor = Skin.Colors.ListBox.Text_Normal;
					}
				}
				else if (null != m_Columns[i])
				{
					RemoveChild(m_Columns[i], true);
					m_Columns[i] = null;
				}
			}

			m_ColumnCount = columnCount;
		}

		/// <summary>
		/// Sets the column width (in pixels).
		/// </summary>
		/// <param name="column">Column index.</param>
		/// <param name="width">Column width.</param>
		public void SetColumnWidth(int column, int width)
		{
			if (null == m_Columns[column]) 
				return;
			if (m_Columns[column].Width == width) 
				return;

			m_Columns[column].Width = width;
		}

		/// <summary>
		/// Sets the text of a specified cell.
		/// </summary>
		/// <param name="columnIndex">Column number.</param>
		/// <param name="text">Text to set.</param>
		public void SetCellText(int columnIndex, string text)
		{
			if (null == m_Columns[columnIndex])
			{
				m_Columns[columnIndex] = new Label(this);
				m_Columns[columnIndex].Padding = Padding.Three;
				m_Columns[columnIndex].Margin = new Margin(0, 0, 2, 0); // to separate them slightly
				m_Columns[columnIndex].TextColor = Skin.Colors.ListBox.Text_Normal;
			}

			if (columnIndex >= m_ColumnCount)
				throw new ArgumentException("Invalid column index", "columnIndex");

			m_Columns[columnIndex].Text = text;
		}

		/// <summary>
		/// Sets the contents of a specified cell.
		/// </summary>
		/// <param name="column">Column number.</param>
		/// <param name="control">Cell contents.</param>
		/// <param name="enableMouseInput">Determines whether mouse input should be enabled for the cell.</param>
		public void SetCellContents(int column, ControlBase control, bool enableMouseInput = false)
		{
			if (null == m_Columns[column]) 
				return;

			control.Parent = m_Columns[column];
			m_Columns[column].MouseInputEnabled = enableMouseInput;
		}

		/// <summary>
		/// Gets the contents of a specified cell.
		/// </summary>
		/// <param name="column">Column number.</param>
		/// <returns>Control embedded in the cell.</returns>
		public ControlBase GetCellContents(int column)
		{
			return m_Columns[column];
		}

		protected virtual void OnRowSelected()
		{
			if (Selected != null)
				Selected.Invoke(this, new ItemSelectedEventArgs(this));
		}

		protected override Size Measure(Size availableSize)
		{
			int width = 0;
			int height = 0;

			for (int i = 0; i < m_ColumnCount; i++)
			{
				if (null == m_Columns[i])
					continue;

				Size size = m_Columns[i].DoMeasure(new Size(availableSize.Width - width, availableSize.Height));

				width += size.Width;
				height = Math.Max(height, size.Height);
			}

			return new Size(width, height);
		}

		protected override Size Arrange(Size finalSize)
		{
			int x = 0;
			int height = 0;

			for (int i = 0; i < m_ColumnCount; i++)
			{
				if (null == m_Columns[i])
					continue;

				if (i == m_ColumnCount - 1)
					m_Columns[i].DoArrange(new Rectangle(x, 0, finalSize.Width - x, m_Columns[i].MeasuredSize.Height));
				else
					m_Columns[i].DoArrange(new Rectangle(x, 0, m_Columns[i].MeasuredSize.Width, m_Columns[i].MeasuredSize.Height));
				x += m_Columns[i].MeasuredSize.Width;
				height = Math.Max(height, m_Columns[i].MeasuredSize.Height);
			}

			return new Size(finalSize.Width, height);
		}

		/// <summary>
		/// Sets the text color for all cells.
		/// </summary>
		/// <param name="color">Text color.</param>
		public void SetTextColor(Color color)
		{
			for (int i = 0; i < m_ColumnCount; i++)
			{
				if (null == m_Columns[i]) continue;
				m_Columns[i].TextColor = color;
			}
		}

		/// <summary>
		/// Returns text of a specified row cell (default first).
		/// </summary>
		/// <param name="column">Column index.</param>
		/// <returns>Column cell text.</returns>
		public string GetText(int column = 0)
		{
			return m_Columns[column].Text;
		}

		/// <summary>
		/// Handler for Copy event.
		/// </summary>
		/// <param name="from">Source control.</param>
		protected override void OnCopy(ControlBase from, EventArgs args)
		{
			Platform.Platform.SetClipboardText(Text);
		}
	}
}
