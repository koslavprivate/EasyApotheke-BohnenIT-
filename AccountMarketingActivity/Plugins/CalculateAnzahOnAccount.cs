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
    public class CalculateAnzahOnAccount : BasePlugin
    {
        public CalculateAnzahOnAccount()
        {
            StepManager
                .NewStep()
                    .EntityName("bit_accountmarketingactivity")
                    .Message("Create")
                    .Stage(PluginStage.PostOperation)
                    .PluginAction(ExecuteAction)
                .Register();
            StepManager
                .NewStep()
                    .EntityName("bit_accountmarketingactivity")
                    .Message("Update")
                    .RequiredPreImages("PreImage")
                    .Stage(PluginStage.PostOperation)
                    .PluginAction(ExecuteAction)
                .Register();
        }

        protected void ExecuteAction(LocalPluginContext context)
        {
            if (!context.PluginExecutionContext.InputParameters.Contains("Target") || !(context.PluginExecutionContext.InputParameters["Target"] is Entity))
                return;
            var target = (Entity)context.PluginExecutionContext.InputParameters["Target"];
            var preImage = context.PluginExecutionContext.PreEntityImages.Contains("PreImage")
                ? context.PluginExecutionContext.PreEntityImages["PreImage"]
                : new Entity();
            try
            {
                new AccountMarketingActivityService(context.InitUserOrganizationService).CalculateTotalActivityOnAccount(target, preImage);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($"An error occurred in AccountMarketingActivity.CalculateAnzahOnAccount.\n{ex.Message}", ex);
            }
        }
    }
}