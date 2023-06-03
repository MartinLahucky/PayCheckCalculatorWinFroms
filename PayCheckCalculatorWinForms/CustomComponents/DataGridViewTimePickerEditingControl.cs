using System;
using System.Windows.Forms;

namespace PayCheckCalculatorWinForms.CustomComponents
{
    public class DataGridViewTimePickerEditingControl : DateTimePicker, IDataGridViewEditingControl
    {
        public DataGridViewTimePickerEditingControl()
        {
            Format = DateTimePickerFormat.Custom;
            CustomFormat = "HH:mm";
            ShowUpDown = true;
        }

        public bool RepositionEditingControlOnValueChange => false;

        public Cursor EditingPanelCursor => base.Cursor;
        public DataGridView EditingControlDataGridView { get; set; }

        public int EditingControlRowIndex { get; set; }
        public bool EditingControlValueChanged { get; set; }

        public object EditingControlFormattedValue
        {
            get => Value.ToString("HH:mm");
            set
            {
                if (value is String)
                {
                    Value = DateTime.Parse((String)value);
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            Font = dataGridViewCellStyle.Font;
            CalendarForeColor = dataGridViewCellStyle.ForeColor;
            CalendarMonthBackground = dataGridViewCellStyle.BackColor;
        }

        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }

        protected override void OnValueChanged(EventArgs eventargs)
        {
            EditingControlValueChanged = true;
            EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(eventargs);
        }
    }
}