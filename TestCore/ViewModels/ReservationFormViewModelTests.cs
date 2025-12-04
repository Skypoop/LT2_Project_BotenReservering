using System;
using NUnit.Framework;
using Moq;
using FluentAssertions;

using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Services;
using ProjectBotenReservering.Core.Constants;

namespace TestCore.ViewModels
{
    public class ReservationFormViewModelTests
    {
        private ReservationService _service;

        [SetUp]
        public void Setup()
        {
            var repoMock = new Mock<IReservationRepository>();
            var authMock = new Mock<IBoatAuthorizationService>();

            _service = new ReservationService(repoMock.Object, authMock.Object);
        }

        [Test]
        public void IsBooking_Within_Allowed_ReservationTime()
        {
            //Arrange
            var allowedDays = ReservationRules.MaxDaysBeforeReservation;
            var startTime = DateTime.Today.AddDays(allowedDays - 1);

            //Act
            var result = _service.IsBookingWithinAllowedReservationTime(startTime);

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsBooking_Not_Within_Allowed_ReservationTime()
        {
            //Arrange
            int allowedDays = ReservationRules.MaxDaysBeforeReservation;
            DateTime startTime = DateTime.Today.AddDays(allowedDays + 1);

            //Act
            bool result = _service.IsBookingWithinAllowedReservationTime(startTime);

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsReservation_Within_Allowed_Length()
        {
            //Arrange
            DateTime startTime = DateTime.Today.AddDays(1);
            DateTime endTime = DateTime.Today.AddDays(1).AddMinutes(110);
            
            //Act
            bool result = _service.IsValidReservationLength(startTime, endTime);

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsReservation_Not_Within_Allowed_Length()
        {
            //Arrange
            DateTime startTime = DateTime.Today.AddDays(1);
            DateTime endTime = DateTime.Today.AddDays(1).AddMinutes(130);

            //Act
            bool result = _service.IsValidReservationLength(startTime, endTime);

            //Assert
            result.Should().BeFalse();
        }
    }
}
