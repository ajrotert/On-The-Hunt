﻿using System;
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
        ////     internal static Audio sound = new Audio();
        Thread AudioThread;
        private int AudioThreadSleep = 15;

        public static bool debugPrint = false;

        private Queue<CLLocationCoordinate2D> Shots_Found = new Queue<CLLocationCoordinate2D>();

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
            LocationFinderManager = new LocationFinder();
            LocationFinderManager.StartLocationUpdates();
            HeaderFinderManager = new HeaderFinder();
            HeaderFinderManager.StartLocationUpdates();
        }

        private readonly CLLocationManager locationManager = new CLLocationManager();

        //Called when the foreground is active, changes can only be made here, sound is updated here, Only UI is changed, referenced in viewdidload
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

            /*if (sound.started)
                SoundBar.Progress = sound.workVolume();*/
            if (debugPrint) Console.WriteLine("Foreground Location Updated");
        }
        //Called when the foreground is active, sound is called here. Chages are only to UI elements, reference viewdidload
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
            /*if(sound.started)
                SoundBar.Progress = sound.workVolume();*/
            if (debugPrint) Console.WriteLine("Foreground Header Updated");
        }

        private void UpdateMap()
        {
            while (Shots_Found.Count > 0)
            {
                ShotDetected(Shots_Found.Dequeue());
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Shows the user loc. the entitlements, and info have to be updated
            MapView.ShowsUserLocation = true;

            //Reduces the visability of the map, appears as a tan tint
            MapView.Alpha = 0.5f;


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
            View.BackgroundColor = UIColor.FromRGB(135, 115, 72);

            MapView.MapType = MapKit.MKMapType.Hybrid;
            MapView.TintColor = UIColor.Black;
            MapView.ShowsCompass = true;
            MapView.ShowsScale = true;
            MapView.ShowsBuildings = false;

            //Adding are own map delegate for custom overlay views
            mapDelegate = new MapDelegate();
            MapView.Delegate = mapDelegate;

            //Initalizing Audio thread to the sound updates function
            ////       AudioThread = new Thread(SoundUpdates);

            AudioThread = new Thread(UpdateSoundBar);

            Pause_Button.Enabled = false;

            UpdateAnnotation();

            if (debugPrint) Console.WriteLine("Finished Loading");

            LowPassAudio au = new LowPassAudio();
            au.AudioSetupStart();

        }

        /* ////   public void SoundUpdates()
            {
                //This function is threaded from teh main thread.
                //Used to constantly update the sound levels
                if (debugPrint) Console.WriteLine("Sounds Update Thread");

                //Only run when the sound is started
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
                    //if (level == 1)
                    if(level > .9)
                    {
                        if (debugPrint) Console.WriteLine("Shot Found");
                        Shots_Found.Enqueue(LocationFinderManager.GetLocation());
                        if (debugPrint) Thread.Sleep(1500);
                    }
                }
            }

        */

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
            LocationFinderManager.EndDataCollection();
            LocationFinderManager.EndLocationUpdates();

            //Stops sound from being recorded.
            ////     sound.stop();

            printdata();

            //Update Map with shot annotations
            UpdateMap();

            //Creates route trace from collected data
            if (LocationFinderManager.data != null && LocationFinderManager.data.locs.Count > 1)
                CreateAnnotation(new Data(LocationFinderManager.data));

            try
            {
                Results(LocationFinderManager.data);
            }
            catch { }

            //CreateAnnotation(LocationFinderManager.data.locs.ToArray());

            //Kills off Audio thread if still active, and resets sound bar
            //**************************************
            AudioThread.Abort();
            SoundBar.Progress = 0;
            //**************************************
            //Changes UI color and control access
            Stop_Color();
            ////    sound = new Audio();
            ////    sound.stop();

            if (debugPrint) Console.WriteLine("Stop");

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

        partial void Pause_Clicked(UIButton sender)
        {
            //Prevents data from being added to the data array
            LocationFinderManager.EndDataCollection();

            //stops sound from recording
            ////    sound.stop();

            //Creating map route trace from collected data, rest collected data
            if (LocationFinderManager.data != null && LocationFinderManager.data.locs.Count > 1)
                CreateAnnotation(new Data(LocationFinderManager.data));

            //CreateAnnotation(LocationFinderManager.data.locs.ToArray());

            //Update Map with shot annotations
            UpdateMap();

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
            ////      sound.autoStart();

            //Thread Audio, begins listening for shots
            //**************************************
            AudioThread.Abort();                    //Abort current thread
                                                    ////      AudioThread = new Thread(SoundUpdates); //Create new instance
                                                    ////      AudioThread = new Thread(SoundUpdates); //Create new instance
            AudioThread = new Thread(UpdateSoundBar);
            AudioThread.Start();                    //Start threadss
            //**************************************

            //Changes UI color and control access
            Start_Color();

            if (debugPrint) Console.WriteLine("Start");
        }

        private void Start_Color()
        {
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
            MapView.AddAnnotations(new ShotsAnnotation("Shots Fired", new CLLocationCoordinate2D(locationManager.Location.Coordinate.Latitude, locationManager.Location.Coordinate.Longitude)));
            if (debugPrint) Console.WriteLine("Shots Fired Annotation");
        }
        private void ShotDetected(CLLocationCoordinate2D coordinate2D)
        {
            MapView.AddAnnotations(new ShotsAnnotation("Shots Detected", coordinate2D));
            if (debugPrint) Console.WriteLine("Shots Fired Annotation, Detected");
        }

        //private void CreateAnnotation(CLLocationCoordinate2D[] route)
        private void Results(Data route)
        {
            int counter = 0;
            foreach (CLLocationCoordinate2D l in route.locs)
            {
                counter++;
                if (debugPrint) Console.WriteLine(counter + " " + l.Latitude + " " + l.Longitude);
            }
            UIAlertView alerting = new UIAlertView()
            {
                Title = "Data Points Collected",
                ////            Message = counter + " points of reference" + Environment.NewLine + Environment.NewLine + "Shots Found: " + sound.High_Level_Detected
            };

            alerting.AddButton("Okay");
            alerting.Show();
        }
        private void CreateAnnotation(Data route)
        {
            //create a path from the route passed in
            //MKPolyline trace = MKPolyline.FromCoordinates(route);

            //Add to database
            DatabaseManagement.Add(route);

            UpdateAnnotation();

            if (debugPrint) Console.WriteLine("Line Trace");
        }
        private void UpdateAnnotation()
        {
            IMKOverlay[] overlays = MapView.Overlays;
            if (overlays != null)
                MapView.RemoveOverlays(MapView.Overlays);

            //Access database, will create table if needed
            DatabaseManagement.Access();
            Data[] traces = DatabaseManagement.GetAll();
            //Add Overlay to map
            foreach (var i in traces)
            {
                MapView.AddOverlay(MKPolyline.FromCoordinates(i.locs.ToArray()));
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void Segment_Changed(UISegmentedControl sender)
        {
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
    }
}