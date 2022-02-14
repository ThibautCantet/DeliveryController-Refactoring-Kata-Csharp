using System;
using System.Collections.Generic;
using System.Globalization;
using DeliveryController;
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
            
            Assert.Empty(((FakeEmailGateway) _emailGateway).Subjects);
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
            
            Assert.Empty(((FakeEmailGateway) _emailGateway).Subjects);
        }
        
        [Fact]
        public void Should_send_a_feedback_email_when_current_delivery_is_scheduled()
        {
            var nom = DateTime.ParseExact($"15/02/2022 13:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            List<Delivery> deliverySchedule = new List<Delivery>() { new Delivery("id", 
                "contact@email.com",
                new Location(0f, 0f),
                nom,
                false,
                false) };
            var deliveryController = new DeliveryController.DeliveryController(deliverySchedule, _emailGateway);

            deliveryController.UpdateDelivery(new DeliveryEvent("id", nom, new Location(0, 0)));
            
            Assert.Contains("Your feedback is important to us",  ((FakeEmailGateway) _emailGateway).Subjects);
            Assert.Contains("Regarding your delivery today at 02/15/2022 13:00:00. How likely would you be to recommend this delivery service to a friend? Click <a href='url'>here</a>",  ((FakeEmailGateway) _emailGateway).Messages);
        }
    }

    internal class FakeEmailGateway : EmailGateway
    {
        public FakeEmailGateway()
        {
            Subjects = new List<string>();
            Messages = new List<string>();
        }

        public bool HasBeenSent { get; private set; }
        public List<String> Subjects { get; }
        public List<String> Messages { get; }
        public override void send(string address, string subject, string message)
        {
            Subjects.Add(subject);
            Messages.Add(message);
            HasBeenSent = true;
        }

    }
}