// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace ARHunter
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem BannerLogo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem BarButtonItem { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem BarButtonItem2 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem BarButtonItem3 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem BarButtonItem4 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem BarButtonItem5 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationItem MainTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView MainView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl MapType_Segmented { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MapKit.MKMapView MapView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton Pause_Button { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIProgressView SoundBar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton Start_Button { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton Stop_Button { get; set; }

        [Action ("BarButtonItem3_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BarButtonItem3_Activated (UIKit.UIBarButtonItem sender);

        [Action ("BarButtonItem5_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BarButtonItem5_Activated (UIKit.UIBarButtonItem sender);

        [Action ("Pause_Clicked:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Pause_Clicked (UIKit.UIButton sender);

        [Action ("Segment_Changed:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Segment_Changed (UIKit.UISegmentedControl sender);

        [Action ("Start_Clicked:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Start_Clicked (UIKit.UIButton sender);

        [Action ("Stop_Clicked:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Stop_Clicked (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BannerLogo != null) {
                BannerLogo.Dispose ();
                BannerLogo = null;
            }

            if (BarButtonItem != null) {
                BarButtonItem.Dispose ();
                BarButtonItem = null;
            }

            if (BarButtonItem2 != null) {
                BarButtonItem2.Dispose ();
                BarButtonItem2 = null;
            }

            if (BarButtonItem3 != null) {
                BarButtonItem3.Dispose ();
                BarButtonItem3 = null;
            }

            if (BarButtonItem4 != null) {
                BarButtonItem4.Dispose ();
                BarButtonItem4 = null;
            }

            if (BarButtonItem5 != null) {
                BarButtonItem5.Dispose ();
                BarButtonItem5 = null;
            }

            if (MainTitle != null) {
                MainTitle.Dispose ();
                MainTitle = null;
            }

            if (MainView != null) {
                MainView.Dispose ();
                MainView = null;
            }

            if (MapType_Segmented != null) {
                MapType_Segmented.Dispose ();
                MapType_Segmented = null;
            }

            if (MapView != null) {
                MapView.Dispose ();
                MapView = null;
            }

            if (Pause_Button != null) {
                Pause_Button.Dispose ();
                Pause_Button = null;
            }

            if (SoundBar != null) {
                SoundBar.Dispose ();
                SoundBar = null;
            }

            if (Start_Button != null) {
                Start_Button.Dispose ();
                Start_Button = null;
            }

            if (Stop_Button != null) {
                Stop_Button.Dispose ();
                Stop_Button = null;
            }
        }
    }
}