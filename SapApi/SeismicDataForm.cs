using SAP2000.models.seismic;using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace SAP2000
{
    public partial class SeismicDataForm : Form
    {
        public SeismicParameters SeismicParameters { get; private set; }

        private TextBox txtSs;
        private TextBox txtS1;

        private ComboBox cmbSiteClass;
        private NumericUpDown numR, numD, numI;

        public SeismicDataForm()
        {
            InitializeComponentUI();
            this.SeismicParameters = new SeismicParameters();
        }

        private void InitializeComponentUI()
        {
            this.Text = "AFAD Deprem Verileri";            
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(480, 380);            
            this.Padding = new Padding(30);           
            this.Font = new Font("Segoe UI", 10F);            
            this.BackColor = Color.White;
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                BackColor = Color.Transparent,                
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));

            txtSs = AddRow(mainLayout, "Kısa P. Tasarım Spektral İvme K. (Ss):");
            txtSs.Text = "1.7";
            ApplyInputControlStyle(txtSs);

            txtS1 = AddRow(mainLayout, "1.0 sn P. Tasarım Spektral İvme K. (S1):");
            txtS1.Text = "0.45";
            ApplyInputControlStyle(txtS1);

            cmbSiteClass = AddRow<ComboBox>(mainLayout, "Yerel Zemin Sınıfı:");
            cmbSiteClass.Items.AddRange(new object[] { "ZA", "ZB", "ZC", "ZD", "ZE" });
            cmbSiteClass.SelectedIndex = 1;
            cmbSiteClass.DropDownStyle = ComboBoxStyle.DropDownList;            
            ApplyInputControlStyle(cmbSiteClass);

            numR = AddRow<NumericUpDown>(mainLayout, "Taşıyıcı Sistem Davranış Katsayısı (R):");
            numR.DecimalPlaces = 1;
            numR.Minimum = 1;
            numR.Maximum = 20;
            numR.Value = 8.0m;
            ApplyInputControlStyle(numR);

            numD = AddRow<NumericUpDown>(mainLayout, "Dayanım Fazlalığı Katsayısı (D):");
            numD.DecimalPlaces = 1;
            numD.Minimum = 1;
            numD.Maximum = 10;
            numD.Value = 6.0m;
            ApplyInputControlStyle(numD);

            numI = AddRow<NumericUpDown>(mainLayout, "Bina Önem Katsayısı (I):");
            numI.DecimalPlaces = 2;
            numI.Minimum = 1;
            numI.Maximum = 2;
            numI.Value = 1.0m;
            ApplyInputControlStyle(numI);

            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));


            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 60,
                Padding = new Padding(0, 10, 0, 0),                
                BackColor = Color.Transparent            
            };

            var btnOk = new Button { Text = "Tamam", DialogResult = DialogResult.OK, Width = 120, Height = 38 };            
            var btnCancel = new Button { Text = "İptal", DialogResult = DialogResult.Cancel, Width = 120, Height = 38 };
            ApplyModernButtonStyle(btnOk, Color.FromArgb(46, 204, 113), Color.White);            
            ApplyModernButtonStyle(btnCancel, Color.FromArgb(100, 100, 100), Color.White);
            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnOk);

            this.Controls.Add(mainLayout);
            this.Controls.Add(buttonPanel);

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            btnOk.Click += (s, e) => SaveData();
        }

        private T AddRow<T>(TableLayoutPanel pnl, string labelText) where T : Control, new()
        {
            pnl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var label = new Label
            {
                Text = labelText,
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 5, 0, 5),               
                Font = new Font("Segoe UI", 10F),                
                ForeColor = Color.FromArgb(45, 45, 45)            
            };
            var control = new T()
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 5, 0, 5)            
            };
            pnl.Controls.Add(label, 0, pnl.RowCount - 1);
            pnl.Controls.Add(control, 1, pnl.RowCount - 1);
            return control;
        }

        private TextBox AddRow(TableLayoutPanel pnl, string labelText) => AddRow<TextBox>(pnl, labelText);

        private void ApplyInputControlStyle(Control control)
        {
            control.Font = new Font("Segoe UI", 10F);
            control.ForeColor = Color.FromArgb(30, 30, 30);
            if (control is TextBox textBox)
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (control is NumericUpDown numericUpDown)
            {
                numericUpDown.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private void ApplyModernButtonStyle(Button button, Color backColor, Color foreColor)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;            
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);           
            button.Cursor = Cursors.Hand;           
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(backColor.R + 15 > 255 ? 255 : backColor.R + 15,
                                                                      backColor.G + 15 > 255 ? 255 : backColor.G + 15,
                                                                      backColor.B + 15 > 255 ? 255 : backColor.B + 15);
           
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(backColor.R - 15 < 0 ? 0 : backColor.R - 15,
                                                                      backColor.G - 15 < 0 ? 0 : backColor.G - 15,
                                                                      backColor.B - 15 < 0 ? 0 : backColor.B - 15);
        }

        private void SaveData()
        {
            try
            {
                SeismicParameters.Ss = double.Parse(txtSs.Text, NumberStyles.Any, CultureInfo.InvariantCulture);
                SeismicParameters.S1 = double.Parse(txtS1.Text, NumberStyles.Any, CultureInfo.InvariantCulture);

                SeismicParameters.SiteClass = cmbSiteClass.SelectedItem.ToString();
                SeismicParameters.R = (double)numR.Value;
                SeismicParameters.D = (double)numD.Value;
                SeismicParameters.I = (double)numI.Value;

                this.DialogResult = DialogResult.OK;
            }
            catch (FormatException)
            {
                MessageBox.Show("Lütfen sayısal alanlara geçerli sayılar girin.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;            
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}\nLütfen tüm alanları doğru formatta doldurun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;           
            }
        }
    }
}
