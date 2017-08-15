using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using MySql.Data.MySqlClient;
using RealifeGM.de.jojoa.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RealifeGM.de.jojoa.mysql
{
    class MySQL_InventoryData
    {
        #region variables
        public static string conString = "SERVER=localhost;" + "DATABASE=realifegm;" + "UID=root;" + "PASSWORD=;";
        public static MySqlConnection con;
        public static MySqlCommand cmd;
        public static MySqlDataReader reader;
        #endregion variables

        public static Inventory createInv()
        {
            int lastid = getLastID();
            con = new MySqlConnection(conString);
            cmd = con.CreateCommand();
            cmd.CommandText = "INSERT INTO InventoryData (Name,id,amount) VALUES (@name,@amount,@id)";
            cmd.Parameters.AddWithValue("@id", lastid + 1);
            cmd.Parameters.AddWithValue("@amount", 1);
            cmd.Parameters.AddWithValue("@name", "dummy");
            con.Open();
            cmd.ExecuteNonQuery();
            Inventory inv = new Inventory(lastid + 1);
            con.Close();

            return inv;

        }

        public static int getLastID()
        {
            int i = 0;
            con = new MySqlConnection(conString);
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM InventoryData";
            con.Open();
            reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                int a = reader.GetInt32("id");
                if(a>i)
                {
                    i = a;
                }
            }
            con.Close();
            return i;
        }

        public static void Update(Inventory inv)
        {
            con = new MySqlConnection(conString);
            cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM InventoryData WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", inv.id);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            foreach(string i in inv.items.Keys)
            {
                con = new MySqlConnection(conString);
                cmd = con.CreateCommand();
                cmd.CommandText = "INSERT INTO InventoryData (id, name, amount) VALUES (@id,@name,@amount)";
                cmd.Parameters.AddWithValue("@id", inv.id);
                cmd.Parameters.AddWithValue("@name", i);
                cmd.Parameters.AddWithValue("@amount", inv.items[i]);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

        }

        public static Boolean isTableCreated()
        {
            try
            {
                con = new MySqlConnection(conString);
                cmd = con.CreateCommand();
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS InventoryData(Name VARCHAR(100),amount int, id int)";
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void loadInvs()
        {
            con = new MySqlConnection(conString);
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM InventoryData WHERE name=@name";
            cmd.Parameters.AddWithValue("@name", "dummy");
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Inventory inv = new Inventory(reader.GetInt32("id"));
            }
            con.Close();

            con = new MySqlConnection(conString);
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM InventoryData";
            cmd.Parameters.AddWithValue("@name", "dummy");
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Inventory inv = methods.getMethods.getInvById(reader.GetInt32("id"));
                inv.addItem(reader.GetString("name"), reader.GetInt32("amount"));
            }
            con.Close();
        }
    }
}
