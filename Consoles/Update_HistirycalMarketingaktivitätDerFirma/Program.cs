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


namespace Update_HistirycalMarketingaktivitätDerFirma

{
    public class Program
    {

        // TODO Enter your Dataverse environment's URL and logon info.
        static string url = "https://easyuat.crm4.dynamics.com";
        //static string url = "https://eacrm-mig.crm4.dynamics.com";
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
            //var dddd = 10;
            using (ServiceClient serviceClient = new ServiceClient(connectionString))
            {
                int counttt = 1;
                if (serviceClient.IsReady)
                {
                    List<Entity> allAccountMarketingActivity2022 = RetrieveAccountMarketingActivity2022(serviceClient);
                    
                    foreach (var accMaerkActiv in allAccountMarketingActivity2022)
                    {
                        Entity relatedAccount = RetrieveAccount(serviceClient, accMaerkActiv);

                        Guid marketingactivitytypeId = accMaerkActiv.Contains("bit_marketingactivitytype")
                            ? accMaerkActiv.GetAttributeValue<EntityReference>("bit_marketingactivitytype").Id
                            : Guid.Empty;

                        if(marketingactivitytypeId != Guid.Empty)
                        {
                            Entity marketingactivitytype = serviceClient.Retrieve("bit_marketingactivitytype", marketingactivitytypeId, new ColumnSet("bit_name", "bit_startdate"));
                            
                            string marketingactivitytypeName = marketingactivitytype.Contains("bit_name")
                                ? marketingactivitytype.GetAttributeValue<String>("bit_name").ToLower().Replace(" ", "")
                                : String.Empty;

                            if (relatedAccount != null && marketingactivitytypeName != String.Empty)
                            {
                                var accountmarketingactivity = new Entity("bit_accountmarketingactivity", accMaerkActiv.Id)
                                {
                                    //["bit_name"] = accountMarketActivName,
                                    //["bit_name"] = account.GetAttributeValue<string>("name").Trim() + "|" + atr.Replace("bit_", "") + "|2022",
                                    //["bit_account"] = new EntityReference("account", account.Id),
                                    //["bit_marketingactivitytype"] = GetBitMarketingActivityTypeByNema(atr),
                                    ["bit_active"] = relatedAccount.GetAttributeValue<bool>("sit_" + marketingactivitytypeName + "_2022"),

                                    //["bit_active"] = account.GetAttributeValue<bool>(atr),
                                    //["bit_startdate"] = new DateTime(2022, 01, 01),
                                };
                                serviceClient.Update(accountmarketingactivity);
                                Console.WriteLine(counttt + "-" + marketingactivitytypeName);
                                counttt++;
                            }
                        }
                        //string marketingactivitytypeName = accMaerkActiv.GetAttributeValue<EntityReference>("bit_marketingactivitytype").Name.ToLower().Replace(" ", "");
                    }
                }
                else
                {
                    Console.WriteLine(
                        "A web service connection was not established.");
                }
            }
            Console.WriteLine("=========== END ============");
            Console.ReadLine();
        }

        public static List<Entity> RetrieveAccountMarketingActivity2022(ServiceClient serviceClient)
        {
            var queryAccountMarketingActivityName = new QueryExpression("bit_accountmarketingactivity")
            {
                ColumnSet = new ColumnSet("bit_account", "bit_marketingactivitytype", "bit_startdate"),

                Criteria = {
                            FilterOperator = LogicalOperator.And,
                            Conditions = {
                                new ConditionExpression("bit_startdate", ConditionOperator.Equal, "2022-01-01"),
                                //new ConditionExpression("customertypecode", ConditionOperator.Equal, 907700003), // 907700003=easyApotheke 
                            },
                }
            };

            List<Entity> accountMarketingActivities = serviceClient.RetrieveMultiple(queryAccountMarketingActivityName).Entities.ToList();
            return accountMarketingActivities;
        }


        public static Entity RetrieveAccount(ServiceClient serviceClient, Entity accMarkActiv)
        {
            Guid accountID = accMarkActiv.Contains("bit_account")
                ? accMarkActiv.GetAttributeValue<EntityReference>("bit_account").Id
                : Guid.Empty;

            var accountColumnSet = new ColumnSet("accountid", "name",
                                      "sit_easytrinkwasser_2022", "sit_easyosterzeit_2022", "sit_easymobil_2022",
                                      "sit_easybescherung_2022", "sit_easyhautrein_2022", "sit_easysparlender_2022",
                                      "sit_easyvmspos_2022", "sit_taschenkalender_2022"); // "sit_schokokalender_2022", ),

            if (accountID != Guid.Empty)
            {
                Entity account = serviceClient.Retrieve("account", accountID, accountColumnSet);
                return account;
            }
            return null;
        }

    }
}

