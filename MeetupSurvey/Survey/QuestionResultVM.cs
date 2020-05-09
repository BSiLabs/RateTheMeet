using System;
using System.Collections.Generic;
using System.Linq;
using MeetupSurvey.DTO;
using Microcharts;
using ReactiveUI.Fody.Helpers;
using SkiaSharp;

namespace MeetupSurvey.Survey
{
    public class QuestionResultVM
    {
        public string Name { get; set; }
        public List<int> Ratings { get; set; }
        public double AverageRating { get; set; } = 0;
        public List<string> Comments { get; set; }
        [Reactive] public PointChart Chart { get; set; }

        public int OneStar { get; set; } = 0;
        public int TwoStars { get; set; } = 0;
        public int ThreeStars { get; set; } = 0;
        public int FourStars { get; set; } = 0;
        public int FiveStars { get; set; } = 0;

        public QuestionResultVM(QuestionResultDTO questionResultDTO)
        {
            this.Name = questionResultDTO.Name;
            this.Ratings = questionResultDTO.Ratings;
            this.AverageRating = questionResultDTO.AverageRating;
            this.Comments = questionResultDTO.Comments.Where(x => !string.IsNullOrEmpty(x)).ToList();

            foreach (int r in Ratings)
            {
                switch (r)
                {
                    case 1:
                        OneStar++;
                        break;
                    case 2:
                        TwoStars++;
                        break;
                    case 3:
                        ThreeStars++;
                        break;
                    case 4:
                        FourStars++;
                        break;
                    case 5:
                        FiveStars++;
                        break;
                    default:
                        break;
                }
            }

            var entries = new List<ChartEntry>
             {
                 new ChartEntry(OneStar)
                 {
                     Label = "1 Star",
                     ValueLabel = OneStar.ToString(),
                     Color = SKColor.Parse("#FFFFFF")
                 },
                 new ChartEntry(TwoStars)
                 {
                     Label = "2 Star",
                     ValueLabel = TwoStars.ToString(),
                     Color = SKColor.Parse("#FFFFFF")
                 },
                 new ChartEntry(ThreeStars)
                 {
                     Label = "3 Star",
                     ValueLabel = ThreeStars.ToString(),
                     Color = SKColor.Parse("#FFFFFF")
                 },
                 new ChartEntry(FourStars)
                 {
                     Label = "4 Star",
                     ValueLabel = FourStars.ToString(),
                     Color = SKColor.Parse("#FFFFFF")
                 },
                 new ChartEntry(FiveStars)
                 {
                     Label = "5 Star",
                     ValueLabel = FiveStars.ToString(),
                     Color = SKColor.Parse("#FFFFFF")
                 }
                 };


            var chart = new PointChart()
            {
                Entries = entries,
                LabelTextSize = 30,
                PointMode = PointMode.Circle,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelColor = new SKColor(255, 255, 255),
                PointSize = 30,
                PointAreaAlpha = 200,
                BackgroundColor = SKColors.Transparent,
                Typeface = SKTypeface.FromFamilyName("Open Sans")
            };

            Chart = chart;
        }
    }
}
