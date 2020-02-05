// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace ARHunter
{
    [Register ("SoundTestingController")]
    partial class SoundTestingController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton Load { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView TextBox { get; set; }

        [Action ("Load_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Load_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (Load != null) {
                Load.Dispose ();
                Load = null;
            }

            if (TextBox != null) {
                TextBox.Dispose ();
                TextBox = null;
            }
        }
    }
}