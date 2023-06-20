using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyFirmaConsApp
{
    public class Program
    {

        // TODO Enter your Dataverse environment's URL and logon info.
        //static string url = "https://eacrm-mig.crm4.dynamics.com";
        static string url = "https://easyuat.crm4.dynamics.com";
        static string userName = "login-email";
        static string password = "password";

        // This service connection string uses the info provided above.
        // The AppId and RedirectUri are provided for sample code testing.
        static string connectionString = $@"
        AuthType = OAuth;
        Url = {url};
        UserName = {userName};
        Password = {password};
        AppId = 51f81489-12ee-4a9e-aaae-a2591f45987d;
        RedirectUri = app://58145B91-0C36-4500-8554-080854F2AC97;
        LoginPrompt=Auto;
        RequireNewInstance = True";

        static void Main(string[] args)
        {
            using (ServiceClient serviceClient = new ServiceClient(connectionString))
            {
                if (serviceClient.IsReady)
                {
                    var qeContractService = new QueryExpression("account")
                    {
                        ColumnSet = new ColumnSet("accountid", "sit_easybescherung_2022", "sit_easyhautrein_2022", "sit_easymobil_2022", "sit_easyosterzeit_2022", "sit_easysparlender_2022", "sit_easytrinkwasser_2022", "sit_easyvmspos_2022", "sit_easyhautrein_2022", "sit_nationale_aktionen_anzahl", "sit_schokokalender_2022", "sit_taschenkalender_2022"),
                        Criteria = {
                            FilterOperator = LogicalOperator.And,
                            Conditions = {
                                new ConditionExpression("statecode", ConditionOperator.Equal, 0), // == active
                            },
                        }
                    };

                    var accounts = RetrieveAllRecords(qeContractService, serviceClient).Entities.ToList();

                    foreach(var account in accounts)
                    {
                        var marketingAktivitaten = new Entity("bit_marketingactivities")
                        {
                            ["bit_firma"] = new EntityReference("account", account.Id),
                            ["bit_easybescherung"] = account.GetAttributeValue<bool>("sit_easybescherung_2022"),
                            ["bit_easymobil"] = account.GetAttributeValue<bool>("sit_easymobil_2022"),
                            ["bit_easyosterzeit"] = account.GetAttributeValue<bool>("sit_easyosterzeit_2022"),
                            ["bit_easysparlender"] = account.GetAttributeValue<bool>("sit_easysparlender_2022"),
                            ["bit_easytrinkwasser"] = account.GetAttributeValue<bool>("sit_easytrinkwasser_2022"),
                            ["bit_easyvmpos"] = account.GetAttributeValue<bool>("sit_easyvmspos_2022"),
                            ["bit_easyhautrein"] = account.GetAttributeValue<bool>("sit_easyhautrein_2022"),
                            ["bit_nationale_aktionen_anzahl"] = account.GetAttributeValue<int>("sit_nationale_aktionen_anzahl"),
                            ["bit_schokokalender"] = account.GetAttributeValue<OptionSetValue>("sit_schokokalender_2022"),
                            ["bit_taschenkalender"] = account.GetAttributeValue<bool>("sit_taschenkalender_2022"),
                            ["bit_startdate"] = new DateTime(2022, 01, 01),
                        };
                        serviceClient.Create(marketingAktivitaten);
                    }

                    //WhoAmIResponse response =
                    //    (WhoAmIResponse)serviceClient.Execute(new WhoAmIRequest());

                    //Console.WriteLine("User ID is {0}.", response.UserId);
                }
                else
                {
                    Console.WriteLine(
                        "A web service connection was not established.");
                }
            }

            // Pause the console so it does not close.
            Console.WriteLine("=============== END =============== Press any key to exit.");
            Console.ReadLine();


            //CrmServiceClient _crmService = new CrmServiceClient(ConfigurationManager.ConnectionStrings["CRM"].ConnectionString);

            ////var qeContractService = new QueryExpression("account")
            ////{
            ////    ColumnSet = new ColumnSet("accountid", "bit_easybescherung", "bit_easyhautrein", "bit_easymobil", "bit_easyosterzeit", "bit_easysparlender", "bit_easytrinkwasser", "bit_easyvmpos", "bit_hasinsurance", "bit_nationale_aktionen_anzahl", "bit_schokokalender", "bit_taschenkalender"),
            ////    Criteria = {
            ////        FilterOperator = LogicalOperator.And,
            ////        Conditions = {
            ////            new ConditionExpression("statecode", ConditionOperator.Equal, 0), // == active
            ////        },
            ////    }
            ////};

            ////var accounts = RetrieveAllRecords(qeContractService, _crmService).Entities.ToList();

            ////Console.WriteLine(accounts.Count);
            //Console.WriteLine("========= END =========");
            //Console.ReadLine();



        }




        public static EntityCollection RetrieveAllRecords(QueryExpression query, IOrganizationService _service, int count = 5000)
        {
            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = count;
            query.PageInfo.PageNumber = 1;
            query.PageInfo.ReturnTotalRecordCount = true;
            EntityCollection entityCollection = _service.RetrieveMultiple(query);
            EntityCollection ecFinal = new EntityCollection();
            foreach (Entity i in entityCollection.Entities)
            {
                ecFinal.Entities.Add(i);
            }
            do
            {
                query.PageInfo.PageNumber += 1;
                query.PageInfo.PagingCookie = entityCollection.PagingCookie;
                entityCollection = _service.RetrieveMultiple(query);
                foreach (Entity i in entityCollection.Entities)
                    ecFinal.Entities.Add(i);
            }
            while (entityCollection.MoreRecords);
            return ecFinal;
        }

    }
}
