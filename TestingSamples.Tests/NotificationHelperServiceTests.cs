using FluentAssertions;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using TestingSamples.Plugins.Services;
using Xunit;

namespace TestingSamples.Tests
{
    public class NotificationHelperServiceTests : FakeXrmEasyTestsBase
    {
        private NotificationHelperService _sut;

        private readonly Guid _account1_Id = Guid.NewGuid();
        private readonly Guid _account2_Id = Guid.NewGuid();

        public NotificationHelperServiceTests()
        {
            var account1 = new Entity("account", _account1_Id)
            {
                ["name"] = "Account 1",
                ["statuscode"] = new OptionSetValue(1)
            };
            var account2 = new Entity("account", _account2_Id)
            {
                ["name"] = "Account 2",
                ["statuscode"] = new OptionSetValue(1)
            };

            var contact1_1 = new Entity("contact", Guid.NewGuid())
            {
                ["firstname"] = "John",
                ["lastname"] = "Doe",
                ["parentcustomerid"] = account1.ToEntityReference(),
                ["statuscode"] = new OptionSetValue(1)
            };
            var contact1_2 = new Entity("contact", Guid.NewGuid())
            {
                ["firstname"] = "Jane",
                ["lastname"] = "Doe",
                ["parentcustomerid"] = account1.ToEntityReference(),
                ["statuscode"] = new OptionSetValue(1)
            };
            var contact2_1 = new Entity("contact", Guid.NewGuid())
            {
                ["firstname"] = "Peter",
                ["lastname"] = "Parker",
                ["parentcustomerid"] = account2.ToEntityReference(),
                ["statuscode"] = new OptionSetValue(1)
            };

            var dataSeed = new List<Entity>
            {
                account1, account2, contact1_1, contact1_2, contact2_1
            };

            _context.Initialize(dataSeed);
        }

        [Fact]
        public void ShouldCreateEmailsToContactsUnderTheAccount()
        {
            // Arrange
            _sut = new NotificationHelperService(BuildLocalPluginContext());

            // Act
            _sut.SendAccountStatusChangeNotification(_account1_Id);

            // Assert
             _context.CreateQuery("email").Should().HaveCount(2);
        }

        [Fact]
        public void ShouldThrowIfAccountNotFound()
        {
            // Arrange
            _sut = new NotificationHelperService(BuildLocalPluginContext());

            // Act
            Action action = () => _sut.SendAccountStatusChangeNotification(Guid.NewGuid());

            // Assert
            action.Should()
                .Throw<FaultException>()
                .WithMessage("account With Id = * Does Not Exist");
        }

        [Fact]
        public void ShouldCreateWelcomeEmailToContact()
        {
            // Arrange
            _sut = new NotificationHelperService(BuildLocalPluginContext());

            // Act
            var contactId = _context.CreateQuery("contact").First().Id;
            _sut.SendWelcomeEmail(contactId);

            // Assert
            _context.CreateQuery("email").Should().HaveCount(1);
        }

        [Fact]
        public void ShouldThrowIfContactNotFound()
        {
            // Arrange
            _sut = new NotificationHelperService(BuildLocalPluginContext());

            // Act
            Action action = () => _sut.SendWelcomeEmail(Guid.NewGuid());

            // Assert
            action.Should()
                .Throw<FaultException>()
                .WithMessage("contact With Id = * Does Not Exist");
        }
    }
}
