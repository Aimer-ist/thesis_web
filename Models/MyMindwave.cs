using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.IO;
using ThinkGearNET;

namespace EyeWeb_Again.Models {
    public class MyMindwave {
        public int userOrder {get; set;}

        private ThinkGearWrapper _thinkGearWrapper = new ThinkGearWrapper();

        public void Connect() {
            _thinkGearWrapper = new ThinkGearWrapper();

            // setup the event
            _thinkGearWrapper.ThinkGearChanged += _thinkGearWrapper_ThinkGearChanged;

            // connect to the device on the specified COM port at 57600 baud
            _thinkGearWrapper.Connect("COM5", 57600, true);
        }

        public void Disconnect() {
            _thinkGearWrapper.Disconnect();
            Debug.WriteLine("Disconnected.");
        }

        void _thinkGearWrapper_ThinkGearChanged(object sender, ThinkGearChangedEventArgs e) {
            // write out one of the many properties
            Debug.WriteLine("——————————————————————————————————————————————————");
            Debug.WriteLine("Raw: " + e.ThinkGearState.Raw);
            Debug.WriteLine("Alpha1: " + e.ThinkGearState.Alpha1);
            Debug.WriteLine("Beta1: " + e.ThinkGearState.Beta1);
            Debug.WriteLine("Delta: " + e.ThinkGearState.Delta);
            Debug.WriteLine("Gamma1: " + e.ThinkGearState.Gamma1);
            Debug.WriteLine("*Att: " + e.ThinkGearState.Attention);
            Debug.WriteLine("*Med: " + e.ThinkGearState.Meditation);

            SaveRecord(e.ThinkGearState.Attention, e.ThinkGearState.Meditation);
        }

        //===================================================
        // 將腦波資料記錄到 Csv 裡
        //===================================================
        public void SaveRecord(double Att, double Med) {
            string time = DateTime.Now.TimeOfDay.ToString();

            // 設定csv檔案位置
            string serverPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string strPath = serverPath + "/mindData/MindTest.csv";
            if (userOrder != null && userOrder.ToString() != "") {
                strPath = serverPath + "/mindData/" + userOrder.ToString() + ".csv";
                // 建立檔案
                if(!System.IO.File.Exists(strPath)) {
                    FileStream fileStream = new FileStream(strPath, FileMode.Create);
                    fileStream.Close();
                }
            }
           
            // 修改檔案為非唯讀屬性(Normal)
            System.IO.FileInfo FileAttribute = new FileInfo(strPath);
            FileAttribute.Attributes = FileAttributes.Normal;

            // 開啟CSV檔案
            StreamWriter sw = new StreamWriter(strPath, append: true, encoding: System.Text.Encoding.Default);
            sw.WriteLine("Time:" + time + ",Att:" + Att.ToString() + ",Med:" + Med.ToString());
            sw.Close();
        }
    }
}