using System;
using System.Collections.Generic;
using DeliveryController;
using Moq;
using Xunit;

namespace DeliveryControllerTest
{
    public class DeliveryControllerTest
    {
        private readonly EmailGateway _emailGateway = new FakeEmailGateway();
        [Fact]
        public void Should_not_send_an_email_when_no_delivery_schedule()
        {
            List<Delivery> deliverySchedule = new List<Delivery>();
            var deliveryController = new DeliveryController.DeliveryController(deliverySchedule, _emailGateway);

            deliveryController.UpdateDelivery(new DeliveryEvent("id", DateTime.Now, new Location(0, 0)));
            
            Assert.False(((FakeEmailGateway) _emailGateway).HasBeenSent);
        }
        
        [Fact]
        public void Should_send_an_email_when_current_delivery_is_not_scheduled()
        {
            List<Delivery> deliverySchedule = new List<Delivery>() { new Delivery("id", 
                "contact@email.com",
                new Location(0f, 0f),
                DateTime.Now,
                false,
                false) };
            var deliveryController = new DeliveryController.DeliveryController(deliverySchedule, _emailGateway);

            deliveryController.UpdateDelivery(new DeliveryEvent("anotherId", DateTime.Now, new Location(0, 0)));
            
            Assert.False(((FakeEmailGateway) _emailGateway).HasBeenSent);
        }
    }

    internal class FakeEmailGateway : EmailGateway
    {
        public bool HasBeenSent { get; private set; }
        public override void send(string address, string subject, string message)
        {
            HasBeenSent = true;
        }

    }
}