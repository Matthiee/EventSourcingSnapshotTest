using EventSourcingSnapshot.Models;
using System.Collections.Generic;

namespace EventSourcingSnapshot.Services
{
    public class BasketRepository
    {
        private BasketViewModel latestInMemoryModel = new (new List<BasketItemViewModel>());

        public void Update(BasketViewModel newViewModel) 
        {
            latestInMemoryModel = newViewModel;
        }

        public BasketViewModel Get()
        {
            return latestInMemoryModel;
        }
    }
}
