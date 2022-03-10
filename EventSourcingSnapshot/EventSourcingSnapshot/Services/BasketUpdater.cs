using EventSourcingSnapshot.Events;
using EventSourcingSnapshot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EventSourcingSnapshot.Services
{
    public class BasketUpdater
    {
        private readonly EventStore eventStore;
        private readonly BasketRepository repository;
        private readonly Timer timer;

        private readonly Queue<IEvent> eventQueue = new();

        public BasketUpdater(EventStore eventStore, BasketRepository repository)
        {
            this.eventStore = eventStore;
            this.repository = repository;

            this.eventStore.EventAdded += (o, e) => eventQueue.Enqueue(e);

            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void DoWork(object state)
        {
            if (!eventQueue.TryDequeue(out var e)) 
            {
                return;
            }

            var currentState = repository.Get();

            var update = ApplyEvent(currentState, e);

            repository.Update(update);
        }

        private BasketViewModel ApplyEvent(BasketViewModel source, IEvent e)
        {
            return e switch
            {
                BasketItemAddedEvent added => ApplyItemAdded(source, added),
                BasketItemRemovedEvent removed => ApplyItemRemoved(source, removed),
                _ => throw new NotImplementedException(),
            };
        }

        private static BasketViewModel ApplyItemAdded(BasketViewModel source, BasketItemAddedEvent e)
        {
            source.Deconstruct(out var items);

            var item = items.FirstOrDefault(item => item.ItemId == e.ItemId);

            if (item is null)
            {
                items.Add(new BasketItemViewModel(e.ItemId, e.Quantity, e.Price));
            }
            else
            {
                items.Remove(item);

                var newQty = item.Quantity + e.Quantity;
                var newAvgPrice = ((item.Quantity * item.AveragePrice) + (e.Quantity * e.Price)) / newQty;

                item = item with { Quantity = newQty, AveragePrice = newAvgPrice };

                items.Add(item);
            }

            return new BasketViewModel(items);
        }

        private static BasketViewModel ApplyItemRemoved(BasketViewModel source, BasketItemRemovedEvent e)
        {
            source.Deconstruct(out var items);

            var item = items.FirstOrDefault(item => item.ItemId == e.ItemId);

            if (item is null)
            {
                throw new InvalidOperationException();
            }
            else
            {
                items.Remove(item);

                var newQty = item.Quantity - e.Quantity;
                var newAvgPrice = ((item.Quantity * item.AveragePrice) - (e.Quantity * e.Price)) / newQty;

                item = item with { Quantity = newQty, AveragePrice = newAvgPrice };

                items.Add(item);
            }

            return new BasketViewModel(items);
        }
    }
}
