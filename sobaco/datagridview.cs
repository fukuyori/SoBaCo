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

namespace sobaco {

    public partial class MainForm : Form {

        //
        // DataGridViewのドラッグ&ドロップでデータ（行）を移動
        //
        // https://social.msdn.microsoft.com/Forums/officeapps/ja-JP/e59a4043-6298-4653-809f-5c8fcf04f2e6/datagridview?forum=csharpgeneralja
        //
        // Cell 上で Drag を始めたのか、
        // 列幅変更時の Drag で Cell 領域に入ったのかを区別するためのフラグ
        private int OwnBeginGrabRowIndex = -1;

        private void DataGridView2_MouseDown(object sender, MouseEventArgs e) {
            OwnBeginGrabRowIndex = -1;

            if ((e.Button & MouseButtons.Left) != MouseButtons.Left) return;

            DataGridView.HitTestInfo hit = dataGridView2.HitTest(e.X, e.Y);
            if (hit.Type != DataGridViewHitTestType.Cell) return;

            // クリック時などは -1 に戻らないが問題なし
            OwnBeginGrabRowIndex = hit.RowIndex;

        }

        private void DataGridView2_MouseMove(object sender, MouseEventArgs e) {
            if ((e.Button & MouseButtons.Left) != MouseButtons.Left) return;
            if (OwnBeginGrabRowIndex == -1) return;

            // ドラッグ＆ドロップの開始
            dataGridView2.DoDragDrop(OwnBeginGrabRowIndex, DragDropEffects.Move);
        }

        private bool DropDestinationIsValid;
        private int DropDestinationRowIndex;
        private bool DropDestinationIsNextRow;

        private void DataGridView2_DragOver(object sender, DragEventArgs e) {
            e.Effect = DragDropEffects.Move;

            bool _valid = DecideDropDestinationRowIndex(
                dataGridView2, e, out int _from, out int _to, out bool _next);

            // ドロップ先マーカーの表示・非表示の制御
            bool needRedRaw = (_valid != DropDestinationIsValid);
            if (_valid) {
                needRedRaw = needRedRaw
                    || (_to != DropDestinationRowIndex)
                    || (_next != DropDestinationIsNextRow);
            }

            if (needRedRaw) {
                if (DropDestinationIsValid)
                    dataGridView2.InvalidateRow(DropDestinationRowIndex);
                if (_valid)
                    dataGridView2.InvalidateRow(_to);
            }

            DropDestinationIsValid = _valid;
            DropDestinationRowIndex = _to;
            DropDestinationIsNextRow = _next;
        }

        private void DataGridView2_DragLeave(object sender, EventArgs e) {
            if (DropDestinationIsValid) {
                DropDestinationIsValid = false;
                dataGridView2.InvalidateRow(DropDestinationRowIndex);
            }
        }

        private void DataGridView2_DragDrop(object sender, DragEventArgs e) {
            if (!DecideDropDestinationRowIndex(
                    dataGridView2, e, out int _from, out int _to, out bool _next))
                return;

            DropDestinationIsValid = false;

            // データの移動
            _to = MoveDataValue(_from, _to, _next);

            dataGridView2.CurrentCell =
                dataGridView2[dataGridView2.CurrentCell.ColumnIndex, _to];

            dataGridView2.Invalidate();
        }

        private void DataGridView2_RowPostPaint(
            object sender, DataGridViewRowPostPaintEventArgs e) {
            // ドロップ先のマーカーを描画
            if (DropDestinationIsValid
                && e.RowIndex == DropDestinationRowIndex) {
                using (Pen _pen = new Pen(Color.Red, 4)) {
                    int y =
                        !DropDestinationIsNextRow
                        ? e.RowBounds.Y + 2 : e.RowBounds.Bottom - 2;

                    e.Graphics.DrawLine(
                        _pen, e.RowBounds.X, y, e.RowBounds.X + 50, y);
                }
            }
        }

        // ドロップ先の行の決定
        private bool DecideDropDestinationRowIndex(
            DataGridView _dataGridView, DragEventArgs e,
            out int _from, out int _to, out bool _next) {
            _from = (int)e.Data.GetData(typeof(int));
            // 元の行が追加用の行であれば、常に false
            if (_dataGridView.NewRowIndex != -1 && _dataGridView.NewRowIndex == _from) {
                _to = 0; _next = false;
                return false;
            }

            Point clientPoint = _dataGridView.PointToClient(new Point(e.X, e.Y));
            // 上下のみに着目するため、横方向は無視する
            clientPoint.X = 1;
            DataGridView.HitTestInfo hit =
                _dataGridView.HitTest(clientPoint.X, clientPoint.Y);

            _to = hit.RowIndex;
            if (_to == -1) {
                int _Top = _dataGridView.ColumnHeadersVisible ? _dataGridView.ColumnHeadersHeight : 0;
                _Top += 1; // ...

                if (_Top > clientPoint.Y)
                    // ヘッダへのドロップ時は表示中の先頭行とする
                    _to = _dataGridView.FirstDisplayedCell.RowIndex;
                else
                    // 最終行へ
                    _to = _dataGridView.Rows.Count - 1;
            }

            // 追加用の行は無視
            if (_to == _dataGridView.NewRowIndex) _to--;



            _next = (_to > _from);
            return (_from != _to);
        }

        // データの移動
        private int MoveDataValue(int from, int to, bool next) {
            DataTable dt = (DataTable)dataGridView2.DataSource;

            // 移動するデータの退避（計算列があればたぶんダメ）
            object[] rowData = dt.Rows[from].ItemArray;
            DataRow row = dt.NewRow();
            row.ItemArray = rowData;

            // 移動元から削除
            dt.Rows.RemoveAt(from);
            if (to > from) to--;

            // 移動先へ追加
            if (next) to++;
            if (to <= dt.Rows.Count)
                dt.Rows.InsertAt(row, to);
            else
                dt.Rows.Add(row);

            // お気に入りに変更有
            bSave.Enabled = true;

            return dt.Rows.IndexOf(row);
        }
    }
}