using BIT.MarketingActivityType.Services;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIT.MarketingActivityType.Plugins
{
    public class SetStartdate : BasePlugin
    {
        public SetStartdate()
        {
            StepManager
                .NewStep()
                    .EntityName("bit_marketingactivitytype")
                    .Message("Create")
                    .Stage(PluginStage.PreOperation)
                    .PluginAction(ExecuteAction)
                .Register();
        }
        protected void ExecuteAction(LocalPluginContext context)
        {
            if (!context.PluginExecutionContext.InputParameters.Contains("Target") || !(context.PluginExecutionContext.InputParameters["Target"] is Entity))
                return;
            var target = (Entity)context.PluginExecutionContext.InputParameters["Target"];
            try
            {
                new MarketingActivityTypeService(context.InitUserOrganizationService).SetStartdate(target);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($"An error occurred in MarketingActivityTypeService.SetStartdate.\n{ex.Message}", ex);
            }
        }
    }
}