using System.Collections.Generic;

namespace EventSourcingSnapshot.Models
{
    public record BasketViewModel(List<BasketItemViewModel> Items);
}
