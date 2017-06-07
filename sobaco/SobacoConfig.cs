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
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace sobaco {
    [DataContract]
    public class SobacoConfig {

        [DataMember]
        private FormWindowState WindowState { get; set; }
        [DataMember]
        private int Width { get; set; }
        [DataMember]
        private int Height { get; set; }
        [DataMember]
        private int Tab { get; set; }
        [DataMember]
        public List<string> Favorites { get; set; }
        [DataMember]
        public ChartStyles ChartStyle { get; set; }
        [DataMember]
        public AveSteps AveStep { get; set; }
        [DataMember]
        public CandleSizes CandleSize { get; set; }
        [DataMember]
        public ChartScales ChartScale { get; set; }
        [DataMember]
        public List<int> IdouheikinLineWidth { get; set; }
        [DataMember]
        public string OkiniiriFileName { get; set; }
        [DataMember]
        public List<List<List<int>>> IdouheikinScales = new List<List<List<int>>>();
        [DataMember]
        public double[] HanbunPoint = new double[4];

        public FormWindowState GetWindowState(Form _form) {
            return _form.WindowState;
        }
        public void SetWindowState(Form _form, FormWindowState _state) {
            _form.WindowState = _state;
        }
        public int GetWidth(Form _form) {
            return _form.Width;
        }
        public void SetWidth(Form _form, int _width) {
            _form.Width = _width;
        }
        public int GetHeight(Form _form) {
            return _form.Height;
        }
        public void SetHeight(Form _form, int _height) {
            _form.Height = _height;
        }
        public int GetTab(TabControl _tabControl) {
            return _tabControl.SelectedIndex;
        }
        public void SetTab(TabControl _tabControl, int _index) {
            _tabControl.SelectedIndex = _index;
        }

        public void MakeSerialize(Form _form, TabControl _tabControl, MeigaraList _meigaraList) {
            this.WindowState = _form.WindowState;
            this.Width = _form.Width;
            this.Height = _form.Height;
            this.Tab = _tabControl.SelectedIndex;
            this.Favorites = _meigaraList.GetFavoriteList();
        }

        public void EvalSerialize(Form _form, TabControl _tabControl, MeigaraList _meigaraList) {
            _form.WindowState = this.WindowState;
            _form.Width = this.Width;
            _form.Height = this.Height;
            _tabControl.SelectedIndex = this.Tab;
            _meigaraList.SetFavoriteList(this.Favorites);
        }

        public string GetIdouheikinToString() {
            string s;
            XmlSerializer serializer = new XmlSerializer(IdouheikinScales.GetType());
            StringBuilder sb = new StringBuilder();
            using (System.IO.StringWriter sw = new System.IO.StringWriter(sb)) {
                serializer.Serialize(sw, IdouheikinScales);
                s = sw.ToString();
            }
            return s;
        }

        public void SetIdouheikinFromString(string s) {
            XmlSerializer serializer = new XmlSerializer(IdouheikinScales.GetType());
            using (System.IO.StringReader sr = new System.IO.StringReader(s)) {
                IdouheikinScales = (List<List<List<int>>>)serializer.Deserialize(sr);
            }
        }

        public List<List<int>> GetUserSettingIdouheikins() {
            return IdouheikinScales[(int)AveSteps.User];
        }

        public void SetUserSettingIdouheikins(List<List<int>> _idouheikins) {
            IdouheikinScales[(int)AveSteps.User] = _idouheikins;
        }
    }
}
