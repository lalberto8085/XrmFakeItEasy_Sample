using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingSamples.Plugins
{
    public class AccountStatusCascadeToContactsPlugin : PluginBase
    {
        public AccountStatusCascadeToContactsPlugin()
            : base(typeof(AccountStatusCascadeToContactsPlugin))
        {
        }

        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            if (localPluginContext.PluginExecutionContext.InputParameters.TryGetValue<Entity>("Target", out var target) && target.LogicalName == "account")
            {
                if (localPluginContext.PluginExecutionContext.PreEntityImages.TryGetValue("PreImage", out var preImage))
                {
                    if(preImage.GetAttributeValue<OptionSetValue>("statuscode").Value == target.GetAttributeValue<OptionSetValue>("statuscode").Value)
                    {
                        return;
                    }

                    var service = localPluginContext.InitiatingUserService;
                    var contacts = GetContactsUnderAccount(localPluginContext, target.Id);

                    foreach (var contact in contacts)
                    {
                        contact["statuscode"] = target.GetAttributeValue<OptionSetValue>("statuscode");

                        localPluginContext.Trace($"Updating contact [{contact.Id}]");
                        service.Update(contact);
                    }
                }
                else
                {
                    throw new InvalidPluginExecutionException("PreImage is not available.");
                }
            }
            else
            {
                throw new InvalidPluginExecutionException("AccountStatusCascadeToContactsPlugin is not registered on the 'Target' of the execution context.");
            }
        }

        private DataCollection<Entity> GetContactsUnderAccount(ILocalPluginContext context, Guid accountId)
        {
            var query = new QueryExpression("contact")
            {
                ColumnSet = new ColumnSet("contactid"),
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression("parentcustomerid", ConditionOperator.Equal, accountId),
                        new ConditionExpression("statecode", ConditionOperator.Equal, 0)
                    }
                }
            };

            var result = context.InitiatingUserService.RetrieveMultiple(query);
            context.Trace($"Retrieved {result.Entities.Count} contacts");

            return result.Entities;
        }
    }
}
