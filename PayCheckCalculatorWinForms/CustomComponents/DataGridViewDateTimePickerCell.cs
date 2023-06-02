using System;
using System.Windows.Forms;

namespace PayCheckCalculatorWinForms.CustomComponents
{
    public class DataGridViewDateTimePickerCell : DataGridViewTextBoxCell
    {
        public DataGridViewDateTimePickerCell()
        {
            Style.Format = "dd/MM/yyyy";
        }

        public override Type EditType => typeof(DataGridViewDateTimePickerEditingControl);

        public override Type ValueType => typeof(DateTime);
        public override object DefaultNewRowValue { get; } = DateTime.Now;
        
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue,
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            var ctl =
                DataGridView.EditingControl as DataGridViewDateTimePickerEditingControl;
            if (Value == null)
            {
                var defaultNewRowValue = this.DefaultNewRowValue;
                if (defaultNewRowValue != null) ctl.Value = (DateTime)defaultNewRowValue;
            }
            else
            {
                if (ctl != null) ctl.Value = (DateTime)this.Value;
            }
        }
    }
}