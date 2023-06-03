using System;
using System.Windows.Forms;

namespace PayCheckCalculatorWinForms.CustomComponents
{
    public class DataGridViewTimePickerColumn : DataGridViewColumn
    {
        public DataGridViewTimePickerColumn() : base(new DataGridViewTimePickerCell())
        {
        }
        // Necessary override 
        public override DataGridViewCell CellTemplate
        {
            get => base.CellTemplate;
            set
            {
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewTimePickerCell)))
                {
                    throw new InvalidCastException("Musí být typ DataGridViewTimePickerCell");
                }

                base.CellTemplate = value;
            }
        }
    }
}