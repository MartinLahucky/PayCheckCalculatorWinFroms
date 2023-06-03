using System;
using System.Windows.Forms;

namespace PayCheckCalculatorWinForms.CustomComponents
{
    public class DataGridViewTimePickerCell : DataGridViewTextBoxCell
    {
        public DataGridViewTimePickerCell()
        {
            Style.Format = "HH:mm";
        }

        public override Type EditType => typeof(DataGridViewTimePickerEditingControl);

        public override Type ValueType => typeof(DateTime);
        public override object DefaultNewRowValue => DateTime.Now;

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue,
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            var ctl = DataGridView.EditingControl as DataGridViewTimePickerEditingControl;
            if (Value == null)
            {
                var defaultNewRowValue = DefaultNewRowValue;
                if (defaultNewRowValue != null) ctl.Value = (DateTime)defaultNewRowValue;
            }
            else
            {
                ctl.Value = (DateTime)Value;
            }
        }
    }
}