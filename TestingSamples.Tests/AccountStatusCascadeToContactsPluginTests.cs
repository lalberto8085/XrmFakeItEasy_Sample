using FakeXrmEasy.Plugins;
using FluentAssertions;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using TestingSamples.Plugins;
using Xunit;

namespace TestingSamples.Tests
{
    public class AccountStatusCascadeToContactsPluginTests : FakeXrmEasyTestsBase
    {
        private readonly Guid _account1_Id = Guid.NewGuid();
        private readonly Guid _account2_Id = Guid.NewGuid();

        public AccountStatusCascadeToContactsPluginTests()
        {
            var account1 = new Entity("account", _account1_Id)
            {
                ["statuscode"] = new OptionSetValue(1)
            };
            var account2 = new Entity("account", _account2_Id)
            {
                ["statuscode"] = new OptionSetValue(1)
            };

            var contact1_1 = new Entity("contact", Guid.NewGuid())
            {
                ["parentcustomerid"] = account1.ToEntityReference(),
                ["statuscode"] = new OptionSetValue(1)
            };
            var contact1_2 = new Entity("contact", Guid.NewGuid())
            {
                ["parentcustomerid"] = account1.ToEntityReference(),
                ["statuscode"] = new OptionSetValue(1)
            };
            var contact2_1 = new Entity("contact", Guid.NewGuid())
            {
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
        public void ShouldThrowExceptionIfTargetNotSpecified()
        {
            // Arrange
            var context = _context.GetDefaultPluginContext();

            // Act
            Func<IPlugin> action = () => _context.ExecutePluginWith<AccountStatusCascadeToContactsPlugin>(context);

            // Assert
            action.Should()
                .ThrowExactly<InvalidPluginExecutionException>()
                .WithMessage("AccountStatusCascadeToContactsPlugin is not registered on the 'Target' of the execution context.");
        }

        [Fact]
        public void ShouldThrowExceptionIfTargetIsNotAccount()
        {
            // Arrange
            var target = new Entity("contact");

            // Act
            Func<IPlugin> action = () => _context.ExecutePluginWithTarget<AccountStatusCascadeToContactsPlugin>(target);

            // Assert
            action.Should()
                .ThrowExactly<InvalidPluginExecutionException>()
                .WithMessage("AccountStatusCascadeToContactsPlugin is not registered on the 'Target' of the execution context.");
        }

        [Fact]
        public void ShouldThrowExceptionWhenPreImageIsNotAvailable()
        {
            // Arrange
            var account = new Entity("account");
            account["statuscode"] = new OptionSetValue(1);

            // Act
            Func<IPlugin> action = () => _context.ExecutePluginWithTarget<AccountStatusCascadeToContactsPlugin>(account);

            // Assert
            action.Should()
                .ThrowExactly<InvalidPluginExecutionException>()
                .WithMessage("PreImage is not available.");
        }

        [Fact]
        public void ShouldNotUpdateContactsWhenStatusNotChanged()
        {
            // Arrange
            var account = new Entity("account", _account1_Id)
            {
                ["statuscode"] = new OptionSetValue(1)
            };

            var preImage = new Entity("account", _account1_Id)
            {
                ["statuscode"] = new OptionSetValue(1)
            };

            var context = new XrmFakedPluginExecutionContext
            {
                InputParameters = new ParameterCollection
                {
                    new KeyValuePair<string, object>("Target", account)
                },
                PreEntityImages = new EntityImageCollection
                {
                    new KeyValuePair<string, Entity>("PreImage", preImage)
                },
            };

            // Act
            _context.ExecutePluginWith<AccountStatusCascadeToContactsPlugin>(context);

            // Assert
            _context.CreateQuery("contact")
                .Where(c => c.GetAttributeValue<EntityReference>("parentcustomerid").Id == _account1_Id)
                .Should()
                .AllSatisfy(c => c.GetAttributeValue<OptionSetValue>("statuscode").Value.Should().Be(1));
        }

        [Fact]
        public void ShouldUpdateContactsWhenStatusChanged()
        {
            // Arrange
            var target = new Entity("account", _account1_Id)
            {
                ["statuscode"] = new OptionSetValue(2)
            };

            var preImage = new Entity("account", _account1_Id)
            {
                ["statuscode"] = new OptionSetValue(1)
            };

            var context = new XrmFakedPluginExecutionContext
            {
                InputParameters = new ParameterCollection
                {
                    new KeyValuePair<string, object>("Target", target)
                },
                PreEntityImages = new EntityImageCollection
                {
                    new KeyValuePair<string, Entity>("PreImage", preImage)
                },
            };

            // Act
            _context.ExecutePluginWith<AccountStatusCascadeToContactsPlugin>(context);

            // Assert
            _context.CreateQuery("contact")
                .Where(c => c.GetAttributeValue<EntityReference>("parentcustomerid").Id == _account1_Id)
                .ToList()
                .Should()
                .HaveCount(2)
                .And
                .AllSatisfy(c => c.GetAttributeValue<OptionSetValue>("statuscode").Value.Should().Be(2));
        }

        [Fact]
        public void ShouldOnlyUpdateContactsUnderAccount()
        {
            // Arrange
            var target = new Entity("account", _account2_Id)
            {
                ["statuscode"] = new OptionSetValue(2)
            };

            var preImage = new Entity("account", _account2_Id)
            {
                ["statuscode"] = new OptionSetValue(1)
            };

            var context = new XrmFakedPluginExecutionContext
            {
                InputParameters = new ParameterCollection
                {
                    new KeyValuePair<string, object>("Target", target)
                },
                PreEntityImages = new EntityImageCollection
                {
                    new KeyValuePair<string, Entity>("PreImage", preImage)
                },
            };

            // Act
            _context.ExecutePluginWith<AccountStatusCascadeToContactsPlugin>(context);

            // Assert
            _context.CreateQuery("contact")
                .Where(c => c.GetAttributeValue<EntityReference>("parentcustomerid").Id == _account2_Id)
                .Should()
                .HaveCount(1)
                .And
                .AllSatisfy(c => c.GetAttributeValue<OptionSetValue>("statuscode").Value.Should().Be(2));

            _context.CreateQuery("contact")
                .Where(c => c.GetAttributeValue<EntityReference>("parentcustomerid").Id == _account1_Id)
                .Should()
                .HaveCount(2)
                .And
                .AllSatisfy(c => c.GetAttributeValue<OptionSetValue>("statuscode").Value.Should().Be(1));
        }
    }
}
