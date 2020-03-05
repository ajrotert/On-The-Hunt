using System;
using CoreLocation;
using UIKit;
using MapKit;
using Foundation;
using CoreGraphics;
using AVFoundation;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace ARHunter
{
    public partial class ViewController : UIViewController
    {
        public static LocationFinder LocationFinderManager;
        public static HeaderFinder HeaderFinderManager;

        MapDelegate mapDelegate;
        internal static Audio sound = new Audio();
        Thread AudioThread;
        private const int AudioThreadSleep = 15;

        public static readonly bool debugPrint = true;

        private bool LocationAllowed;

        Thread Locations;
        UIAlertController alertController;

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.

            //Aks/gets permissions for location services
            LocationFinderManager = new LocationFinder();
            LocationAllowed = LocationFinderManager.LocationServeciesStarted(true);
            LocationAllowed = LocationFinderManager.StartLocationUpdates();
            HeaderFinderManager = new HeaderFinder();
            HeaderFinderManager.StartLocationUpdates();
        }

        private readonly CLLocationManager locationManager = new CLLocationManager();

        //Called when the foreground is active, updates the location course information.
        public void HandleLocationChanged(object sender, LocationUpdatedEventArgs e)
        {
            CLLocation loc = e.Location;

            BarButtonItem.Title = Convert.ToInt32(loc.Altitude * 3.280839895) + "ft";
            if (Convert.ToInt32(loc.Speed) > 0)
            {
                if ((loc.Course >= 315 && loc.Course <= 360) || (loc.Course >= 0 && loc.Course < 45))
                    BarButtonItem2.Image = UIImage.FromFile("ARH_Bar_North.png");
                else if (loc.Course >= 45 && loc.Course < 135)
                    BarButtonItem2.Image = UIImage.FromFile("ARH_Bar_East.png");
                else if (loc.Course >= 135 && loc.Course < 225)
                    BarButtonItem2.Image = UIImage.FromFile("ARH_Bar_South.png");
                else if (loc.Course >= 225 && loc.Course < 315)
                    BarButtonItem2.Image = UIImage.FromFile("ARH_Bar_West.png");

            }
            else
                BarButtonItem2.Image = UIImage.FromFile("ARH_Bar_Blank.png");

            MapView.UserTrackingMode = MKUserTrackingMode.Follow;


            if (debugPrint) Console.WriteLine("Foreground Location Updated");
        }
        //Called when the foreground is active, updates the heading information.
        public void HandleHeaderChanged(object sender, HeadingUpdatedEventArgs e)
        {
            CLHeading loc = e.GShead;
            if ((loc.TrueHeading >= 330 && loc.TrueHeading <= 360) || (loc.TrueHeading < 30 && loc.TrueHeading >= 0))
            {
                BarButtonItem4.Image = UIImage.FromFile("ARH_Bar_Compass.png");
            }
            else if (loc.TrueHeading >= 30 && loc.TrueHeading < 60)
            {
                BarButtonItem4.Image = UIImage.FromFile("ARH_Bar_Compass_NE.png");
            }
            else if (loc.TrueHeading >= 60 && loc.TrueHeading < 120)
            {
                BarButtonItem4.Image = UIImage.FromFile("ARH_Bar_Compass_E.png");
            }
            else if (loc.TrueHeading >= 120 && loc.TrueHeading < 150)
            {
                BarButtonItem4.Image = UIImage.FromFile("ARH_Bar_Compass_SE.png");
            }
            else if (loc.TrueHeading >= 150 && loc.TrueHeading < 210)
            {
                BarButtonItem4.Image = UIImage.FromFile("ARH_Bar_Compass_S.png");
            }
            else if (loc.TrueHeading >= 210 && loc.TrueHeading < 240)
            {
                BarButtonItem4.Image = UIImage.FromFile("ARH_Bar_Compass_SW.png");
            }
            else if (loc.TrueHeading >= 240 && loc.TrueHeading < 300)
            {
                BarButtonItem4.Image = UIImage.FromFile("ARH_Bar_Compass_W.png");
            }
            else if (loc.TrueHeading >= 300 && loc.TrueHeading < 330)
            {
                BarButtonItem4.Image = UIImage.FromFile("ARH_Bar_Compass_NW.png");
            }
            else
            {
                if (debugPrint) Console.WriteLine("NOT FOUND::" + loc.TrueHeading);
                BarButtonItem4.Image = UIImage.FromFile("ARH_Bar_Blank.png");
            }
            
            if (debugPrint) Console.WriteLine("Foreground Header Updated");
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Shows the user location if the entitlements.plist, and info.plist have the correct information
            MapView.ShowsUserLocation = true;

            //Reduces the visability of the map, appears as a tan tint
            MapView.Alpha = 0.5f;
            View.BackgroundColor = UIColor.FromRGB(135, 115, 72);

            //Setting UI colors
            BarButtonItem.TintColor = UIColor.LightGray;
            BarButtonItem5.TintColor = UIColor.LightGray;
            MapDelegate.color = UIColor.Black;

            //When the user enters the foreground
            UIApplication.Notifications.ObserveDidBecomeActive((sender, args) =>
            {       //THIS WILL HANDLE UPDATES TO THE USER INTERFACE
                LocationFinderManager.LocationUpdated += HandleLocationChanged;
                HeaderFinderManager.HeadingUpdated += HandleHeaderChanged;
                if (debugPrint) Console.WriteLine("Active::");
            });
            //When the user leaves the app
            UIApplication.Notifications.ObserveDidEnterBackground((sender, args) =>
            {       //STOPS UPDATES FROM THE USER INTERFACE
                LocationFinderManager.LocationUpdated -= HandleLocationChanged;
                HeaderFinderManager.HeadingUpdated -= HandleHeaderChanged;
                if (debugPrint) Console.WriteLine("Deactive::");
            });

            //Map settings
            MapView.MapType = MapKit.MKMapType.Hybrid;
            MapView.TintColor = UIColor.Black;
            MapView.ShowsCompass = true;
            MapView.ShowsScale = true;
            MapView.ShowsBuildings = false;

            //Adding are own map delegate for custom overlay views
            mapDelegate = new MapDelegate();
            MapView.Delegate = mapDelegate;

            //Running audio updates concurrently 
            AudioThread = new Thread(SoundUpdates);

            Pause_Button.Enabled = false;

            //Adds any existing map traces to the map
            UpdateAnnotation();

            if (debugPrint) Console.WriteLine("Finished Loading");

        }

        public override void ViewDidAppear(bool animated)
        {
            //Starts collecting data
            //Can only ask for permission if the main view is loaded
            StartFuction();
        }

        //Starts collecting data, or asks for permisssion
        private void StartFuction()
        {
            if(!LocationAllowed)
            {
                Locations = new Thread(AskForLocation);
                Locations.Start();
            }
            else
            {
                LocationFinderManager.StartDataCollection();
            }
        }

        //Stops collecting data, and stops any location updates
        private void StopFunction()
        {
            if (Locations != null)
                Locations.Abort();
            LocationFinderManager.EndDataCollection();
            LocationFinderManager.EndLocationUpdates();

            if(MapView != null)
            {
                MapView.ShowsUserLocation = false;
                MapView.UserTrackingMode = MKUserTrackingMode.None;
            }
        }

        //Thread to handle location permissions.
        //Threaded so that the user keeps getting asked until they answer.
        private void AskForLocation()
        {
            int WaitTime = 5000;
            while (!LocationAllowed)
            {
                Thread.Sleep(WaitTime);
                ShowErrorMessage("Location Services Not Allowed", "Please turn on location services in settings.\nTurn on: Privacy->Location Services");
                LocationAllowed = LocationFinderManager.LocationServeciesStarted(false);
                LocationAllowed = LocationFinderManager.StartLocationUpdates();
                WaitTime += WaitTime;
                if (WaitTime > 60000)
                    WaitTime = 60000;
            }
            LocationFinderManager.StartDataCollection();
        }

        //Creates a popup with custom error message
        public void ShowErrorMessage(string title = "Location Services OFF", string message = "Please turn on location services in settings.\nTurn on: Privacy->Location Services")
        {
            InvokeOnMainThread(delegate
            {
                alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.ActionSheet);
                //Add Action
                alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                // Present Alert
                PresentViewController(alertController, true, null);
            });
        }
        public void SoundUpdates()
        {
            //This function is threaded from the main thread.
            //Used to constantly update the sound levels
            if (debugPrint) Console.WriteLine("Sounds Update Thread");

            //If the sound is not started the thread will end
            while (sound.started)
            {
                //Gets the sound level
                float level = sound.workVolume();

                //If the application is in the foreground then the progress bar in the main thread will be updated
                InvokeOnMainThread(delegate
                {
                    UIApplicationState state = UIApplication.SharedApplication.ApplicationState;
                    if (state == UIApplicationState.Active)
                    {
                        SoundBar.Progress = level;
                    }
                });

                //Time delay, uses a variable, power saving mode may change this to update less
                Thread.Sleep(AudioThreadSleep);

                if(level > .9)
                {
                    if (debugPrint) Console.WriteLine("Shot Found");
                    if (debugPrint) Thread.Sleep(1500);
                }
            }
        }


        partial void BarButtonItem5_Activated(UIBarButtonItem sender)
        {
            //Right now this just places a shots detected annotation on the map
            //Should eventually allow for custom text to be applied
            ShotDetected();
            if (debugPrint) Console.WriteLine("Annotation Added");
        }

        partial void BarButtonItem3_Activated(UIBarButtonItem sender)
        {
            //Centers the map around the users location
            Center();
            if (debugPrint) Console.WriteLine("Centered Clicked");
        }

        partial void Stop_Clicked(UIButton sender)
        {

            //Prevents data from being added to the data array, Stops location from being updated
            StopFunction();

            //Stops sound from being recorded.
            sound.stop();

            //Creates route trace from collected data
            if (LocationFinderManager.data != null && LocationFinderManager.data.locs.Count > 1)
                CreateAnnotation(new Data(LocationFinderManager.data));

            try
            {
                Results(LocationFinderManager.data);
            }
            catch { }

            //Kills off Audio thread if still active, and resets sound bar
            //**************************************
            AudioThread.Abort();
            SoundBar.Progress = 0;
            //**************************************
            //Changes UI color and control access
            Stop_Color();
            sound = new Audio();
            sound.stop();

            if (debugPrint) Console.WriteLine("Stop");

        }



        partial void Pause_Clicked(UIButton sender)
        {
            //Prevents data from being added to the data array
            LocationFinderManager.EndDataCollection();

            //stops sound from recording
            sound.stop();

            //Creating map route trace from collected data, rest collected data
            if (LocationFinderManager.data != null && LocationFinderManager.data.locs.Count > 1)
                CreateAnnotation(new Data(LocationFinderManager.data));

            //Data is already saved removes everything, prevents duplicated data
            LocationFinderManager.data.locs.Clear();

            //Changes UI color and control access
            Pause_Color();

            if (debugPrint) Console.WriteLine("Pause");
        }

        partial void Start_Clicked(UIButton sender)
        {
            //Centers the screen around the user location
            Center();

            //Starts collecting data, overriding anything priorly stored. Starts Location updates if they were not already started
            LocationFinderManager.StartLocationUpdates();
            LocationFinderManager.StartDataCollection();

            //Starts a new AudioQueue
            sound.autoStart();

            //Thread Audio, begins listening for shots
            //**************************************
            AudioThread.Abort();                    //Abort current thread
            AudioThread = new Thread(SoundUpdates); //Create new instance
            AudioThread.Start();                    //Start threadss
            //**************************************

            //Changes UI color and control access
            Start_Color();

            if (debugPrint) Console.WriteLine("Start");
        }

        private void Start_Color()
        {
            //Update colors and button access
            MapView.TintColor = UIColor.FromRGB(38, 102, 24);
            MapType_Segmented.TintColor = UIColor.FromRGB(38, 102, 24);
            //MapDelegate.color = UIColor.FromRGB(38, 102, 24);
            MapType_Segmented.BackgroundColor = UIColor.LightGray;
            if (debugPrint) Console.WriteLine("Start Color");

            Start_Button.Enabled = false;
            Pause_Button.Enabled = true;
            Stop_Button.Enabled = true;
        }
        private void Pause_Color()
        {
            //Update colors and button access
            MapView.TintColor = UIColor.FromRGB(186, 186, 44);
            MapType_Segmented.TintColor = UIColor.FromRGB(186, 186, 44);
            //MapDelegate.color = UIColor.FromRGB(186, 186, 44);
            MapType_Segmented.BackgroundColor = UIColor.DarkGray;
            if (debugPrint) Console.WriteLine("Pause Color");

            Start_Button.Enabled = true;
            Pause_Button.Enabled = false;
            Stop_Button.Enabled = true;
        }
        private void Stop_Color()
        {
            //Update colors and button access
            MapView.TintColor = UIColor.FromRGB(175, 32, 24);
            MapType_Segmented.TintColor = UIColor.FromRGB(175, 32, 24);
            //MapDelegate.color = UIColor.FromRGB(175, 32, 24);
            MapDelegate.color = UIColor.Black;
            MapType_Segmented.BackgroundColor = UIColor.LightGray;
            if (debugPrint) Console.WriteLine("Stop Color");

            Start_Button.Enabled = true;
            Pause_Button.Enabled = false;
            Stop_Button.Enabled = false;
        }
        private void Center()
        {
            //Centers the map around the user location
            double lat = MapView.UserLocation.Coordinate.Latitude;
            double lon = MapView.UserLocation.Coordinate.Longitude;
            var mapCenter = new CLLocationCoordinate2D(lat, lon);
            var mapRegion = MKCoordinateRegion.FromDistance(mapCenter, 300, 300);
            MapView.CenterCoordinate = mapCenter;
            MapView.Region = mapRegion;
            if (debugPrint) Console.WriteLine("Center");
        }
        private void ShotDetected()
        {
            //Creates a map annotation on the users current location
            MapView.AddAnnotations(new ShotsAnnotation("Shots Fired", new CLLocationCoordinate2D(locationManager.Location.Coordinate.Latitude, locationManager.Location.Coordinate.Longitude)));
            if (debugPrint) Console.WriteLine("Shots Fired Annotation");
        }
        private void ShotDetected(CLLocationCoordinate2D coordinate2D)
        {
            //Creates a map annotation on the paramater location
            MapView.AddAnnotations(new ShotsAnnotation("Shots Detected", coordinate2D));
            if (debugPrint) Console.WriteLine("Shots Fired Annotation, Detected");
        }

        private void Results(Data route)
        {
            //Used for debuging. Shows the users locations and the number of data points
            int counter = 0;
            foreach (CLLocationCoordinate2D l in route.locs)
            {
                counter++;
                if (debugPrint) Console.WriteLine(counter + " " + l.Latitude + " " + l.Longitude);
            }
            UIAlertView alerting = new UIAlertView()
            {
                Title = "Data Points Collected",
                Message = counter + " points of reference" + Environment.NewLine + Environment.NewLine + "Shots Found: " + sound.High_Level_Detected
            };

            alerting.AddButton("Okay");
            alerting.Show();
        }
        private void CreateAnnotation(Data route)
        {
            //create a path from the route passed in

            //Add to database
            int key = DatabaseManagement.AddTrace(route);


            if (route.locs.Count > 1)
            {
                //Create annotation data for the start of the route 
                annotationData annotation;
                annotation.title = "Route Trace";
                annotation.key = key;
                annotation.data = route.locs[1];

                //Add annotatoin to database
                DatabaseManagement.AddAnnotation(annotation);
            }

            //Adds annotation and route to map
            UpdateAnnotation();

            if (debugPrint) Console.WriteLine("Line Trace");
        }
        private void UpdateAnnotation()
        {
            IMKOverlay[] overlays = MapView.Overlays;
            if (overlays != null)
                MapView.RemoveOverlays(MapView.Overlays);
            IMKAnnotation[] mKAnnotation = MapView.Annotations;
            if (mKAnnotation != null)
                MapView.RemoveAnnotations(mKAnnotation);

            //Access database, will create table if needed
            DatabaseManagement.BuildAllTables();
            Data[] traces = DatabaseManagement.GetAllTraces();
            //Add Overlay to map
            foreach (var i in traces)
            {
                MapView.AddOverlay(MKPolyline.FromCoordinates(i.locs.ToArray()));
            }

            annotationData[] annotations = DatabaseManagement.GetAllAnnotations();
            //Add Overlay to map
            foreach (var j in annotations)
            {
                MapView.AddAnnotations(new TraceAnnotation(j));
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void Segment_Changed(UISegmentedControl sender)
        {
            //Different Map views
            nint index = MapType_Segmented.SelectedSegment;
            if (index == 0)
            {
                MapView.MapType = MapKit.MKMapType.Hybrid;
                MapView.Alpha = .5f;
                View.BackgroundColor = UIColor.FromRGB(135, 115, 72);
            }

            else if (index == 1)
            {
                MapView.MapType = MapKit.MKMapType.Satellite;
                MapView.Alpha = .5f;
                View.BackgroundColor = UIColor.FromRGB(135, 115, 72);
            }
            else if (index == 2)
            {
                MapView.MapType = MapKit.MKMapType.Standard;
                MapView.Alpha = .75f;
                View.BackgroundColor = UIColor.Black;
            }
        }

    }
}


