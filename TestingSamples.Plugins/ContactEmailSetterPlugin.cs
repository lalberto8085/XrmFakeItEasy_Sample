using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingSamples.Plugins
{
    public class ContactEmailSetterPlugin : PluginBase
    {
        public ContactEmailSetterPlugin()
            : base(typeof(ContactEmailSetterPlugin))
        {
        }

        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            if (context.MessageName == "Create" && context.Stage == 20 && context.PrimaryEntityName == "contact")
            {
                var contact = (Entity)context.InputParameters["Target"];
                if(contact.Contains("emailaddress1"))
                {
                    localPluginContext.Trace("Email address is already set.");
                    return;
                }

                contact["emailaddress1"] = "default@example.com";
            }
            else
            {
                throw new InvalidPluginExecutionException("This plugin is only registered for Pre-Operation 'Create' message of the 'Contact' entity.");
            }
        }
    }
}
