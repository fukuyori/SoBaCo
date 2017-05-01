using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ActiveMarket;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Specialized;
using iTextSharp;
using System.IO;

namespace sobaco {

    public enum ChartStyles { None = 0, Candle, Line };
    public enum CandleSizes { XL = 0, L, M, S, XS };


    public partial class MainForm : Form {


        Meigara MyMeigara;
        MeigaraList MyMeigaraList;

        ChartStyles ChartStyle;
        AveSteps AveStep;

        CandleSizes CandleSize;
        class CandleSizeScale {
            public int Days { get; private set; }
            public double Candle { get; private set; }

            public CandleSizeScale(int days, double candle) {
                this.Days = days;
                this.Candle = candle;
            }
        }
        List<CandleSizeScale> CandleSizeList = new List<CandleSizeScale>() {
            new CandleSizeScale(  20, 0.9),
            new CandleSizeScale(  60, 0.8),
            new CandleSizeScale( 120, 0.6),
            new CandleSizeScale( 240, 0.5),
            new CandleSizeScale( 720, 0.5)
        };

        ChartScales ChartScale;
        int IdouheikinLine1, IdouheikinLine2, IdouheikinLine3, IdouheikinLine4, IdouheikinLine5; // 移動平均線表示フラグ

        private static int InnerPlotPositionH = 96;
        private static int InnerPlotPositionW = 95;
        private int Tab;
        private string OkiniiriFileName;

        public MainForm() {
            InitializeComponent();
            
            // マウス移動イベントを追加
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            this.MouseUp += new MouseEventHandler(Form1_MouseUp);
            this.MouseMove += new MouseEventHandler(Form1_MouseMove);
            // MouseWheelイベントハンドラを追加
            this.chart1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Chart1_MouseWheel);
            //ApplicationExitイベントハンドラを追加
            Application.ApplicationExit += new EventHandler(this.MainForm_Exit);
            // 印刷イベントハンドラを追加
            printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(Pd_PrintPage);
        }

        /// <summary>
        /// フォーム初期化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e) {

            //　銘柄名一覧の作成
            MyMeigaraList = new MeigaraList();
            int n = MyMeigaraList.SearchMeigaraList("");

            WindowState = (FormWindowState)Properties.Settings.Default.WindowState;
            Width = Properties.Settings.Default.Width;
            Height = Properties.Settings.Default.Height;
            ChartStyle = (ChartStyles)Properties.Settings.Default.Style;
            AveStep = (AveSteps)Properties.Settings.Default.AveStep;
            CandleSize = (CandleSizes)Properties.Settings.Default.CandleSize;
            ChartScale = (ChartScales)Properties.Settings.Default.Scale;
            IdouheikinLine1 = Properties.Settings.Default.H1;
            IdouheikinLine2 = Properties.Settings.Default.H2;
            IdouheikinLine3 = Properties.Settings.Default.H3;
            IdouheikinLine4 = Properties.Settings.Default.H4;
            IdouheikinLine5 = Properties.Settings.Default.H5;
            Tab = Properties.Settings.Default.Tab;
            OkiniiriFileName = Properties.Settings.Default.MeigaraFileName;
            string[] codes = Properties.Settings.Default.Favorite.Split(',');
            foreach (string s in codes)
                MyMeigaraList.FavoriteAdd(s, MeigaraList.Proc.ADD);


            // ローソク足チャート初期設定
            CandlePreparation(chart1);
            // 日足、週足、月足の表示設定
            ChangeChartScaleDisplay(ChartScale);
            // 表示幅の表示設定
            SizeCheckOnClick(CandleSize);
            // 移動平均の表示設定
            AveStepChangeOnClick(AveStep);
            // 以前保存した銘柄ファイルがあるかチェック
            if (OkiniiriFileName.Length == 0 || !System.IO.File.Exists(OkiniiriFileName))
                OkiniiriFileName = "";
            if (OkiniiriEquals())
                bSave.Enabled = false;
            else
                bSave.Enabled = true;

            // 銘柄リストの書式
            void DataGridViewSetup(DataGridView dgv, DataTable dt)
            {
                dgv.DataSource = dt;
                dgv.AutoGenerateColumns = false;
                dgv.AllowUserToResizeColumns = false;
                dgv.AllowUserToResizeRows = false;
                dgv.Columns[0].Width = 30;
                dgv.Columns[0].ReadOnly = true;
                dgv.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgv.Columns[1].ReadOnly = true;
                dgv.MultiSelect = false;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }

            DataGridViewSetup(dataGridView1, MyMeigaraList.SearchTable);
            DataGridViewSetup(dataGridView2, MyMeigaraList.FavoriteTable);
            DataGridViewSetup(dataGridView3, MyMeigaraList.HistoryTable);

            // お気に入りリストの書式
            dataGridView2.DataSource = MyMeigaraList.FavoriteTable;
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.AllowUserToResizeColumns = false;
            dataGridView2.AllowUserToResizeRows = false;
            dataGridView2.Columns[0].Width = 30;
            dataGridView2.Columns[0].ReadOnly = true;
            dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView2.Columns[1].ReadOnly = true;

            this.dataGridView1.SelectionChanged += new System.EventHandler(this.DataGridView1_SelectionChanged);
            this.dataGridView2.SelectionChanged += new System.EventHandler(this.DataGridView2_SelectionChanged);
            this.dataGridView3.SelectionChanged += new System.EventHandler(this.DataGridView3_SelectionChanged);

            tabControl1.SelectedIndex = Tab;
            toolStripTextBox1.Focus();
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (!OkiniiriEquals() && MyMeigaraList.FavoritetableCount > 0) {
                DialogResult result = MessageBox.Show("お気に入りリストを保存しますか？",
                    "相場子",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                    OkiniiriSave();
                if (result == DialogResult.Cancel)
                    e.Cancel = true;

            }
        }

        private bool OkiniiriEquals() {
            var codes = new List<string>();
            if (OkiniiriFileName.Length > 0) {
                using (System.IO.StreamReader sr =
                            new System.IO.StreamReader(OkiniiriFileName, Encoding.GetEncoding("Shift_JIS"))) {

                    while (!sr.EndOfStream) {
                        string[] col = sr.ReadLine().Split(',');
                        codes.Add(col[0]);
                    }
                    sr.Close();
                }
            }
            return MyMeigaraList.FavoriteEquals(codes);
        }

