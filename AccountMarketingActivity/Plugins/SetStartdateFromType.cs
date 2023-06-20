using BIT.AccountMarketingActivity.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Services;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BIT.AccountMarketingActivity.Plugins
{
    public class SetStartdateFromType : BasePlugin
    {
        public SetStartdateFromType()
        {
            StepManager
                .NewStep()
                    .EntityName("bit_accountmarketingactivity")
                    .Message("Create")
                    .Stage(PluginStage.PreOperation)
                    .PluginAction(ExecuteAction)
                .Register();
            StepManager
                .NewStep()
                    .EntityName("bit_accountmarketingactivity")
                    .Message("Update")
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
                new AccountMarketingActivityService(context.InitUserOrganizationService).SetStartdateFromType(target);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($"An error occurred in AccountMarketingActivityService.SetStartdateFromType.\n{ex.Message}", ex);
            }
        }
    }
}