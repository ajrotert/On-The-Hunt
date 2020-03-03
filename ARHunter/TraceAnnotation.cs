using System;
using CoreLocation;
using UIKit;
using MapKit;
using Foundation;
using CoreGraphics;
using AVFoundation;

namespace ARHunter
{
    public struct annotationData
    {
        public string title;
        public int key;
        public CLLocationCoordinate2D data;
    }
    public class TraceAnnotation : MKAnnotation
    {
        string title;
        int primarykey;
        CLLocationCoordinate2D coordinate;
        public TraceAnnotation(annotationData d)
        {
            this.coordinate = d.data;
            this.title = d.title;
            primarykey = d.key;
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
