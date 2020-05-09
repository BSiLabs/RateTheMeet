using System;
namespace MeetupSurvey.Core
{
    public enum ContextAction { Delete, Archive, ViewPrizes, Edit, Gallery, Camera, Unpublish }
    public class ContextMenuAction
    {
        public string Id { get; set; }
        public ContextAction Action { get; set; }
    }
}
