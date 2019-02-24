using AGS.Engine;

namespace AGS.Editor
{
    //This empty class is needed to have a different type than IUIEvents, as we're passing a different UI aggregator
    //here which gets the input from the editor but hit-tests against the game. 
    //This is needed for the entity designer decorators (the resize/rotate/pivot handles).
    public class EditorUIEvents : AGSUIEvents
    {
        public EditorUIEvents(UIEventsAggregator aggregator) : base(aggregator)
        {
        }
    }
}