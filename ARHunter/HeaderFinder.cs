using System;
using CoreLocation;
using UIKit;
using MapKit;
using Foundation;

namespace ARHunter
{
    public class HeaderFinder
    {
        private readonly CLLocationManager locationManager;
        public event EventHandler<HeadingUpdatedEventArgs> HeadingUpdated = delegate { };
        public static readonly bool debugPrint = false;

        public HeaderFinder()
        {
            locationManager = new CLLocationManager();
            HeadingUpdated += PrintHeading;
        }
        public void StartLocationUpdates()
        {
            if (CLLocationManager.LocationServicesEnabled)
            {
                //set the desired accuracy, in meters
                locationManager.DesiredAccuracy = 1;
                locationManager.DistanceFilter = 10;
                locationManager.UpdatedHeading += (object sender, CLHeadingUpdatedEventArgs e) =>
                {
                    // fire our custom Location Updated event
                    HeadingUpdated(this, new HeadingUpdatedEventArgs(e.NewHeading));
                };
                locationManager.StartUpdatingHeading();
            }
        }

        public void PrintHeading(object sender, HeadingUpdatedEventArgs e)
        {
            //Used for debugging. Prints the heading.

            CLHeading head = e.GShead;

            if (head.TrueHeading >= 0 && head.TrueHeading < 90)
                if(debugPrint) Console.WriteLine("Course: True North");
            else if (head.TrueHeading >= 90 && head.TrueHeading < 180)
                if (debugPrint) Console.WriteLine("Course: True East");
            else if (head.TrueHeading >= 180 && head.TrueHeading < 270)
                if (debugPrint) Console.WriteLine("Course: True South");
            else if (head.TrueHeading >= 270 && head.TrueHeading < 360)
                if (debugPrint) Console.WriteLine("Course: True West");
            else
                if (debugPrint) Console.WriteLine("True Course: " + head.TrueHeading);

        }

    }
}
public class HeadingUpdatedEventArgs : EventArgs
{
    CLHeading head;

    public HeadingUpdatedEventArgs(CLHeading head)
    {
        this.head = head;
    }

    public CLHeading GShead
    {
        get { return head; }
    }
}

/*
Print heading
            //if (ViewController.sound.started)
            //    Console.WriteLine("SOUND UPDATED " + ViewController.sound.workVolume());
*/