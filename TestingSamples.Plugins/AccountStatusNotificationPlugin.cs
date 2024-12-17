using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestingSamples.Plugins.Services;

namespace TestingSamples.Plugins
{
    public class AccountStatusNotificationPlugin : PluginBase
    {
        public AccountStatusNotificationPlugin()
            : base(typeof(AccountStatusNotificationPlugin))
        {
        }

        public NotificationHelperService NotificationHelper { get; set; }

        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            //! Do not set the NotificationHelper property as it might get cached between plugin executions
            var notificationHelper = NotificationHelper ?? new NotificationHelperService(localPluginContext);

            if (localPluginContext.PluginExecutionContext.InputParameters.TryGetValue<Entity>("Target", out var target) && target.LogicalName == "account")
            {
                if (localPluginContext.PluginExecutionContext.PreEntityImages.TryGetValue("PreImage", out var preImage))
                {
                    if (preImage.GetAttributeValue<OptionSetValue>("statuscode").Value == target.GetAttributeValue<OptionSetValue>("statuscode").Value)
                    {
                        return;
                    }

                    notificationHelper.SendAccountStatusChangeNotification(target.Id);
                }
                else
                {
                    throw new InvalidPluginExecutionException("PreImage is not available.");
                }
            }
            else
            {
                throw new InvalidPluginExecutionException("AccountStatusNotificationPlugin is not registered on the 'Target' of the execution context.");
            }
        }
    }
}
