using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//DXライブラリのusing追加
using DxLibDLL;

namespace desktopmascot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //DXライブラリの初期設定全般
            //Log.txtを生成しないように設定
            DX.SetOutApplicationLogValidFlag(DX.FALSE);
            //DXライブラリの親ウィンドウをこのフォームに設定
            DX.SetUserWindow(Handle);
            //Zバッファの深度を24bitに変更
            //裏画面のZバッファの深度を24bitに変更
            DX.SetFullSceneAntiAliasingMode(4, 2);
            // DXライブラリの初期化処理
            DX.DxLib_Init();
            //描画先を裏画面に設定
            DX.SetDrawScreen(DX.DX_SCREEN_BACK);
        }
        ///<summary>
        ///この関数をメインループとする
        ///</summary>
        public void MainLoop()
        {
            //裏画面を消す
            DX.ClearDrawScreen();
            //裏画面を表画面にコピー
            DX.ScreenFlip();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //DxLibの終了処理
            DX.DxLib_End();
        }

    }
}
