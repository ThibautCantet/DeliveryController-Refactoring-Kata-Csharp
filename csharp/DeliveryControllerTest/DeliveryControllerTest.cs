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

            Assert.Empty(((FakeEmailGateway)_emailGateway).Subjects);
        }

        [Fact]
        public void Should_not_send_an_email_when_current_delivery_is_not_scheduled()
        {
            List<Delivery> deliverySchedule = new List<Delivery>()
            {
                new Delivery("id",
                    "contact@email.com",
                    new Location(0f, 0f),
                    DateTime.Now,
                    false,
                    false)
            };
            var deliveryController = new DeliveryController.DeliveryController(deliverySchedule, _emailGateway);

            deliveryController.UpdateDelivery(new DeliveryEvent("anotherId", DateTime.Now, new Location(0, 0)));

            Assert.Empty(((FakeEmailGateway)_emailGateway).Subjects);
        }

        [Fact]
        public void Should_send_a_feedback_email_when_current_delivery_is_scheduled()
        {
            var timeOfDelivery =
                DateTime.ParseExact($"15/02/2022 13:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            List<Delivery> deliverySchedule = new List<Delivery>
            {
                new Delivery("id",
                    "contact@email.com",
                    new Location(0f, 0f),
                    timeOfDelivery,
                    false,
                    false)
            };
            var deliveryController =
                new DeliveryController.DeliveryController(deliverySchedule, _emailGateway);

            deliveryController.UpdateDelivery(new DeliveryEvent("id", timeOfDelivery, new Location(0, 0)));

            Assert.Contains("Your feedback is important to us", ((FakeEmailGateway)_emailGateway).Subjects);
            Assert.Contains(
                "Regarding your delivery today at 02/15/2022 13:00:00. How likely would you be to recommend this delivery service to a friend? Click <a href='url'>here</a>",
                ((FakeEmailGateway)_emailGateway).Messages);
        }

        [Fact]
        public void Should_update_average_speed_when_not_on_time_and_not_unique_delivery_scheduled_and_not_for_the_first_schedule_delivery()
        {
            var timeOfDelivery =
                DateTime.ParseExact($"15/02/2022 13:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            List<Delivery> deliverySchedule = new List<Delivery>
            {
                new Delivery("id",
                    "contact@email.com",
                    new Location(0f, 0f),
                    DateTime.ParseExact($"15/02/2022 12:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    false,
                    false),
                new Delivery("id2",
                    "contact2@email.com",
                    new Location(0f, 0f),
                    DateTime.ParseExact($"15/02/2022 12:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    false,
                    false)
            };
            var spyMapService = new SpyMapService();
            var deliveryController = new DeliveryController.DeliveryController(deliverySchedule, _emailGateway, spyMapService);

            deliveryController.UpdateDelivery(new DeliveryEvent("id2", timeOfDelivery, new Location(0, 0)));

            Assert.Equal(new Location(0f,0f),spyMapService.Location1);
            Assert.Equal(new Location(0f,0f),spyMapService.Location2);
            Assert.Equal(new TimeSpan(1, 0, 0),spyMapService.ElapsedTime);
        }
    }

    public class SpyMapService : MapService
    {
        public Location Location1 { get; private set; }
        public Location Location2 { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }

        public override void UpdateAverageSpeed(Location location1, Location location2, TimeSpan elapsedTime)
        {
            Location1 = location1;
            Location2 = location2;
            ElapsedTime = elapsedTime;
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