using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sobaco {
    static class Program {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {
            // .Net Framework 4.5.2 が必要です
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\")) {
                if (ndpKey == null ||
                    ndpKey.GetValue("Release") == null ||
                    (int)CheckFor45DotVersion((int)ndpKey.GetValue("Release")) < (int)DotNetVersion.V4_5_2) {

                    MessageBox.Show("このソフトの利用には、.Net Framework 4.5.2以降が必要です。\n\n"
                        + "https://www.microsoft.com/ja-JP/download/details.aspx?id=42643 \n"
                        + "からダウンロードし、インストールをしてください。",
                            "相場子", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // パンローリングのActiveMarketライブラリが必要です。
            try {
                ActiveMarket.Prices price = new ActiveMarket.Prices();
                price.ReadBegin = 0;
                price.ReadEnd = 0;
                price.Read("1001");
                price = null;
            } catch (Exception ex) {
                MessageBox.Show("このソフトの利用には、\n"
                    + "パンローリング社の相場アプリケーションが必要です。\n\n"
                    + "http://www.panrolling.com/pansoft/amarket/",
                        "相場子", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //二重起動をチェックする
            if (System.Diagnostics.Process.GetProcessesByName(
                System.Diagnostics.Process.GetCurrentProcess().ProcessName).Length > 1) {
                //すでに起動していると判断して終了
                MessageBox.Show("多重起動はできません。");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        // Checking the version using >= will enable forward compatibility, 
        // however you should always compile your code on newer versions of
        // the framework to ensure your app works the same.
        enum DotNetVersion { No4_5 = 0, V4_5 = 1, V4_5_1 = 2, V4_5_2 = 3, V4_6 = 4};

        private static DotNetVersion CheckFor45DotVersion(int releaseKey) {
            if (releaseKey >= 393295) {
                return DotNetVersion.V4_6; // "4.6 or later";
            }
            if ((releaseKey >= 379893)) {
                return DotNetVersion.V4_6; // "4.5.2 or later";
            }
            if ((releaseKey >= 378675)) {
                return DotNetVersion.V4_5_1; // "4.5.1 or later";
            }
            if ((releaseKey >= 378389)) {
                return DotNetVersion.V4_5; // "4.5 or later";
            }
            // This line should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            return DotNetVersion.No4_5; // "No 4.5 or later version detected";
        }
    }
}
