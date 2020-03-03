using System;
using CoreLocation;
using UIKit;
using MapKit;
using Foundation;
using CoreGraphics;
using AVFoundation;

namespace ARHunter
{
    public class TraceAnnotation : MKAnnotation
    {
        string title;
        int primarykey;
        CLLocationCoordinate2D coordinate;
        public TraceAnnotation(string title, int key, CLLocationCoordinate2D coordinate)
        {
            this.coordinate = coordinate;
            this.title = title;
            primarykey = key;
        }

        public override string Title
        {
            get
            {
                return title;
            }
        }

        public override CLLocationCoordinate2D Coordinate
        {
            get
            {
                return coordinate;
            }
        }
    }
}
