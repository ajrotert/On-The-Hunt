using System;
using CoreLocation;
using UIKit;
using MapKit;
using Foundation;
using CoreGraphics;
using AVFoundation;

namespace ARHunter
{
    public class ShotsAnnotation : MKAnnotation
    {
        string title;
        CLLocationCoordinate2D coordinate;
        public ShotsAnnotation(string title, CLLocationCoordinate2D coordinate)
        {
            this.coordinate = coordinate;
            this.title = title;

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
