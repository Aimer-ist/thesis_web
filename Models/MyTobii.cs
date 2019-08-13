using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Tobii.Interaction;
using System.IO;

namespace EyeWeb_Again.Models {


    // 在使用之前：
    //   1. 需要先在 Nuget 引入 Tobii.Interaction
    //   2. 需要先下載 Tobii 官方的驅動程式

    // 官方示例：https://github.com/Tobii/CoreSDK/tree/master/samples

    public class MyTobii {

        public int userOrder { get; set; }

        // Host = 官方的 Tobii 控制器
        Host host;

        double NowTs;
        double NowX;
        double NowY;
        List<double> Data_List_Ts = new List<double>();
        List<double> Data_List_X = new List<double>();
        List<double> Data_List_Y = new List<double>();
        List<int> Data_List_Area = new List<int>();

        // View 注視的區塊
        int NowFlag;  

        //===================================================
        // 在輸出欄顯示眼動座標
        //===================================================
        public void ShowGaze() {

            // 建立與眼動儀的連線
            // Tobii 官方已經將連線和基本操作等放置於 Host 中
            host = new Host(name: "myHost");

            // 建立眼動數據資料流的連線
            var gazePointDataStream = host.Streams.CreateGazePointDataStream();
            
            // 取得資料，格式是 ((x軸 y軸 時間) => 要執行的動作) # Linq 語法
            gazePointDataStream.GazePoint((x, y, ts) => Record(x, y, ts));
            // gazePointDataStream.GazePoint((x, y, ts) => Debug.WriteLine("【眼動儀】時間： {0}\t X: {1} \t Y:{2}", ts, x, y));

            // 顯示其他資料
            // Console.ReadKey();
        }

        //===================================================
        // 自製函式，處理眼動儀回傳的數據
        //===================================================
        public void Record(double x, double y, double ts) {

            // 顯示在下面視窗
            Debug.WriteLine("【眼動儀】時間： {0}\t X: {1} \t Y:{2}", ts, x, y);

            // 更新現在的位置資料
            NowTs = ts;
            NowX = x;
            NowY = y;

            if (x < 800 && x > 160 && y > 95 && y < 945) {
                NowFlag = 1;
            }
            else if (x < 1760 && x > 1140 && y > 95 && y < 945) {
                NowFlag = 2;
            }
            else {
                NowFlag = 0;
            }
            Debug.WriteLine("注視區域：" + NowFlag);
            //SaveRecord(x, y, NowFlag);

            Data_List_Ts.Add(ts);
            Data_List_X.Add(x);
            Data_List_Y.Add(y);
            Data_List_Area.Add(NowFlag);
        }

       


        //===================================================
        // 將眼動儀資料記錄到 Csv 裡
        //===================================================
        public void SaveRecord(double time, double x, double y, int Flag) {

            double[] Gaze = new double[3] { x, y,Flag };
            //string time = DateTime.Now.TimeOfDay.ToString();

            // 設定csv檔案位置
            string serverPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string strPath = serverPath + "/tobiiData/TobiiTest.csv";
            if (userOrder != null && userOrder.ToString() != "") {
                strPath = serverPath + "/tobiiData/" + userOrder.ToString() +".csv";
                // 建立檔案
                if (!System.IO.File.Exists(strPath)) {
                    FileStream fileStream = new FileStream(strPath, FileMode.Create);
                    fileStream.Close();
                }
            }
            //Debug.WriteLine("注視區域：", Gaze[2]);
            // 修改檔案為非唯讀屬性(Normal)
            System.IO.FileInfo FileAttribute = new FileInfo(strPath);
            FileAttribute.Attributes = FileAttributes.Normal;

            // 開啟CSV檔案
            StreamWriter sw = new StreamWriter(strPath, append: true, encoding: System.Text.Encoding.Default);
            sw.WriteLine("Time:" + time + "," + "X:" + Gaze[0] + "," + "Y:" + Gaze[1] + "," + "注視區域：" + Gaze[2]);
            sw.Close();
        }

        //===================================================
        // 將眼動儀資料記錄到 Csv 裡
        //===================================================
        public void SaveRecord2(List<double> time, List<double> x, List<double> y, List<int> Flag) {
            

            // 設定csv檔案位置
            string serverPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string strPath = serverPath + "/tobiiData/TobiiTest.csv";
            if (userOrder != null && userOrder.ToString() != "") {
                strPath = serverPath + "/tobiiData/" + userOrder.ToString() + ".csv";
                // 建立檔案
                if (!System.IO.File.Exists(strPath)) {
                    FileStream fileStream = new FileStream(strPath, FileMode.Create);
                    fileStream.Close();
                }
            }
            //Debug.WriteLine("注視區域：", Gaze[2]);
            // 修改檔案為非唯讀屬性(Normal)
            System.IO.FileInfo FileAttribute = new FileInfo(strPath);
            FileAttribute.Attributes = FileAttributes.Normal;

            // 開啟CSV檔案
            StreamWriter sw = new StreamWriter(strPath, append: true, encoding: System.Text.Encoding.Default);
            for (int i = 0; i < Data_List_Area.Count; i++) {
                sw.WriteLine("Time:" + Data_List_Ts[i] + "," + "X:" + Data_List_X[i] + "," + "Y:" + Data_List_Y[i] + "," + "注視區域：" + Data_List_Area[i]);
            }
            
            sw.Close();
        }

