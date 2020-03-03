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

        public string data { get; set; }

        public DateTime date { get; set; }
    }
    [Table("Annotation")]
    public class DatabaseManager_Annotation
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public string data { get; set; }

        public int foreignKey { get; set; }

        public string title { get; set; }

        public DateTime date { get; set; }
    }

    public static class DatabaseManagement
    {
        public static readonly bool debugPrint = false;

        public static void BuildAllTables()
        {
            string output = "";
            output += "\nCreating database, if it doesn't already exist";
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdata.db3");

            SQLiteConnection db = new SQLiteConnection(dbPath);
            db.CreateTable<DatabaseManager_Trace>();
            db.CreateTable<DatabaseManager_Annotation>();

            if (debugPrint) Console.WriteLine(output);
        }

        private static string format_trace(Data d)
        {
            string s="";

            for (int a = 0; a < d.locLatitude.Count && a < d.locLongitude.Count; a++)
            {
                s += d.locLatitude[a].ToString() + "_" + d.locLongitude[a].ToString() + Environment.NewLine;
            }
            s = s.Remove(s.Length - 1);

            return s;
        }
        private static string format_annotation(CLLocationCoordinate2D d)
        {
            string s = "";

            s = "" + d.Latitude+ '_' + d.Longitude;

            return s;
        }
        public static void AddTrace(Data trace)
        { 
            string output = "";
            output += "\nCreating database, if it doesn't already exist";
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdata.db3");

            SQLiteConnection db = new SQLiteConnection(dbPath);
            db.CreateTable<DatabaseManager_Trace>();

            DatabaseManager_Trace newData = new DatabaseManager_Trace();

            newData.data = format_trace(trace);
            newData.date = DateTime.Now;

            db.Insert(newData);

            if (debugPrint) Console.WriteLine(output);
        }
        public static void AddAnnotation(annotationData d)
        {
            string output = "";
            output += "\nCreating database, if it doesn't already exist";
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdata.db3");

            SQLiteConnection db = new SQLiteConnection(dbPath);
            db.CreateTable<DatabaseManager_Annotation>();

            DatabaseManager_Annotation newData = new DatabaseManager_Annotation();

            newData.title = d.title;
            newData.foreignKey = d.key;
            newData.data = format_annotation(d.data);

            if (debugPrint) Console.WriteLine(output);
        }

        public static Data[] GetAllTraces()
        {
            string output = "";
            output += "\nGet query example: ";
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdata.db3");

            SQLiteConnection db = new SQLiteConnection(dbPath);

            List<Data> list = new List<Data>();
            TableQuery<DatabaseManager_Trace> table = db.Table<DatabaseManager_Trace>();//get all items
            foreach (var s in table)
            {
                list.Add( deformater_trace(s.data) );
                output += "\n" + s.Id;
            }
            if (debugPrint) Console.WriteLine(output);
            return list.ToArray();
        }
        public static annotationData[] GetAllAnnotations()
        {   //title,key,data

            string output = "";
            output += "\nGet query example: ";
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdata.db3");

            SQLiteConnection db = new SQLiteConnection(dbPath);

            List<annotationData> list = new List<annotationData>();
            TableQuery<DatabaseManager_Annotation> table = db.Table<DatabaseManager_Annotation>();//get all items
            foreach (var s in table)
            {
                annotationData annotation;
                annotation.data = deformater_annotation(s.data);
                annotation.title = s.title;
                annotation.key = s.foreignKey;

                list.Add(annotation);
                output += "\n" + s.Id;
            }
            if (debugPrint) Console.WriteLine(output);
            return list.ToArray();
        }

        private static Data deformater_trace(string s)
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
        private static CLLocationCoordinate2D deformater_annotation(string s)
        {
            CLLocationCoordinate2D d;

            string[] dataSplit = s.Split('_');

            d.Latitude = Convert.ToDouble(dataSplit[0]);
            d.Longitude = Convert.ToDouble(dataSplit[1]);

            return d;
        }

        public static string DeleteTrace(int id)
        {
            string output = "";
            output += "\nDelete query example: ";
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdata.db3");

            SQLiteConnection db = new SQLiteConnection(dbPath);

            var rowcount = db.Delete(new DatabaseManager_Trace() { Id = id });

            return output;
        }
        public static string DeleteAnnotation(int id)
        {
            string output = "";
            output += "\nDelete query example: ";
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdata.db3");

            SQLiteConnection db = new SQLiteConnection(dbPath);

            var rowcount = db.Delete(new DatabaseManager_Annotation() { Id = id });

            return output;
        }

    }
}
/*

        //public CLLocationCoordinate2D[] MapTrace { get; set; }
        //public MKPolyline MapTrace { get; set; }
        //public Data MapTrace { get; set; }
        //public double[] Longitutde { get; set; }
        //public double[] Latitude { get; set; }

                //var returned = db.Get<Data>(2);//get one item

*/
