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
    public class AccountStatusNotificationPluginTests : FakeXrmEasyTestsBase
    {
        [Fact]
        public void ShouldThrowExceptionIfTargetNotSpecified()
        {
            // Arrange
            var context = _context.GetDefaultPluginContext();

            // Act
            Func<IPlugin> action = () => _context.ExecutePluginWith<AccountStatusNotificationPlugin>(context);

            // Assert
            action.Should()
                .ThrowExactly<InvalidPluginExecutionException>()
                .WithMessage("AccountStatusNotificationPlugin is not registered on the 'Target' of the execution context.");
        }

        [Fact]
        public void ShouldThrowExceptionIfPreImageNotAvailable()
        {
            // Arrange
            var context = _context.GetDefaultPluginContext();
            context.InputParameters["Target"] = new Entity("account");

            // Act
            Func<IPlugin> action = () => _context.ExecutePluginWith<AccountStatusNotificationPlugin>(context);

            // Assert
            action.Should()
                .ThrowExactly<InvalidPluginExecutionException>()
                .WithMessage("PreImage is not available.");
        }

        [Fact]
        public void ShouldNotSendNotificationIfStatusNotChanged()
        {
            // Arrange
            var account = new Entity("account")
            {
                ["statuscode"] = new OptionSetValue(1)
            };

            var preImage = new Entity("account")
            {
                ["statuscode"] = new OptionSetValue(1)
            };

            var context = _context.GetDefaultPluginContext();
            context.InputParameters["Target"] = account;
            context.PreEntityImages.Add("PreImage", preImage);

            var mockContext = new Mock<ILocalPluginContext>();
            var fakeNotificationHelper = new Mock<NotificationHelperService>(mockContext.Object);

            fakeNotificationHelper.Setup(x => x.SendAccountStatusChangeNotification(It.IsAny<Guid>()))
                .Verifiable(Times.Never());

            var pluginInstance = new AccountStatusNotificationPlugin
            {
                NotificationHelper = fakeNotificationHelper.Object
            };

            // Act
            _context.ExecutePluginWith(context, pluginInstance);

            // Assert
            fakeNotificationHelper.Verify();
        }

        [Fact]
        public void ShouldSendNotificationIfStatusChanged()
        {
            // Arrange
            var account = new Entity("account")
            {
                ["statuscode"] = new OptionSetValue(1)
            };

            var preImage = new Entity("account")
            {
                ["statuscode"] = new OptionSetValue(2)
            };

            var context = _context.GetDefaultPluginContext();
            context.InputParameters["Target"] = account;
            context.PreEntityImages.Add("PreImage", preImage);

            var mockContext = new Mock<ILocalPluginContext>();
            var fakeNotificationHelper = new Mock<NotificationHelperService>(mockContext.Object);

            fakeNotificationHelper.Setup(x => x.SendAccountStatusChangeNotification(It.IsAny<Guid>()))
                .Verifiable(Times.Once());

            var pluginInstance = new AccountStatusNotificationPlugin
            {
                NotificationHelper = fakeNotificationHelper.Object
            };

            // Act
            _context.ExecutePluginWith(context, pluginInstance);

            // Assert
            fakeNotificationHelper.Verify();
        }
    }
}
