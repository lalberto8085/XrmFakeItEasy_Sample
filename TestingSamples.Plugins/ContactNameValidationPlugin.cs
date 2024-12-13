using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingSamples.Plugins
{
    public class ContactNameValidationPlugin : PluginBase
    {
        public ContactNameValidationPlugin() : base(typeof(ContactNameValidationPlugin))
        {
        }

        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            // simple validation
            if (localPluginContext.PluginExecutionContext.InputParameters.Contains("Target") &&
                localPluginContext.PluginExecutionContext.InputParameters["Target"] is Entity entity)
            {
                if (entity.LogicalName == "contact")
                {
                    if (!entity.TryGetAttributeValue<string>("firstname", out var firstName) || string.IsNullOrWhiteSpace(firstName))
                    {
                        throw new InvalidPluginExecutionException("First Name must not be empty.");
                    }

                    if (!entity.TryGetAttributeValue<string>("lastname", out var lastname) || string.IsNullOrWhiteSpace(lastname))
                    {
                        throw new InvalidPluginExecutionException("Last Name must not be empty.");
                    }
                }
                else
                {
                    throw new InvalidPluginExecutionException("This plugin is only registered on the 'Contact' entity.");
                }
            }
            else
            {
                throw new InvalidPluginExecutionException("ContactNameValidationPlugin is not registered on the 'Target' of the execution context.");
            }
        }
    }
}
