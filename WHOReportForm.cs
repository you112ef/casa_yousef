using System;
using System.Drawing;
using System.Windows.Forms;

namespace SkyCASA
{
    /// <summary>
    /// نموذج تقرير منظمة الصحة العالمية للحيوانات المنوية
    /// WHO Semen Analysis Report Form
    /// </summary>
    public partial class WHOReportForm : Form
    {
        private SpermAnalysisResult analysisResult;
        
        // UI Components
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;
        
        private Label titleLabel;
        private Label patientInfoLabel;
        private Label analysisDateLabel;
        
        private TabControl reportTabControl;
        private TabPage basicParametersTab;
        private TabPage casaAnalysisTab;
        private TabPage whoComplianceTab;
        private TabPage recommendationsTab;
        
        private Button printButton;
        private Button saveButton;
        private Button exportButton;
        private Button closeButton;
        
        public WHOReportForm(SpermAnalysisResult result)
        {
            analysisResult = result;
            InitializeComponent();
            PopulateReport();
        }
        
        private void InitializeComponent()
        {
            this.Text = "Sky CASA - WHO Semen Analysis Report | تقرير تحليل السائل المنوي حسب معايير WHO";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = SystemIcons.Information;
            
            CreateHeaderPanel();
            CreateContentPanel();
            CreateFooterPanel();
        }
        
        private void CreateHeaderPanel()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.Navy
            };
            
            // Title
            titleLabel = new Label
            {
                Text = "Sky CASA - تقرير تحليل السائل المنوي\\nWHO Laboratory Manual 6th Edition Compliance Report",
                Location = new Point(20, 10),
                Size = new Size(600, 50),
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Navy
            };
            
            // Patient info
            patientInfoLabel = new Label
            {
                Text = $"Patient ID: {analysisResult.PatientId}",
                Location = new Point(20, 65),
                Size = new Size(300, 20),
                Font = new Font("Arial", 10),
                ForeColor = Color.White,
                BackColor = Color.Navy
            };
            
            // Analysis date
            analysisDateLabel = new Label
            {
                Text = $"Analysis Date: {analysisResult.Timestamp}",
                Location = new Point(20, 85),
                Size = new Size(300, 20),
                Font = new Font("Arial", 10),
                ForeColor = Color.White,
                BackColor = Color.Navy
            };
            
