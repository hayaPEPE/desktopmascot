using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace desktopmascot
{
    //internal static class Program
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            Form1 form = new Form1();
            form.Show();

            //自分で作成したループを使用
            while (form.Created)
            {
                form.MainLoop();
                Application.DoEvents();
            }
        }
    }
}
