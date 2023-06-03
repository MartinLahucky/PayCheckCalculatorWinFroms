using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PayCheckCalculatorWinForms.CustomComponents;

namespace PayCheckCalculatorWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PrepareForm();
            dataGridView1.CellEndEdit += CellEChanged;
            dataGridView1.DefaultValuesNeeded += SetDefaultTimes;
        }

        private void PrepareForm()
        {
            // Columns in DataGridView
            var dateTimePickerColumn = new DataGridViewDateTimePickerColumn();
            dateTimePickerColumn.HeaderText = "Datum";
            dataGridView1.Columns.Add(dateTimePickerColumn);

            dataGridView1.Columns.Add("Project", "Projekt");
            dataGridView1.Columns.Add("Task", "Úkol");

            var startTimeColumn = new DataGridViewTimePickerColumn
            {
                Name = "StartTime",
                HeaderText = "Začátek"
            };

            dataGridView1.Columns.Add(startTimeColumn);

            var endTimeColumn = new DataGridViewTimePickerColumn
            {
                Name = "EndTime",
                HeaderText = "Konec"
            };
            dataGridView1.Columns.Add(endTimeColumn);

            var hoursWorkedColumn = new DataGridViewTextBoxColumn
            {
                Name = "Hours",
                HeaderText = "Hodiny",
                ValueType = typeof(string),
                ReadOnly = true
            };
            dataGridView1.Columns.Add(hoursWorkedColumn);
        }

        private void SetDefaultTime(DataGridViewRow row)
        {
            row.Cells["StartTime"].Value = DateTime.Today.AddHours(9); // Sets value 9:00
            row.Cells["EndTime"].Value = DateTime.Today.AddHours(17); // Sets value 17:00
            row.Cells["Hours"].Value =
                (((DateTime)row.Cells["EndTime"].Value).TimeOfDay - ((DateTime)row.Cells["StartTime"].Value).TimeOfDay)
                .TotalHours.ToString("0.00");
        }

        private void SetDefaultTimes(object sender, DataGridViewRowEventArgs e)
        {
            SetDefaultTime(e.Row);
        }

        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = "Import.csv"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.Rows.Clear();

                using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                {
                    // Load header
                    var headerLine = reader.ReadLine();
                    if (headerLine != null)
                    {
                        var headerValues = headerLine.Split(',');

                        for (int i = 0; i < headerValues.Length && i < dataGridView1.Columns.Count; i++)
                        {
                            dataGridView1.Columns[i].HeaderText = headerValues[i];
                        }
                    }

                    // Load data
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line == null) continue;
                        var values = line.Split(',');
                        dataGridView1.Rows.Add(values);
                    }
                }
            }
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = "Export.csv"
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
            var hasIncompleteRows = false;

            // Check for incomplete rows
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                DataGridViewRow row = dataGridView1.Rows[i];

                if (row.IsNewRow) continue;
                var hasEmptyCell = false;

                for (int j = 0; j < row.Cells.Count; j++)
                {
                    var cell = row.Cells[j];
                    if (cell.Value != null && !string.IsNullOrWhiteSpace(cell.Value.ToString())) continue;
                    // If last row is incomplete, don't show warning
                    if (i == dataGridView1.Rows.Count - 2 && j == 0)
                    {
                        break;
                    }

                    hasEmptyCell = true;
                }

                if (!hasEmptyCell) continue;
                hasIncompleteRows = true;
                break;
            }

            if (hasIncompleteRows)
            {
                var result =
                    MessageBox.Show(
                        "Některé řádky obsahují prázdné buňky a nebudou exportovány. Chcete pokračovat v exportu?",
                        "Upozornění na možnou ztrátu dat", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
            {
                // Header
                var columnNames = dataGridView1.Columns
                    .Cast<DataGridViewColumn>()
                    .Select(column => column.HeaderText)
                    .ToArray();
                writer.WriteLine(string.Join(",", columnNames));

                // Data
                // For loop to avoid last row being written 
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    var row = dataGridView1.Rows[i];

                    if (row.IsNewRow) continue;
                    var hasEmptyCell = false;
                    var rowData = new List<string>();

                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                        {
                            hasEmptyCell = true;
                            rowData.Add("");
                        }
                        else
                        {
                            rowData.Add(cell.Value.ToString());
                        }
                    }

                    if (!hasEmptyCell)
                    {
                        writer.WriteLine(string.Join(",", rowData));
                    }
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Opravdu chcete ukončit aplikaci?", "Ukončit", MessageBoxButtons.YesNo) ==
                DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // Different name because of shadowing
        private void CellEChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && !dataGridView1.Rows[e.RowIndex].IsNewRow)
                {
                    var row = dataGridView1.Rows[e.RowIndex];
                    if (dataGridView1.Columns.Contains("StartTime") && dataGridView1.Columns.Contains("EndTime") &&
                        dataGridView1.Columns.Contains("Hours"))
                    {
                        var startValue = row.Cells["StartTime"].Value;
                        var endValue = row.Cells["EndTime"].Value;

                        if (startValue != null && endValue != null &&
                            DateTime.TryParse(startValue.ToString(), out DateTime startTime) &&
                            DateTime.TryParse(endValue.ToString(), out DateTime endTime))
                        {
                            row.Cells["Hours"].Value =
                                (endTime.TimeOfDay - startTime.TimeOfDay).TotalHours.ToString("0.00");
                        }
                        else
                        {
                            row.Cells["Hours"].Value = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při zpracování změny hodnoty buňky: " + ex.Message);
                Console.WriteLine(ex.Message);
            }
        }
    }
}