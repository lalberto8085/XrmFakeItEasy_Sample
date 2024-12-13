using FakeXrmEasy.Plugins;
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
    public class ContactNameValidationPluginTests : FakeXrmEasyTestsBase
    {
        [Fact]
        public void ShouldThrowExceptionWhenFirstNameIsEmpty()
        {
            // Arrange
            var contact = new Entity("contact");
            contact["firstname"] = string.Empty;
            contact["lastname"] = "Doe";

            // Act
            Func<IPlugin> action = () => _context.ExecutePluginWithTarget<ContactNameValidationPlugin>(contact);

            // Assert
            action.Should()
                .Throw<InvalidPluginExecutionException>()
                .WithMessage("First Name must not be empty.");
        }

        [Fact]
        public void ShouldThrowExceptionWhenLastNameIsEmpty()
        {
            // Arrange
            var contact = new Entity("contact");
            contact["firstname"] = "John";
            contact["lastname"] = string.Empty;

            // Act
            Func<IPlugin> action = () => _context.ExecutePluginWithTarget<ContactNameValidationPlugin>(contact);

            // Assert
            action.Should()
                .Throw<InvalidPluginExecutionException>()
                .WithMessage("Last Name must not be empty.");
        }

        [Fact]
        public void ShouldThrowExceptionWhenEntityIsNotContact()
        {
            // Arrange
            var account = new Entity("account");

            // Act
            Func<IPlugin> action = () => _context.ExecutePluginWithTarget<ContactNameValidationPlugin>(account);

            // Assert
            action.Should()
                .Throw<InvalidPluginExecutionException>()
                .WithMessage("This plugin is only registered on the 'Contact' entity.");
        }

        [Fact]
        public void ShouldThrowExceptionWhenTargetIsNotSet()
        {
            // Arrange
            var context = _context.GetDefaultPluginContext();

            // Act
            Func<IPlugin> action = () => _context.ExecutePluginWith<ContactNameValidationPlugin>(context);

            // Assert
            action.Should()
                .Throw<InvalidPluginExecutionException>()
                .WithMessage("ContactNameValidationPlugin is not registered on the 'Target' of the execution context.");
        }

        [Fact]
        public void ShouldNotThrowExceptionWhenFirstNameAndLastNameAreNotEmpty()
        {
            // Arrange
            var contact = new Entity("contact");
            contact["firstname"] = "John";
            contact["lastname"] = "Doe";

            // Act
            Func<IPlugin> action = () => _context.ExecutePluginWithTarget<ContactNameValidationPlugin>(contact);

            // Assert
            action.Should().NotThrow();
        }
    }
}
