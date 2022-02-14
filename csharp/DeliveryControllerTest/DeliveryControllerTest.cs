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
        public void Dont_send_an_email_when_no_delivery_schedule()
        {
            List<Delivery> deliverySchedule = new List<Delivery>();
            var deliveryController = new DeliveryController.DeliveryController(deliverySchedule, _emailGateway);

            deliveryController.UpdateDelivery(new DeliveryEvent("id", DateTime.Now, new Location(0, 0)));
            
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