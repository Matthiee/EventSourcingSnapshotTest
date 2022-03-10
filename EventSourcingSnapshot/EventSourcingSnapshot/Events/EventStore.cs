using System;

namespace EventSourcingSnapshot.Events
{
    public class EventStore
    {
        public event EventHandler<IEvent> EventAdded;

        public void PublishEvent(IEvent @event)
        {
            EventAdded?.Invoke(this, @event);
        }
    }
}