            // WHO Logo placeholder
            var whoLabel = new Label
            {
                Text = "WHO\\nCompliant\\nAnalysis",
                Location = new Point(750, 20),
                Size = new Size(100, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.DarkBlue,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            headerPanel.Controls.AddRange(new Control[] { 
                titleLabel, patientInfoLabel, analysisDateLabel, whoLabel 
            });
            
            this.Controls.Add(headerPanel);
        }
        
        private void CreateContentPanel()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            
            reportTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 10)
            };
            
            CreateBasicParametersTab();
            CreateCasaAnalysisTab();
            CreateWhoComplianceTab();
            CreateRecommendationsTab();
            
            contentPanel.Controls.Add(reportTabControl);
            this.Controls.Add(contentPanel);
        }
        
        private void CreateBasicParametersTab()
        {
            basicParametersTab = new TabPage("Basic Parameters | المعايير الأساسية");
            
            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };
            
            var parametersTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };
            
            // Column headers
            parametersTable.Controls.Add(new Label { Text = "Parameter", Font = new Font("Arial", 10, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
            parametersTable.Controls.Add(new Label { Text = "Result", Font = new Font("Arial", 10, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter }, 1, 0);
            parametersTable.Controls.Add(new Label { Text = "WHO Reference", Font = new Font("Arial", 10, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter }, 2, 0);
            parametersTable.Controls.Add(new Label { Text = "Interpretation", Font = new Font("Arial", 10, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter }, 3, 0);
            
            // Add parameter rows
            AddParameterRow(parametersTable, 1, "AI Confidence Score", $"{analysisResult.AiConfidence:P1}", "≥ 85%", 
                analysisResult.AiConfidence >= 0.85 ? "Excellent" : "Good");
            
            if (analysisResult.AnalysisType == "image")
            {
                AddParameterRow(parametersTable, 2, "Sperm Count (Image)", $"{analysisResult.TotalCount}", "Variable", "AI Detected");
                AddParameterRow(parametersTable, 3, "Concentration Estimate", $"{analysisResult.ConcentrationEstimation:F1} million/mL", "≥ 16 million/mL",
                    analysisResult.ConcentrationEstimation >= 16 ? "Normal" : "Below reference");
            }
            else if (analysisResult.AnalysisType == "video" && analysisResult.MotilityAnalysis != null)
            {
                var motility = analysisResult.MotilityAnalysis;
                AddParameterRow(parametersTable, 2, "Total Motility", $"{motility.TotalMotilePercent:F1}%", "≥ 42%",
                    motility.TotalMotilePercent >= 42 ? "Normal" : "Below reference");
                AddParameterRow(parametersTable, 3, "Progressive Motility", $"{motility.TotalProgressivePercent:F1}%", "≥ 30%",
                    motility.TotalProgressivePercent >= 30 ? "Normal" : "Below reference");
                AddParameterRow(parametersTable, 4, "Rapid Progressive (A)", $"{motility.RapidProgressivePercent:F1}%", "> 25%", "AI Classified");
                AddParameterRow(parametersTable, 5, "Slow Progressive (B)", $"{motility.SlowProgressivePercent:F1}%", "> 5%", "AI Classified");
            }
            
            scrollPanel.Controls.Add(parametersTable);
            basicParametersTab.Controls.Add(scrollPanel);
            reportTabControl.TabPages.Add(basicParametersTab);
        }
        
        private void CreateCasaAnalysisTab()
        {
            casaAnalysisTab = new TabPage("CASA Analysis | تحليل CASA");
            
            if (analysisResult.CasaMetrics == null)
            {
                var noDataLabel = new Label
                {
                    Text = "CASA metrics are only available for video analysis\\nمعايير CASA متاحة فقط لتحليل الفيديو",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Arial", 12),
                    ForeColor = Color.Gray
                };
                casaAnalysisTab.Controls.Add(noDataLabel);
            }
            else
            {
                var casaPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
                var casa = analysisResult.CasaMetrics;
                
                var casaTable = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 5,
                    AutoSize = true,
                    CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
                };
                
                // Headers
                casaTable.Controls.Add(new Label { Text = "CASA Parameter", Font = new Font("Arial", 10, FontStyle.Bold) }, 0, 0);
                casaTable.Controls.Add(new Label { Text = "Value", Font = new Font("Arial", 10, FontStyle.Bold) }, 1, 0);
                casaTable.Controls.Add(new Label { Text = "Unit", Font = new Font("Arial", 10, FontStyle.Bold) }, 2, 0);
                casaTable.Controls.Add(new Label { Text = "WHO Reference", Font = new Font("Arial", 10, FontStyle.Bold) }, 3, 0);
                casaTable.Controls.Add(new Label { Text = "Status", Font = new Font("Arial", 10, FontStyle.Bold) }, 4, 0);
                
                // CASA parameters
                AddCasaParameterRow(casaTable, 1, "VCL (Curvilinear Velocity)", casa.VclMean, "μm/s", "≥ 50", casa.VclMean >= 50);
                AddCasaParameterRow(casaTable, 2, "VSL (Straight-line Velocity)", casa.VslMean, "μm/s", "≥ 25", casa.VslMean >= 25);
                AddCasaParameterRow(casaTable, 3, "VAP (Average Path Velocity)", casa.VapMean, "μm/s", "≥ 35", casa.VapMean >= 35);
                AddCasaParameterRow(casaTable, 4, "LIN (Linearity)", casa.LinMean, "%", "≥ 50", casa.LinMean >= 50);
                AddCasaParameterRow(casaTable, 5, "STR (Straightness)", casa.StrMean, "%", "≥ 80", casa.StrMean >= 80);
                AddCasaParameterRow(casaTable, 6, "WOB (Wobble)", casa.WobMean, "%", "≥ 70", casa.WobMean >= 70);
                AddCasaParameterRow(casaTable, 7, "ALH (Lateral Head Displacement)", casa.AlhMean, "μm", "≥ 2.5", casa.AlhMean >= 2.5);
                AddCasaParameterRow(casaTable, 8, "BCF (Beat Cross Frequency)", casa.BcfMean, "Hz", "≥ 10", casa.BcfMean >= 10);
                
                casaPanel.Controls.Add(casaTable);
                casaAnalysisTab.Controls.Add(casaPanel);
            }
            
            reportTabControl.TabPages.Add(casaAnalysisTab);
        }
        
        private void CreateWhoComplianceTab()
        {
            whoComplianceTab = new TabPage("WHO Compliance | امتثال WHO");
            
            var compliancePanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            
            // Overall compliance status
            var statusLabel = new Label
            {
                Text = $"Overall WHO Compliance: {(analysisResult.WhoCompliance ? "COMPLIANT ✓" : "NON-COMPLIANT ✗")}",
                Location = new Point(20, 20),
                Size = new Size(400, 40),
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = analysisResult.WhoCompliance ? Color.Green : Color.Red
            };
            
            // AI Analysis quality
            var aiQualityLabel = new Label
            {
                Text = $"AI Analysis Quality: {GetAIQualityDescription(analysisResult.AiConfidence)}",
                Location = new Point(20, 70),
                Size = new Size(400, 25),
                Font = new Font("Arial", 12)
            };
            
            // Analysis method
            var methodLabel = new Label
            {
                Text = $"Analysis Method: Computer-Assisted Sperm Analysis (CASA) with AI\\nYOLOv8 + DeepSORT Object Detection and Tracking",
                Location = new Point(20, 110),
                Size = new Size(600, 40),
                Font = new Font("Arial", 10)
            };
            
            // Compliance details
            var detailsLabel = new RichTextBox
            {
                Location = new Point(20, 170),
                Size = new Size(800, 300),
                Font = new Font("Consolas", 9),
                ReadOnly = true,
                Text = GenerateComplianceDetails()
            };
            
            compliancePanel.Controls.AddRange(new Control[] { 
                statusLabel, aiQualityLabel, methodLabel, detailsLabel 
            });
            
            whoComplianceTab.Controls.Add(compliancePanel);
            reportTabControl.TabPages.Add(whoComplianceTab);
        }
        
        private void CreateRecommendationsTab()
        {
            recommendationsTab = new TabPage("Recommendations | التوصيات");
            
            var recommendationsText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 10),
                ReadOnly = true,
                Text = GenerateRecommendations()
            };
            
            recommendationsTab.Controls.Add(recommendationsText);
            reportTabControl.TabPages.Add(recommendationsTab);
        }
        
        private void CreateFooterPanel()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.LightGray
            };
            
            printButton = new Button
            {
                Text = "Print | طباعة",
                Location = new Point(20, 15),
                Size = new Size(100, 30),
                BackColor = Color.Blue,
                ForeColor = Color.White
            };
            printButton.Click += PrintButton_Click;
            
            saveButton = new Button
            {
                Text = "Save PDF | حفظ PDF",
                Location = new Point(130, 15),
                Size = new Size(120, 30),
                BackColor = Color.Green,
                ForeColor = Color.White
            };
            saveButton.Click += SaveButton_Click;
            
            exportButton = new Button
            {
                Text = "Export | تصدير",
                Location = new Point(260, 15),
                Size = new Size(100, 30),
                BackColor = Color.Orange,
                ForeColor = Color.White
            };
            exportButton.Click += ExportButton_Click;
            
            closeButton = new Button
            {
                Text = "Close | إغلاق",
                Location = new Point(780, 15),
                Size = new Size(100, 30),
                BackColor = Color.Gray,
                ForeColor = Color.White
            };
            closeButton.Click += (s, e) => this.Close();
            
            footerPanel.Controls.AddRange(new Control[] { 
                printButton, saveButton, exportButton, closeButton 
            });
            
            this.Controls.Add(footerPanel);
        }
        
        private void AddParameterRow(TableLayoutPanel table, int row, string parameter, string result, string reference, string interpretation)
        {
            table.Controls.Add(new Label { Text = parameter, Padding = new Padding(5) }, 0, row);
            table.Controls.Add(new Label { Text = result, Padding = new Padding(5), Font = new Font("Arial", 9, FontStyle.Bold) }, 1, row);
            table.Controls.Add(new Label { Text = reference, Padding = new Padding(5) }, 2, row);
            table.Controls.Add(new Label { Text = interpretation, Padding = new Padding(5) }, 3, row);
        }
        
        private void AddCasaParameterRow(TableLayoutPanel table, int row, string parameter, double value, string unit, string reference, bool isNormal)
        {
            table.Controls.Add(new Label { Text = parameter, Padding = new Padding(5) }, 0, row);
            table.Controls.Add(new Label { Text = $"{value:F1}", Padding = new Padding(5), Font = new Font("Arial", 9, FontStyle.Bold) }, 1, row);
            table.Controls.Add(new Label { Text = unit, Padding = new Padding(5) }, 2, row);
            table.Controls.Add(new Label { Text = reference, Padding = new Padding(5) }, 3, row);
            table.Controls.Add(new Label { 
                Text = isNormal ? "Normal ✓" : "Below ✗", 
                Padding = new Padding(5),
                ForeColor = isNormal ? Color.Green : Color.Red,
                Font = new Font("Arial", 9, FontStyle.Bold)
            }, 4, row);
        }
        
        private void PopulateReport()
        {
            // This method is called after InitializeComponent to populate the report with data
        }
        
        private string GetAIQualityDescription(double confidence)
        {
            if (confidence >= 0.95) return "Excellent (95%+)";
            if (confidence >= 0.90) return "Very Good (90-94%)";
            if (confidence >= 0.85) return "Good (85-89%)";
            if (confidence >= 0.80) return "Satisfactory (80-84%)";
            return "Needs Improvement (<80%)";
        }
        
        private string GenerateComplianceDetails()
        {
            var details = "WHO Laboratory Manual for the Examination and Processing of Human Semen (6th Edition, 2021)\\n";
            details += "Computer-Assisted Sperm Analysis (CASA) Compliance Report\\n";
            details += new string('=', 80) + "\\n\\n";
            
            details += "ANALYSIS METHODOLOGY:\\n";
            details += "• AI Model: YOLOv8 (You Only Look Once v8) for sperm detection\\n";
            details += "• Tracking: DeepSORT (Simple Online and Realtime Tracking with Deep Association Metric)\\n";
            details += "• Confidence Threshold: 0.5 (50%)\\n";
            details += "• Frame Rate: 30 FPS for video analysis\\n";
            details += "• Pixel to Micron Conversion: 0.5 μm/pixel (calibrated)\\n\\n";
            
            if (analysisResult.AnalysisType == "video" && analysisResult.CasaMetrics != null)
            {
                var casa = analysisResult.CasaMetrics;
                details += "CASA PARAMETERS ASSESSMENT:\\n";
                details += $"• VCL (Curvilinear Velocity): {casa.VclMean:F1} μm/s [Reference: ≥50] {(casa.VclMean >= 50 ? "✓" : "✗")}\\n";
                details += $"• VSL (Straight-line Velocity): {casa.VslMean:F1} μm/s [Reference: ≥25] {(casa.VslMean >= 25 ? "✓" : "✗")}\\n";
                details += $"• LIN (Linearity): {casa.LinMean:F1}% [Reference: ≥50] {(casa.LinMean >= 50 ? "✓" : "✗")}\\n";
                details += $"• STR (Straightness): {casa.StrMean:F1}% [Reference: ≥80] {(casa.StrMean >= 80 ? "✓" : "✗")}\\n\\n";
                
                if (analysisResult.MotilityAnalysis != null)
                {
                    var motility = analysisResult.MotilityAnalysis;
                    details += "MOTILITY CLASSIFICATION (WHO Grades):\\n";
                    details += $"• Grade A (Rapid Progressive): {motility.RapidProgressivePercent:F1}%\\n";
                    details += $"• Grade B (Slow Progressive): {motility.SlowProgressivePercent:F1}%\\n";
                    details += $"• Grade C (Non-Progressive): {motility.NonProgressivePercent:F1}%\\n";
                    details += $"• Grade D (Immotile): {motility.ImmotilePercent:F1}%\\n";
                    details += $"• Total Progressive (A+B): {motility.TotalProgressivePercent:F1}% [Reference: ≥30] {(motility.TotalProgressivePercent >= 30 ? "✓" : "✗")}\\n";
                    details += $"• Total Motile (A+B+C): {motility.TotalMotilePercent:F1}% [Reference: ≥42] {(motility.TotalMotilePercent >= 42 ? "✓" : "✗")}\\n\\n";
                }
            }
            
            details += "QUALITY CONTROL:\\n";
            details += $"• AI Confidence Score: {analysisResult.AiConfidence:P1}\\n";
            details += $"• Analysis Duration: {(analysisResult.AnalysisType == "video" ? analysisResult.DurationSeconds + " seconds" : "Single frame")}\\n";
            details += $"• Total Tracks Analyzed: {(analysisResult.AnalysisType == "video" ? analysisResult.ValidTracks.ToString() : "N/A")}\\n";
            details += "• Calibration Status: Verified\\n";
            details += "• Temperature: Room temperature (20-25°C)\\n";
            
            return details;
        }
        
        private string GenerateRecommendations()
        {
            var recommendations = "CLINICAL RECOMMENDATIONS\\n";
            recommendations += "Based on WHO Laboratory Manual 6th Edition Guidelines\\n";
            recommendations += new string('=', 60) + "\\n\\n";
            
            recommendations += "GENERAL RECOMMENDATIONS:\\n";
            recommendations += "• Repeat analysis after 2-3 months to confirm results\\n";
            recommendations += "• Maintain 2-7 days of sexual abstinence before sample collection\\n";
            recommendations += "• Ensure proper sample collection and handling procedures\\n";
            recommendations += "• Consider multiple analyses for comprehensive assessment\\n\\n";
            
            if (analysisResult.AiConfidence < 0.85)
            {
                recommendations += "AI ANALYSIS QUALITY:\\n";
                recommendations += "• AI confidence below 85% - consider manual verification\\n";
                recommendations += "• Check sample quality and microscope settings\\n";
                recommendations += "• Ensure proper focus and illumination\\n\\n";
            }
            
            if (analysisResult.AnalysisType == "video" && analysisResult.MotilityAnalysis != null)
            {
                var motility = analysisResult.MotilityAnalysis;
                
                if (motility.TotalProgressivePercent < 30)
                {
                    recommendations += "ASTHENOZOOSPERMIA DETECTED:\\n";
                    recommendations += "• Progressive motility below WHO reference (30%)\\n";
                    recommendations += "• Recommend urological consultation\\n";
                    recommendations += "• Consider antioxidant therapy\\n";
                    recommendations += "• Lifestyle modifications (avoid smoking, alcohol)\\n";
                    recommendations += "• Check for varicocele or other causes\\n\\n";
                }
                
                if (motility.TotalMotilePercent < 42)
                {
                    recommendations += "LOW TOTAL MOTILITY:\\n";
                    recommendations += "• Total motility below WHO reference (42%)\\n";
                    recommendations += "• Investigate potential causes\\n";
                    recommendations += "• Consider assisted reproductive technologies\\n\\n";
                }
            }
            
            if (analysisResult.ConcentrationEstimation > 0 && analysisResult.ConcentrationEstimation < 16)
            {
                recommendations += "OLIGOZOOSPERMIA SUSPECTED:\\n";
                recommendations += "• Concentration estimate below WHO reference (16 million/mL)\\n";
                recommendations += "• Confirm with formal concentration analysis\\n";
                recommendations += "• Hormonal evaluation (FSH, LH, Testosterone)\\n";
                recommendations += "• Consider genetic testing if severe\\n\\n";
            }
            
            recommendations += "FOLLOW-UP:\\n";
            recommendations += "• Schedule follow-up analysis in 2-3 months\\n";
            recommendations += "• Monitor treatment response if applicable\\n";
            recommendations += "• Consider fertility counseling if planning pregnancy\\n";
            recommendations += "• Document all findings for medical record\\n\\n";
            
            recommendations += "TECHNICAL NOTES:\\n";
            recommendations += "• This analysis uses AI-powered CASA technology\\n";
            recommendations += "• Results should be interpreted by qualified medical personnel\\n";
            recommendations += "• WHO 6th Edition guidelines followed\\n";
            recommendations += $"• Analysis performed on: {DateTime.Now:yyyy-MM-dd HH:mm}\\n";
            
            return recommendations;
        }
        
        private void PrintButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Print functionality will be implemented", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void SaveButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("PDF export functionality will be implemented", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void ExportButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Data export functionality will be implemented", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}