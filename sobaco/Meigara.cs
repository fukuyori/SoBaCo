using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ActiveMarket;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace sobaco {
    // 日付取得の方向
    enum Directions { Befor, After };

    public class Meigara : IDisposable {

        private class KabukaRow {
            public int StartPos { get; set; }
            public int EndPos { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public double High { get; set; }
            public double Low { get; set; }
            public double Open { get; set; }
            public double Close { get; set; }
            public double Heikin1 { get; set; }
            public double Heikin2 { get; set; }
            public double Heikin3 { get; set; }
            public double Heikin4 { get; set; }
            public double Heikin5 { get; set; }
        }

        // 証券コード
        public string Code { get; private set; }
        // 会社名
        public string Name { get; private set; }
        // 株価テーブル
        public DataTable PriceTable;
        public DateTime GetPriceBeginDate() {
            ActiveMarket.Calendar calendar = new ActiveMarket.Calendar();
            return calendar.Date(Price.Begin());
        }
        public DateTime GetPriceEndDate() {
            ActiveMarket.Calendar calendar = new ActiveMarket.Calendar();
            return calendar.Date(Price.End());
        }
        // 日足、週足、月足
        public ChartScales ChartScale { get; private set; }
        // 移動平均線の刻み値
        public AveSteps AveStep { get; private set; }

        private List<List<List<int>>> IdouheikinScales;
        private ActiveMarket.Prices Price;
        private bool IsDisposed = false;
        SafeFileHandle handle = new SafeFileHandle(IntPtr.Zero, true); // dispose

        public int GetPtStartPos(int pos) { return (int)PriceTable.Rows[pos]["StartPos"]; }
        public int GetPtEndPos(int pos) { return (int)PriceTable.Rows[pos]["EndPos"]; }
        public DateTime GetPtStartDate(int pos) { return (DateTime)PriceTable.Rows[pos]["StartDate"]; }
        public DateTime GetPtEndDate(int pos) { return (DateTime)PriceTable.Rows[pos]["EndDate"]; }
        public string GetPtDateLabel(int pos) { return GetPtEndDate(pos).ToString("d"); }
        public double GetPtHigh(int pos) { return (double)PriceTable.Rows[pos]["High"]; }
        public double GetPtLow(int pos) { return (double)PriceTable.Rows[pos]["Low"]; }
        public double GetPtOpen(int pos) { return (double)PriceTable.Rows[pos]["Open"]; }
        public double GetPtClose(int pos) { return (double)PriceTable.Rows[pos]["Close"]; }

        public double GetPtHeikin1(int pos) { return (double)PriceTable.Rows[pos]["Heikin1"]; }
        public double GetPtHeikin2(int pos) { return (double)PriceTable.Rows[pos]["Heikin2"]; }
        public double GetPtHeikin3(int pos) { return (double)PriceTable.Rows[pos]["Heikin3"]; }
        public double GetPtHeikin4(int pos) { return (double)PriceTable.Rows[pos]["Heikin4"]; }
        public double GetPtHeikin5(int pos) { return (double)PriceTable.Rows[pos]["Heikin5"]; }
        public void SetPtHeikin1(int pos, double value) { PriceTable.Rows[pos]["Heikin1"] = value; }
        public void SetPtHeikin2(int pos, double value) { PriceTable.Rows[pos]["Heikin2"] = value; }
        public void SetPtHeikin3(int pos, double value) { PriceTable.Rows[pos]["Heikin3"] = value; }
        public void SetPtHeikin4(int pos, double value) { PriceTable.Rows[pos]["Heikin4"] = value; }
        public void SetPtHeikin5(int pos, double value) { PriceTable.Rows[pos]["Heikin5"] = value; }
        public void SetPtHeikin(int n, int pos, double value) {
            switch (n) {
                case 1:
                    SetPtHeikin1(pos, value);
                    break;
                case 2:
                    SetPtHeikin2(pos, value);
                    break;
                case 3:
                    SetPtHeikin3(pos, value);
                    break;
                case 4:
                    SetPtHeikin4(pos, value);
                    break;
                case 5:
                    SetPtHeikin5(pos, value);
                    break;
            }
        }


        public int PtCount {
            get { return PriceTable.Rows.Count; }
        }

        // 株価テーブル開始位置
        public int? PlotStart {
            get {
                if (PtCount > 0)
                    return GetPtStartPos(PtCount - 1);
                else
                    return null;
            }
        }
        public DateTime PlotStartDate {
            get {
                ActiveMarket.Calendar calendar = new ActiveMarket.Calendar();
                return calendar.Date(PlotStart ?? PlotEnd);
            }
        }
        // 株価テーブル終了位置
        public int PlotEnd {
            get {
                var calendar = new ActiveMarket.Calendar();
                if (PtCount > 0)
                    return GetPtEndPos(0);
                else
                    return calendar.DatePosition(DateTime.Today, -1);
            }
        }
        public DateTime PlotEndDate {
            get {
                ActiveMarket.Calendar calendar = new ActiveMarket.Calendar();
                return calendar.Date(PlotEnd);
            }
        }
        public int? PlotWide { get; private set; }

        // 株価テーブル内の最高値
        public double? KabukaHigh {
            get {
                if (PtCount > 0)
                    return PriceTable
                        .AsEnumerable()
                        .Select(n => n.Field<double>("HIGH"))
                        .Max();
                else
                    return null;
            }
        }
        // 株価テーブル内の最安値
        public double? KabukaLow {
            get {
                if (PtCount > 0)
                    return PriceTable
                            .AsEnumerable()
                            .Select(n => n.Field<double>("LOW"))
                            .Min();
                else
                    return null;
            }
        }
        // 株価のグリッド幅設定
        public double? KabukaGridInterval {
            get {
                if (KabukaHigh.HasValue & KabukaLow.HasValue) {
                    double n = KabukaHigh.Value - KabukaLow.Value;

                    if (n < 10) return 1;
                    else if (n < 20) return 2;
                    else if (n < 50) return 5;
                    else if (n < 100) return 10;
                    else if (n < 200) return 20;
                    else if (n < 500) return 50;
                    else if (n < 1000) return 100;
                    else if (n < 2000) return 200;
                    else if (n < 5000) return 500;
                    else if (n < 10000) return 1000;
                    else if (n < 20000) return 2000;
                    else if (n < 50000) return 5000;
                    else if (n < 100000) return 10000;
                    else if (n < 200000) return 20000;
                    return 50000;
                } else
                    return null;
            }
        }

        // 終値リスト
        private class Owarine {
            public int DatePos { get; private set; }
            public double Value { get; private set; }
            
            public Owarine(int datePos, double value) {
                this.DatePos = datePos;
                this.Value = value;
            }
        }
        private List<Owarine> OwarineList;


        /// <summary>
        /// 銘柄オブジェクトの作成
        /// </summary>
        /// <param name="code"></param>
        public Meigara(string code, List<List<List<int>>> idouheikin) {
            this.Price = new ActiveMarket.Prices();
            this.Code = code;
            this.Price.AdjustExRights = 1; // 権利落ち修正した値段を読み込みます
            Price.Read(code);
            this.Name = Price.Name();

            IdouheikinScales = idouheikin;


            // 株価テーブル設定
            PriceTable = new DataTable();
            PriceTable.Columns.Add("StartPos", typeof(int));
            PriceTable.Columns.Add("EndPos", typeof(int));
            PriceTable.Columns.Add("StartDate", typeof(DateTime));
            PriceTable.Columns.Add("EndDate", typeof(DateTime));
            PriceTable.Columns.Add("High", typeof(double));
            PriceTable.Columns.Add("Low", typeof(double));
            PriceTable.Columns.Add("Open", typeof(double));
            PriceTable.Columns.Add("Close", typeof(double));
            PriceTable.Columns.Add("Heikin1", typeof(double));
            PriceTable.Columns.Add("Heikin2", typeof(double));
            PriceTable.Columns.Add("Heikin3", typeof(double));
            PriceTable.Columns.Add("Heikin4", typeof(double));
            PriceTable.Columns.Add("Heikin5", typeof(double));
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (IsDisposed)
                return;
            if (disposing) {
                handle.Dispose();
            }
            IsDisposed = true;
        }

        /// <summary>
        /// 株価テーブルを過去、未来へずらす
        /// </summary>
        /// <param name="shift"></param>
        public bool ShiftKabukaTable(int shift) {
            var calendar = new ActiveMarket.Calendar();
            KabukaRow _kabukaRow = new KabukaRow();
            int? _datepos;

            if (shift < 0) {
                // 過去日付へ１つずらす
                _datepos = NextDatePosition(this.PlotStart.Value, Directions.Befor);
                if (!_datepos.HasValue)
                    return false;

                var cnt = 0;
                while (cnt < Math.Abs(shift)) {
                    _kabukaRow = GetOneRow(_datepos.Value);
                    if (_kabukaRow != null) {
                        DataRow row = PriceTable.NewRow();
                        row["StartPos"] = _kabukaRow.StartPos;
                        row["EndPos"] = _kabukaRow.EndPos;
                        row["StartDate"] = _kabukaRow.StartDate;
                        row["EndDate"] = _kabukaRow.EndDate;
                        row["High"] = _kabukaRow.High;
                        row["Low"] = _kabukaRow.Low;
                        row["Open"] = _kabukaRow.Open;
                        row["Close"] = _kabukaRow.Close;
                        row["Heikin1"] = 0;
                        row["Heikin2"] = 0;
                        row["Heikin3"] = 0;
                        row["Heikin4"] = 0;
                        row["Heikin5"] = 0;
                        PriceTable.Rows.RemoveAt(0);
                        PriceTable.Rows.Add(row);

                        // 終値リストを過去日付に１つずらす
                        OwarineList.RemoveAt(0);
                        var _OwarineDatePos = NextDatePosition(
                                                ((Owarine)OwarineList[OwarineList.Count - 1]).DatePos, Directions.Befor);
                        if (_OwarineDatePos.HasValue) 
                            OwarineList.Add(new Owarine(_OwarineDatePos.Value, Price.Close(_OwarineDatePos.Value)));
                        for (var j = 0; j < 5; j++) {
                            SetHeikin(PtCount - 1, j);
                        }

                        cnt++;
                    }
                    _datepos = NextDatePosition(_datepos.Value, Directions.Befor);
                    if (!_datepos.HasValue)
                        return true;
                }
            } else {
                // 未来日付に１つずらす
                _datepos = NextDatePosition(this.PlotEnd, Directions.After);
                if (!_datepos.HasValue)
                    return false;

                var cnt = 0;
                while (cnt < Math.Abs(shift)) {
                    _kabukaRow = GetOneRow(_datepos.Value);
                    if (_kabukaRow != null) {
                        DataRow row = PriceTable.NewRow();
                        row["StartPos"] = _kabukaRow.StartPos;
                        row["EndPos"] = _kabukaRow.EndPos;
                        row["StartDate"] = _kabukaRow.StartDate;
                        row["EndDate"] = _kabukaRow.EndDate;
                        row["High"] = _kabukaRow.High;
                        row["Low"] = _kabukaRow.Low;
                        row["Open"] = _kabukaRow.Open;
                        row["Close"] = _kabukaRow.Close;
                        row["Heikin1"] = 0;
                        row["Heikin2"] = 0;
                        row["Heikin3"] = 0;
                        row["Heikin4"] = 0;
                        row["Heikin5"] = 0;
                        PriceTable.Rows.RemoveAt(PtCount - 1);
                        PriceTable.Rows.InsertAt(row, 0);

                        // 終値を未来日付にずらす
                        OwarineList.RemoveAt(OwarineList.Count - 1);
                        OwarineList.Insert(0, new Owarine(_kabukaRow.EndPos, _kabukaRow.Close));

                        for (var j = 0; j < 5; j++) {
                            SetHeikin(0, j);
                        }


                        cnt++;
                    }
                    _datepos = NextDatePosition(_datepos.Value, Directions.After);
                    if (!_datepos.HasValue)
                        return true;
                }
            }
            return true;
        }

        /// <summary>
        /// 1単位分のデータを作成
        /// </summary>
        /// <param name="datePos"></param>
        /// <returns>KabukaRowを返す。１日も開場日がなかったら、null</returns>
        private KabukaRow GetOneRow(int datePos) {
            var calendar = new ActiveMarket.Calendar();
            var _kabukaRow = new KabukaRow();
            var _date = calendar.Date(datePos);
            DateTime _startDate;
            DateTime _endDate;
            int _startPos;
            int _endPos;

            // 開始・終了位置決め
            // 週足の場合は、月曜から金曜
            switch (ChartScale) {
                case ChartScales.Weekly:
                    // 該当日の週の月曜日
                    _startDate = _date.AddDays((int)_date.DayOfWeek * -1 + 1);
                    if (_startDate < calendar.Date(Price.Begin()))
                        _startDate = calendar.Date(Price.Begin());
                    if (_startDate > calendar.Date(Price.End()))
                        return null;
                    // 該当日の週の金曜日
                    _endDate = _startDate.AddDays(4);
                    if (_endDate > calendar.Date(Price.End()))
                        _endDate = calendar.Date(Price.End());
                    if (_endDate < calendar.Date(Price.Begin()))
                        return null;
                    break;
                case ChartScales.Monthly:
                    // 月の1日
                    _startDate =
                        DateTime.Parse(_date.Year.ToString() + "/" + _date.Month.ToString() + "/1");
                    if (_startDate < calendar.Date(Price.Begin()))
                        _startDate = calendar.Date(Price.Begin());
                    if (_startDate > calendar.Date(Price.End()))
                        return null;
                    // 月の末日 (翌月の１日前）
                    _endDate =
                        DateTime.Parse(_date.Year.ToString() + "/" + _date.Month.ToString() + "/" +
                            DateTime.DaysInMonth(_date.Year, _date.Month).ToString());
                    if (_endDate > calendar.Date(Price.End()))
                        _endDate = calendar.Date(Price.End());
                    if (_endDate < calendar.Date(Price.Begin()))
                        return null;
                    break;
                // デフォルトは日足
                default:
                    _startDate = _date;
                    _endDate = _date;
                    break;
            }

            //　開場日の開始、終了の位置と高値、安値を決める
            _startPos = calendar.DatePosition(_startDate, 1);
            _endPos = calendar.DatePosition(_endDate, -1);

            _kabukaRow.EndPos = 0;

            for (var i = _startPos; i <= _endPos; i++) {
                if (Price.IsClosed(i) == 0) {
                    if (_kabukaRow.EndPos == 0) {
                        _kabukaRow.StartPos = i;
                        _kabukaRow.StartDate = calendar.Date(i);
                        _kabukaRow.EndPos = i;
                        _kabukaRow.Open = Price.Open(i);
                        _kabukaRow.High = Price.High(i);
                        _kabukaRow.Low = Price.Low(i);
                    } else {
                        _kabukaRow.EndPos = i;
                        _kabukaRow.High = Math.Max(_kabukaRow.High, Price.High(i));
                        _kabukaRow.Low = Math.Min(_kabukaRow.Low, Price.Low(i));
                    }
                }
            }

            if (_kabukaRow.EndPos == 0)
                // 一日も開場日がなかった場合,nullが返される
                return null;

            _kabukaRow.EndDate = calendar.Date(_kabukaRow.EndPos);
            _kabukaRow.Close = Price.Close(_kabukaRow.EndPos);

            return _kabukaRow;
        }

        /// <summary>
        /// 次の位置を取得
        /// </summary>
        /// <param name="datePos"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private int? NextDatePosition(int datePos, Directions direction) {
            var calendar = new ActiveMarket.Calendar();

            DateTime _date = calendar.Date(datePos);
            DateTime _dateLimit;
            int _datePos;

            if (direction == Directions.Befor) {
                // 過去へ
                switch (ChartScale) {
                    // 週足
                    case ChartScales.Weekly:
                        // 前の週の金曜日から遡った開場日
                        _date = _date.AddDays(-1 * ((int)_date.DayOfWeek + 2));
                        if (_date < calendar.Date(Price.Begin()))
                            return null;
                        _datePos = calendar.DatePosition(_date, -1);
                        while (Price.IsClosed(_datePos) != 0)
                            _datePos--;
                        if (Price.Begin() <= _datePos & _datePos <= Price.End())
                            return _datePos;
                        else
                            return null;
                    // 月足
                    case ChartScales.Monthly:
                        // 前の月の月末から遡った開場日
                        _date = DateTime.Parse(_date.AddMonths(-1).Year.ToString() + "/" + 
                                    _date.AddMonths(-1).Month.ToString() + "/" +
                                    DateTime.DaysInMonth(_date.AddMonths(-1).Year, _date.AddMonths(-1).Month).ToString());
                        if (_date < calendar.Date(Price.Begin()))
                            return null;
                        _datePos = calendar.DatePosition(_date, -1);
                        while (Price.IsClosed(_datePos) != 0) {
                            _datePos--;
                            if (Price.Begin() > _datePos | _datePos > Price.End())
                                return null;
                        }
                        return _datePos;
                        //if (Price.Begin() <= _datePos & _datePos <= Price.End())
                        //    return _datePos;
                        //else
                        //    return null;
                    //　日足
                    default:
                        // 日足の１つ前の開場日
                        do {
                            datePos--;
                            if (datePos < Price.Begin())
                                return null;
                        } while (Price.IsClosed(datePos) != 0);
                        if (Price.Begin() <= datePos & datePos <= Price.End())
                            return datePos;
                        else
                            return null;
                }
            } else {
                // 未来へ
                switch (ChartScale) {
                    // 週足
                    case ChartScales.Weekly:
                        // 翌週の月
                        _dateLimit = _date.AddDays(8 - (int)_date.DayOfWeek);
                        if (_dateLimit > calendar.Date(Price.End()))
                            return null;
                        //翌週の金
                        _date = _date.AddDays(12 - (int)_date.DayOfWeek);
                        _datePos = (_date > calendar.Date(Price.End())) ? Price.End() : calendar.DatePosition(_date, -1);
                        while (Price.IsClosed(_datePos) != 0)
                            _datePos--;
                        if (Price.Begin() <= _datePos & _datePos <= Price.End())
                            if (calendar.Date(_datePos) >= _dateLimit)
                                return _datePos;
                            else
                                return NextDatePosition(_datePos, Directions.After);
                        else
                            return null;
                    // 月足
                    case ChartScales.Monthly:
                        // 次の月の日末日から遡った開場日
                        _dateLimit = DateTime.Parse(_date.AddMonths(1).Year.ToString() + "/" + 
                                        _date.AddMonths(1).Month.ToString() + "/1");
                        if (_dateLimit > calendar.Date(Price.End()))
                            return null;
                        _date = DateTime.Parse(_date.AddMonths(1).Year.ToString() + "/" + 
                                    _date.AddMonths(1).Month.ToString() + "/" + 
                                    DateTime.DaysInMonth(_date.AddMonths(1).Year, _date.AddMonths(1).Month));
                        _datePos = (_date > calendar.Date(Price.End())) ? Price.End() : calendar.DatePosition(_date, -1);

                        while (Price.IsClosed(_datePos) != 0)
                            _datePos--;
                        if (Price.Begin() <= _datePos & _datePos <= Price.End())
                            if (calendar.Date(_datePos) >= _dateLimit)
                                return _datePos;
                            else
                                return NextDatePosition(_datePos, Directions.After);
                        else
                            return null;
                    // 日足
                    default:
                        do {
                            datePos++;
                            if (datePos > Price.End())
                                break;
                        } while (Price.IsClosed(datePos) != 0);
                        if (Price.Begin() <= datePos & datePos <= Price.End())
                            return datePos;
                        else
                            return null;
                }
            }
        }

        /// <summary>
        /// 株価テーブル作成
        /// </summary>
        /// <param name="plotEnd"></param>
        /// <param name="plotWide"></param>
        public void MakeTable(int plotEnd, int plotWide, ChartScales chartScale, AveSteps aveStep) {
            // 株価テーブル作成
            var calendar = new ActiveMarket.Calendar();
            var _kabukaRow = new KabukaRow();
            int? _datePos = plotEnd;

            this.PlotWide = plotWide;
            this.ChartScale = chartScale;
            this.AveStep = aveStep;
            OwarineList = new List<Owarine>();

            PriceTable.Clear();

            var cnt = 0;
            while (cnt < this.PlotWide) {
                if (_datePos.Value < Price.Begin())
                    break;
                _kabukaRow = GetOneRow(_datePos.Value);
                if (_kabukaRow != null) {
                    DataRow row = PriceTable.NewRow();
                    row["StartPos"] = _kabukaRow.StartPos;
                    row["EndPos"] = _kabukaRow.EndPos;
                    row["StartDate"] = _kabukaRow.StartDate;
                    row["EndDate"] = _kabukaRow.EndDate;
                    row["High"] = _kabukaRow.High;
                    row["Low"] = _kabukaRow.Low;
                    row["Open"] = _kabukaRow.Open;
                    row["Close"] = _kabukaRow.Close;
                    row["Heikin1"] = 0;
                    row["Heikin2"] = 0;
                    row["Heikin3"] = 0;
                    row["Heikin4"] = 0;
                    row["Heikin5"] = 0;
                    PriceTable.Rows.Add(row);

                    OwarineList.Add(new Owarine(_kabukaRow.EndPos, _kabukaRow.Close));

                    cnt++;
                }
                _datePos = NextDatePosition(_datePos.Value, Directions.Befor);
                if (!_datePos.HasValue)
                    break;
            }

            // 終値リストの作成（PlotStart以前３００件)
            var MaxCount = 300;
            _datePos = OwarineList.Select(n => n.DatePos).Min();
            cnt = 0;
            for (var i = 0; i < MaxCount; i++) { 
                _datePos = NextDatePosition(_datePos.Value, Directions.Befor);
                if (!_datePos.HasValue)
                    break;
                OwarineList.Add(new Owarine(_datePos.Value, Price.Close(_datePos.Value)));
            }
            for (var i = 0; i < PtCount; i++) {
                for (var j = 0; j < 5; j++) {
                    SetHeikin(i, j);
                }
            }
        }


        public int GetIdouheikinPoint(AveSteps _aveStep, ChartScales _chartScale, int n) {
            List<List<int>> Idouheikins;

            Idouheikins = IdouheikinScales[(int)_aveStep];

            return Idouheikins[(int)_chartScale][n];
        }

        /// <summary>
        /// 移動平均線計算
        /// </summary>
        /// <param name="i">DatePos</param>
        /// <param name="n">移動平均線１～５</param>
        private void SetHeikin(int i, int n) {
            if (GetIdouheikinPoint(AveStep, ChartScale, n) == 0)
                return;

            if (OwarineList
                .Where((x, index) => (i + GetIdouheikinPoint(AveStep, ChartScale, n)) > index & index >= i)
                .Count()
                == GetIdouheikinPoint(AveStep, ChartScale, n))
                SetPtHeikin(n + 1, i, OwarineList
                    .Select(x => x.Value)
                    .Where((x, index) => (i + GetIdouheikinPoint(AveStep, ChartScale, n)) > index & index >= i)
                    .Average());
            else
                SetPtHeikin(n + 1, i, 0);
        }
    }
}
