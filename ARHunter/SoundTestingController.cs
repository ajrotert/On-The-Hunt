using Foundation;
using System;
using System.IO;
using UIKit;

namespace ARHunter
{
    public partial class SoundTestingController : UIViewController
    {
        public SoundTestingController (IntPtr handle) : base (handle)
        {
        }

        public static string AudioDataTestPoints ="";

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TextBox.Text = AudioDataTestPoints;

        }

        partial void Load_TouchUpInside(UIButton sender)
        {
            try
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var filename = Path.Combine(documents, "Write2.txt");
                var results = File.ReadAllText(filename);
                TextBox.Text = results;
            }
            catch
            {
                TextBox.Text = AudioDataTestPoints;
            }
        }
    }
}