/*
View Controller Top
     private Queue<CLLocationCoordinate2D> Shots_Found = new Queue<CLLocationCoordinate2D>();



View Controller Functions
        private void UpdateMap()
        {
            while (Shots_Found.Count > 0)
            {
                ShotDetected(Shots_Found.Dequeue());
            }
        }

            private void printdata()
        {
            //int size = sound.data2d.Count;
            //Console.WriteLine(size);
            //for (int a = 0; a < size; a++)
            //{
            //   Console.Write("(" + sound.data2d[a].time + "," + sound.data2d[a].AvgPw + "," + sound.data2d[a].PekPw+ ") , ");
            //    test += ("(" + sound.data2d[a].time + "," + sound.data2d[a].AvgPw + "," + sound.data2d[a].PekPw + ") , ");

            //}
            //Console.WriteLine();
            //test += Environment.NewLine;

            //SoundTestingController.AudioDataTestPoints = test;

            //var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //var filename = Path.Combine(documents, "Write.txt");
            //var results = File.ReadAllText(filename);
            //Console.WriteLine(results);

        }
        //private void CreateAnnotation(CLLocationCoordinate2D[] route)






        //Used to update the low pass audio
        public void UpdateSoundBar()
        {
            while (LowPassAudio.started)
            {
                while (LowPassAudio.FloatQueue.Count > 0)
                {
                    InvokeOnMainThread(delegate
                    {
                        UIApplicationState state = UIApplication.SharedApplication.ApplicationState;
                        if (state == UIApplicationState.Active)
                        {
                            SoundBar.Progress = LowPassAudio.FloatQueue.Dequeue();
                        }
                    });
                    Thread.Sleep(15);
                }
                Thread.Sleep(500);
            }


        }





Sound updates
                        Shots_Found.Enqueue(LocationFinderManager.GetLocation());
                        printdata();

Stop Clicked
            //Update Map with shot annotations
            UpdateMap();
            //CreateAnnotation(LocationFinderManager.data.locs.ToArray());

Paused Clicked
            //CreateAnnotation(LocationFinderManager.data.locs.ToArray());
            //Update Map with shot annotations
            UpdateMap();

Start Clicked
            // AudioThread = new Thread(UpdateSoundBar);

Create annotation
            //MKPolyline trace = MKPolyline.FromCoordinates(route);


    //Removed from the view controller
    //location handler, and heading handler
               if (sound.started)
                SoundBar.Progress = sound.workVolume();

*/
