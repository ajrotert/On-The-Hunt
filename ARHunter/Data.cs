using System;
using CoreLocation;
using UIKit;
using MapKit;
using Foundation;
using CoreGraphics;
using AVFoundation;

using System.Collections.Generic;

namespace ARHunter
{
    public class Data
    {
        public List<CLLocationCoordinate2D> locs = new List<CLLocationCoordinate2D>();
        public List<double> locLongitude = new List<double>();
        public List<double> locLatitude = new List<double>();

        public Data(CLLocationCoordinate2D cL)
        {
            locs.Add(cL);//starting location
        }
        public Data(Data d)
        {
            locs = d.locs;
            locLatitude = d.locLatitude;
            locLongitude = d.locLongitude;
        }
        public Data(double[] longitude, double[] latitude)
        {
            for(int a =0; a<longitude.Length && a<latitude.Length; a++)
            {
                locs.Add(new CLLocationCoordinate2D(latitude[a], longitude[a]));
            }
            //locLongitude = longitude;
            //locLatitude = latitude;
        }
        public void Add(CLLocationCoordinate2D coordinate)
        {
            //double a = Math.Sqrt(Math.Pow(locs[locs.Count-1].Latitude, 2) - Math.Pow(coordinate.Latitude, 2) + Math.Pow(locs[locs.Count-1].Longitude, 2) - Math.Pow(coordinate.Longitude, 2));
            //Console.WriteLine("Distance: " + a);
            //if(a>.0025)
            //locs.Add(coordinate);

            if (GetDistance(locs[locs.Count - 1].Longitude, locs[locs.Count - 1].Latitude, coordinate.Longitude, coordinate.Latitude) > 8)
            {
                locs.Add(coordinate);
                locLongitude.Add(coordinate.Longitude);
                locLatitude.Add(coordinate.Latitude);
                if (false) Console.WriteLine("Coordinate Added");
            }

        }
        public double GetDistance(double longitude, double latitude, double otherLongitude, double otherLatitude)
        {
            var d1 = latitude * (Math.PI / 180.0);
            var num1 = longitude * (Math.PI / 180.0);
            var d2 = otherLatitude * (Math.PI / 180.0);
            var num2 = otherLongitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            var d4 = 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
            if (false) Console.WriteLine("D4: " + d4);
            return d4;
        }
    }
}
