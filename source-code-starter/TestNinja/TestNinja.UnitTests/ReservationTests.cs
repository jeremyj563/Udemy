using NUnit.Framework;
using TestNinja.Fundamentals;

namespace TestNinja.UnitTests
{
    [TestFixture]
    public class ReservationTests
    {
        [Test]
        public void CanBeCancelledBy_AdminCancelling_ReturnTrue()
        {
            var reservation = new Reservation();
            var result = reservation.CanBeCancelledBy(new User { IsAdmin = true });
            Assert.That(result, Is.True);
        }

        [Test]
        public void CanBeCancelledBy_SameUserCancelling_ReturnTrue()
        {
            var user = new User();
            var reservation = new Reservation { MadeBy = user };
            var result = reservation.CanBeCancelledBy(user);
            Assert.IsTrue(result);
        }

        [Test]
        public void CanBeCancelledBy_AnotherUserCancelling_ReturnFalse()
        {
            var user = new User();
            var reservation = new Reservation();
            var anotherUser = new User();
            var result = reservation.CanBeCancelledBy(anotherUser);
            Assert.IsFalse(result);
        }
    }
}
