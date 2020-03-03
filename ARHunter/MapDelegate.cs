using System;
using CoreLocation;
using UIKit;
using MapKit;
using Foundation;
using CoreGraphics;
using AVFoundation;

namespace ARHunter
{
    public class MapDelegate : MKMapViewDelegate
    {
        public static UIColor color = UIColor.Red;
        private static string[] annotationId = { "ShotsAnnotation", "TraceAnnotation" };
        public MapDelegate()
        {
        }
        public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView annotationView = null;

            if (annotation is MKUserLocation)
                return null;

            else if (annotation is ShotsAnnotation)
            {
                //show annotation
                annotationView = mapView.DequeueReusableAnnotation(annotationId[0]);

                if (annotationView == null)
                    annotationView = new MKAnnotationView(annotation, annotationId[0]);

                annotationView.Image = UIImage.FromFile("ShotAnnotationpng.png");
                annotationView.CanShowCallout = true;
            }
            else if(annotation is TraceAnnotation)
            {
                //show annotation
                annotationView = mapView.DequeueReusableAnnotation(annotationId[1]);

                if (annotationView == null)
                    annotationView = new MKAnnotationView(annotation, annotationId[1]);

                annotationView.CanShowCallout = true;
            }

            return annotationView;
        }

        public override MKOverlayView GetViewForOverlay(MKMapView mapView, IMKOverlay overlay)
        {
            
            MKPolyline polyline = overlay as MKPolyline;
            try
            {
                MKPolylineView polylineView = new MKPolylineView(polyline);

                polylineView.StrokeColor = color;
                polylineView.LineWidth = 8;
                return polylineView;
            }
            catch { return null; }

        }
    }
}
/*
// return a view for the polygon
            MKPolygon polygon = overlay as MKPolygon;
            MKPolygonView polygonView = new MKPolygonView(polygon);
            polygonView.FillColor = UIColor.Clear;
            polygonView.StrokeColor = color;
            return polygonView;
*/