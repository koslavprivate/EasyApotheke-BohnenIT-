using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIT.MarketingActivityType.Services
{
    internal class MarketingActivityTypeService
    {
        private readonly IOrganizationService service;

        public MarketingActivityTypeService(IOrganizationService service)
        {
            this.service = service;
        }

        internal void SetStartdate(Entity target)
        {
            var createdOn = target.GetAttributeValue<DateTime>("createdon");
            int year = createdOn.Month < 10 ? createdOn.Year : createdOn.Year + 1;
            target["bit_startdate"] = new DateTime(year, 1, 1);
        }
    }
}