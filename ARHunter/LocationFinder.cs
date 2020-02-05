using System;
using CoreLocation;
using UIKit;
using MapKit;
using Foundation;

namespace ARHunter
{
    public class LocationFinder
    {
        private readonly CLLocationManager locationManager;
        internal Data data;
        private bool started = false;
        public event EventHandler<LocationUpdatedEventArgs>LocationUpdated = delegate { };

        public LocationFinder()
        {
            locationManager = new CLLocationManager();
            locationManager.PausesLocationUpdatesAutomatically = false;

            if (CLLocationManager.LocationServicesEnabled)
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                {
                    locationManager.RequestAlwaysAuthorization();
                    if (ViewController.debugPrint) Console.WriteLine("Requested Always");
                    //locationManager.RequestWhenInUseAuthorization();
                }
                else
                {
                    locationManager.RequestWhenInUseAuthorization();
                    if (ViewController.debugPrint) Console.WriteLine("Requested In Use");
                }
            }
            else
            {
                UIAlertView alerting = new UIAlertView()
                {
                    Title = "Location Services OFF",
                    Message = "Please turn on location services in settings." + Environment.NewLine + "Turn on: Privacy->Location Services"
                };

                alerting.AddButton("OK");
                alerting.Show();
            }
            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                locationManager.AllowsBackgroundLocationUpdates = true;
            }

            /*else if (MapView.UserLocation == null || !MapView.UserLocationVisible)
            {
                UIAlertView alert = new UIAlertView()
                {
                    Title = "Location",
                    Message = "Locatoin services not found." + Environment.NewLine + "Turn on: Privacy->Location Services->ARHunter"
                    };


                alert.AddButton("OK");
                alert.Show();
            } //Debug*/

            LocationUpdated += PrintLocation;
            LocationUpdated += UpdateData;

        }

        public void StartLocationUpdates()
        {
            if (CLLocationManager.LocationServicesEnabled) {
                locationManager.DesiredAccuracy = 1;
                locationManager.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
                {
                LocationUpdated (this, new LocationUpdatedEventArgs (e.Locations [e.Locations.Length - 1]));
                };
                locationManager.StartUpdatingLocation();
            }

        }
        public void StartDataCollection()
        {
            //// Add a try catch
            try
            {
                data = new Data(locationManager.Location.Coordinate);//Initiallized data with the starting coordinate
            }
            catch { }
            started = true;
        }
        public void EndDataCollection()
        {
            started = false;
        }
        public void EndLocationUpdates()
        {
            started = false;
            locationManager.StopUpdatingLocation();
        }
        public CLLocationCoordinate2D GetLocation()
        {
            return locationManager.Location.Coordinate;
        }
        public void UpdateData(object sender, LocationUpdatedEventArgs e)
        {
            if (started)
            {
                if (ViewController.debugPrint) Console.WriteLine("::Data::");
                data.Add(e.Location.Coordinate);    //Adds each coordinate to the list in data
                //if (ViewController.sound.started)   
                //    Console.WriteLine("SOUND UPDATED " + ViewController.sound.workVolume());
            }
        }

        public void PrintLocation(object sender, LocationUpdatedEventArgs e) 
        { 
            CLLocation loc = e.Location;
            if (ViewController.debugPrint) Console.WriteLine("Altitue: " + (loc.Altitude * 3.280839895) + "ft");
            if (ViewController.debugPrint) Console.WriteLine("Lon: " + loc.Coordinate.Longitude);
            if (ViewController.debugPrint) Console.WriteLine("Lat: "+ loc.Coordinate.Latitude);
            if(loc.Course >=0 && loc.Course <90 )
                if (ViewController.debugPrint) Console.WriteLine("Course: North");
            else if (loc.Course >= 90 && loc.Course < 180)
                    if (ViewController.debugPrint) Console.WriteLine("Course: East");
            else if (loc.Course >= 180 && loc.Course < 270)
                        if (ViewController.debugPrint) Console.WriteLine("Course: South");
            else if (loc.Course >= 270 && loc.Course < 360)
                            if (ViewController.debugPrint) Console.WriteLine("Course: West");
            else
                if (ViewController.debugPrint) Console.WriteLine("Course: " + loc.Course);
            if(loc.Speed > 0)
                if (ViewController.debugPrint) Console.WriteLine("Speed: " +  (loc.Speed * 2.23694) + "MPH");
            else
                if (ViewController.debugPrint) Console.WriteLine("Speed: 0" + "MPH");

        }



    }
}
public class LocationUpdatedEventArgs : EventArgs
{
    CLLocation  location;

    public LocationUpdatedEventArgs(CLLocation location)
    {
        this.location = location;
    }

    public CLLocation Location
    {
       get { return location; }
    }
}

