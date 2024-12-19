using FakeXrmEasy.Abstractions.Plugins.Enums;
using FakeXrmEasy.Pipeline;
using FakeXrmEasy.Plugins;
using FakeXrmEasy.Plugins.PluginSteps;
using FluentAssertions;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestingSamples.Plugins;
using Xunit;

namespace TestingSamples.Tests
{
    public class ContactWelcomeEmailIntegrationTests : FakeXrmEasyTestsBase
    {
        public ContactWelcomeEmailIntegrationTests()
        {
            _context.RegisterPluginStep<ContactEmailSetterPlugin>(new PluginStepDefinition
            {
                MessageName = "Create",
                Stage = ProcessingStepStage.Preoperation,
                EntityLogicalName = "contact",
            });
            _context.RegisterPluginStep<SendWelcomeEmailPlugin>(new PluginStepDefinition
            {
                MessageName = "Create",
                Stage = ProcessingStepStage.Postoperation,
                EntityLogicalName = "contact",
            });
        }

        [Fact]
        public void ShouldSendWelcomeEmailForNewContacts()
        {
            // Arrange
            var contact = new Entity("contact", Guid.NewGuid())
            {
                ["firstname"] = "John",
                ["lastname"] = "Doe",
                ["emailaddress1"] = "email1@example.com"
            };

            // Act
            _service.Create(contact);

            // Assert
            _context.CreateQuery("email").Should().HaveCount(1);
        }

        [Fact]
        public void ShouldNotSendWelcomeEmailForContactWithNoEmail()
        {
            // Arrange
            var contact = new Entity("contact", Guid.NewGuid())
            {
                ["firstname"] = "John",
                ["lastname"] = "Doe",
            };

            // Act
            _service.Create(contact);

            // Assert
            _context.CreateQuery("email").Should().HaveCount(0);
        }
    }
}
