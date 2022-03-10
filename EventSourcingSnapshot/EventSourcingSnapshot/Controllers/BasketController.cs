using EventSourcingSnapshot.Events;
using EventSourcingSnapshot.Models;
using EventSourcingSnapshot.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingSnapshot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly BasketRepository repository;
        private readonly EventStore eventStore;

        public BasketController(BasketRepository repository, EventStore eventStore)
        {
            this.repository = repository;
            this.eventStore = eventStore;
        }

        [HttpGet]
        public ActionResult<BasketViewModel> GetBasket()
        {
            return Ok(repository.Get());
        }

        [HttpPost("add")]
        public ActionResult PostAdd([FromBody] ModelUpdateDto model)
        {
            eventStore.PublishEvent(new BasketItemAddedEvent(model.ItemId, model.Quantity, model.Price));

            return Ok();
        }

        [HttpPost("remove")]
        public ActionResult PostRemove([FromBody] ModelUpdateDto model)
        {
            eventStore.PublishEvent(new BasketItemRemovedEvent(model.ItemId, model.Quantity, model.Price));

            return Ok();
        }
    }
}
