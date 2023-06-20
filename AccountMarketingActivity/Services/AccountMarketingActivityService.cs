using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIT.AccountMarketingActivity.Services
{
    internal class AccountMarketingActivityService
    {

        private readonly IOrganizationService service;

        public AccountMarketingActivityService(IOrganizationService service)
        {
            this.service = service;
        }

        internal void SetStartdateFromType(Entity target)
        {
            var marketingActivityTypeRef = target.GetAttributeValue<EntityReference>("bit_marketingactivitytype");
            if (marketingActivityTypeRef == null)
                return;

            var marketingActivityType = service.Retrieve(marketingActivityTypeRef.LogicalName, marketingActivityTypeRef.Id, new ColumnSet("bit_startdate"));
            var startDate = marketingActivityType.GetAttributeValue<DateTime?>("bit_startdate");

            target["bit_startdate"] = startDate;
        }

        internal void CalculateTotalActivityOnAccount(Entity target, Entity preImage)
        {

            Guid? firmaID = target.Attributes.Contains("bit_account")
                ? target.GetAttributeValue<EntityReference>("bit_account").Id
                : preImage.GetAttributeValue<EntityReference>("bit_account")?.Id;


            if (firmaID == null) return;

            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='bit_accountmarketingactivity'>
                                <attribute name='bit_accountmarketingactivityid' />
                                <attribute name='bit_name' />
                                <attribute name='bit_account' />
                                <attribute name='bit_marketingactivitytype' />
                                <attribute name='bit_active' />
                                <order attribute='bit_name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='bit_active' operator='eq' value='1' />
                                  <condition attribute='bit_startdate' operator='this-year' />
                                  <condition attribute='bit_account' operator='eq' uitype='account' value='" + firmaID + @"' />
                                </filter>
                                <link-entity name='bit_marketingactivitytype' from='bit_marketingactivitytypeid'
                                  to='bit_marketingactivitytype' link-type='inner'>
                                  <filter type='and'>
                                    <condition attribute='bit_iscountable' operator='eq' value='1' />
                                  </filter>
                                </link-entity>
                              </entity>
                            </fetch>";


            EntityCollection marketingActivityTypeRef = service.RetrieveMultiple(new FetchExpression(fetchXml));

            int anzahl = marketingActivityTypeRef.Entities.Count;
            

            Entity firma = new Entity("account", (Guid)firmaID);
            firma["bit_nationale_aktionen_anzahl"] = anzahl;
            service.Update(firma);
        }


       
    }
}