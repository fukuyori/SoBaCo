using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ActiveMarket;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace sobaco {
    public class MeigaraList {
        public enum Proc { ADD, INSERT };

        public DataTable NamesTable;
        public string GetNameTableCode(int pos) { return (string)NamesTable.Rows[pos]["CODE"]; }
        public string GetNameTableName(int pos) { return (string)NamesTable.Rows[pos]["NAME"]; }
        public int NameTableCount { get { return NamesTable.Rows.Count; } }
        public string GetNameByCode(string code) {
            return (string)NamesTable.AsEnumerable()
                                     .Where(x => x.Field<string>("CODE") == code)
                                     .Select(x => x.Field<string>("NAME"))
                                     .First();
        }
        public DataTable SearchTable;
        public string GetSearchTableCode(int pos) { return (string)SearchTable.Rows[pos]["CODE"]; }
        public string GetSearchTableName(int pos) { return (string)SearchTable.Rows[pos]["NAME"]; }
        public int SearchTableCount { get { return SearchTable.Rows.Count; } }

        public DataTable FavoriteTable;
        public string GetFavoriteTableCode(int pos) { return (string)FavoriteTable.Rows[pos]["CODE"]; }
        public List<string> GetFavoriteList() {
            return FavoriteTable
                    .AsEnumerable()
                    .Select(x => x.Field<string>("CODE"))
                    .ToList();
        }
        public string GetFavoriteTableName(int pos) { return (string)FavoriteTable.Rows[pos]["NAME"]; }
        public int FavoritetableCount { get { return FavoriteTable.Rows.Count; } }
        public int FavoriteTableColumnsCount { get { return FavoriteTable.Columns.Count; } }

        public DataTable HistoryTable;
        public string GetHistoryTableCode(int pos) { return (string)HistoryTable.Rows[pos]["CODE"]; }
        public string GetHistoryTableName(int pos) { return (string)HistoryTable.Rows[pos]["NAME"]; }
        public int HistoryTableCount { get { return HistoryTable.Rows.Count; } }

        public ActiveMarket.Names MyNames;

        public MeigaraList() {
            MyNames = new ActiveMarket.Names();
            MyNames.AllNames(_KindFlag.AM_KINDFLAG_SPOTS, out Array codes, out Array names);

            NamesTable = new DataTable();
            NamesTable.Columns.Add("CODE", typeof(string));
            NamesTable.Columns.Add("NAME", typeof(string));
            NamesTable.PrimaryKey = new DataColumn[] { NamesTable.Columns["CODE"] };

            FavoriteTable = new DataTable();
            FavoriteTable.Columns.Add("CODE", typeof(string));
            FavoriteTable.Columns.Add("NAME", typeof(string));
            FavoriteTable.PrimaryKey = new DataColumn[] { FavoriteTable.Columns["CODE"] };

            HistoryTable = new DataTable();
            HistoryTable.Columns.Add("CODE", typeof(string));
            HistoryTable.Columns.Add("NAME", typeof(string));
            HistoryTable.PrimaryKey = new DataColumn[] { HistoryTable.Columns["CODE"] };

            // 全銘柄データの登録
            foreach (string s in codes) {
                DataRow row = NamesTable.NewRow();
                row["CODE"] = s;
                NamesTable.Rows.Add(row);
            }
            int i = 0;
            foreach (string s in names) {
                NamesTable.Rows[i]["NAME"] = s;
                i++;
            }

            NamesTable.TableName = "MeigaraList";
        }

        /// <summary>
        /// お気に入りリストの読み込み
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public int SetFavoriteList(IEnumerable<string> codes) {
            int cnt = 0;
            if (FavoriteTable != null)
                FavoriteTable.Clear();
            foreach (string s in codes) {
                DataRow row = FavoriteTable.NewRow();
                row["CODE"] = s;
                row["NAME"] = GetNameByCode(s);
                FavoriteTable.Rows.Add(row);
                cnt++;
            }
            return cnt;
        }

        /// <summary>
        /// お気に入りリストの比較
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public bool FavoriteEquals(IEnumerable<string> codes) {
            return FavoriteTable.AsEnumerable()
                                .Select(x => x.Field<string>("CODE"))
                                .SequenceEqual(codes);
        }


        /// <summary>
        ///  お気に入りに追加
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public bool AddFavorite(string s, Proc p) {

            DataRow _favoriteRow, _foundRow;

            _favoriteRow = FavoriteTable.NewRow();
            _favoriteRow["CODE"] = s;
            // FavoriteTableに既にあるか.あったら追加しない
            _foundRow = FavoriteTable.Rows.Find(s);
            if (_foundRow != null)
                return false;

            // 銘柄リストになかったら追加しない
            _foundRow = NamesTable.Rows.Find(s);
            if (_foundRow != null) {
                _favoriteRow["NAME"] = _foundRow["NAME"];
                if (p == Proc.ADD)
                    FavoriteTable.Rows.Add(_favoriteRow);
                else
                    FavoriteTable.Rows.InsertAt(_favoriteRow, 0);
                return true; //追加されたら true を返す
            }

            // 追加されない場合
            return false;
        }

        /// <summary>
        /// お気に入りから削除
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public void SubFavorite(string s) {

            DataRow[] removeRow = FavoriteTable
                                    .AsEnumerable()
                                    .Where(x => x.Field<string>("CODE").Equals(s))
                                    .ToArray();
            Array.ForEach<DataRow>(removeRow, row => FavoriteTable.Rows.Remove(row));
        }

        /// <summary>
        /// お気に入り全削除
        /// </summary>
        public void ClearFavorite() {
            while (FavoritetableCount > 0)
                FavoriteTable.Clear();
        }

        /// <summary>
        ///  お気に入りに追加
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public bool AddHistory(string s) {

            DataRow _historyRow, _foundRow;

            _historyRow = HistoryTable.NewRow();
            _historyRow["CODE"] = s;
            // HistoryTableに既にあるか.あったら追加しない
            _foundRow = HistoryTable.Rows.Find(s);
            if (_foundRow != null)
                return false;

            // 銘柄リストになかったら追加しない
            _foundRow = NamesTable.Rows.Find(s);
            if (_foundRow != null) {
                _historyRow["NAME"] = _foundRow["NAME"];
                HistoryTable.Rows.InsertAt(_historyRow, 0);
                return true; //追加されたら true を返す
            }

            // 追加されない場合
            return false;
        }

        /// <summary>
        /// 銘柄検索
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public int SearchMeigaraList(string s) {
            DataRow[] _DataRows;
            if (Regex.IsMatch(s, @"^\d{1,4}$")) {
                _DataRows = NamesTable
                    .AsEnumerable()
                    .Where(n => n.Field<string>("CODE").StartsWith(s))
                    .OrderBy(n => n.Field<string>("CODE"))
                    .ToArray();
            } else {
                _DataRows = NamesTable
                    .AsEnumerable()
                    .Where(n => n.Field<string>("NAME").ToUpper().Contains(s.ToUpper()))
                    .OrderBy(n => n.Field<string>("CODE"))
                    .ToArray();
            }

            if (_DataRows.Count<DataRow>() > 0) {
                SearchTable = _DataRows.CopyToDataTable<DataRow>();
                return SearchTableCount;
            } else
                return 0;
        }
    }
}
