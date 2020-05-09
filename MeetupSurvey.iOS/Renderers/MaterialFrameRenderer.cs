using System;
using CoreGraphics;
using MeetupSurvey.Controls;
using MeetupSurvey.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FloatingFrame), typeof(MaterialFrameRenderer))]
namespace MeetupSurvey.iOS.Renderers
{
    //https://alexdunn.org/2017/05/01/xamarin-tips-making-your-ios-frame-shadows-more-material/
    public class MaterialFrameRenderer : FrameRenderer
    {
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);


            if (Element.HasShadow)
            {
                // Update shadow to match better material design standards of elevation
                Layer.ShadowRadius = 2.0f;
                Layer.ShadowColor = UIColor.LightGray.CGColor;
                Layer.ShadowOffset = new CGSize(2, 2);
                Layer.ShadowOpacity = 0.80f;
                Layer.ShadowPath = UIBezierPath.FromRect(Layer.Bounds).CGPath;
                //Layer.ShadowPath = UIBezierPath.FromRoundedRect(Layer.Bounds, 23).CGPath;
                Layer.MasksToBounds = false;
            }
        }
    }
}