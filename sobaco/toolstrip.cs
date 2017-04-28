﻿using System;
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

namespace sobaco {
    public partial class MainForm : Form {

        /// <summary>
        /// 移動平均線を５、２０、６０、１００、３００に変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AibaToolStripMenuItem_CheckedChanged(object sender, EventArgs e) {
            AveStepChangeOnClick(AveSteps.Aiba);
            if (MyMeigara != null)
                NewMeigaraMakeAndDraw(MyMeigara.Code);
        }

        /// <summary>
        /// 移動平均線を５、２５、７５、２００に変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NormalToolStripMenuItem_CheckedChanged(object sender, EventArgs e) {
            AveStepChangeOnClick(AveSteps.Normal);
            if (MyMeigara != null)
                NewMeigaraMakeAndDraw(MyMeigara.Code);
        }

        private void NormalToolStripMenuItem_Click(object sender, EventArgs e) {
            AveStepChangeOnClick(AveSteps.Normal);
            if (MyMeigara != null)
                NewMeigaraMakeAndDraw(MyMeigara.Code);
        }

        private void AibaToolStripMenuItem_Click(object sender, EventArgs e) {
            AveStepChangeOnClick(AveSteps.Aiba);
            if (MyMeigara != null)
                NewMeigaraMakeAndDraw(MyMeigara.Code);
        }

        private void AveStepChangeOnClick(AveSteps aveStep) {
            this.NormalToolStripMenuItem.CheckedChanged -= new System.EventHandler(this.NormalToolStripMenuItem_CheckedChanged);
            this.AibaToolStripMenuItem.CheckedChanged -= new System.EventHandler(this.AibaToolStripMenuItem_CheckedChanged);

            AveStep = aveStep;

            switch (aveStep) {
                case AveSteps.Normal:
                    NormalToolStripMenuItem.Checked = true;
                    AibaToolStripMenuItem.Checked = false;
                    NormalToolStripMenuItem.CheckOnClick = false;
                    AibaToolStripMenuItem.CheckOnClick = true;
                    標準ToolStripMenuItem.Checked = true;
                    相場師朗式ToolStripMenuItem.Checked = false;
                    break;
                case AveSteps.Aiba:
                    AibaToolStripMenuItem.Checked = true;
                    NormalToolStripMenuItem.Checked = false;
                    AibaToolStripMenuItem.CheckOnClick = false;
                    NormalToolStripMenuItem.CheckOnClick = true;
                    標準ToolStripMenuItem.Checked = false;
                    相場師朗式ToolStripMenuItem.Checked = true;
                    break;
            }

            this.NormalToolStripMenuItem.CheckedChanged += new System.EventHandler(this.NormalToolStripMenuItem_CheckedChanged);
            this.AibaToolStripMenuItem.CheckedChanged += new System.EventHandler(this.AibaToolStripMenuItem_CheckedChanged);
        }


        /// <summary>
        /// 1ページ前に移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButton1_Click(object sender, EventArgs e) {
            if (MyMeigara != null) {
                System.Windows.Forms.Cursor _Cursor = this.Cursor;
                this.Cursor = Cursors.WaitCursor;
                MyMeigara.ShiftKabukaTable(-1 * CandleSizeList[(int)CandleSize].Days);
                DrawTable(MyMeigara, chart1);
                this.Cursor = _Cursor;
            }
        }

        /// <summary>
        /// １日前に移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButton2_Click(object sender, EventArgs e) {
            if (MyMeigara != null) {
                System.Windows.Forms.Cursor _Cursor = this.Cursor;
                this.Cursor = Cursors.WaitCursor;
                MyMeigara.ShiftKabukaTable(-1);
                DrawTable(MyMeigara, chart1);
                this.Cursor = _Cursor;
            }
        }

        /// <summary>
        /// １日後に移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButton3_Click(object sender, EventArgs e) {
            if (MyMeigara != null) {
                System.Windows.Forms.Cursor _Cursor = this.Cursor;
                this.Cursor = Cursors.WaitCursor;
                MyMeigara.ShiftKabukaTable(1);
                DrawTable(MyMeigara, chart1);
                this.Cursor = _Cursor;
            }
        }

        /// <summary>
        /// １ページ後へ移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButton4_Click(object sender, EventArgs e) {
            if (MyMeigara != null) {
                System.Windows.Forms.Cursor _Cursor = this.Cursor;
                this.Cursor = Cursors.WaitCursor;
                MyMeigara.ShiftKabukaTable(CandleSizeList[(int)CandleSize].Days);
                DrawTable(MyMeigara, chart1);
                this.Cursor = _Cursor;
            }
        }

        /// <summary>
        /// 表示幅の変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void XLToolStripMenuItem_CheckedChanged(object sender, EventArgs e) {
            SizeCheckOnClick(CandleSizes.XL);
            SizeChange();
        }

        private void LToolStripMenuItem_CheckedChanged(object sender, EventArgs e) {
            SizeCheckOnClick(CandleSizes.L);
            SizeChange();
        }

        private void MToolStripMenuItem_CheckedChanged(object sender, EventArgs e) {
            SizeCheckOnClick(CandleSizes.M);
            SizeChange();
        }

        private void SToolStripMenuItem_CheckedChanged(object sender, EventArgs e) {
            SizeCheckOnClick(CandleSizes.S);
            SizeChange();
        }

        private void XSToolStripMenuItem_CheckedChanged(object sender, EventArgs e) {
            SizeCheckOnClick(CandleSizes.XS);
            SizeChange();
        }

        private void XLToolStripMenuItem_Click(object sender, EventArgs e) {
            SizeCheckOnClick(CandleSizes.XL);
            SizeChange();
        }

        private void LToolStripMenuItem_Click(object sender, EventArgs e) {
            SizeCheckOnClick(CandleSizes.L);
            SizeChange();
        }

        private void MToolStripMenuItem_Click(object sender, EventArgs e) {
            SizeCheckOnClick(CandleSizes.M);
            SizeChange();
        }

        private void SToolStripMenuItem_Click(object sender, EventArgs e) {
            SizeCheckOnClick(CandleSizes.S);
            SizeChange();
        }

        private void XSToolStripMenuItem_Click(object sender, EventArgs e) {
            SizeCheckOnClick(CandleSizes.XS);
            SizeChange();
        }

        /// <summary>
        /// 移動平均線表示フラグの変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButton6_Click(object sender, EventArgs e) {
            IdouheikinLine1 = CycleSw(IdouheikinLine1, 3);
            DrawTable(MyMeigara, chart1);
        }

        private void ToolStripButton7_Click(object sender, EventArgs e) {
            IdouheikinLine2 = CycleSw(IdouheikinLine2, 3);
            DrawTable(MyMeigara, chart1);
        }

        private void ToolStripButton8_Click(object sender, EventArgs e) {
            IdouheikinLine3 = CycleSw(IdouheikinLine3, 3);
            DrawTable(MyMeigara, chart1);
        }

        private void ToolStripButton9_Click(object sender, EventArgs e) {
            IdouheikinLine4 = CycleSw(IdouheikinLine4, 3);
            DrawTable(MyMeigara, chart1);
        }

        private void ToolStripButton10_Click(object sender, EventArgs e) {
            IdouheikinLine5 = CycleSw(IdouheikinLine5, 3);
            DrawTable(MyMeigara, chart1);
        }

        /// <summary>
        /// 0からcycle-1 の数字を繰り返す
        /// </summary>
        /// <param name="n"></param>
        /// <param name="cycle"></param>
        /// <returns></returns>
        private int CycleSw(int n, int cycle) {
            n++;
            if (n > (cycle - 1))
                return 0;
            return n;
        }

        private void ToolStripButton11_Click(object sender, EventArgs e) {
            ChartStyle = (ChartStyles)CycleSw((int)ChartStyle, 3);
            DrawTable(MyMeigara, chart1);
        }

        /// <summary>
        /// 表示幅の設定
        /// </summary>
        /// <param name="size"></param>
        private void SizeCheckOnClick(CandleSizes size) {
            this.XLToolStripMenuItem.CheckedChanged -= new System.EventHandler(this.XLToolStripMenuItem_CheckedChanged);
            this.LToolStripMenuItem.CheckedChanged -= new System.EventHandler(this.LToolStripMenuItem_CheckedChanged);
            this.MToolStripMenuItem.CheckedChanged -= new System.EventHandler(this.MToolStripMenuItem_CheckedChanged);
            this.SToolStripMenuItem.CheckedChanged -= new System.EventHandler(this.SToolStripMenuItem_CheckedChanged);
            this.XSToolStripMenuItem.CheckedChanged -= new System.EventHandler(this.XSToolStripMenuItem_CheckedChanged);

            XLToolStripMenuItem.CheckOnClick = true;
            LToolStripMenuItem.CheckOnClick = true;
            MToolStripMenuItem.CheckOnClick = true;
            SToolStripMenuItem.CheckOnClick = true;
            XSToolStripMenuItem.CheckOnClick = true;

            XLToolStripMenuItem.Checked = false;
            LToolStripMenuItem.Checked = false;
            MToolStripMenuItem.Checked = false;
            SToolStripMenuItem.Checked = false;
            XSToolStripMenuItem.Checked = false;

            最大ToolStripMenuItem.Checked = false;
            大ToolStripMenuItem.Checked = false;
            中ToolStripMenuItem.Checked = false;
            小ToolStripMenuItem.Checked = false;
            最小ToolStripMenuItem.Checked = false;

            switch (size) {
                case CandleSizes.XL:
                    XLToolStripMenuItem.Checked = true;
                    XLToolStripMenuItem.CheckOnClick = false;
                    最大ToolStripMenuItem.Checked = true;
                    break;
                case CandleSizes.L:
                    LToolStripMenuItem.Checked = true;
                    LToolStripMenuItem.CheckOnClick = false;
                    大ToolStripMenuItem.Checked = true;
                    break;
                case CandleSizes.M:
                    MToolStripMenuItem.Checked = true;
                    MToolStripMenuItem.CheckOnClick = false;
                    中ToolStripMenuItem.Checked = true;
                    break;
                case CandleSizes.S:
                    SToolStripMenuItem.Checked = true;
                    SToolStripMenuItem.CheckOnClick = false;
                    小ToolStripMenuItem.Checked = true;
                    break;
                case CandleSizes.XS:
                    XSToolStripMenuItem.Checked = true;
                    XSToolStripMenuItem.CheckOnClick = false;
                    最小ToolStripMenuItem.Checked = true;
                    break;
            }
            CandleSize = size;

            this.XLToolStripMenuItem.CheckedChanged += new System.EventHandler(this.XLToolStripMenuItem_CheckedChanged);
            this.LToolStripMenuItem.CheckedChanged += new System.EventHandler(this.LToolStripMenuItem_CheckedChanged);
            this.MToolStripMenuItem.CheckedChanged += new System.EventHandler(this.MToolStripMenuItem_CheckedChanged);
            this.SToolStripMenuItem.CheckedChanged += new System.EventHandler(this.SToolStripMenuItem_CheckedChanged);
            this.XSToolStripMenuItem.CheckedChanged += new System.EventHandler(this.XSToolStripMenuItem_CheckedChanged);
        }

        /// <summary>
        /// 表示幅の変更
        /// </summary>
        /// <param name="candleSize"></param>
        private void SizeChange() {
            if (MyMeigara != null) {
                System.Windows.Forms.Cursor _Cursor = this.Cursor;
                this.Cursor = Cursors.WaitCursor;
                // テーブル作成
                MyMeigara.MakeTable(MyMeigara.PlotEnd,
                            CandleSizeList[(int)CandleSize].Days,
                            ChartScale,
                            AveStep);
                // 描画
                DrawTable(MyMeigara, chart1);
                this.Cursor = _Cursor;
            }
        }


        /// <summary>
        /// 日足表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HiashiButton_Click(object sender, EventArgs e) {
            ChangeChartScaleDisplay(ChartScales.Daily);
            MakeTableAndDraw();
        }

        /// <summary>
        /// 週足表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyuashiButton_Click(object sender, EventArgs e) {
            ChangeChartScaleDisplay(ChartScales.Weekly);
            MakeTableAndDraw();
        }

        /// <summary>
        /// 月足表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TukiashiButton_Click(object sender, EventArgs e) {
            ChangeChartScaleDisplay(ChartScales.Monthly);
            MakeTableAndDraw();
        }


        private void DailyToolStripMenuItem_Click(object sender, EventArgs e) {
            ChangeChartScaleDisplay(ChartScales.Daily);
            MakeTableAndDraw();
        }

        private void WeeklyToolStripMenuItem_Click(object sender, EventArgs e) {
            ChangeChartScaleDisplay(ChartScales.Weekly);
            MakeTableAndDraw();
        }

        private void MonthlyToolStripMenuItem_Click(object sender, EventArgs e) {
            ChangeChartScaleDisplay(ChartScales.Monthly);
            MakeTableAndDraw();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chartScale"></param>
        private void MakeTableAndDraw() {
            if (MyMeigara != null) {
                MyMeigara.MakeTable(MyMeigara.PlotEnd,
                            MyMeigara.PlotWide.Value,
                            ChartScale,
                            AveStep);
                DrawTable(MyMeigara, chart1);
            }
        }

        private void MakeTableAndDraw(int plotEnd) {
            if (MyMeigara != null) {
                MyMeigara.MakeTable(plotEnd,
                            MyMeigara.PlotWide.Value,
                            ChartScale,
                            AveStep);
                DrawTable(MyMeigara, chart1);
            }
        }

        /// <summary>
        /// ツールストリップボタンの日足、週足、月足の表示設定
        /// </summary>
        /// <param name="chartScale"></param>
        private void ChangeChartScaleDisplay(ChartScales chartScale) {
            HiashiButton.BackColor = SystemColors.Control;
            HiashiButton.ForeColor = SystemColors.ControlText;
            SyuashiButton.BackColor = SystemColors.Control;
            SyuashiButton.ForeColor = SystemColors.ControlText;
            TukiashiButton.BackColor = SystemColors.Control;
            TukiashiButton.ForeColor = SystemColors.ControlText;
            日足ToolStripMenuItem.Checked = false;
            週足ToolStripMenuItem.Checked = false;
            月足ToolStripMenuItem.Checked = false;

            ChartScale = chartScale;

            switch (chartScale) {
                case ChartScales.Daily:
                    HiashiButton.BackColor = SystemColors.Highlight;
                    HiashiButton.ForeColor = SystemColors.ControlLightLight;
                    日足ToolStripMenuItem.Checked = true;
                    break;
                case ChartScales.Weekly:
                    SyuashiButton.BackColor = SystemColors.Highlight;
                    SyuashiButton.ForeColor = SystemColors.ControlLightLight;
                    週足ToolStripMenuItem.Checked = true;
                    break;
                case ChartScales.Monthly:
                    TukiashiButton.BackColor = SystemColors.Highlight;
                    TukiashiButton.ForeColor = SystemColors.ControlLightLight;
                    月足ToolStripMenuItem.Checked = true;
                    break;
            }
        }

        /// <summary>
        /// 日付選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalendarDialogStripButton5_Click(object sender, EventArgs e) {
            ActiveMarket.Calendar calendar = new ActiveMarket.Calendar();
            DateTime _date;
            int _pos;

            if (MyMeigara != null) {
                CalendarDialog _CalendarDialog = new CalendarDialog(
                                                        MyMeigara.GetPriceBeginDate(),
                                                        MyMeigara.GetPriceEndDate(),
                                                        MyMeigara.PlotEndDate) {
                    StartPosition = FormStartPosition.Manual,
                    Left = this.Left + 340,
                    Top = this.Top + 80,
                    Owner = this
                };
                _CalendarDialog.ShowDialog(this);

                _date = _CalendarDialog.SelectedDate;

                if (_date != DateTime.Parse("0001/01/01")) {
                    try {
                        _pos = calendar.DatePosition(_date, -1);
                    } catch {
                        _pos = calendar.DatePosition(_date, 1);
                    }

                    MakeTableAndDraw(_pos);
                }

                _CalendarDialog.Dispose();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            About _about = new About();
            _about.ShowDialog();
            _about.Dispose();
        }

        private void bSave_Click(object sender, EventArgs e) {
            OkiniiriSave();
        }

        private void bSaveAs_Click(object sender, EventArgs e) {
            OkiniiriSaveAs();
        }
    }
}