        //===================================================
        // 自製函式，用來需要取得眼動儀數據時使用
        //===================================================
        public double[] GetNowGaze() {
            double[] NowGaze = new double[3] { NowX, NowY, NowFlag };
            return NowGaze;
        }

        //===================================================
        // 停止顯示眼動座標
        //===================================================
        public void StopShowGaze() {
            if (host != null) {
                SaveRecord2(Data_List_Ts, Data_List_X, Data_List_Y, Data_List_Area);
                host.DisableConnection();
            }
        }

        //===================================================
        // 建立監視某個區域有沒有被看到的交互器
        // 官方範例：https://github.com/Tobii/CoreSDK/blob/master/samples/Interactors/Interaction_Interactors_101/Program.cs
        // 類別文檔：https://tobii.github.io/CoreSDK/api/Tobii.Interaction.InteractorAgent.html
        //===================================================
        public void CreateInteractors() {
            host = new Host(name: "myHost");

            Debug.WriteLine("CreateInteractors");

            // 利用 Windoes 的功能取得現在執行中視窗的執行緒(Handle) => 跟系統拿現在這個程式的身分證
            //var currentWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
            var myProcess = Process.Start("notepad");
            myProcess.Refresh();
            var currentWindowHandle = myProcess.MainWindowHandle;
            Debug.WriteLine("ProcessName:" + myProcess.ProcessName);

            // 然後把現在視窗的執行緒丟到下面的函式以取得視窗邊界
            var currentWindowBounds = GetWindowBounds(currentWindowHandle);
            Debug.WriteLine("currentWindowHandle:" + currentWindowHandle);

            // 建立交互器
            var interactorAgent = host.InitializeVirtualInteractorAgent(currentWindowHandle, "ConsoleWindowAgent");

            // 定義交互器的行為
            // * 實戰中只會修改到這裡，如果可以用的話 *
            interactorAgent
                .AddInteractorFor(currentWindowBounds) // 將交互器添加在…
                .WithGazeAware() // 是否有在看螢幕
                .HasGaze(() => Debug.WriteLine("看我！！")) // 視線進入交互器的時候
                .LostGaze(() => Debug.WriteLine("嗚……")); // 視線從交互器離開的時候

            Debug.WriteLine("OK!!!!");
        }

        //===================================================
        // 範例給的函式：取得視窗資料。需要傳入該視窗的執行緒
        // 可以不用太深入理解 *這是魔法*
        //===================================================
        private static Rectangle GetWindowBounds(IntPtr windowHandle) {

            // 宣告一個 Rect （有點像是邊界資料的概念）
            NativeRect nativeNativeRect;
            // 如果能夠順利取得當前視窗的 Rect
            if (GetWindowRect(windowHandle, out nativeNativeRect)) {
                // 就把當前視窗的邊界資料包一包回傳
                Debug.WriteLine(nativeNativeRect.Top + "," + nativeNativeRect.Left + "," + nativeNativeRect.Bottom + "," + nativeNativeRect.right);
                return new Rectangle {
                    X = nativeNativeRect.Left,
                    Y = nativeNativeRect.Top,
                    Width = nativeNativeRect.right,
                    Height = nativeNativeRect.Bottom
                };
            }
            Debug.WriteLine(nativeNativeRect.Top + "," + nativeNativeRect.Left + "," + nativeNativeRect.Bottom + "," + nativeNativeRect.right);
            // 不行的話就隨便包一圈丟回去
            return new Rectangle(0d, 0d, 1920d, 1080d);
        }

        //===================================================
        // 範例給的資料：從 user32.dll 引入取得視窗的 Rect 的方法
        // 傳入：目標程式的 UID/執行緒；傳出：目標程式的 Rect
        // 可以不用太深入理解 *這是魔法*
        // extern => 已在別處定義，叫程式自已出門去找啦的意思
        //===================================================
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out NativeRect nativeRect);

        //===================================================
        // 範例給的資料：邊界資料，用來放 Rect 的上下左右邊界
        // 可以不用太深入理解 *這是魔法*
        // StructLayout => 用來控制記憶體的欄位實際配置
        //===================================================
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeRect {
            public int Left;
            public int Top;
            public int right;
            public int Bottom;
        }
    }
}