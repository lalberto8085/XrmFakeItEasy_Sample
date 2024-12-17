using FakeXrmEasy.Plugins;
using FluentAssertions;
using Microsoft.Xrm.Sdk;
using System;
using TestingSamples.Plugins;
using TestingSamples.Tests.Utils;
using Xunit;

namespace TestingSamples.Tests
{
    public class ContactEmailSetterTests : FakeXrmEasyTestsBase
    {
        [Fact]
        public void ShouldThrowIfNotOnCreate()
        {
            // Arrange
            var context = GetContext(action: "Update");

            // Act
            Action action = () => _context.ExecutePluginWith<ContactEmailSetterPlugin>(context);

            // Assert
            action.Should()
                .Throw<InvalidPluginExecutionException>()
                .WithMessage("This plugin is only registered for Pre-Operation 'Create' message of the 'Contact' entity.");
        }

        [Fact]
        public void ShouldThrowIfNotOnPreOperationStage()
        {
            // Arrange
            var context = GetContext(stage: 10);

            // Act
            Action action = () => _context.ExecutePluginWith<ContactEmailSetterPlugin>(context);

            // Assert
            action.Should()
                .Throw<InvalidPluginExecutionException>()
                .WithMessage("This plugin is only registered for Pre-Operation 'Create' message of the 'Contact' entity.");
        }

        [Fact]
        public void ShouldThrowIfNotOnContactEntity()
        {
            // Arrange
            var context = GetContext(entityName: "account");

            // Act
            Action action = () => _context.ExecutePluginWith<ContactEmailSetterPlugin>(context);

            // Assert
            action.Should()
                .Throw<InvalidPluginExecutionException>()
                .WithMessage("This plugin is only registered for Pre-Operation 'Create' message of the 'Contact' entity.");
        }

        [Fact]
        public void ShouldSetDefaultEmail()
        {
            // Arrange
            var context = GetContext(entityName: "account");
            var contact = new Entity("contact", Guid.NewGuid());
            context.InputParameters["Target"] = contact;

            // Act
            _context.ExecutePluginWith<ContactEmailSetterPlugin>(context);

            // Assert
            contact.Should().HaveAttributeValue("emailaddress1", "default@example.com");
        }

        private XrmFakedPluginExecutionContext GetContext(string action = "Create", int stage = 20, string entityName = "contact")
        {
            var context = _context.GetDefaultPluginContext();
            context.MessageName = action;
            context.Stage = stage;
            context.PrimaryEntityName = entityName;

            return context;
        }
    }
}