        private void MainForm_Exit(object sender, EventArgs e) {
            //Configurationの作成

            Properties.Settings.Default.WindowState = (int)this.WindowState;
            Properties.Settings.Default.Width = this.Width;
            Properties.Settings.Default.Height = this.Height;
            Properties.Settings.Default.Style = (int)ChartStyle;
            Properties.Settings.Default.AveStep = (int)AveStep;
            Properties.Settings.Default.CandleSize = (int)CandleSize;
            Properties.Settings.Default.Scale = (int)ChartScale;
            Properties.Settings.Default.H1 = IdouheikinLine1;
            Properties.Settings.Default.H2 = IdouheikinLine2;
            Properties.Settings.Default.H3 = IdouheikinLine3;
            Properties.Settings.Default.H4 = IdouheikinLine4;
            Properties.Settings.Default.H5 = IdouheikinLine5;
            Properties.Settings.Default.Tab = tabControl1.SelectedIndex;
            Properties.Settings.Default.Favorite = string.Join(",", MyMeigaraList.FavoriteCodes);
            Properties.Settings.Default.MeigaraFileName = OkiniiriFileName;

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// グラフ作成
        /// </summary>
        /// <param name="_meigara"></param>
        /// <param name="_chart"></param>
        private void DrawTable(Meigara _meigara, Chart _chart) {
            if (_meigara == null)
                return;

            // 銘柄名表示
            labelMeigara.Text = _meigara.Code + " " + _meigara.Name;

            // ローソク足チャートの設定
            CandleSetting(_meigara, _chart);

            for (int i = 0; i < _meigara.PtCount; i++) {
                // ローソク足チャートの描画
                CandleDraw(_meigara, _chart, i);
            }


            if (_meigara.PtCount > 0) {
                labelDate.Text = _meigara.GetPtDateLabel(0);
                labelValue.Text = string.Format("始 {0:#,0.##} / 高 {1:#,0.##} / 安 {2:#,0.##} / 終 {3:#,0.##}",
                    _meigara.GetPtOpen(0), _meigara.GetPtHigh(0), _meigara.GetPtLow(0), _meigara.GetPtClose(0));
            } else
                labelValue.Text = "";

            // 上げなら青、下げなら赤で％表示
            double n;
            if (_meigara.PtCount > 2) {
                n = _meigara.GetPtClose(0) - _meigara.GetPtClose(1);
                if (n > 0)
                    labelc.ForeColor = Color.Blue;
                else if (n == 0)
                    labelc.ForeColor = Color.Black;
                else
                    labelc.ForeColor = Color.Red;
                labelc.Text = string.Format("{0,5} {1,6}% ({2,3})",
                    n.ToString("+#,0.##;-#,0.##"),
                    (n / _meigara.GetPtClose(1) * 100).ToString("+#,0.##;-#,0.##"),
                    RenzokuKaisuu(_meigara));

                Hikaku(labelh1, _meigara.GetPtHeikin1(0), _meigara.GetPtHeikin1(1));
                Hikaku(labelh2, _meigara.GetPtHeikin2(0), _meigara.GetPtHeikin2(1));
                Hikaku(labelh3, _meigara.GetPtHeikin3(0), _meigara.GetPtHeikin3(1));
                Hikaku(labelh4, _meigara.GetPtHeikin4(0), _meigara.GetPtHeikin4(1));
                if (MyMeigara.AveStep == AveSteps.Aiba)
                    Hikaku(labelh5, _meigara.GetPtHeikin5(0), _meigara.GetPtHeikin5(1));

                // 上げから下げ、下げから上げに転じたら背景色を黄色に
                if (_meigara.PtCount > 3) {
                    Tenkan(labelc, _meigara.GetPtClose(0), _meigara.GetPtClose(1), _meigara.GetPtClose(2));
                    Tenkan(labelh1, _meigara.GetPtHeikin1(0), _meigara.GetPtHeikin1(1), _meigara.GetPtHeikin1(2));
                    Tenkan(labelh2, _meigara.GetPtHeikin2(0), _meigara.GetPtHeikin2(1), _meigara.GetPtHeikin2(2));
                    Tenkan(labelh3, _meigara.GetPtHeikin3(0), _meigara.GetPtHeikin3(1), _meigara.GetPtHeikin3(2));
                    Tenkan(labelh4, _meigara.GetPtHeikin4(0), _meigara.GetPtHeikin4(1), _meigara.GetPtHeikin4(2));
                    if (MyMeigara.AveStep == AveSteps.Aiba)
                        Tenkan(labelh5, _meigara.GetPtHeikin5(0), _meigara.GetPtHeikin5(1), _meigara.GetPtHeikin5(2));
                }
                // 移動平均線の順位入れ替わり
                var d1 = Junni(_meigara, 0);
                var d2 = Junni(_meigara, 1);
                for (var i = 0; i < d1.Count(); i++) {
                    Debug.WriteLine($"{d1[i]} {d2[i]}");
                    if (d1[i] != d2[i])
                        switch (d1[i]) {
                            case "H1":
                                labelh1.BackColor = Color.LightPink;
                                break;
                            case "H2":
                                labelh2.BackColor = Color.LightPink;
                                break;
                            case "H3":
                                labelh3.BackColor = Color.LightPink;
                                break;
                            case "H4":
                                labelh4.BackColor = Color.LightPink;
                                break;
                            case "H5":
                                labelh5.BackColor = Color.LightPink;
                                break;
                        }
                }
            }
        }

        /// <summary>
        /// 何日連続で上げあるいは下げが続いているか
        /// </summary>
        /// <param name="_meigara"></param>
        /// <returns></returns>
        private int RenzokuKaisuu(Meigara _meigara) {
            bool isUp = (_meigara.GetPtClose(0) - _meigara.GetPtClose(1) >= 0) ? true : false;

            var _count = 1;
            var _close = _meigara.GetPtClose(1);
            if (isUp)
                while (_meigara.GetPtClose(_count) >= _meigara.GetPtClose(_count + 1)) {
                    _count++;
                }
            else
                while (_meigara.GetPtClose(_count) < _meigara.GetPtClose(_count + 1)) {
                    _count++;
                }
            return _count;          
        }

        private List<string> Junni(Meigara _meigara, int pos) {
            var _junni = new Dictionary<string, double> {
                { "H1", _meigara.GetPtHeikin1(pos) },
                { "H2", _meigara.GetPtHeikin2(pos) },
                { "H3", _meigara.GetPtHeikin3(pos) },
                { "H4", _meigara.GetPtHeikin4(pos) },
                { "H5", _meigara.GetPtHeikin5(pos) }
            };
            return _junni.OrderByDescending(x => x.Value)
                         .Select(x => x.Key)
                         .ToList<string>();
        }

        /// <summary>
        /// n1とn2を比較し、上げなら青文字、同じなら黒文字、下げなら赤文字にする
        /// </summary>
        /// <param name="lab"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        private void Hikaku(ToolStripLabel lab, double n1, double n2) {
            double n = (n1 - n2) / n2 * 100;
            if (n > 0)
                lab.ForeColor = Color.Blue;
            else if (n == 0)
                lab.ForeColor = Color.Black;
            else
                lab.ForeColor = Color.Red;
            lab.Text = string.Format("{0,6}%", n.ToString("+#,0.##;-#,0.##"));
        }

