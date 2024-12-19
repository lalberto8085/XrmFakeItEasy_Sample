using FakeXrmEasy.Plugins;
using FluentAssertions;
using Microsoft.Xrm.Sdk;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestingSamples.Plugins;
using TestingSamples.Plugins.Services;
using Xunit;

namespace TestingSamples.Tests
{
    public class SendWelcomeEmailMessagePluginTests : FakeXrmEasyTestsBase
    {
        [Fact]
        public void ShouldThrowIfNotOnCreate()
        {
            // Arrange
            var context = GetContext(action: "Update");

            // Act
            Action action = () => _context.ExecutePluginWith<SendWelcomeEmailPlugin>(context);

            // Assert

            action.Should()
                .Throw<InvalidPluginExecutionException>()
                .WithMessage("This plugin is only registered for Post-Operation 'Create' message of the 'Contact' entity.");
        }

        [Fact]
        public void ShouldThrowIfNotOnPostOperationStage()
        {
            // Arrange
            var context = GetContext(stage: 10);

            // Act
            Action action = () => _context.ExecutePluginWith<SendWelcomeEmailPlugin>(context);

            // Assert

            action.Should()
                .Throw<InvalidPluginExecutionException>()
                .WithMessage("This plugin is only registered for Post-Operation 'Create' message of the 'Contact' entity.");
        }

        [Fact]
        public void ShouldThrowIfNotOnContact()
        {
            // Arrange
            var context = GetContext(entityName: "account");

            // Act
            Action action = () => _context.ExecutePluginWith<SendWelcomeEmailPlugin>(context);

            // Assert

            action.Should()
                .Throw<InvalidPluginExecutionException>()
                .WithMessage("This plugin is only registered for Post-Operation 'Create' message of the 'Contact' entity.");
        }

        [Fact]
        public void ShouldThrowIfNoTarget()
        {
            // Arrange
            var context = GetContext();

            // Act
            Action action = () => _context.ExecutePluginWith<SendWelcomeEmailPlugin>(context);

            // Assert
            action.Should()
                .Throw<InvalidPluginExecutionException>()
                .WithMessage("The 'Target' is not available in the execution context.");
        }

        [Fact]
        public void ShouldSendWelcomeEmail()
        {
            // Arrange
            var context = GetContext();
            context.InputParameters["Target"] = new Entity("contact", Guid.NewGuid())
            {
                ["firstname"] = "John",
                ["lastname"] = "Doe",
                ["emailaddress1"] = "some@email.com",
            };

            var mockContext = new Mock<ILocalPluginContext>();
            var fakeNotificationHelper = new Mock<NotificationHelperService>(mockContext.Object);

            fakeNotificationHelper.Setup(service => service.SendWelcomeEmail(It.IsAny<Guid>()))
                .Verifiable(Times.Once());

            var pluginInstance = new SendWelcomeEmailPlugin
            {
                NotificationHelper = fakeNotificationHelper.Object
            };

            // Act
            _context.ExecutePluginWith(context, pluginInstance);

            // Assert
            fakeNotificationHelper.Verify();
        }

        private XrmFakedPluginExecutionContext GetContext(string action = "Create", int stage = 40, string entityName = "contact")
        {
            var context = _context.GetDefaultPluginContext();
            context.MessageName = action;
            context.Stage = stage;
            context.PrimaryEntityName = entityName;

            return context;
        }
    }
}
