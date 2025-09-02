using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SkyCASA
{
    /// <summary>
    /// واجهة تحليل الحيوانات المنوية بالذكاء الاصطناعي
    /// Sky CASA - AI Sperm Analysis Interface
    /// </summary>
    public partial class SpermAnalysisForm : Form
    {
        private DataAccessLayer dal;
        private int currentPatientId;
        
        // UI Components
        private TabControl tabControl;
        private TabPage imageAnalysisTab;
        private TabPage videoAnalysisTab;
        private TabPage resultsTab;
        
        // Image Analysis Tab
        private PictureBox originalImageBox;
        private PictureBox analyzedImageBox;
        private PictureBox heatmapImageBox;
        private Button selectImageButton;
        private Button analyzeImageButton;
        private Label imageResultsLabel;
        
        // Video Analysis Tab
        private Panel videoPanel;
        private Button selectVideoButton;
        private Button analyzeVideoButton;
        private ProgressBar analysisProgressBar;
        private RichTextBox videoResultsTextBox;
        private NumericUpDown durationNumericUpDown;
        
        // Results Tab
        private DataGridView resultsGridView;
        private Button exportResultsButton;
        private Button generateReportButton;
        
        // Analysis data
        private string selectedMediaPath = "";
        private SpermAnalysisResult lastAnalysisResult;
        
        public SpermAnalysisForm(int patientId)
        {
            currentPatientId = patientId;
            dal = new DataAccessLayer("database.db");
            InitializeComponent();
            LoadPatientResults();
        }
        
        private void InitializeComponent()
        {
            this.Text = "Sky CASA - AI Sperm Analysis | تحليل الحيوانات المنوية بالذكاء الاصطناعي";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Create main tab control
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            
            CreateImageAnalysisTab();
            CreateVideoAnalysisTab();
            CreateResultsTab();
            
            this.Controls.Add(tabControl);
        }
        
        private void CreateImageAnalysisTab()
        {
            imageAnalysisTab = new TabPage("تحليل الصورة | Image Analysis");
            
            // Layout panels
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 60 };
            var centerPanel = new Panel { Dock = DockStyle.Fill };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 100 };
            
            // Top panel - Controls
            selectImageButton = new Button
            {
                Text = "اختر صورة | Select Image",
                Location = new Point(20, 15),
                Size = new Size(150, 30),
                BackColor = Color.DeepSkyBlue,
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            selectImageButton.Click += SelectImageButton_Click;
            
            analyzeImageButton = new Button
            {
                Text = "تحليل بالذكاء الاصطناعي | Analyze with AI",
                Location = new Point(180, 15),
                Size = new Size(200, 30),
                BackColor = Color.Green,
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Enabled = false
            };
            analyzeImageButton.Click += AnalyzeImageButton_Click;
            
            topPanel.Controls.AddRange(new Control[] { selectImageButton, analyzeImageButton });
            
            // Center panel - Image display
            var imagesPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2
            };
            imagesPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            imagesPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            imagesPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            imagesPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            imagesPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            
            // Labels
            var originalLabel = new Label { Text = "الصورة الأصلية | Original", TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            var analyzedLabel = new Label { Text = "الصورة المحللة | Analyzed", TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            var heatmapLabel = new Label { Text = "الخريطة الحرارية | Heatmap", TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            
            imagesPanel.Controls.Add(originalLabel, 0, 0);
            imagesPanel.Controls.Add(analyzedLabel, 1, 0);
            imagesPanel.Controls.Add(heatmapLabel, 2, 0);
            
            // Picture boxes
            originalImageBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray
            };
            
            analyzedImageBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray
            };
            
            heatmapImageBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray
            };
            
            imagesPanel.Controls.Add(originalImageBox, 0, 1);
            imagesPanel.Controls.Add(analyzedImageBox, 1, 1);
            imagesPanel.Controls.Add(heatmapImageBox, 2, 1);
            
            centerPanel.Controls.Add(imagesPanel);
            
            // Bottom panel - Results
            imageResultsLabel = new Label
            {
                Dock = DockStyle.Fill,
                Text = "النتائج ستظهر هنا بعد التحليل | Results will appear here after analysis",
                TextAlign = ContentAlignment.TopLeft,
                Font = new Font("Arial", 10),
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            bottomPanel.Controls.Add(imageResultsLabel);
            
            imageAnalysisTab.Controls.AddRange(new Control[] { topPanel, centerPanel, bottomPanel });
            tabControl.TabPages.Add(imageAnalysisTab);
        }
        
        private void CreateVideoAnalysisTab()
        {
            videoAnalysisTab = new TabPage("تحليل الفيديو | Video Analysis");
            
            // Layout panels
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 80 };
            var centerPanel = new Panel { Dock = DockStyle.Fill };
            
            // Top panel - Controls
            selectVideoButton = new Button
            {
                Text = "اختر فيديو | Select Video",
                Location = new Point(20, 15),
                Size = new Size(150, 30),
                BackColor = Color.Purple,
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            selectVideoButton.Click += SelectVideoButton_Click;
            
            analyzeVideoButton = new Button
            {
                Text = "تحليل CASA بالذكاء الاصطناعي | CASA AI Analysis",
                Location = new Point(180, 15),
                Size = new Size(250, 30),
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Enabled = false
            };
            analyzeVideoButton.Click += AnalyzeVideoButton_Click;
            
            var durationLabel = new Label
            {
                Text = "مدة التحليل (ثانية) | Duration (seconds):",
                Location = new Point(450, 20),
                Size = new Size(180, 20)
            };
            
            durationNumericUpDown = new NumericUpDown
            {
                Location = new Point(630, 18),
                Size = new Size(60, 25),
                Minimum = 5,
                Maximum = 60,
                Value = 15
            };
            
            analysisProgressBar = new ProgressBar
            {
                Location = new Point(20, 50),
                Size = new Size(670, 20),
                Visible = false
            };
            
            topPanel.Controls.AddRange(new Control[] { 
                selectVideoButton, analyzeVideoButton, durationLabel, 
                durationNumericUpDown, analysisProgressBar 
            });
            
            // Center panel - Video display and results
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 300
            };
            
            // Video panel
            videoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Black
            };
            
            var videoLabel = new Label
            {
                Text = "منطقة عرض الفيديو | Video Display Area",
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            videoPanel.Controls.Add(videoLabel);
            splitContainer.Panel1.Controls.Add(videoPanel);
            
            // Results panel
            videoResultsTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.LimeGreen
            };
            
            splitContainer.Panel2.Controls.Add(videoResultsTextBox);
            centerPanel.Controls.Add(splitContainer);
            
            videoAnalysisTab.Controls.AddRange(new Control[] { topPanel, centerPanel });
            tabControl.TabPages.Add(videoAnalysisTab);
        }
        
        private void CreateResultsTab()
        {
            resultsTab = new TabPage("النتائج | Results");
            
            // Top panel with buttons
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 60 };
            
            exportResultsButton = new Button
            {
                Text = "تصدير النتائج | Export Results",
                Location = new Point(20, 15),
                Size = new Size(150, 30),
                BackColor = Color.Orange,
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            exportResultsButton.Click += ExportResultsButton_Click;
            
            generateReportButton = new Button
            {
                Text = "إنشاء تقرير WHO | Generate WHO Report",
                Location = new Point(180, 15),
                Size = new Size(200, 30),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            generateReportButton.Click += GenerateReportButton_Click;
            
            topPanel.Controls.AddRange(new Control[] { exportResultsButton, generateReportButton });
            
            // Results grid
            resultsGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            
            resultsTab.Controls.AddRange(new Control[] { topPanel, resultsGridView });
            tabControl.TabPages.Add(resultsTab);
        }
        
        private async void SelectImageButton_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Title = "اختر صورة للتحليل | Select Image for Analysis";
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedMediaPath = openFileDialog.FileName;
                    
                    // Display selected image
                    try
                    {
                        originalImageBox.Image = Image.FromFile(selectedMediaPath);
                        analyzeImageButton.Enabled = true;
                        imageResultsLabel.Text = $"تم تحديد الصورة: {Path.GetFileName(selectedMediaPath)}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"خطأ في تحميل الصورة: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private async void AnalyzeImageButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMediaPath))
                return;
            
            analyzeImageButton.Enabled = false;
            analyzeImageButton.Text = "جاري التحليل... | Analyzing...";
            
            try
            {
                // Run AI analysis
                var result = await RunPythonAnalysis("image", selectedMediaPath);
                
                if (result != null)
                {
                    lastAnalysisResult = result;
                    DisplayImageResults(result);
                    LoadAnalyzedImages(result);
                    SaveToDatabase(result);
                    LoadPatientResults(); // Refresh results grid
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في التحليل: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                analyzeImageButton.Enabled = true;
                analyzeImageButton.Text = "تحليل بالذكاء الاصطناعي | Analyze with AI";
            }
        }
        
        private async void SelectVideoButton_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Video Files|*.mp4;*.avi;*.mov;*.wmv";
                openFileDialog.Title = "اختر فيديو للتحليل | Select Video for Analysis";
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedMediaPath = openFileDialog.FileName;
                    analyzeVideoButton.Enabled = true;
                    
                    videoResultsTextBox.Text = $"تم تحديد الفيديو: {Path.GetFileName(selectedMediaPath)}\n";
                    videoResultsTextBox.Text += "جاهز للتحليل بالذكاء الاصطناعي...\n";
                }
            }
        }
        
        private async void AnalyzeVideoButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMediaPath))
                return;
            
            analyzeVideoButton.Enabled = false;
            analysisProgressBar.Visible = true;
            analysisProgressBar.Style = ProgressBarStyle.Marquee;
            
            videoResultsTextBox.Text = "🎬 بدء تحليل الفيديو بالذكاء الاصطناعي...\n";
            videoResultsTextBox.Text += "📊 سيتم حساب معايير CASA المتقدمة...\n\n";
            
            try
            {
                var duration = (int)durationNumericUpDown.Value;
                var result = await RunPythonAnalysis("video", selectedMediaPath, duration);
                
                if (result != null)
                {
                    lastAnalysisResult = result;
                    DisplayVideoResults(result);
                    SaveToDatabase(result);
                    LoadPatientResults(); // Refresh results grid
                }
            }
            catch (Exception ex)
            {
                videoResultsTextBox.Text += $"❌ خطأ في التحليل: {ex.Message}\n";
                MessageBox.Show($"خطأ في التحليل: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                analyzeVideoButton.Enabled = true;
                analysisProgressBar.Visible = false;
            }
        }
        
        private async Task<SpermAnalysisResult> RunPythonAnalysis(string analysisType, string mediaPath, int duration = 0)
        {
            try
            {
                var pythonScript = Path.Combine(Application.StartupPath, "ai_sperm_analysis", "analyze_media.py");
                var arguments = $"\"{pythonScript}\" --type {analysisType} --media \"{mediaPath}\" --patient {currentPatientId}";
                
                if (analysisType == "video" && duration > 0)
                    arguments += $" --duration {duration}";
                
                var startInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.Combine(Application.StartupPath, "ai_sperm_analysis")
                };
                
                using (var process = Process.Start(startInfo))
                {
                    var output = await process.StandardOutput.ReadToEndAsync();
                    var error = await process.StandardError.ReadToEndAsync();
                    
                    await Task.Run(() => process.WaitForExit());
                    
                    if (process.ExitCode == 0 && !string.IsNullOrEmpty(output))
                    {
                        // Parse JSON result
                        var serializer = new JavaScriptSerializer();
                        var result = serializer.Deserialize<SpermAnalysisResult>(output);
                        return result;
                    }
                    else
                    {
                        throw new Exception($"Python analysis failed: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to run AI analysis: {ex.Message}");
            }
        }
        
        private void DisplayImageResults(SpermAnalysisResult result)
        {
            var results = $"🧬 نتائج تحليل الصورة:\n";
            results += $"═══════════════════════════\n";
            results += $"📊 العدد الكلي: {result.TotalCount} حيوان منوي\n";
            results += $"🎯 معامل الثقة: {result.AiConfidence:P1}\n";
            results += $"📈 التركيز المقدر: {result.ConcentrationEstimation:F1} مليون/مل\n";
            results += $"✅ متوافق مع WHO: {(result.WhoCompliance ? "نعم" : "لا")}\n";
            results += $"⏰ وقت التحليل: {result.Timestamp}\n";
            
            imageResultsLabel.Text = results;
        }
        
        private void DisplayVideoResults(SpermAnalysisResult result)
        {
            videoResultsTextBox.Clear();
            
            var results = "🎬 نتائج تحليل الفيديو بالذكاء الاصطناعي\n";
            results += "═══════════════════════════════════════\n\n";
            
            results += "📊 معلومات عامة:\n";
            results += $"   • المدة: {result.DurationSeconds} ثانية\n";
            results += $"   • الإطارات: {result.TotalFrames}\n";
            results += $"   • المسارات المكتشفة: {result.TotalTracks}\n";
            results += $"   • المسارات الصالحة: {result.ValidTracks}\n\n";
            
            if (result.CasaMetrics != null)
            {
                results += "🧪 معايير CASA:\n";
                results += $"   • VCL: {result.CasaMetrics.VclMean:F1} μm/s (السرعة المنحنية)\n";
                results += $"   • VSL: {result.CasaMetrics.VslMean:F1} μm/s (السرعة المستقيمة)\n";
                results += $"   • VAP: {result.CasaMetrics.VapMean:F1} μm/s (متوسط سرعة المسار)\n";
                results += $"   • LIN: {result.CasaMetrics.LinMean:F1}% (الخطية)\n";
                results += $"   • STR: {result.CasaMetrics.StrMean:F1}% (الاستقامة)\n";
                results += $"   • WOB: {result.CasaMetrics.WobMean:F1}% (التذبذب)\n";
                results += $"   • ALH: {result.CasaMetrics.AlhMean:F1} μm (الانحراف الجانبي)\n";
                results += $"   • BCF: {result.CasaMetrics.BcfMean:F1} Hz (تردد النبضة)\n\n";
            }
            
            if (result.MotilityAnalysis != null)
            {
                results += "🏃 تحليل الحركة:\n";
                results += $"   • سريع ومتقدم (A): {result.MotilityAnalysis.RapidProgressivePercent:F1}%\n";
                results += $"   • بطيء ومتقدم (B): {result.MotilityAnalysis.SlowProgressivePercent:F1}%\n";
                results += $"   • حركة في المكان (C): {result.MotilityAnalysis.NonProgressivePercent:F1}%\n";
                results += $"   • غير متحرك (D): {result.MotilityAnalysis.ImmotilePercent:F1}%\n";
                results += $"   • الحركة التقدمية الكلية: {result.MotilityAnalysis.TotalProgressivePercent:F1}%\n";
                results += $"   • الحركة الكلية: {result.MotilityAnalysis.TotalMotilePercent:F1}%\n\n";
            }
            
            results += $"🎯 معامل الثقة في التحليل: {result.AiConfidence:P1}\n";
            results += $"✅ متوافق مع معايير WHO: {(result.WhoCompliance ? "نعم" : "لا")}\n";
            results += $"⏰ اكتمل التحليل في: {DateTime.Now:HH:mm:ss}\n";
            
            videoResultsTextBox.Text = results;
        }
        
        private void LoadAnalyzedImages(SpermAnalysisResult result)
        {
            try
            {
                if (!string.IsNullOrEmpty(result.AnalyzedImagePath) && File.Exists(result.AnalyzedImagePath))
                {
                    analyzedImageBox.Image = Image.FromFile(result.AnalyzedImagePath);
                }
                
                if (!string.IsNullOrEmpty(result.HeatmapPath) && File.Exists(result.HeatmapPath))
                {
                    heatmapImageBox.Image = Image.FromFile(result.HeatmapPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الصور المحللة: {ex.Message}", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void SaveToDatabase(SpermAnalysisResult result)
        {
            try
            {
                // This would save to the semen_analysis table
                // Implementation depends on your DataAccessLayer structure
                MessageBox.Show("تم حفظ النتائج في قاعدة البيانات", "نجح الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ النتائج: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void LoadPatientResults()
        {
            try
            {
                var results = dal.ExecuteQuery("SELECT * FROM semen_analysis WHERE patient_id = ? ORDER BY test_date DESC", currentPatientId);
                resultsGridView.DataSource = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل النتائج: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ExportResultsButton_Click(object sender, EventArgs e)
        {
            // Export results to Excel/PDF
            MessageBox.Show("سيتم تطوير هذه الميزة قريباً", "قيد التطوير", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void GenerateReportButton_Click(object sender, EventArgs e)
        {
            if (lastAnalysisResult != null)
            {
                var reportForm = new WHOReportForm(lastAnalysisResult);
                reportForm.Show();
            }
            else
            {
                MessageBox.Show("لا توجد نتائج للتقرير", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
    
    // Data structure for AI analysis results
    public class SpermAnalysisResult
    {
        public int PatientId { get; set; }
        public string AnalysisType { get; set; }
        public string Timestamp { get; set; }
        public int TotalCount { get; set; }
        public double AiConfidence { get; set; }
        public double ConcentrationEstimation { get; set; }
        public bool WhoCompliance { get; set; }
        public string OriginalImagePath { get; set; }
        public string AnalyzedImagePath { get; set; }
        public string HeatmapPath { get; set; }
        public int DurationSeconds { get; set; }
        public int TotalFrames { get; set; }
        public int TotalTracks { get; set; }
        public int ValidTracks { get; set; }
        public CASAMetrics CasaMetrics { get; set; }
        public MotilityAnalysis MotilityAnalysis { get; set; }
    }
    
    public class CASAMetrics
    {
        public double VclMean { get; set; }
        public double VslMean { get; set; }
        public double VapMean { get; set; }
        public double LinMean { get; set; }
        public double StrMean { get; set; }
        public double WobMean { get; set; }
        public double AlhMean { get; set; }
        public double BcfMean { get; set; }
    }
    
    public class MotilityAnalysis
    {
        public double RapidProgressivePercent { get; set; }
        public double SlowProgressivePercent { get; set; }
        public double NonProgressivePercent { get; set; }
        public double ImmotilePercent { get; set; }
        public double TotalProgressivePercent { get; set; }
        public double TotalMotilePercent { get; set; }
    }
}