        /// <summary>
        /// n1-n2と、n2-3をみて、上げから下げ、下げからあげに転じたら
        /// </summary>
        /// <param name="lab"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="n3"></param>
        private void Tenkan(ToolStripLabel lab, double n1, double n2, double n3) {
            double n = (n1 - n2) * (n2 - n3);
            if (n <= 0)
                lab.BackColor = Color.Yellow;
            else
                lab.BackColor = SystemColors.Control;
        }

        /// <summary>
        /// ローソク足チャートの値設定
        /// </summary>
        /// <param name="_meigara"></param>
        /// <param name="_chart"></param>
        private void CandleSetting(Meigara _meigara, Chart _chart) {
            // 株価グラフの設定
            _chart.ChartAreas[0].AxisY.Maximum = ((int)(_meigara.KabukaHigh / _meigara.KabukaGridInterval.Value) + 1) * _meigara.KabukaGridInterval.Value;
            _chart.ChartAreas[0].AxisY.Minimum = (int)(_meigara.KabukaLow / _meigara.KabukaGridInterval.Value) * _meigara.KabukaGridInterval.Value;
            _chart.ChartAreas[0].AxisY.Interval = _meigara.KabukaGridInterval.Value;
            _chart.ChartAreas[0].AxisY.LabelStyle.Format = "##,###";
            _chart.ChartAreas[0].AxisX.Interval = 20;
            _chart.ChartAreas[0].AxisX.IntervalOffset = 1;

            // ローソク足の間隔
            _chart.Series["CANDLE"]["PointWidth"] = CandleSizeList[(int)CandleSize].Candle.ToString();

            // 各グラフを初期化
            _chart.Series["CANDLE"].Points.Clear();
            _chart.Series["OWARINE"].Points.Clear();
            _chart.Series["IDOU1"].Points.Clear();
            _chart.Series["IDOU2"].Points.Clear();
            _chart.Series["IDOU3"].Points.Clear();
            _chart.Series["IDOU4"].Points.Clear();
            _chart.Series["IDOU5"].Points.Clear();
        }

        /// <summary>
        /// ローソク足チャート描画
        /// </summary>
        /// <param name="_chart"></param>
        /// <param name="dr"></param>
        /// <param name="xlabel"></param>
        private void CandleDraw(Meigara _meigara, Chart _chart,  int i) {
            string _labelString = _meigara.GetPtDateLabel(i);
            switch (ChartStyle) {
                case ChartStyles.Candle:
                    _chart.Series["CANDLE"].Points.AddXY(_labelString, _meigara.GetPtHigh(i));
                    _chart.Series["CANDLE"].Points[i].YValues[1] = _meigara.GetPtLow(i);
                    _chart.Series["CANDLE"].Points[i].YValues[2] = _meigara.GetPtOpen(i);
                    _chart.Series["CANDLE"].Points[i].YValues[3] = _meigara.GetPtClose(i);
                    break;
                case ChartStyles.Line:
                    _chart.Series["OWARINE"].Points.AddXY(_labelString, _meigara.GetPtClose(i));
                    break;
            }


            //// 移動平均線の描画
            void DrawLine(double n, Series ser, int w)
            {
                if (n > 0) {
                    ser.BorderWidth = w;
                    ser.Points.AddXY(_labelString, n);
                }
            }
            DrawLine(_meigara.GetPtHeikin1(i), _chart.Series["IDOU1"], IdouheikinLine1);
            DrawLine(_meigara.GetPtHeikin2(i), _chart.Series["IDOU2"], IdouheikinLine2);
            DrawLine(_meigara.GetPtHeikin3(i), _chart.Series["IDOU3"], IdouheikinLine3);
            DrawLine(_meigara.GetPtHeikin4(i), _chart.Series["IDOU4"], IdouheikinLine4);
            if (AveStep == AveSteps.Aiba)
                DrawLine(_meigara.GetPtHeikin5(i), _chart.Series["IDOU5"], IdouheikinLine5);
        }

        /// <summary>
        /// ローソク足チャート初期設定
        /// </summary>
        /// <param name="_chart"></param>
        private void CandlePreparation(Chart _chart) {
            ChartArea _chartArea = new ChartArea("AREA");
            _chart.ChartAreas.Add(_chartArea);

            Series _series = new Series("CANDLE") {
                ChartType = SeriesChartType.Candlestick
            };
            _chart.Series.Add(_series);

            _chart.ChartAreas[0].InnerPlotPosition.X = 0;
            _chart.ChartAreas[0].InnerPlotPosition.Y = 0;
            _chart.ChartAreas[0].InnerPlotPosition.Height = InnerPlotPositionH;
            _chart.ChartAreas[0].InnerPlotPosition.Width = InnerPlotPositionW;
            _chart.ChartAreas[0].AxisX.IsReversed = true;

            // ローソク足の色
            _chart.Series["CANDLE"].Color = Color.Black;
            _chart.Series["CANDLE"].BorderColor = Color.Black;
            _chart.Series["CANDLE"]["PriceUpColor"] = "White";
            _chart.Series["CANDLE"]["PriceDownColor"] = "Black";

            // X軸
            _chart.Series["CANDLE"].XValueType = ChartValueType.String;

            // 凡例非表示
            _chart.Series["CANDLE"].IsVisibleInLegend = false;

            // チャートカラーの設定
            _chart.BackColor = Color.White;
            _chart.ChartAreas[0].BackColor = Color.Transparent;
            // グリッド線
            _chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Black;
            _chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Black;
            _chart.ChartAreas[0].AxisY.LabelAutoFitMaxFontSize = 8;
            _chart.ChartAreas[0].AxisX.LabelAutoFitMaxFontSize = 8;
            _chart.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            _chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Aquamarine;
            _chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Aquamarine;

            // 移動平均線の書式
            void line_setup(Series ser, Color co)
            {
                ser.ChartType = SeriesChartType.Line;
                _chart.Series.Add(ser);
                ser.IsVisibleInLegend = false;
                ser.Color = co;
            }

            line_setup(new Series("OWARINE"), Color.Black);
            line_setup(new Series("IDOU1"), Color.Red);
            line_setup(new Series("IDOU2"), Color.Green);
            line_setup(new Series("IDOU3"), Color.Blue);
            line_setup(new Series("IDOU4"), Color.Purple);
            line_setup(new Series("IDOU5"), Color.Orange);
        }


