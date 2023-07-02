using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;
//DXライブラリのusing追加
using DxLibDLL;
using static DxLibDLL.DX;
using System.Net;
using System.Media;
using System.Net.Http.Headers;


namespace desktopmascot
{
    public partial class Form1 : Form
    {
        private int _model_handle;

        private int _attach_index;
        private float _total_time;
        private float _play_time = 6.0f;  //初めの再生箇所
        private float _play_speed = 0.4f; //再生スピード
        private int _motion_id = 0;
        public Form1()
        {
            InitializeComponent();
            //IMEを使用するように設定
            DX.SetUseIMEFlag(1);
            //画面サイズの設定
            ClientSize = new Size(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);

            //DXライブラリの初期設定全般
            //Log.txtを生成しないように設定
            DX.SetOutApplicationLogValidFlag(DX.FALSE);
            //DXライブラリの親ウィンドウをこのフォームに設定
            DX.SetUserWindow(Handle);
            //Zバッファの深度を24bitに変更
            DX.SetZBufferBitDepth(24);
            //裏画面のZバッファの深度を24bitに変更
            DX.SetCreateDrawValidGraphZBufferBitDepth(24);
            //画面のフルスクリーンアンチエイリアンスモードの設定をする→なくてもいいかも
            DX.SetFullSceneAntiAliasingMode(4, 2);
            // DXライブラリの初期化処理
            DX.DxLib_Init();
            //描画先を裏画面に設定→なくてもいいかも
            DX.SetDrawScreen(DX.DX_SCREEN_BACK);

            //3Dモデルの読み込み
            this._model_handle = DX.MV1LoadModel("Models/Model.pmx");

            //モーションの選択（モデル名＋数字のアニメーションが自動で読み込まれる？）
            this._attach_index = DX.MV1AttachAnim(this._model_handle, this._motion_id, -1, DX.FALSE);
            //モーションのそう再生時間を取得
            this._total_time = DX.MV1GetAttachAnimTotalTime(this._model_handle, this._attach_index);

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

            //時間を進める
            this._play_time += _play_speed;
            //モーションの再生位置が終端まで来たら最初に戻す
            if (this._play_time >= this._total_time)
            {
                if(this._motion_id == 0)
                {
                this._play_time = 18.0f; //モーションが初めに戻るときに変になるのを防ぐため18.0fにしている

                }
            }
            //モーションの再生位置を設定
            DX.MV1SetAttachAnimTime(this._model_handle, this._attach_index, this._play_time);

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

        //マウスのクリック位置を記憶
        private Point mousePoint;

        //Form1のMouseDownイベントハンドラ
        //マウスのボタンが押されたとき
        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                //位置を記憶する
                mousePoint = new Point(e.X, e.Y);
            }
        }

        //Form1のMouseMoveイベントハンドラ
        //マウスが動いたとき
        private void Form1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                this.Left += e.X - mousePoint.X;
                this.Top += e.Y - mousePoint.Y;
                //または、次のようにする
                //this.Location.X = e.X - mousePoint.X,
                //this.Location.Y = e.Y - mousePoint.Y);
            }
        }


        // Form1のダブルクリックイベントハンドラ
        private async void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {           
            DX.MV1DetachAnim(this._model_handle, this._attach_index);//今まで再生していたモーションを解除

            _motion_id = 1;  // ダブルクリック時に再生するモーションのIDを指定する
            _play_time = 40.0f; // 40フレームから再生させる
            _attach_index = DX.MV1AttachAnim(this._model_handle, this._motion_id, -1, DX.FALSE);// ダブルクリック時のモーションを選択
            _total_time = DX.MV1GetAttachAnimTotalTime(this._model_handle, this._attach_index) - this._play_time;
            await Task.Delay((int)_total_time * 10);　//どういう数字にすれば良いのかわからない
                
            DX.MV1DetachAnim(this._model_handle, this._attach_index);
                
            _motion_id = 0;
            _attach_index = DX.MV1AttachAnim(this._model_handle, this._motion_id, -1, DX.FALSE);
            _total_time = DX.MV1GetAttachAnimTotalTime(this._model_handle, this._attach_index);

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        // イベントハンドラの中でEnterキーが押されたかどうかをチェックする
        private void textBox1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            // Enterキーが押された場合
            if (e.KeyChar == (char)Keys.Return)
            {
                //テキストボックスの中身が空の場合は何もしない
                if (textBox1.Text == "")
                {
                    return;
                }

                //読み込む
                //var player = new SoundPlayer("test.wav");
                //再生する
                //player.PlaySync();
                //Console.WriteLine("再生完了");

                //テキストボックスの中身をNode-REDに送信する

                //Node-REDのサーバにHTTPリクエストを送信
                WebClient client = new WebClient();
                NameValueCollection collection = new NameValueCollection();
                collection.Add("message", $"{textBox1.Text}");
                Uri url = new Uri("http://localhost:1880/test");

                client.UploadValuesAsync(url, collection);
                client.Dispose();

                // テキストボックスの中身を空にする
                textBox1.Text = "";

                // Enterキーの入力を無効にする
                e.Handled = true;
            }
        }
    }
}
