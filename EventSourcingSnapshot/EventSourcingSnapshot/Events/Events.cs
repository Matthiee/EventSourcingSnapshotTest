namespace EventSourcingSnapshot.Events
{
    public interface IEvent { }

    public record BasketItemAddedEvent(int ItemId, int Quantity, double Price):IEvent;
    public record BasketItemRemovedEvent(int ItemId, int Quantity, double Price):IEvent;
    public record BasketItemRebaseEvent(int ItemId, double Adjustor):IEvent;
}
