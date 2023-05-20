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
        private int _model_handle;
        public Form1()
        {
            InitializeComponent();

            //画面サイズの設定（全画面）
            ClientSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            //DXライブラリの初期設定全般
            //Log.txtを生成しないように設定
            DX.SetOutApplicationLogValidFlag(DX.FALSE);
            //DXライブラリの親ウィンドウをこのフォームに設定
            DX.SetUserWindow(Handle);
            //Zバッファの深度を24bitに変更
            DX.SetZBufferBitDepth(24);
            //裏画面のZバッファの深度を24bitに変更
            DX.SetCreateDrawValidGraphZBufferBitDepth(24);
            //画面のフルスクリーンアンチエイリアンスモードの設定をする
            DX.SetFullSceneAntiAliasingMode(4, 2);
            // DXライブラリの初期化処理
            DX.DxLib_Init();
            //描画先を裏画面に設定
            DX.SetDrawScreen(DX.DX_SCREEN_BACK);

            //3Dモデルの読み込み
            this._model_handle = DX.MV1LoadModel("Models/Model1.pmx");

            //カメラの設定
            //奥行0.1～1000をカメラの描画範囲とする
            DX.SetCameraNearFar(0.1f, 1000.0f);
            //第１引数からの位置から第２引数の位置を見る角度にカメラを設置
            DX.SetCameraPositionAndTarget_UpVecY(DX.VGet(0.0f, 10.0f, -20.0f), DX.VGet(0.0f, 10.0f, 0.0f));

        }
        ///<summary>
        ///この関数をメインループとする
        ///</summary>
        public void MainLoop()
        {
            //裏画面を消す
            DX.ClearDrawScreen();

            //背景を設定（透過させる）
            DX.DrawBox(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, DX.GetColor(1, 1, 1), DX.TRUE);

            //3Dモデルの描画
            DX.MV1DrawModel(this._model_handle);

            //裏画面を表画面にコピー
            DX.ScreenFlip();

            //枠がなくなるので、ESCキーで終了できるように設定
            if(DX.CheckHitKey(DX.KEY_INPUT_ESCAPE) != 0)
            {
                Close();
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //DxLibの終了処理
            DX.DxLib_End();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //フォームの枠を非表示にする
            FormBorderStyle = FormBorderStyle.None;
            //透過色を設定
            TransparencyKey = Color.FromArgb(1, 1, 1);
        }
    }
}
