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
        }
        public bool LocationServeciesStarted(bool PrintErrorMessage)
        {
            bool pass = true;
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
                if(PrintErrorMessage)
                {
                    try
                    {
                        var presentRootController = GetRootController();
                        presentRootController?.InvokeOnMainThread(delegate {
                            string TitleM = "Location Services OFF", MessageM = "Please turn on location services in settings." + Environment.NewLine + "Turn on: Privacy->Location Services";
                            UIAlertController alertController = UIAlertController.Create(TitleM, MessageM, UIAlertControllerStyle.ActionSheet);
                            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Destructive, null));
                            presentRootController?.PresentViewController(alertController, true, null);
                        });
                    }
                    catch
                    {
                        if (ViewController.debugPrint) Console.WriteLine("Error");
                    }
                }
                pass = false;
            }
            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                locationManager.AllowsBackgroundLocationUpdates = true;
            }

            LocationUpdated += PrintLocation;
            LocationUpdated += UpdateData;
            return pass;
        }

        public bool StartLocationUpdates()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                locationManager.AllowsBackgroundLocationUpdates = true;
            }
            if (CLLocationManager.LocationServicesEnabled) {
                locationManager.DesiredAccuracy = 1;
                locationManager.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
                {
                LocationUpdated (this, new LocationUpdatedEventArgs (e.Locations [e.Locations.Length - 1]));
                };
                locationManager.StartUpdatingLocation();
                return true;
            }
            else
            {
                if (ViewController.debugPrint) Console.WriteLine("Location services not turned on");
                return false;
            }

        }
        public void StartDataCollection()
        {
            try
            {
                data = new Data(locationManager.Location.Coordinate);
            }
            catch
            {
                if (ViewController.debugPrint) Console.WriteLine("Data Collection Failed");
            }
            started = true;
        }
        public void EndDataCollection()
        {
            /*From my other build
            try { data.locs.Clear(); }
            catch { Console.WriteLine("Data Empty"); }
             */
            started = false;
        }
        public void EndLocationUpdates()
        {
            started = false;
            locationManager.StopUpdatingLocation();
            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                locationManager.AllowsBackgroundLocationUpdates = false;
            }
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
                if (data == null)
                {
                    if (ViewController.debugPrint) Console.WriteLine("::Data is null::");
                    try
                    {
                        data = new Data(locationManager.Location.Coordinate);//Initiallized data with the starting coordinate

                    }
                    catch
                    {
                        data = new Data(new CLLocationCoordinate2D(0, 0));//Initiallized data with the starting coordinate
                    }
                }
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

        public UIViewController GetRootController()
        {
            var root = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (true)
            {
                switch (root)
                {
                    case UINavigationController navigationController:
                        root = navigationController.VisibleViewController;
                        continue;
                    case UITabBarController uiTabBarController:
                        root = uiTabBarController.SelectedViewController;
                        continue;
                }

                if (root.PresentedViewController == null) return root;
                root = root.PresentedViewController;
            }
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

