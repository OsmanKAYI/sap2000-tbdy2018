// SapApi/Form1.cs
using SAP2000.API.models.excel;
using SAP2000.API.services.excel;
using SAP2000.enums;
using SAP2000.factories;
using SAP2000.models.materials;
using SAP2000.models.placements;
using SAP2000.models.sections;
using SAP2000.models.seismic;
using SAP2000.services;
using SAP2000.services.validators;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace SAP2000
{
    public partial class Form1 : Form
    {
        private readonly ISap2000ApiService _Sap2000ApiService;
        private readonly IExcelDataReaderService _excelDataReaderService; // Excel okuyucu servi güncellemesi
        private readonly List<IMaterialProperties> _materialsToExport = new List<IMaterialProperties>();
        private readonly List<ISectionProperties> _sectionsToExport = new List<ISectionProperties>();
        private readonly List<ColumnPlacementInfo> _columnplacements = new List<ColumnPlacementInfo>();
        private readonly List<BeamPlacementInfo> _beamplacements = new List<BeamPlacementInfo>();
        private SeismicParameters _seismicParameters;
        private int _nextColumnId = 101;
        private int _nextBeamId = 101;





        private DataGridView dgvAddedsections;
        private ComboBox cmbMaterials, cmbMaterialType;
        private readonly Dictionary<string, eCustomMatType> matTypeDisplayMap = new Dictionary<string, eCustomMatType>();
        private TextBox txtMaterialName, txtFck, txtFy, txtFu;
        private Label lblFck, lblFy, lblFu;
        private TextBox txtColName, txtColWidth, txtColHeight, txtColCover;
        private ComboBox cmbColConcrete, cmbColRebar;
        private TextBox txtBeamName, txtBeamWidth, txtBeamHeight, txtBeamCoverTop, txtBeamCoverBottom;
        private ComboBox cmbBeamConcrete, cmbBeamRebar;
        private TextBox txtSlabName, txtSlabThickness;
        private ComboBox cmbSlabMaterial;
        private NumericUpDown numTotalStories;
        private TextBox txtFirstStoryHeight, txtTypicalStoryHeight;
        private DataGridView dgvColumnPlacement;
        private NumericUpDown numNewColumnX, numNewColumnY;
        private ComboBox cmbNewColumnplacementsection;
        private DataGridView dgvBeamPlacement;
        private ComboBox cmbBeamStartColumn;
        private ComboBox cmbBeamEndColumn;
        private ComboBox cmbBeamSection;

        private readonly Dictionary<Control, IInputValidator> _validators = new Dictionary<Control, IInputValidator>();
        private readonly Dictionary<Control, Label> _errorLabels = new Dictionary<Control, Label>();

        public Form1(ISap2000ApiService Sap2000ApiService)
        {
            _Sap2000ApiService = Sap2000ApiService;
            _excelDataReaderService = new ExcelDataReaderService(); 
            InitializeComponentUI(); 
            InitializeComponent(); 
            this.MinimumSize = new Size(1200, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += OnFormLoad;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            LoadMaterialTypes();
            LoadDefaultMaterials();
            RegisterValidators();
        }

        private void InitializeComponentUI()
        {
            this.SuspendLayout();

            var mainLayoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                BackColor = SystemColors.ControlLightLight,
                Padding = new Padding(10),
                Margin = new Padding(0)
            };
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var gbMaterials = BuildMaterialGroupBox();
            var gbStories = BuildStoryDefinitionGroupBox();
            var gbsections = BuildSectionDefinitionGroupBox();
            var gbPlacement = BuildPlacementGroupBox();
            var bottomPanel = BuildBottomPanel(); 

            mainLayoutPanel.Controls.Add(gbMaterials, 0, 0);

            mainLayoutPanel.Controls.Add(gbsections, 1, 0);
            mainLayoutPanel.SetRowSpan(gbsections, 2);

            mainLayoutPanel.Controls.Add(gbStories, 0, 1);

            mainLayoutPanel.Controls.Add(gbPlacement, 0, 2);
            mainLayoutPanel.SetColumnSpan(gbPlacement, 2);

            this.Controls.Add(mainLayoutPanel);
            this.Controls.Add(bottomPanel); 

            this.ResumeLayout(false);
        }

        private GroupBox BuildMaterialGroupBox()
        {
            var gb = new GroupBox { Text = "Malzeme Tanımlama", Dock = DockStyle.Fill, Padding = new Padding(10), Font = new Font("Segoe UI", 10F, FontStyle.Bold), AutoSize = true, BackColor = Color.White };
            var pnl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, AutoSize = true, Font = new Font("Segoe UI", 9F), BackColor = Color.White };
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            txtMaterialName = AddRow(pnl, "Malzeme Adı:");
            var btnAddMaterial = CreateButton("Malzeme Ekle", Color.FromArgb(40, 167, 69), BtnAddMaterialToList_Click);
            pnl.Controls.Add(btnAddMaterial, 2, pnl.RowCount - 1);
            cmbMaterialType = AddRow(pnl, "Malzeme Tipi:", true);
            var btnClearMaterials = CreateButton("Listeyi Temizle", Color.FromArgb(220, 53, 69), (s, e) => { _materialsToExport.Clear(); UpdateUIDataSources(); });
            pnl.Controls.Add(btnClearMaterials, 2, pnl.RowCount - 1);
            cmbMaterialType.SelectedIndexChanged += (s, e) =>
            {
                if (cmbMaterialType.SelectedItem == null || !matTypeDisplayMap.ContainsKey(cmbMaterialType.SelectedItem.ToString())) return;
                var selectedType = matTypeDisplayMap[cmbMaterialType.SelectedItem.ToString()];
                var isRebar = selectedType == eCustomMatType.Rebar;
                lblFck.Visible = txtFck.Visible = !isRebar;
                lblFy.Visible = txtFy.Visible = isRebar;
                lblFu.Visible = txtFu.Visible = isRebar;
                txtMaterialName.Text = cmbMaterialType.Text == "Beton" ? "C25/30" : "B500C";
            };
            txtFck = AddRow(pnl, "fck (MPa):", out lblFck);
            txtFck.Text = "25";
            txtFy = AddRow(pnl, "fy (MPa):", out lblFy);
            txtFy.Text = "500";
            txtFu = AddRow(pnl, "fu (MPa):", out lblFu);
            txtFu.Text = "550";
            lblFy.Visible = txtFy.Visible = false;
            lblFu.Visible = txtFu.Visible = false;
            cmbMaterials = AddRow(pnl, "Tanımlı Malzemeler:", true);
            pnl.SetColumnSpan(cmbMaterials, 2);
            gb.Controls.Add(pnl);
            return gb;
        }

        private GroupBox BuildStoryDefinitionGroupBox()
        {
            var gb = new GroupBox { Text = "Kat Bilgileri Tanımlama", Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, Padding = new Padding(10), Font = new Font("Segoe UI", 10F, FontStyle.Bold), AutoSize = true, Margin = new Padding(3, 10, 3, 3), BackColor = Color.White };
            var pnl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, AutoSize = true, Font = new Font("Segoe UI", 9F), BackColor = Color.White };
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            pnl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnl.Controls.Add(new Label { Text = "Toplam Kat Sayısı", Anchor = AnchorStyles.Bottom | AnchorStyles.Left, AutoSize = true }, 0, 0);
            numTotalStories = new NumericUpDown { Dock = DockStyle.Top, Minimum = 1, Maximum = 100, Value = 5, Font = new Font("Segoe UI", 9F) };
            pnl.Controls.Add(numTotalStories, 0, 1);
            pnl.Controls.Add(new Label { Text = "İlk Kat Yüks. (m)", Anchor = AnchorStyles.Bottom | AnchorStyles.Left, AutoSize = true }, 1, 0);
            txtFirstStoryHeight = new TextBox { Dock = DockStyle.Top, Text = "3", Font = new Font("Segoe UI", 9F) };
            pnl.Controls.Add(txtFirstStoryHeight, 1, 1);
            pnl.Controls.Add(new Label { Text = "Normal Kat Yüks. (m)", Anchor = AnchorStyles.Bottom | AnchorStyles.Left, AutoSize = true }, 2, 0);
            txtTypicalStoryHeight = new TextBox { Dock = DockStyle.Top, Text = "2.8", Font = new Font("Segoe UI", 9F) };
            pnl.Controls.Add(txtTypicalStoryHeight, 2, 1);
            gb.Controls.Add(pnl);
            return gb;
        }

        private GroupBox BuildSectionDefinitionGroupBox()
        {
            var gb = new GroupBox
            {
                Text = "Kesit Tanımlama",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Height = 480,
                Padding = new Padding(10),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.White
            };
            var sectionPanel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, BackColor = Color.White };
            sectionPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            sectionPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            gb.Controls.Add(sectionPanel);
            var tcsections = new TabControl { Dock = DockStyle.Top, Height = 300, Font = new Font("Segoe UI", 9F) };
            tcsections.TabPages.Add(BuildFrameTabPage("Kolon", out txtColName, out cmbColConcrete, out cmbColRebar, out txtColWidth, out txtColHeight, out var colSpecificControls));
            tcsections.TabPages.Add(BuildFrameTabPage("Kiriş", out txtBeamName, out cmbBeamConcrete, out cmbBeamRebar, out txtBeamWidth, out txtBeamHeight, out var beamSpecificControls));
            tcsections.TabPages.Add(BuildSlabTabPage());
            txtColCover = colSpecificControls["Paspayı (mm):"] as TextBox;
            txtColCover.Text = "20";
            txtBeamCoverTop = beamSpecificControls["Üst Paspayı (mm):"] as TextBox;
            txtBeamCoverTop.Text = "20";
            txtBeamCoverBottom = beamSpecificControls["Alt Paspayı (mm):"] as TextBox;
            txtBeamCoverBottom.Text = "20";
            sectionPanel.Controls.Add(tcsections, 0, 0);
            dgvAddedsections = new DataGridView { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F), AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, Margin = new Padding(0, 10, 0, 0) };
            dgvAddedsections.Columns.Add("SectionName", "Kesit Adı");
            dgvAddedsections.Columns.Add("SectionType", "Tip");
            dgvAddedsections.Columns.Add("Material", "Beton Sınıfı");
            dgvAddedsections.Columns.Add("Rebar", "Donatı Çeliği");
            dgvAddedsections.Columns.Add("Dimensions", "Boyutlar");
            sectionPanel.Controls.Add(dgvAddedsections, 0, 1);
            return gb;
        }

        private GroupBox BuildPlacementGroupBox()
        {
            var gb = new GroupBox
            {
                Text = "Kesit Yerleşimi",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Height = 240,
                Padding = new Padding(10),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.White,
                Margin = new Padding(3, 10, 3, 3)
            };
            var tabControl = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };
            var tabKolon = new TabPage("Kolon") { BackColor = Color.White, Padding = new Padding(5) };
            tabKolon.Controls.Add(BuildColumnPlacementTab());
            var tabKiris = new TabPage("Kiriş") { BackColor = Color.White, Padding = new Padding(5) };
            tabKiris.Controls.Add(BuildBeamPlacementTab());
            tabControl.TabPages.Add(tabKolon);
            tabControl.TabPages.Add(tabKiris);
            gb.Controls.Add(tabControl);
            return gb;
        }

        private Panel BuildBottomPanel()
        {
            var bottomFlowPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 60, Padding = new Padding(0, 10, 10, 10), BackColor = Color.White };

            Button btnExportToSap2000 = CreateButton("Modeli Sap2000'e Aktar", Color.DarkGreen, BtnExportToSap2000_Click, 200, 40);
            Button btnSeismicInfo = CreateButton("Deprem Bilgilerini Gir", Color.DarkSlateBlue, BtnShowSeismicForm_Click, 200, 40);

            Button btnImportExcel = CreateButton("Kolon/Kiriş Kordinatlarını Excel'den Aktar", Color.DarkRed, btnImportExcel_Click, 300, 40); 

            bottomFlowPanel.Controls.Add(btnExportToSap2000);
            bottomFlowPanel.Controls.Add(btnSeismicInfo);
            bottomFlowPanel.Controls.Add(btnImportExcel); 

            return bottomFlowPanel;
        }

        private void BtnShowSeismicForm_Click(object sender, EventArgs e)
        {
            using (var form = new SeismicDataForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    _seismicParameters = form.SeismicParameters;
                    MessageBox.Show("Deprem parametreleri başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnExportToSap2000_Click(object sender, EventArgs e)
        {
            try
            {
                var btn = sender as Button;
                btn.Text = "Aktarılıyor...";
                btn.Enabled = false;
                Application.DoEvents();

                var gridData = CreateGridDataFromInputs();

                _Sap2000ApiService.createProjectInNewModel(
                    gridData,
                    _seismicParameters,
                    _materialsToExport,
                    _sectionsToExport,
                    _columnplacements,
                    _beamplacements,
                    true);

                MessageBox.Show("Model başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sap2000'e aktarımda hata oldu: \n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sender is Button btn)
                {
                    btn.Text = "Modeli Sap2000'e Aktar";
                    btn.Enabled = true;
                }
            }
        }

        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Dosyaları (*.xlsx)|*.xlsx|Tüm Dosyalar (*.*)|*.*";
                openFileDialog.Title = "Kolon ve Kiriş Verilerini İçeri Aktarmak İçin Excel Dosyası Seçin";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    try
                    {
                        List<ExcelColumnData> columnData = _excelDataReaderService.readColumnData(filePath);
                        List<ExcelBeamData> beamData = _excelDataReaderService.readBeamData(filePath);

                        foreach (var col in columnData)
                        {
                            string columnName = string.IsNullOrWhiteSpace(col.Name) ? $"S{_nextColumnId++}" : col.Name;

                            if (!_columnplacements.Any(p => (p.X == col.X && p.Y == col.Y) || p.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase)))
                            {
                                var placement = new ColumnPlacementInfo
                                {
                                    ColumnName = columnName,
                                    X = col.X * 1000, 
                                    Y = col.Y * 1000, 
                                    SectionName = col.SectionName
                                };
                                _columnplacements.Add(placement);
                            }
                            else
                            {
                                MessageBox.Show($"Kolon '{columnName}' (X:{col.X}, Y:{col.Y}) zaten mevcut veya aynı isimde bir kolon var. Atlandı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }

                        foreach (var beam in beamData)
                        {
                            string beamName = string.IsNullOrWhiteSpace(beam.Name) ? $"K{_nextBeamId++}" : beam.Name;

                            bool startColExists = _columnplacements.Any(c => c.ColumnName.Equals(beam.StartColumnName, StringComparison.OrdinalIgnoreCase));
                            bool endColExists = _columnplacements.Any(c => c.ColumnName.Equals(beam.EndColumnName, StringComparison.OrdinalIgnoreCase));

                            if (!startColExists || !endColExists)
                            {
                                MessageBox.Show($"Kiriş '{beamName}' için başlangıç kolonu ('{beam.StartColumnName}') veya bitiş kolonu ('{beam.EndColumnName}') bulunamadı. Kiriş atlandı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                continue;
                            }

                            if (!_beamplacements.Any(b => b.StartColumnName.Equals(beam.StartColumnName, StringComparison.OrdinalIgnoreCase) &&
                                                          b.EndColumnName.Equals(beam.EndColumnName, StringComparison.OrdinalIgnoreCase) &&
                                                          b.SectionName.Equals(beam.SectionName, StringComparison.OrdinalIgnoreCase)))
                            {
                                var placement = new BeamPlacementInfo
                                {
                                    BeamName = beamName,
                                    StartColumnName = beam.StartColumnName,
                                    EndColumnName = beam.EndColumnName,
                                    SectionName = beam.SectionName
                                };
                                _beamplacements.Add(placement);
                            }
                            else
                            {
                                MessageBox.Show($"Kiriş '{beamName}' (Başlangıç: {beam.StartColumnName}, Bitiş: {beam.EndColumnName}) zaten mevcut. Atlandı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }

                        UpdateUIDataSources();

                        MessageBox.Show("Excel verileri başarıyla içeri aktarıldı ve listelere eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Excel dosyasını okurken veya verileri işlerken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private GridSystemData CreateGridDataFromInputs()
        {
            var gridData = new GridSystemData();

            gridData.XCoordinates = _columnplacements
                .Select(c => c.X)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            gridData.YCoordinates = _columnplacements
                .Select(c => c.Y)
                .Distinct()
                .OrderBy(y => y)
                .ToList();

            double firstStoryHeight = double.Parse(txtFirstStoryHeight.Text, CultureInfo.InvariantCulture) * 1000;
            double typicalStoryHeight = double.Parse(txtTypicalStoryHeight.Text, CultureInfo.InvariantCulture) * 1000;
            int totalStories = (int)numTotalStories.Value;

            gridData.ZCoordinates.Add(0);
            double currentHeight = 0;
            for (int i = 0; i < totalStories; i++)
            {
                currentHeight += (i == 0) ? firstStoryHeight : typicalStoryHeight;
                gridData.ZCoordinates.Add(currentHeight);
            }

            return gridData;
        }

        private void RegisterValidators()
        {
            _validators[txtColWidth] = new NumericTextBoxValidator(300, "Kolon Genişliği");
            _validators[txtColHeight] = new NumericTextBoxValidator(300, "Kolon Yüksekliği");
            _validators[txtColCover] = new NumericTextBoxValidator(20, "Kolon Paspayı");
            _validators[txtBeamWidth] = new NumericTextBoxValidator(200, "Kiriş Genişliği");
            _validators[txtBeamHeight] = new NumericTextBoxValidator(300, "Kiriş Yüksekliği");
            _validators[txtBeamCoverTop] = new NumericTextBoxValidator(20, "Kiriş Üst Paspayı");
            _validators[txtBeamCoverBottom] = new NumericTextBoxValidator(20, "Kiriş Alt Paspayı");

            foreach (var pair in _validators)
            {
                if (pair.Key is TextBox textBox)
                {
                    textBox.Validating += OnValidatedTextBoxValidating;
                }
            }
        }

        private void OnValidatedTextBoxValidating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is Control control && _validators.TryGetValue(control, out var validator))
            {
                bool isValid = validator.validate(control, out string error);
                validator.applyValidationStyle(control, isValid);

                if (_errorLabels.TryGetValue(control, out Label errorLabel))
                {
                    errorLabel.Text = error; 
                }

                control.Tag = isValid ? null : error;
            }
        }

        private TabPage BuildFrameTabPage(string type, out TextBox txtName, out ComboBox cmbConcrete, out ComboBox cmbRebar, out TextBox txtWidth, out TextBox txtHeight, out Dictionary<string, Control> specificControls)
        {
            var tabPage = new TabPage(type) { BackColor = Color.White, Padding = new Padding(10) };
            var pnl = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 4, AutoSize = true, BackColor = Color.White };

            pnl.ColumnStyles.Clear();
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));             // 0: Açıklama Etiketi
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));         // 1: Kontrol (TextBox/ComboBox)
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));              // 2: Hata Etiketi
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));        // 3: Butonlar

            txtName = AddRow(pnl, "Kesit Adı:");
            pnl.SetColumnSpan(txtName, 2); 
            txtName.Text = type == "Kolon" ? $"S30/30" : "K20/30";

            cmbConcrete = AddRow(pnl, "Beton Malzemesi:", true);
            pnl.SetColumnSpan(cmbConcrete, 2);

            cmbRebar = AddRow(pnl, "Donatı Malzemesi:", true);
            pnl.SetColumnSpan(cmbRebar, 2);

            void CreateAndRegisterErrorLabel(TextBox validatedControl)
            {
                var errorLabel = new Label
                {
                    Text = "",
                    ForeColor = Color.DarkRed,
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    Margin = new Padding(3, 8, 3, 3)
                };
                _errorLabels[validatedControl] = errorLabel; 
                pnl.Controls.Add(errorLabel, 2, pnl.RowCount - 1); 
            }

            txtWidth = AddRow(pnl, "Genişlik (b, mm):");
            CreateAndRegisterErrorLabel(txtWidth);
            txtWidth.Text = type == "Kolon" ? "300" : "200";

            txtHeight = AddRow(pnl, "Yükseklik (h, mm):");
            CreateAndRegisterErrorLabel(txtHeight);
            txtHeight.Text = type == "Kolon" ? "300" : "300";

            specificControls = new Dictionary<string, Control>();
            if (type == "Kolon")
            {
                var txtCover = AddRow(pnl, "Paspayı (mm):");
                CreateAndRegisterErrorLabel(txtCover);
                pnl.SetColumnSpan(txtCover, 1);
                specificControls.Add("Paspayı (mm):", txtCover);
            }
            else if (type == "Kiriş")
            {
                var txtCoverTop = AddRow(pnl, "Üst Paspayı (mm):");
                CreateAndRegisterErrorLabel(txtCoverTop);

                var txtCoverBottom = AddRow(pnl, "Alt Paspayı (mm):");
                CreateAndRegisterErrorLabel(txtCoverBottom);

                specificControls.Add("Üst Paspayı (mm):", txtCoverTop);
                specificControls.Add("Alt Paspayı (mm):", txtCoverBottom);
            }

            var btnAdd = CreateButton($"{type} Ekle", Color.FromArgb(40, 167, 69), (s, e) => AddSection(type));
            var btnDelete = CreateButton("Seçili Sil", Color.FromArgb(220, 53, 69), BtnDeleteSection_Click);
            pnl.Controls.Add(btnAdd, 3, 0);
            pnl.Controls.Add(btnDelete, 3, 1);

            tabPage.Controls.Add(pnl);
            return tabPage;
        }

        private TabPage BuildSlabTabPage()
        {
            var tabPage = new TabPage("Döşeme") { BackColor = Color.White, Padding = new Padding(10) };
            var pnl = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 4, AutoSize = true, BackColor = Color.White };
            txtSlabName = AddRow(pnl, "Kesit Adı");
            txtSlabName.Text = "D12";
            pnl.SetColumnSpan(txtSlabName, 2);
            cmbSlabMaterial = AddRow(pnl, "Döşeme Malzemesi:", true);
            pnl.SetColumnSpan(cmbSlabMaterial, 2);
            txtSlabThickness = AddRow(pnl, "Kalınlık (mm):");
            txtSlabThickness.Text = "120";
            pnl.SetColumnSpan(txtSlabThickness, 2);
            var btnAdd = CreateButton("Döşeme Ekle", Color.FromArgb(40, 167, 69), (s, e) => AddSection("Döşeme"));
            var btnDelete = CreateButton("Seçili Sil", Color.FromArgb(220, 53, 69), BtnDeleteSection_Click);
            pnl.Controls.Add(btnAdd, 3, 0);
            pnl.Controls.Add(btnDelete, 3, 1);
            tabPage.Controls.Add(pnl);
            return tabPage;
        }

        private Control BuildColumnPlacementTab()
        {
            var mainPanel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 1, ColumnCount = 2, BackColor = Color.White };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            dgvColumnPlacement = new DataGridView { Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left, Height = 148, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false, BackgroundColor = Color.White };
            dgvColumnPlacement.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColumnName", DataPropertyName = "ColumnName", HeaderText = "Kolon Adı" });
            dgvColumnPlacement.Columns.Add(new DataGridViewTextBoxColumn { Name = "X", DataPropertyName = "X", HeaderText = "X (mm)" });
            dgvColumnPlacement.Columns.Add(new DataGridViewTextBoxColumn { Name = "Y", DataPropertyName = "Y", HeaderText = "Y (mm)" });
            dgvColumnPlacement.Columns.Add(new DataGridViewTextBoxColumn { Name = "SectionName", DataPropertyName = "SectionName", HeaderText = "Kesit Adı" });
            mainPanel.Controls.Add(dgvColumnPlacement, 0, 0);
            var rightPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 2,
                Padding = new Padding(10, 0, 0, 0)
            };

            rightPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            var btnAdd = CreateButton("Kolon Ekle", Color.FromArgb(40, 167, 69), BtnAddNewColumn_Click, 120);
            var btnDelete = CreateButton("Seçili Sil", Color.FromArgb(220, 53, 69), BtnDeleteSelectedColumn_Click, 120);
            var buttonFlowPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 10, 0, 0)
            };
            buttonFlowPanel.Controls.Add(btnAdd);
            buttonFlowPanel.Controls.Add(btnDelete);
            rightPanel.Controls.Add(buttonFlowPanel, 0, 0);
            rightPanel.SetColumnSpan(buttonFlowPanel, 2);

            var lblSection = new Label { Text = "Kolon Kesiti:", Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(3, 8, 3, 3) };
            cmbNewColumnplacementsection = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
            rightPanel.Controls.Add(lblSection, 0, 1);
            rightPanel.Controls.Add(cmbNewColumnplacementsection, 1, 1);

            var lblX = new Label { Text = "X (m):", Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(3, 8, 3, 3) };
            numNewColumnX = new NumericUpDown { DecimalPlaces = 2, Minimum = -1000, Maximum = 1000, Dock = DockStyle.Fill };
            rightPanel.Controls.Add(lblX, 0, 2);
            rightPanel.Controls.Add(numNewColumnX, 1, 2);

            var lblY = new Label { Text = "Y (m):", Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(3, 8, 3, 3) };
            numNewColumnY = new NumericUpDown { DecimalPlaces = 2, Minimum = -1000, Maximum = 1000, Dock = DockStyle.Fill };
            rightPanel.Controls.Add(lblY, 0, 3);
            rightPanel.Controls.Add(numNewColumnY, 1, 3);
            mainPanel.Controls.Add(rightPanel, 1, 0);
            return mainPanel;
        }

        private Control BuildBeamPlacementTab()
        {
            var mainPanel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 1, ColumnCount = 2, BackColor = Color.White };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            dgvBeamPlacement = new DataGridView { Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left, Height = 148, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false, BackgroundColor = Color.White };
            dgvBeamPlacement.Columns.Add(new DataGridViewTextBoxColumn { Name = "BeamName", DataPropertyName = "BeamName", HeaderText = "Kiriş Adı" });
            dgvBeamPlacement.Columns.Add(new DataGridViewTextBoxColumn { Name = "StartColumnName", DataPropertyName = "StartColumnName", HeaderText = "Başlangıç Kolonu" });
            dgvBeamPlacement.Columns.Add(new DataGridViewTextBoxColumn { Name = "EndColumnName", DataPropertyName = "EndColumnName", HeaderText = "Bitiş Kolonu" });
            dgvBeamPlacement.Columns.Add(new DataGridViewTextBoxColumn { Name = "SectionName", DataPropertyName = "SectionName", HeaderText = "Kiriş Kesiti" });
            mainPanel.Controls.Add(dgvBeamPlacement, 0, 0);
            var rightPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 2,
                Padding = new Padding(10, 0, 0, 0)
            };

            rightPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var btnAdd = CreateButton("Kiriş Ekle", Color.FromArgb(23, 162, 184), BtnAddNewBeam_Click, 120);
            var btnDelete = CreateButton("Seçili Sil", Color.FromArgb(220, 53, 69), BtnDeleteSelectedBeam_Click, 120);
            var buttonFlowPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 10, 0, 0)
            };
            buttonFlowPanel.Controls.Add(btnAdd);
            buttonFlowPanel.Controls.Add(btnDelete);
            rightPanel.Controls.Add(buttonFlowPanel, 0, 0);
            rightPanel.SetColumnSpan(buttonFlowPanel, 2);

            var lblStartCol = new Label { Text = "Başlangıç Kolonu:", Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(3, 8, 3, 3) };
            cmbBeamStartColumn = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
            rightPanel.Controls.Add(lblStartCol, 0, 1);
            rightPanel.Controls.Add(cmbBeamStartColumn, 1, 1);

            var lblEndCol = new Label { Text = "Bitiş Kolonu:", Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(3, 8, 3, 3) };
            cmbBeamEndColumn = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
            rightPanel.Controls.Add(lblEndCol, 0, 2);
            rightPanel.Controls.Add(cmbBeamEndColumn, 1, 2);

            var lblSection = new Label { Text = "Kiriş Kesiti:", Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(3, 8, 3, 3) };
            cmbBeamSection = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
            rightPanel.Controls.Add(lblSection, 0, 3);
            rightPanel.Controls.Add(cmbBeamSection, 1, 3);
            mainPanel.Controls.Add(rightPanel, 1, 0);
            return mainPanel;
        }

        private void BtnAddMaterialToList_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbMaterialType.SelectedItem == null || !matTypeDisplayMap.ContainsKey(cmbMaterialType.SelectedItem.ToString())) throw new Exception("Malzeme tipi seçiniz.");
                var matType = matTypeDisplayMap[cmbMaterialType.SelectedItem.ToString()];
                var parameters = new Dictionary<string, object> { { "MaterialName", txtMaterialName.Text } };
                IMaterialFactory factory = (matType == eCustomMatType.Rebar) ? (IMaterialFactory)new RebarMaterialFactory() : new ConcreteMaterialFactory();
                if (matType == eCustomMatType.Rebar) { parameters["Fy"] = Convert.ToDouble(txtFy.Text, CultureInfo.InvariantCulture); parameters["Fu"] = Convert.ToDouble(txtFu.Text, CultureInfo.InvariantCulture); }
                else { parameters["Fck"] = Convert.ToDouble(txtFck.Text, CultureInfo.InvariantCulture); }
                var material = factory.createMaterial(parameters);
                if (string.IsNullOrWhiteSpace(material.MaterialName) || _materialsToExport.Any(m => m.MaterialName.Equals(material.MaterialName, StringComparison.OrdinalIgnoreCase))) throw new Exception("Malzeme adı boş olamaz veya bu isimde malzeme zaten var.");
                _materialsToExport.Add(material);
                UpdateUIDataSources();
                txtMaterialName.Clear();
            }
            catch (Exception ex) { MessageBox.Show("Malzeme eklenirken hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void AddSection(string type)
        {
            try
            {
                ISectionProperties section = null;
                string sectionName = "";

                if (type == "Kolon")
                {
                    // Genişlik ve yüksekliği mm'den cm'ye çevirerek kesit adını oluştur
                    if (!double.TryParse(txtColWidth.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double colWidth) ||
                        !double.TryParse(txtColHeight.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double colHeight))
                    {
                        throw new Exception("Kolon genişliği veya yüksekliği geçerli bir sayı olmalıdır.");
                    }
                    int widthCm = (int)Math.Round(colWidth / 10.0);
                    int heightCm = (int)Math.Round(colHeight / 10.0);
                    sectionName = $"S{widthCm}/{heightCm}";

                    section = new ColumnSectionProperties
                    {
                        SectionName = sectionName, // Otomatik oluşturulan adı kullan
                        MaterialName = cmbColConcrete.SelectedItem?.ToString(),
                        RebarMaterialName = cmbColRebar.SelectedItem?.ToString(),
                        Width = colWidth,
                        Depth = colHeight,
                        ConcreteCover = double.Parse(txtColCover.Text, CultureInfo.InvariantCulture)
                    };
                }
                else if (type == "Kiriş")
                {
                    // Genişlik ve yüksekliği mm'den cm'ye çevirerek kesit adını oluştur
                    if (!double.TryParse(txtBeamWidth.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double beamWidth) ||
                        !double.TryParse(txtBeamHeight.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double beamHeight))
                    {
                        throw new Exception("Kiriş genişliği veya yüksekliği geçerli bir sayı olmalıdır.");
                    }
                    int widthCm = (int)Math.Round(beamWidth / 10.0);
                    int heightCm = (int)Math.Round(beamHeight / 10.0);
                    sectionName = $"K{widthCm}/{heightCm}";

                    section = new BeamSectionProperties
                    {
                        SectionName = sectionName, // Otomatik oluşturulan adı kullan
                        MaterialName = cmbBeamConcrete.SelectedItem?.ToString(),
                        RebarMaterialName = cmbBeamRebar.SelectedItem?.ToString(),
                        Width = beamWidth,
                        Depth = beamHeight,
                        CoverTop = double.Parse(txtBeamCoverTop.Text, CultureInfo.InvariantCulture),
                        CoverBottom = double.Parse(txtBeamCoverBottom.Text, CultureInfo.InvariantCulture)
                    };
                }
                else if (type == "Döşeme")
                {
                    sectionName = txtSlabName.Text; // Döşeme için mevcut adı kullan
                    section = new SlabSectionProperties
                    {
                        SectionName = sectionName,
                        SlabMaterialName = cmbSlabMaterial.SelectedItem?.ToString(),
                        Thickness = double.Parse(txtSlabThickness.Text, CultureInfo.InvariantCulture)
                    };
                }

                // Ortak kontroller
                if (section == null || string.IsNullOrWhiteSpace(section.SectionName))
                {
                    throw new Exception("Kesit adı boş olamaz veya kesit bilgileri eksik/hatalı.");
                }

                if (_sectionsToExport.Any(s => s.SectionName.Equals(section.SectionName, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new Exception($"Bu isimde ({section.SectionName}) bir kesit zaten var.");
                }

                // Malzeme seçimi kontrolü
                if (section is ColumnSectionProperties colProps && (string.IsNullOrWhiteSpace(colProps.MaterialName) || string.IsNullOrWhiteSpace(colProps.RebarMaterialName)))
                {
                    throw new Exception("Kolon için beton ve donatı malzemesi seçilmelidir.");
                }
                else if (section is BeamSectionProperties beamProps && (string.IsNullOrWhiteSpace(beamProps.MaterialName) || string.IsNullOrWhiteSpace(beamProps.RebarMaterialName)))
                {
                    throw new Exception("Kiriş için beton ve donatı malzemesi seçilmelidir.");
                }
                else if (section is SlabSectionProperties slabProps && string.IsNullOrWhiteSpace(slabProps.SlabMaterialName))
                {
                    throw new Exception("Döşeme için malzeme seçilmelidir.");
                }

                _sectionsToExport.Add(section);
                UpdateUIDataSources();
                // txtColName, txtBeamName, txtSlabName'i temizlemeye gerek yok, çünkü otomatik oluşturuluyorlar veya kullanıcı girmeli.
                // İsterseniz otomatik oluşturulan isimleri txtName.Text'e yansıtabilirsiniz.
                if (type == "Kolon") txtColName.Text = sectionName;
                else if (type == "Kiriş") txtBeamName.Text = sectionName;
                // Döşeme için txtSlabName.Text zaten kullanılıyor.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{type} bilgileri hatalı veya eksik.\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeleteSection_Click(object sender, EventArgs e)
        {
            if (dgvAddedsections.SelectedRows.Count == 0) return;
            var namesToDelete = dgvAddedsections.SelectedRows.Cast<DataGridViewRow>().Select(r => r.Cells["SectionName"].Value?.ToString()).Where(name => name != null).ToList();
            _sectionsToExport.RemoveAll(s => namesToDelete.Contains(s.SectionName));
            UpdateUIDataSources();
        }

        private void BtnAddNewColumn_Click(object sender, EventArgs e)
        {
            if (cmbNewColumnplacementsection.SelectedItem == null) { MessageBox.Show("Lütfen bir kesit seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            string newColumnName = $"S{_nextColumnId++}";
            var placement = new ColumnPlacementInfo
            {
                ColumnName = newColumnName,
                X = (double)numNewColumnX.Value * 1000,
                Y = (double)numNewColumnY.Value * 1000,
                SectionName = cmbNewColumnplacementsection.SelectedItem.ToString()
            };
            _columnplacements.Add(placement);
            UpdateUIDataSources();
        }

        private void BtnDeleteSelectedColumn_Click(object sender, EventArgs e)
        {
            if (dgvColumnPlacement.SelectedRows.Count == 0) return;
            var selectedName = dgvColumnPlacement.SelectedRows[0].Cells["ColumnName"].Value.ToString();
            if (_beamplacements.Any(b => b.StartColumnName == selectedName || b.EndColumnName == selectedName)) { MessageBox.Show("Bu kolon bir kirişe bağlı olduğu için silinemez.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            var itemToRemove = _columnplacements.FirstOrDefault(c => c.ColumnName == selectedName);
            if (itemToRemove != null) { _columnplacements.Remove(itemToRemove); UpdateUIDataSources(); }
        }

        private void BtnAddNewBeam_Click(object sender, EventArgs e)
        {
            if (cmbBeamStartColumn.SelectedItem == null || cmbBeamEndColumn.SelectedItem == null || cmbBeamSection.SelectedItem == null) { MessageBox.Show("Lütfen başlangıç, bitiş kolonu ve kesit seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            var startCol = cmbBeamStartColumn.SelectedItem.ToString();
            var endCol = cmbBeamEndColumn.SelectedItem.ToString();
            if (startCol == endCol) { MessageBox.Show("Başlangıç ve bitiş kolonları aynı olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            string newBeamName = $"K{_nextBeamId++}";
            var placement = new BeamPlacementInfo { BeamName = newBeamName, StartColumnName = startCol, EndColumnName = endCol, SectionName = cmbBeamSection.SelectedItem.ToString() };
            _beamplacements.Add(placement);
            UpdateUIDataSources();
        }

        private void BtnDeleteSelectedBeam_Click(object sender, EventArgs e)
        {
            if (dgvBeamPlacement.SelectedRows.Count > 0 && dgvBeamPlacement.SelectedRows[0].DataBoundItem is BeamPlacementInfo info)
            { _beamplacements.Remove(info); UpdateUIDataSources(); }
        }

        private void UpdateUIDataSources() { UpdateMaterialLists(); UpdateSectionLists(); UpdatePlacementGridsAndInputs(); }
        private void UpdateMaterialLists()
        {
            var concreteMaterials = _materialsToExport.OfType<ConcreteMaterialProperties>().Select(m => m.MaterialName).ToArray();
            var rebarMaterials = _materialsToExport.OfType<RebarMaterialProperties>().Select(m => m.MaterialName).ToArray();
            UpdateCombo(cmbColConcrete, concreteMaterials); UpdateCombo(cmbBeamConcrete, concreteMaterials); UpdateCombo(cmbSlabMaterial, concreteMaterials);
            UpdateCombo(cmbColRebar, rebarMaterials); UpdateCombo(cmbBeamRebar, rebarMaterials);
            cmbMaterials.Items.Clear();
            var materialDescriptions = _materialsToExport.Select(m => $"{EnumHelper.getDescription(m.MaterialType)} - {m.MaterialName}").ToArray();
            if (materialDescriptions.Any()) { cmbMaterials.Items.AddRange(materialDescriptions); cmbMaterials.SelectedIndex = 0; }
        }

        private void UpdateSectionLists()
        {
            UpdateAddedsectionsGrid();
            var columnSectionNames = _sectionsToExport.OfType<ColumnSectionProperties>().Select(cs => cs.SectionName).ToArray();
            UpdateCombo(cmbNewColumnplacementsection, columnSectionNames);
            var beamSectionNames = _sectionsToExport.OfType<BeamSectionProperties>().Select(bs => bs.SectionName).ToArray();
            UpdateCombo(cmbBeamSection, beamSectionNames);
        }

        private void UpdateAddedsectionsGrid()
        {
            dgvAddedsections.Rows.Clear();
            foreach (var section in _sectionsToExport)
            {
                string type = "Bilinmeyen", dims = "-", mat = "-", rebar = "N/A";
                if (section is ColumnSectionProperties col) { type = "Kolon"; dims = $"{col.Depth}/{col.Width}"; mat = col.MaterialName; rebar = col.RebarMaterialName; }
                else if (section is BeamSectionProperties beam) { type = "Kiriş"; dims = $"{beam.Depth}/{beam.Width}"; mat = beam.MaterialName; rebar = beam.RebarMaterialName; }
                else if (section is SlabSectionProperties slab) { type = "Döşeme"; dims = $"Kalınlık: {slab.Thickness}"; mat = slab.SlabMaterialName; }
                dgvAddedsections.Rows.Add(section.SectionName, type, mat, rebar, dims);
            }
        }

        private void UpdatePlacementGridsAndInputs()
        {
            dgvColumnPlacement.DataSource = null; if (_columnplacements.Any()) dgvColumnPlacement.DataSource = new BindingSource { DataSource = _columnplacements.ToList() };
            dgvBeamPlacement.DataSource = null; if (_beamplacements.Any()) dgvBeamPlacement.DataSource = new BindingSource { DataSource = _beamplacements.ToList() };
            var columnNames = _columnplacements.Select(c => c.ColumnName).ToArray();
            UpdateCombo(cmbBeamStartColumn, columnNames); UpdateCombo(cmbBeamEndColumn, columnNames);
        }

        private void LoadMaterialTypes()
        {
            cmbMaterialType.Items.Clear(); matTypeDisplayMap.Clear();
            foreach (eCustomMatType type in Enum.GetValues(typeof(eCustomMatType))) { string desc = EnumHelper.getDescription(type); cmbMaterialType.Items.Add(desc); matTypeDisplayMap[desc] = type; }
            if (cmbMaterialType.Items.Count > 0) cmbMaterialType.SelectedIndex = 0;
        }

        private void LoadDefaultMaterials()
        {
            _materialsToExport.Clear(); _sectionsToExport.Clear();
            _materialsToExport.AddRange(new List<IMaterialProperties> { new ConcreteMaterialProperties { MaterialName = "C30/37", Fck = 30 }, new RebarMaterialProperties { MaterialName = "B420C", Fy = 420, Fu = 550 } });
            UpdateUIDataSources();
        }


        private T AddRow<T>(TableLayoutPanel pnl, string labelText, out Label label, bool isComboBox = false) where T : Control, new()
        {
            pnl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            label = new Label { Text = labelText, Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(3, 8, 3, 3), Width=100 };
            var control = new T() { Margin = new Padding(3, 6, 3, 3), Width=150};
            if (isComboBox && control is ComboBox cmb) cmb.DropDownStyle = ComboBoxStyle.DropDownList;
            int nextRow = pnl.RowCount++;
            pnl.Controls.Add(label, 0, nextRow); pnl.Controls.Add(control, 1, nextRow);
            return control;
        }

        private T AddRow<T>(TableLayoutPanel pnl, string labelText, bool isComboBox = false) where T : Control, new() => AddRow<T>(pnl, labelText, out _, isComboBox);
        private TextBox AddRow(TableLayoutPanel pnl, string labelText) => AddRow<TextBox>(pnl, labelText, out _);
        private TextBox AddRow(TableLayoutPanel pnl, string labelText, out Label label) => AddRow<TextBox>(pnl, labelText, out label);
        private ComboBox AddRow(TableLayoutPanel pnl, string labelText, bool isComboBox) => AddRow<ComboBox>(pnl, labelText, out _, isComboBox);

        private T AddControlToPanel<T>(TableLayoutPanel panel, string labelText, T control) where T : Control
        {
            var label = new Label { Text = labelText, Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(3, 8, 3, 3) };
            control.Margin = new Padding(3, 6, 3, 3); control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            int nextRow = panel.RowCount; panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.Controls.Add(label, 0, nextRow); panel.Controls.Add(control, 1, nextRow);
            panel.RowCount++; return control;
        }

        private Button CreateButton(string text, Color color, EventHandler click, int width = 120, int height = 35)
        {
            var btn = new Button { Text = text, Width = width, Height = height, FlatStyle = FlatStyle.Flat, BackColor = color, ForeColor = Color.White, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Margin = new Padding(5) };
            btn.FlatAppearance.BorderSize = 0; btn.Click += click; return btn;
        }

        private void UpdateCombo(ComboBox cmb, string[] items)
        {
            var selected = cmb.SelectedItem?.ToString();
            cmb.DataSource = null; cmb.Items.Clear();
            if (items != null && items.Any())
            {
                cmb.Items.AddRange(items);
                if (selected != null && cmb.Items.Contains(selected)) cmb.SelectedItem = selected;
                else if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
            }
        }
    }
}
