using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Diagnostics;

namespace EyeWeb_Again.Models {
    public class DBconnPhone {

        private readonly string ConnStr = @"Data Source=igouist.database.windows.net;Initial Catalog=IgouistDB;Persist Security Info=True;User ID=igouist;Password=*";

        // 取得手機資料=============================================
        public List<Phone> GetPhones() {

            List<Phone> phones = new List<Phone>();

            // 先訂出來的'SQL指令'(連線資料庫字串，搜尋全部資料表內容)暫且還未執行。開啟連線
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Phone");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            // 是執行先前訂出來的'SQL指令'再去讀取執行的結果(全部的資料表內容)
            SqlDataReader reader = sqlCommand.ExecuteReader();

            if (reader.HasRows) {
                while (reader.Read()) {
                    Phone phone = new Phone {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        no = reader.GetInt32(reader.GetOrdinal("no")),
                        brand = reader.GetString(reader.GetOrdinal("brand")),
                        product = reader.GetString(reader.GetOrdinal("product")),
                        cpu = reader.GetString(reader.GetOrdinal("cpu")),
                        system = reader.GetString(reader.GetOrdinal("system")),
                        rom = reader.GetString(reader.GetOrdinal("rom")),
                        ram = reader.GetString(reader.GetOrdinal("ram")),
                        size = reader.GetString(reader.GetOrdinal("size")),
                        battery = reader.GetString(reader.GetOrdinal("battery")),
                        camera = reader.GetString(reader.GetOrdinal("camera")),
                        topcamera = reader.GetString(reader.GetOrdinal("topcamera")),
                        waterproof = reader.GetString(reader.GetOrdinal("waterproof")),
                        memory = reader.GetString(reader.GetOrdinal("memory")),
                        bluebud = reader.GetString(reader.GetOrdinal("bluebud")),
                        s1 = reader.GetString(reader.GetOrdinal("s1")),
                        s2 = reader.GetString(reader.GetOrdinal("s2")),
                        s3 = reader.GetString(reader.GetOrdinal("s3")),
                        sc1 = reader.GetString(reader.GetOrdinal("sc1")),
                        sc2 = reader.GetString(reader.GetOrdinal("sc2")),
                        sc3 = reader.GetString(reader.GetOrdinal("sc3")),
                    };
                    phones.Add(phone);
                }
            }
            else {
                Console.WriteLine("資料庫內容是空的");
            }

            sqlConnection.Close();

            return phones;
        }



        // 取得受測者資料=============================================
        public List<User> GetUsers() {

            List<User> users = new List<User>();

            // 先訂出來的'SQL指令'(連線資料庫字串，搜尋全部資料表內容)暫且還未執行。開啟連線
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM User");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            // 是執行先前訂出來的'SQL指令'再去讀取執行的結果(全部的資料表內容)
            SqlDataReader reader = sqlCommand.ExecuteReader();

            if (reader.HasRows) {
                while (reader.Read()) {

                    User user = new User {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        order = reader.GetInt32(reader.GetOrdinal("order")),
                        left = reader.GetInt32(reader.GetOrdinal("left")),
                        right = reader.GetInt32(reader.GetOrdinal("right")),
                    };
                    users.Add(user);
                }
            }
            else {
                Console.WriteLine("資料庫內容是空的");
            }
            sqlConnection.Close();
            return users;
        }


        //新增使用者資料
        public void NewUser(User user) {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand(@"INSERT INTO [User] ([order], [left], [right]) VALUES (@order, @left, @right)");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@order", user.order));
            sqlCommand.Parameters.Add(new SqlParameter("@left", user.left));
            sqlCommand.Parameters.Add(new SqlParameter("@right", user.right));

            sqlConnection.Open();

            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
}