        /// <summary>
        /// 銘柄の生成とテーブルを作成してグラフを描く
        /// </summary>
        /// <param name="s"></param>
        private void NewMeigaraMakeAndDraw(string s) {
            if (MyMeigara != null)
                MyMeigara.Dispose();

            System.Windows.Forms.Cursor _Cursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            try {
                MyMeigara = new Meigara(s);

                // テーブル作成
                MyMeigara.MakeTable(MyMeigara.PlotEnd,
                            CandleSizeList[(int)CandleSize].Days,
                            ChartScale,
                            AveStep);
                // 描画
                DrawTable(MyMeigara, chart1);
            } catch {
                Debug.WriteLine("MakeTableAndDraw Error : " + s);
                this.Cursor = _Cursor;
                return;
            }
            // 履歴登録
            if (MyMeigaraList.AddHistory(s))
                dataGridView3.Rows[0].Selected = true;
            this.Cursor = _Cursor;
        }

        /// <summary>
        /// データグリッドビューの銘柄変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView1_SelectionChanged(object sender, EventArgs e) {
            if (dataGridView1.SelectedRows.Count > 0 && tabControl1.SelectedIndex == 0)
                NewMeigaraMakeAndDraw(MyMeigaraList.GetSearchTableCode(dataGridView1.SelectedRows[0].Index));
        }

        /// <summary>
        /// お気に入りグリッドビューの銘柄変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView2_SelectionChanged(object sender, EventArgs e) {
            if (dataGridView2.SelectedRows.Count > 0 && tabControl1.SelectedIndex == 1) 
                NewMeigaraMakeAndDraw((string)dataGridView2.SelectedRows[0].Cells[0].Value);
        }

        /// <summary>
        /// 履歴グリッドビューの銘柄変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView3_SelectionChanged(object sender, EventArgs e) {
            if (dataGridView3.SelectedRows.Count > 0 && tabControl1.SelectedIndex == 2)
                NewMeigaraMakeAndDraw(MyMeigaraList.GetHistoryTableCode(dataGridView3.CurrentCell.RowIndex));
        }

        /// <summary>
        /// 銘柄タブ変更時に表示変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e) {
            switch (tabControl1.SelectedIndex) {
                case 0:
                    if (tabControl1.SelectedIndex == 0) {
                        if (dataGridView1.Rows.Count > 0 && dataGridView1.SelectedRows.Count > 0) {
                            dataGridView1.Rows[0].Selected = true;
                            NewMeigaraMakeAndDraw(MyMeigaraList.GetSearchTableCode(dataGridView1.SelectedCells[0].RowIndex));
                        }
                    }
                    break;
                case 1:
                    if (tabControl1.SelectedIndex == 1) {
                        if (dataGridView2.Rows.Count > 0 && dataGridView2.SelectedRows.Count > 0) {
                            dataGridView2.Rows[0].Selected = true;
                            NewMeigaraMakeAndDraw(MyMeigaraList.GetFavoriteTableCode(dataGridView2.SelectedCells[0].RowIndex));
                        }
                    }
                    break;
                case 2:
                    if (tabControl1.SelectedIndex == 2) {
                        if (dataGridView3.Rows.Count > 0 && dataGridView3.SelectedRows.Count > 0) {
                            dataGridView3.Rows[0].Selected = true;
                            NewMeigaraMakeAndDraw(MyMeigaraList.GetHistoryTableCode(dataGridView3.SelectedCells[0].RowIndex));
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 印刷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintToolStripMenuItem_Click(object sender, EventArgs e) {
            PrintProc();
        }

        /// <summary>
        /// 印刷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButton13_Click(object sender, EventArgs e) {
            PrintProc();
        }

        private int PrintPageCount;

        private void PrintProc() {

            printDocument1.DefaultPageSettings.Landscape = true;
            // 余白設定 左上右下
            System.Drawing.Printing.Margins margins = new System.Drawing.Printing.Margins(25, 25, 100, 25);
            printDocument1.DefaultPageSettings.Margins = margins;
            printDialog1.Document = printDocument1;
            //printDialog1.AllowCurrentPage = true;
            printDialog1.AllowSelection = true;

            // お気に入りタブが表示されていたら、ページ指定可
            if (tabControl1.SelectedIndex == 1) {
                printDialog1.AllowSomePages = true;
                //ページ指定の最小値と最大値を指定する
                printDialog1.PrinterSettings.MinimumPage = 1;
                printDialog1.PrinterSettings.MaximumPage = MyMeigaraList.FavoritetableCount;
                //印刷開始と終了ページを指定する
                printDialog1.PrinterSettings.FromPage = printDialog1.PrinterSettings.MinimumPage;
                printDialog1.PrinterSettings.ToPage = printDialog1.PrinterSettings.MaximumPage;
            } else
                printDialog1.AllowSomePages = false;
            // 印刷範囲を選択した部分
            printDialog1.PrinterSettings.PrintRange = System.Drawing.Printing.PrintRange.Selection;
            printDialog1.UseEXDialog = false;
            PrintPageCount = 1;

            if (printDialog1.ShowDialog() == DialogResult.OK) {
                printDocument1.Print();
            }
        }

        /// <summary>
        /// 印刷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pd_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) {
            // お気に入りタブが表示されている場合、印刷範囲がすべての場合は、お気に入り登録されている銘柄すべてを印刷
            if (tabControl1.SelectedIndex == 1) {
                // お気に入りに銘柄が1つ以上ある場合
                if (MyMeigaraList.FavoritetableCount > 0) {
                    switch (e.PageSettings.PrinterSettings.PrintRange) {
                        case System.Drawing.Printing.PrintRange.AllPages:
                            if (PrintPageCount < MyMeigaraList.FavoritetableCount)
                                e.HasMorePages = true;
                            else
                                e.HasMorePages = false;
                            PagePrint(MyMeigaraList.GetFavoriteTableCode(PrintPageCount - 1), MyMeigara.PlotEnd, e);
                            break;
                        case System.Drawing.Printing.PrintRange.SomePages:
                            if (PrintPageCount < printDialog1.PrinterSettings.ToPage) {
                                //ページ範囲が指定されており、始めのページのときは、
                                //印刷開始ページまで飛ばす
                                if (PrintPageCount == 1)
                                    PrintPageCount = e.PageSettings.PrinterSettings.FromPage;

                                e.HasMorePages = true;
                            } else
                                e.HasMorePages = false;
                            PagePrint(MyMeigaraList.GetFavoriteTableCode(PrintPageCount - 1), MyMeigara.PlotEnd, e);
                            break;
                        default:
                            e.HasMorePages = false;
                            PagePrint(MyMeigara.Code, MyMeigara.PlotEnd, e);
                            break;
                    }
                }
            } else {
                // 現在のページを印刷
                e.HasMorePages = false;
                PagePrint(MyMeigara.Code, MyMeigara.PlotEnd, e);
            }
            PrintPageCount++;
        }

        private void PagePrint(string code, int endpoint, System.Drawing.Printing.PrintPageEventArgs e) {
            // 印刷用チャートの作成
            Chart printChart = new Chart();
            Meigara printMeigara = new Meigara(code);

            printChart.Width = e.MarginBounds.Width;
            printChart.Height = e.MarginBounds.Height;
            CandlePreparation(printChart);
            printMeigara.MakeTable(endpoint,
                        CandleSizeList[(int)CandleSize].Days,
                        ChartScale, 
                        AveStep);
            CandleSetting(printMeigara, printChart);

            for (int i = 0; i < printMeigara.PtCount; i++) {
                // ローソク足チャートの描画
                CandleDraw(printMeigara, printChart, i);
            }

            // 600dpi 7016 x 4961
            Bitmap chartBitmap = new Bitmap(printChart.Width, printChart.Height);
            printChart.DrawToBitmap(chartBitmap, new Rectangle(0, 0, printChart.Width, printChart.Height));
            e.Graphics.DrawImage(chartBitmap, e.MarginBounds);

            Font _font = new Font("MS UI Gothic", 14, FontStyle.Regular);
            e.Graphics.DrawString(printMeigara.Code + " " + printMeigara.Name, _font, Brushes.Black, 40, 60);
            _font = new Font("MS UI Gothic", 10, FontStyle.Regular);
            if (printMeigara.PtCount > 0) {
                string _ValueString = string.Format("{0} : 始 {1:#,0.##} / 高 {2:#,0.##} / 安 {3:#,0.##} / 終 {4:#,0.##}",
                printMeigara.GetPtDateLabel(0), printMeigara.GetPtOpen(0), printMeigara.GetPtHigh(0), printMeigara.GetPtLow(0), printMeigara.GetPtClose(0));
                e.Graphics.DrawString(_ValueString, _font, Brushes.Black, 200, 80);
                e.Graphics.DrawString(PrintPageCount.ToString(), _font, Brushes.Black, 600, 800);
            }
            chartBitmap.Dispose();
            printChart.Dispose();
            printMeigara.Dispose();
        }

        /// <summary>
        /// PDF出力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportToPDFToolStripMenuItem_Click(object sender, EventArgs e) {
            ExportToPDF();
        }

        private void ExportToPDF() {
            if (MyMeigara == null)
                return;

            iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 8, 8, 40, 20);
            //ファイルの出力先を設定
            saveFileDialog1.Filter = "ＰＤＦファイル(*.pdf)|*.pdf";
            saveFileDialog1.Title = "ＰＤＦファイル出力";
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.FileName = "株価チャート.pdf";

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                // カーソール変更
                System.Windows.Forms.Cursor _Cursor = this.Cursor;
                this.Cursor = Cursors.WaitCursor;



                if (tabControl1.SelectedIndex == 1) {
                    //しおり
                    Dictionary<string, object> bookmark;
                    List<Dictionary<string, object>> outlines;
                    List<Dictionary<string, object>> root = new List<Dictionary<string, object>>();

                    var tempFile = Path.GetTempFileName();

                    using (FileStream fs = new FileStream(tempFile, FileMode.Create)) {
                        iTextSharp.text.pdf.PdfWriter.GetInstance(pdfDoc, fs);


                        pdfDoc.Open();
                        var cnt = 1;
                        foreach (string _code in MyMeigaraList.FavoriteCodes) {
                            if (pdfDoc.PageNumber != 0)
                                pdfDoc.NewPage();
                            pdfDoc.Add(MakePdfPage(_code));

                            outlines = new List<Dictionary<string, object>>();
                            bookmark = new Dictionary<string, object>();
                            bookmark.Add("Title", $"{_code} {MyMeigaraList.GetNameByCode(_code)}");
                            bookmark.Add("Page", $"{cnt.ToString()} FitH");
                            bookmark.Add("Action", "GoTo");
                            root.Add(bookmark);
                            cnt++;
                        }
                        pdfDoc.Close();
                    }
                    // しおり作成
                    using (iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(tempFile)) {
                        using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create)) {
                            iTextSharp.text.Document pdfCopyDoc = new iTextSharp.text.Document();
                            iTextSharp.text.pdf.PdfCopy copy = new iTextSharp.text.pdf.PdfCopy(pdfCopyDoc, fs);
                            pdfCopyDoc.Open();
                            copy.AddDocument(reader);
                            copy.Outlines = root;
                            copy.Close();
                            pdfCopyDoc.Close();
                        }
                    }
                    System.IO.File.Delete(tempFile);

                } else {
                    using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create)) {
                        iTextSharp.text.pdf.PdfWriter.GetInstance(pdfDoc, fs);

                        pdfDoc.Open();
                        pdfDoc.Add(MakePdfPage(MyMeigara.Code));
                        pdfDoc.Open();
                    }
                }
                // カーソルを元に戻して終了
                this.Cursor = _Cursor;
                //MessageBox.Show("出力が終了しました");
            }
        }

        private iTextSharp.text.Image MakePdfPage(string code) {
            // 印刷用チャートの作成
            Chart pdfChart = new Chart();
            Meigara pdfMeigara = new Meigara(code);

            pdfChart.Width = 1200;
            pdfChart.Height = 848;
            CandlePreparation(pdfChart);
            pdfMeigara.MakeTable(MyMeigara.PlotEnd,
                        CandleSizeList[(int)CandleSize].Days,
                        ChartScale, 
                        AveStep);
            CandleSetting(pdfMeigara, pdfChart);

            for (int i = 0; i < pdfMeigara.PtCount; i++) {
                // ローソク足チャートの描画
                CandleDraw(pdfMeigara, pdfChart, i);
            }

            Bitmap chartBitmap = new Bitmap(pdfChart.Width, pdfChart.Height);
            pdfChart.DrawToBitmap(chartBitmap, new Rectangle(0, 0, pdfChart.Width, pdfChart.Height));
            Graphics _graphics = Graphics.FromImage(chartBitmap);

            Font _font = new Font("MS UI Gothic", 12, FontStyle.Regular);
            _graphics.DrawString(pdfMeigara.Code + " " + pdfMeigara.Name, _font, Brushes.Black, 20, 0);
            _font = new Font("MS UI Gothic", 10, FontStyle.Regular);
            if (pdfMeigara.PtCount > 0) {
                string _ValueString = string.Format("{0} : 始 {1:#,0.##} / 高 {2:#,0.##} / 安 {3:#,0.##} / 終 {4:#,0.##}",
                pdfMeigara.GetPtDateLabel(0), pdfMeigara.GetPtOpen(0), pdfMeigara.GetPtHigh(0), pdfMeigara.GetPtLow(0), pdfMeigara.GetPtClose(0));
                _graphics.DrawString(_ValueString, _font, Brushes.Black, 300, 3);
            }

            iTextSharp.text.Image pdfImage = iTextSharp.text.Image.GetInstance(chartBitmap, System.Drawing.Imaging.ImageFormat.Bmp);
            pdfImage.ScalePercent(70f, 64f);

            chartBitmap.Dispose();
            pdfChart.Dispose();
            pdfMeigara.Dispose();

            return pdfImage;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 画像編集画面へ移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PaintToolStripMenuItem_Click(object sender, EventArgs e) {
            PaintProc();
        }

        /// <summary>
        /// 画像編集画面作成
        /// </summary>
        private void PaintProc() {
            if (MyMeigara == null)
                return;
            // 印刷用チャートの作成
            Chart paintChart = new Chart();
            Meigara paintMeigara = new Meigara(MyMeigara.Code);
            // 
            paintChart.Width = 1200;
            paintChart.Height = 848;

            CandlePreparation(paintChart);
            paintMeigara.MakeTable(MyMeigara.PlotEnd,
                        CandleSizeList[(int)CandleSize].Days,
                        ChartScale, 
                        AveStep);
            CandleSetting(paintMeigara, paintChart);

            for (int i = 0; i < paintMeigara.PtCount; i++) {
                // ローソク足チャートの描画
                CandleDraw(paintMeigara, paintChart, i);
            }

            Bitmap chartBitmap = new Bitmap(paintChart.Width, paintChart.Height);
            paintChart.DrawToBitmap(chartBitmap, new Rectangle(0, 0, paintChart.Width, paintChart.Height));

            Graphics _graphics = Graphics.FromImage(chartBitmap);

            Font _font = new Font("MS UI Gothic", 14, FontStyle.Regular);
            _graphics.DrawString(paintMeigara.Code + " " + paintMeigara.Name, _font, Brushes.Black, 10, 0);
            _font = new Font("MS UI Gothic", 10, FontStyle.Regular);
            if (paintMeigara.PtCount > 0) {
                string _ValueString = string.Format("{0} : 始 {1:#,0.##} / 高 {2:#,0.##} / 安 {3:#,0.##} / 終 {4:#,0.##}",
                    paintMeigara.GetPtDateLabel(0), paintMeigara.GetPtOpen(0), paintMeigara.GetPtHigh(0), paintMeigara.GetPtLow(0), paintMeigara.GetPtClose(0));
                _graphics.DrawString(_ValueString, _font, Brushes.Black, 400, 16);
            }

            ChartPaint chartPaint = new ChartPaint(chartBitmap) {
                StartPosition = FormStartPosition.Manual,
                Left = 0,
                Top = 0,
            };

            chartPaint.Show();

            chartBitmap.Dispose();
            paintChart.Dispose();
            paintMeigara.Dispose();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// ショートカットキー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Left:
                    if (MyMeigara != null) {
                        System.Windows.Forms.Cursor _Cursor = this.Cursor;
                        this.Cursor = Cursors.WaitCursor;
                        MyMeigara.ShiftKabukaTable(-1);
                        DrawTable(MyMeigara, chart1);
                        this.Cursor = _Cursor;
                    }
                    break;
                case Keys.Right:
                    if (MyMeigara != null) {
                        System.Windows.Forms.Cursor _Cursor = this.Cursor;
                        this.Cursor = Cursors.WaitCursor;
                        MyMeigara.ShiftKabukaTable(1);
                        DrawTable(MyMeigara, chart1);
                        this.Cursor = _Cursor;
                    }
                    break;
                case Keys.D1:
                    if (e.Control == true) {
                        SizeCheckOnClick(CandleSizes.XS);
                        SizeChange();
                    }
                    break;
                case Keys.D2:
                    if (e.Control == true) {
                        SizeCheckOnClick(CandleSizes.S);
                        SizeChange();
                    }
                    break;
                case Keys.D3:
                    if (e.Control == true) {
                        SizeCheckOnClick(CandleSizes.M);
                        SizeChange();
                    }
                    break;
                case Keys.D4:
                    if (e.Control == true) {
                        SizeCheckOnClick(CandleSizes.L);
                        SizeChange();
                    }
                    break;
                case Keys.D5:
                    if (e.Control == true) {
                        SizeCheckOnClick(CandleSizes.XL);
                        SizeChange();
                    }
                    break;
            }

            switch (e.KeyCode) {
                case Keys.D:
                    if (e.Control == true) {
                        ChangeChartScaleDisplay(ChartScales.Daily);
                        MakeTableAndDraw();
                    }
                    break;
                case Keys.W:
                    if (e.Control == true) {
                        ChangeChartScaleDisplay(ChartScales.Weekly);
                        MakeTableAndDraw();
                    }
                    break;
                case Keys.M:
                    if (e.Control == true) {
                        ChangeChartScaleDisplay(ChartScales.Monthly);
                        MakeTableAndDraw();
                    }
                    break;
                case Keys.H:
                    if (e.Control == true) 
                        toolStripTextBox1.Focus();
                    break;
                case Keys.O:
                    if (e.Control == true)
                        OkiniiriRead();
                    break;
                case Keys.S:
                    if (e.Control == true)
                        OkiniiriSave();
                    break;
                case Keys.A:
                    if (e.Control == true)
                        OkiniiriSaveAs();
                    break;
                case Keys.E:
                    if (e.Control == true) 
                        ExportToPDF();
                    break;
                case Keys.P:
                    if (e.Control == true)
                        PrintProc();
                    break;
                case Keys.Z:
                    if (e.Control == true)
                        PaintProc();
                    break;
                case Keys.X:
                    if (e.Control == true)
                        this.Close();
                    break;
            }
        }

        /// <summary>
        /// お気に入りリストの読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e) {
            OkiniiriRead();
        }

        /// <summary>
        /// お気に入りリストの読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButton12_Click(object sender, EventArgs e) {
            OkiniiriRead();
        }

        /// <summary>
        /// お気に入りリストの上書き保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e) {
            OkiniiriSave();
        }


        /// <summary>
        /// お気に入りリストの保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            OkiniiriSaveAs();
        }

        /// <summary>
        /// お気に入りリストの読み込み
        /// </summary>
        private void OkiniiriRead() {
            // Displays an OpenFileDialog so the user can select a Cursor.
            openFileDialog1.Filter = "お気に入り銘柄(*.sob;*.csv)|*.sob;*.csv";
            openFileDialog1.Title = "お気に入り銘柄ファイルの選択";
            openFileDialog1.FileName = "お気に入り.sob";

            // Show the Dialog.
            // If the user clicked OK in the dialog and
            // a .CUR file was selected, open it.
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                // Assign the cursor in the Stream to the Form's Cursor property.
                using (System.IO.StreamReader sr =
                        new System.IO.StreamReader(openFileDialog1.FileName, Encoding.GetEncoding("Shift_JIS"))) {
                    var codes = new List<string>();
                    while (!sr.EndOfStream) {
                        string[] col = sr.ReadLine().Split(',');
                        codes.Add(col[0]);
                    }
                    sr.Close();

                    int n = MyMeigaraList.FavoriteRead(codes);
                    OkiniiriFileName = openFileDialog1.FileName;
                }

                tabControl1.SelectedIndex = 1;
                if (MyMeigaraList.FavoritetableCount > 0)
                    NewMeigaraMakeAndDraw(MyMeigaraList.GetFavoriteTableCode(0));
            }
        }

        /// <summary>
        /// お気に入りリストの上書き
        /// </summary>
        private void OkiniiriSave() {
            if (bSave.Enabled) {
                // ファイル名が登録されていないなら名前を付けて保存
                if (OkiniiriFileName.Length == 0)
                    OkiniiriSaveAs();
                else
                    try {
                        using (System.IO.StreamWriter sw =
                                    new System.IO.StreamWriter(OkiniiriFileName, false, Encoding.GetEncoding("Shift_JIS"))) {

                            foreach (DataRow row in MyMeigaraList.FavoriteTable.Rows) {
                                for (int i = 0; i < MyMeigaraList.FavoriteTableColumnsCount; i++) {
                                    string _field = row[i].ToString();
                                    _field = EncloseDoubleQuotesIfNeed(_field);
                                    sw.Write(_field);
                                    if ((MyMeigaraList.FavoritetableCount - 1) > i)
                                        sw.Write(',');
                                }
                                sw.Write("\n");
                            }
                        }
                        bSave.Enabled = false;

                    } catch (Exception ex) {
                        Debug.WriteLine("File Save Error");
                    }
            }
        }

        /// <summary>
        /// お気に入りリストの保存
        /// </summary>
        private void OkiniiriSaveAs() {
            saveFileDialog1.Filter = "銘柄ファイル(*.sob;*.csv)|*.sob;*.csv";
            saveFileDialog1.Title = "お気に入り銘柄ファイルの保存";
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.FileName = "お気に入り.sob";

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                using (System.IO.StreamWriter sw =
                    new System.IO.StreamWriter(saveFileDialog1.FileName, false, Encoding.GetEncoding("Shift_JIS"))) {

                    foreach (DataRow row in MyMeigaraList.FavoriteTable.Rows) {
                        for (int i = 0; i < MyMeigaraList.FavoriteTableColumnsCount; i++) {
                            string _field = row[i].ToString();
                            _field = EncloseDoubleQuotesIfNeed(_field);
                            sw.Write(_field);
                            if ((MyMeigaraList.FavoritetableCount - 1) > i)
                                sw.Write(',');
                        }
                        sw.Write("\n");
                    }
                }
                OkiniiriFileName = saveFileDialog1.FileName;
                bSave.Enabled = false;
            }
        }

        /// <summary>
        /// 必要ならば、文字列をダブルクォートで囲む
        /// </summary>
        private string EncloseDoubleQuotesIfNeed(string field) {
            if (NeedEncloseDoubleQuotes(field)) {
                return EncloseDoubleQuotes(field);
            }
            return field;
        }

        /// <summary>
        /// 文字列をダブルクォートで囲む
        /// </summary>
        private string EncloseDoubleQuotes(string field) {
            if (field.IndexOf('"') > -1) {
                //"を""とする
                field = field.Replace("\"", "\"\"");
            }
            return "\"" + field + "\"";
        }

        /// <summary>
        /// 検索文字列入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripTextBox1_TextChanged(object sender, EventArgs e) {
            MyMeigaraList.SearchMeigaraList(toolStripTextBox1.Text);
            dataGridView1.DataSource = MyMeigaraList.SearchTable;
            if (tabControl1.SelectedIndex > 0) {
                tabControl1.SelectedIndex = 0;
                toolStripTextBox1.Focus();
            }
        }

        /// <summary>
        /// 文字列をダブルクォートで囲む必要があるか調べる
        /// </summary>
        private bool NeedEncloseDoubleQuotes(string field) {
            return field.IndexOf('"') > -1 ||
                field.IndexOf(',') > -1 ||
                field.IndexOf('\r') > -1 ||
                field.IndexOf('\n') > -1 ||
                field.StartsWith(" ") ||
                field.StartsWith("\t") ||
                field.EndsWith(" ") ||
                field.EndsWith("\t");
        }

        /// <summary>
        /// お気に入りリストの追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButton14_Click(object sender, EventArgs e) {
            if (MyMeigara != null)
                if (MyMeigaraList.FavoriteAdd(MyMeigara.Code, MeigaraList.Proc.ADD)) {
                    dataGridView2.CurrentCell = dataGridView2[0, dataGridView2.Rows.Count - 1];
                    bSave.Enabled = true;
                }
        }

        private void AddFavoriteToolStripMenuItem_Click(object sender, EventArgs e) {
            if (MyMeigara != null)
                if (MyMeigaraList.FavoriteAdd(MyMeigara.Code, MeigaraList.Proc.ADD)) {
                    dataGridView2.CurrentCell = dataGridView2[0, dataGridView2.Rows.Count - 1];
                    bSave.Enabled = true;
                }
        }

        /// <summary>
        /// お気に入り全削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearAllToolStripMenuItem_Click(object sender, EventArgs e) {
            if (MyMeigara != null) {
                MyMeigaraList.FavoriteClear();
                bSave.Enabled = true;
            }
        }

        /// <summary>
        /// お気に入りリストから削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButton15_Click(object sender, EventArgs e) {
            if (MyMeigara != null) {
                MyMeigaraList.FavoriteSub(MyMeigara.Code);
                bSave.Enabled = true;
            }
        }

        private void RemoveFavoriteToolStripMenuItem_Click(object sender, EventArgs e) {
            if (MyMeigara != null) {
                MyMeigaraList.FavoriteSub(MyMeigara.Code);
                bSave.Enabled = true;
            }
        }


        /// <summary>
        /// お気に入りリストの移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveTopToolStripMenuItem_Click(object sender, EventArgs e) {
            var pos = dataGridView2.SelectedRows[0].Index;
            string code = MyMeigaraList.GetFavoriteTableCode(dataGridView2.SelectedRows[0].Index);
            MyMeigaraList.FavoriteSub(code);
            MyMeigaraList.FavoriteAdd(code, MeigaraList.Proc.INSERT);
            if ((MyMeigaraList.FavoritetableCount - 1) > pos)
                dataGridView2.Rows[pos + 1].Selected = true;
            else
                dataGridView2.Rows[pos].Selected = true;
            bSave.Enabled = true;
        }

        /// <summary>
        /// お気に入りの削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e) {
            var pos = dataGridView2.SelectedRows[0].Index;
            string code = MyMeigaraList.GetFavoriteTableCode(pos);
            MyMeigaraList.FavoriteSub(code);
            if (MyMeigaraList.FavoritetableCount > pos)
                dataGridView2.Rows[pos].Selected = true;
            else
                dataGridView2.Rows[pos - 1].Selected = true;

            bSave.Enabled = true;
        }

        /// <summary>
        /// マウスホイールによる日付移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chart1_MouseWheel(object sender, MouseEventArgs e) {

            if (MyMeigara != null) {
                if ((e.Delta * SystemInformation.MouseWheelScrollLines / 120) < 0) {

                    MyMeigara.ShiftKabukaTable(1);
                    DrawTable(MyMeigara, chart1);
                } else if ((e.Delta * SystemInformation.MouseWheelScrollLines / 120) > 0) {
                    MyMeigara.ShiftKabukaTable(-1);
                    DrawTable(MyMeigara, chart1);
                }

            }
        }

        // マウスポインタの位置を保存する
        private Point MousePoint;
        private bool IsMouseDown = false;

        //マウスのボタンが押されたとき
        private void Form1_MouseDown(object sender,
            System.Windows.Forms.MouseEventArgs e) {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left && IsMouseDown == false) {
                //位置を記憶する
                MousePoint = new Point(e.X, e.Y);
                IsMouseDown = true;
                //カーソルの形を変更
                this.Cursor = Cursors.SizeWE;
            }
        }

        //マウスのボタンが離れたとき
        private void Form1_MouseUp(object sender,
            System.Windows.Forms.MouseEventArgs e) {
            if (IsMouseDown) {
                IsMouseDown = false;
                //カーソルの形を変更
                this.Cursor = Cursors.Default;
            }
        }

        //マウスが動いたとき
        private void Form1_MouseMove(object sender,
            System.Windows.Forms.MouseEventArgs e) {

            Func<MouseEventArgs, int, int, bool> dragMouse;

            if (MyMeigara != null) {
                if ((e.Button & MouseButtons.Left) == MouseButtons.Left) {

                    if ((e.X - MousePoint.X) > 0)
                        dragMouse = DragMoveRewind;
                    else
                        dragMouse = DragMoveForward;

                    if (dragMouse(e, 12, 5)) { } else if (dragMouse(e, 8, 3)) { } else if (dragMouse(e, 4, 2)) { } else if (dragMouse(e, 2, 1)) { }
                }
            }
        }

        private bool DragMoveForward(System.Windows.Forms.MouseEventArgs e, int mouseMove, int chartMove) {
            if (IsMouseDown && Math.Abs(e.X - MousePoint.X) > mouseMove) {
                MyMeigara.ShiftKabukaTable(chartMove);
                DrawTable(MyMeigara, chart1);
                MousePoint = new Point(e.X, e.Y);
                return true;
            }
            return false;
        }

        private bool DragMoveRewind(System.Windows.Forms.MouseEventArgs e, int mouseMove, int chartMove) {
            if (IsMouseDown && Math.Abs(e.X - MousePoint.X) > mouseMove) {
                MyMeigara.ShiftKabukaTable(-1 * chartMove);
                DrawTable(MyMeigara, chart1);
                MousePoint = new Point(e.X, e.Y);
                return true;
            }
            return false;
        }
    }
}