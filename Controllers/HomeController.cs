using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EyeWeb_Again.Models;
using Tobii.Interaction;
using ThinkGearNET;

namespace EyesWebTest.Controllers {
    public class HomeController : Controller {

        static int jump = 0;
        static int order = (new Random()).Next(10000);
        static MyTobii tobii;
        static MyMindwave tGC;
        static List<Phone> phones;

        public ActionResult Index() {
            DBconnPhone dBconnPhone = new DBconnPhone();
            phones = dBconnPhone.GetPhones();
            phones = RandomList(phones);
            ViewBag.order = order;
            return View();
        }

        public ActionResult Test() {
            ViewBag.phones = phones[0];
            ViewBag.order = order;
            Start(order);
            return View();
        }


        // 使用者的量表傳送
        [HttpPost]
        public ActionResult PostData(User user) {
            DBconnPhone dBconnPhone = new DBconnPhone();
            dBconnPhone.NewUser(user);
            TempData["order"] = user.order;
            return RedirectToAction("GoFinish");
        }

        public ActionResult Finish() {
            ViewBag.user = TempData["order"].ToString();
            return View();
        }

        // 商品亂數排序
        public List<Phone> RandomList(List<Phone> list) {

            Random rand = new Random(Guid.NewGuid().GetHashCode());
            List<Phone> newList = new List<Phone>();//儲存結果的集合
            foreach (Phone item in list) {
                newList.Insert(rand.Next(0, newList.Count), item);
            }
            newList.Remove(list[0]);//移除list[0]的值(Apple)
            newList.Insert(rand.Next(0, newList.Count), list[0]);//再重新隨機插入
            return newList;
        }



        //----------------------------------------------------------------------
        // 眼動區域
        //----------------------------------------------------------------------
        public String Start(int order) {
            tobii = new MyTobii();
            tobii.userOrder = order;
            tobii.ShowGaze();
            return "眼動儀已開始偵測";
        }

        public String Stop() {
            tobii.StopShowGaze();
            return "眼動儀已停止偵測";
        }

        //----------------------------------------------------------------------
        // 腦波區域
        //----------------------------------------------------------------------
        public void ConnectOK() {
            tGC = new MyMindwave();
            tGC.userOrder = order;
            tGC.Connect();
        }

        public void ConnectNG() {
            tGC.Disconnect();
            Debug.WriteLine("已關閉連線");
        }


        //------------測試跳頁用------------//
        ////////////////////////////////////

        public ActionResult GoFinish() {
            Stop();
            /*
            jump += 1;

            if (jump < 3) {
                return RedirectToAction("Test");
            } ;*/

            return RedirectToAction("Finish");
        }
    }
}