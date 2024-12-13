using FakeXrmEasy.Plugins;
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
            var ex = Assert.Throws<InvalidPluginExecutionException>(() => _context.ExecutePluginWithTarget<ContactNameValidationPlugin>(contact));

            // Assert
            Assert.Equal("First Name must not be empty.", ex.Message);
        }

        [Fact]
        public void ShouldThrowExceptionWhenLastNameIsEmpty()
        {
            // Arrange
            var contact = new Entity("contact");
            contact["firstname"] = "John";
            contact["lastname"] = string.Empty;

            // Act
            var ex = Assert.Throws<InvalidPluginExecutionException>(() => _context.ExecutePluginWithTarget<ContactNameValidationPlugin>(contact));

            // Assert
            Assert.Equal("Last Name must not be empty.", ex.Message);
        }

        [Fact]
        public void ShouldThrowExceptionWhenEntityIsNotContact()
        {
            // Arrange
            var account = new Entity("account");

            // Act
            var ex = Assert.Throws<InvalidPluginExecutionException>(() => _context.ExecutePluginWithTarget<ContactNameValidationPlugin>(account));

            // Assert
            Assert.Equal("This plugin is only registered on the 'Contact' entity.", ex.Message);
        }

        [Fact]
        public void ShouldThrowExceptionWhenTargetIsNotSet()
        {
            // Arrange
            var context = new XrmFakedPluginExecutionContext()
            {
                InputParameters = new ParameterCollection()
            };

            // Act
            var ex = Assert.Throws<InvalidPluginExecutionException>(() => _context.ExecutePluginWith<ContactNameValidationPlugin>(context));

            // Assert
            Assert.Equal("ContactNameValidationPlugin is not registered on the 'Target' of the execution context.", ex.Message);
        }

        [Fact]
        public void ShouldNotThrowExceptionWhenFirstNameAndLastNameAreNotEmpty()
        {
            // Arrange
            var contact = new Entity("contact");
            contact["firstname"] = "John";
            contact["lastname"] = "Doe";

            // Act
            _context.ExecutePluginWithTarget<ContactNameValidationPlugin>(contact);

            // Assert
            // No exception is thrown
        }
    }
}
