using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingSamples.Plugins.Services
{
    public class NotificationHelperService
    {
        public ILocalPluginContext Context { get; }

        public NotificationHelperService(ILocalPluginContext context)
        {
            Context = context;
        }

        public virtual void SendAccountStatusChangeNotification(Guid accountId)
        {
            var account = Context.InitiatingUserService.Retrieve("account", accountId, new ColumnSet("name", "statuscode"));
            var contacts = RetrieveContactsToNotify(accountId);
            NotifyContacts(account, contacts);
        }

        public virtual void SendWelcomeEmail(Guid contactId)
        {
            var contact = Context.InitiatingUserService.Retrieve("contact", contactId, new ColumnSet("fullname"));
            SendWelcomeEmailNotification(contact);
        }

        protected virtual void NotifyContacts(Entity account, DataCollection<Entity> contacts)
        {
            foreach (var contact in contacts)
            {
                SendAccountStatusChangeEmailNotification(account, contact);
            }
        }

        protected virtual void SendWelcomeEmailNotification(Entity contact)
        {
            var email = new Entity("email")
            {
                ["subject"] = "Welcome",
                ["description"] = $"Welcome to our organization, {contact.GetAttributeValue<string>("fullname")}.",
                ["to"] = new EntityCollection(new[]
                {
                    new Entity("activityparty")
                    {
                        ["partyid"] = contact.ToEntityReference()
                    }
                })
            };
            Context.InitiatingUserService.Create(email);
        }

        protected virtual void SendAccountStatusChangeEmailNotification(Entity account, Entity contact)
        {
            var email = new Entity("email")
            {
                ["subject"] = "Account Status Change",
                ["description"] = $"The status of the account '{account.GetAttributeValue<string>("name")}' has changed.",
                ["to"] = new EntityCollection(new[]
                {
                    new Entity("activityparty")
                    {
                        ["partyid"] = contact.ToEntityReference()
                    }
                })
            };
            Context.InitiatingUserService.Create(email);
        }

        protected DataCollection<Entity> RetrieveContactsToNotify(Guid accountId)
        {
            var query = new QueryExpression("contact")
            {
                ColumnSet = new ColumnSet("contactid"),
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression("parentcustomerid", ConditionOperator.Equal, accountId)
                    }
                }
            };

            var response = Context.InitiatingUserService.RetrieveMultiple(query);
            Context.Trace($"Found {response.Entities.Count} contacts to notify.");
            return response.Entities;
        }
    }
}
