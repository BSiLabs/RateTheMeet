using System;
using MeetupSurvey.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ViewCell), typeof(CustomViewCellRenderer))]
namespace MeetupSurvey.iOS
{
        public class CustomViewCellRenderer : ViewCellRenderer
        {

            public override UIKit.UITableViewCell GetCell(Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv)
            {
                var cell = base.GetCell(item, reusableCell, tv);
                switch (item.StyleId)
                {
                    case "none":
                        cell.Accessory = UIKit.UITableViewCellAccessory.None;
                        cell.BackgroundColor = UIColor.Clear;
                        cell.TintColor = UIColor.Clear;
                        break;
                    case "checkmark":
                        cell.Accessory = UIKit.UITableViewCellAccessory.Checkmark;
                        break;
                    case "detail-button":
                        cell.Accessory = UIKit.UITableViewCellAccessory.DetailButton;
                        break;
                    case "detail-disclosure-button":
                        cell.Accessory = UIKit.UITableViewCellAccessory.DetailDisclosureButton;
                        break;
                    case "disclosure":
                        cell.Accessory = UIKit.UITableViewCellAccessory.DisclosureIndicator;
                        break;
                    default:
                        cell.Accessory = UIKit.UITableViewCellAccessory.DisclosureIndicator;
                        break;
                }
                return cell;
            }

        }
}
