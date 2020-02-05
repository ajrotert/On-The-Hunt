using System;
using System.IO;
using SQLite;
using System.Collections.Generic;
using MapKit;
using CoreLocation;

namespace ARHunter
{
    [Table("Items")]
    public class DatabaseManager_Trace
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        //public CLLocationCoordinate2D[] MapTrace { get; set; }
        //public MKPolyline MapTrace { get; set; }
        //public Data MapTrace { get; set; }
        //public double[] Longitutde { get; set; }
        //public double[] Latitude { get; set; }
        public string data { get; set; }

        public DateTime date { get; set; }
    }
    public static class DatabaseManagement
    {

        public static void Access()
        {
            string output = "";
            output += "\nCreating database, if it doesn't already exist";
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdata.db3");

            SQLiteConnection db = new SQLiteConnection(dbPath);
            db.CreateTable<DatabaseManager_Trace>();

            if (false) Console.WriteLine(output);
        }

        private static string format(Data d)
        {
            string s="";

            for (int a = 0; a < d.locLatitude.Count && a < d.locLongitude.Count; a++)
            {
                s += d.locLatitude[a].ToString() + "_" + d.locLongitude[a].ToString() + Environment.NewLine;
            }
            s = s.Remove(s.Length - 1);

            return s;
        }
        public static void Add(Data trace)
        {
            string output = "";
            output += "\nCreating database, if it doesn't already exist";
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdata.db3");

            SQLiteConnection db = new SQLiteConnection(dbPath);
            db.CreateTable<DatabaseManager_Trace>();

            DatabaseManager_Trace newData = new DatabaseManager_Trace();

            newData.data = format(trace);
            newData.date = DateTime.Now;

            db.Insert(newData);

            if (false) Console.WriteLine(output);
        }

        public static Data[] GetAll()
        {
            string output = "";
            output += "\nGet query example: ";
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdata.db3");

            SQLiteConnection db = new SQLiteConnection(dbPath);

            //var returned = db.Get<Data>(2);//get one item
            List<Data> list = new List<Data>();
            TableQuery<DatabaseManager_Trace> table = db.Table<DatabaseManager_Trace>();//get all items
            foreach (var s in table)
            {
                list.Add( deformater(s.data) );
                output += "\n" + s.Id;
            }
            if (false) Console.WriteLine(output);
            return list.ToArray();
        }
        private static Data deformater(string s)
        {
            Data d;

            string[] stringCoordinates = s.Split(Environment.NewLine);
            List<double> latitude = new List<double>();
            List<double> longitude = new List<double>();

            foreach (string coordinate in stringCoordinates)
            {
                string[] loc = coordinate.Split("_");
                latitude.Add(Convert.ToDouble(loc[0]));
                longitude.Add(Convert.ToDouble(loc[1]));
            }

            d = new Data(longitude.ToArray(), latitude.ToArray());
            return d;
        }

        public static string Delete(int id)
        {
            string output = "";
            output += "\nDelete query example: ";
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdata.db3");

            SQLiteConnection db = new SQLiteConnection(dbPath);

            var rowcount = db.Delete(new DatabaseManager_Trace() { Id = id });

            return output;
        }
        
    }
}
