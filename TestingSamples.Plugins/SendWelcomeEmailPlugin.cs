using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestingSamples.Plugins.Services;

namespace TestingSamples.Plugins
{
    public class SendWelcomeEmailPlugin : PluginBase
    {
        public SendWelcomeEmailPlugin()
            : base(typeof(SendWelcomeEmailPlugin))
        {
        }

        public NotificationHelperService NotificationHelper { get; set; }

        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            //! Do not set the NotificationHelper property as it might get cached between plugin executions
            var notificationHelper = NotificationHelper ?? new NotificationHelperService(localPluginContext);

            var context = localPluginContext.PluginExecutionContext;
            if(context.MessageName == "Create" && context.Stage == 40 && context.PrimaryEntityName == "contact")
            {
                if (context.InputParameters.TryGetValue<Entity>("Target", out var target))
                {
                    var contact = (Entity)context.InputParameters["Target"];
                    notificationHelper.SendWelcomeEmail(contact.Id);
                }
                else
                {
                    throw new InvalidPluginExecutionException("The 'Target' is not available in the execution context.");
                }
            }
            else
            {
                throw new InvalidPluginExecutionException("This plugin is only registered for Post-Operation 'Create' message of the 'Contact' entity.");
            }
        }
    }
}
