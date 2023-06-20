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


namespace Create_HistirycalMarketingaktivitätDerFirma

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
            using (ServiceClient serviceClient = new ServiceClient(connectionString))
            {
                if (serviceClient.IsReady)
                {
                    var queryAccount = new QueryExpression("account")
                    {
                        ColumnSet = new ColumnSet("accountid", "name",
                                      "sit_easytrinkwasser_2022", "sit_easyosterzeit_2022", "sit_easymobil_2022",
                                      "sit_easybescherung_2022", "sit_easyhautrein_2022", "sit_easysparlender_2022",
                                      "sit_easyvmspos_2022", "sit_taschenkalender_2022"), // "sit_schokokalender_2022", ),
                        Criteria = {
                            FilterOperator = LogicalOperator.And,
                            Conditions = {
                                new ConditionExpression("statecode", ConditionOperator.Equal, 0), // == active
                                new ConditionExpression("customertypecode", ConditionOperator.Equal, 907700003), // 907700003=easyApotheke 
                            },
                        }
                    };

                    var accounts = RetrieveAllRecords(queryAccount, serviceClient).Entities.ToList();
                    string[] arrAtribute = { "bit_easytrinkwasser", "bit_easyosterzeit", "bit_easymobil",
                                             "bit_easybescherung", "bit_easyhautrein", "bit_easysparlender",
                                             "bit_easyvmpos", "bit_taschenkalender"}; //,"bit_schokokalender"};

                    foreach (var account in accounts)
                    {
                        string accountName = account.GetAttributeValue<string>("name");
                        string accountNameShort = accountName.Length > 71
                            ? accountName.Substring(0, 70)
                            : accountName;

                        foreach (var atr in arrAtribute)
                        {
                            string atrName = atr.Replace("bit_", "sit_") + "_2022";
                            
                            var accountmarketingactivity = new Entity("bit_accountmarketingactivity")
                            {
                                ["bit_name"] = accountNameShort + "|" + atr.Replace("bit_", "") + "|2022",
                                //["bit_name"] = account.GetAttributeValue<string>("name").Trim() + "|" + atr.Replace("bit_", "") + "|2022",
                                ["bit_account"] = new EntityReference("account", account.Id),
                                ["bit_marketingactivitytype"] = GetBitMarketingActivityTypeByNema(atr),
                                ["bit_active"] = account.GetAttributeValue<bool>(atrName),
                                //["bit_active"] = account.GetAttributeValue<bool>(atr),
                                ["bit_startdate"] = new DateTime(2022, 01, 01),
                            };

                            //if (atrName.Contains("schokokalender"))
                            //{
                            //    OptionSetValue atrNameValueOptinSet = account.GetAttributeValue<OptionSetValue>(atrName);
                            //}
                            //else
                            //{
                            //    bool atrNameValueBool = account.GetAttributeValue<bool>(atrName);
                            //}

                            serviceClient.Create(accountmarketingactivity);

                        }
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
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        public static EntityReference GetBitMarketingActivityTypeByNema(string name)
        {
            switch (name)
            {
                case "bit_easybauchgefhl": // "easyBauchgefühl":
                    return new EntityReference("bit_marketingactivitytype", new Guid("cf498154-9df8-ed11-8849-6045bd8f9db0"));
                case "bit_easybescherung":// "easyBescherung":
                    return new EntityReference("bit_marketingactivitytype", new Guid("1defea7e-9df8-ed11-8849-6045bd8f9db0"));
                case "bit_easyeingepackt": // "easyEi(n)gepackt":
                    return new EntityReference("bit_marketingactivitytype", new Guid("c6048f5a-9df8-ed11-8849-6045bd8f9db0"));
                case "bit_easyeinseifen":// "easyEinseifen":
                    return new EntityReference("bit_marketingactivitytype", new Guid("4B2DD690-9DF8-ED11-8849-6045BD8F9DB0"));
                case "bit_easyhautrein": // "easyHautrein":
                    return new EntityReference("bit_marketingactivitytype", new Guid("DBBE1BBB-9BF8-ED11-8849-6045BD8F9DB0"));
                case "bit_easymobil": // "easyMobil":
                    return new EntityReference("bit_marketingactivitytype", new Guid("b1db1fa9-9bf8-ed11-8849-6045bd8f9db0"));
                case "bit_easyosterzeit": //"easyOsterzeit":
                                          //    return new EntityReference("bit_marketingactivitytype", new Guid("09628B96-9BF8-ED11-8849-6045BD8F9DB0"));
                    return new EntityReference("bit_marketingactivitytype", new Guid("f980d79b-2d09-ee11-8f6e-6045bd8f99e4"));
                case "bit_easysommerzeit":// "easySommerzeit":
                    return new EntityReference("bit_marketingactivitytype", new Guid("c02bfb9c-9df8-ed11-8849-6045bd8f9db0"));
                case "bit_easysparlender": // "easySparlender":
                    return new EntityReference("bit_marketingactivitytype", new Guid("b61c9390-9bf8-ed11-8849-6045bd8f9db0"));
                case "bit_easysparwochen":// "easySparwochen":
                    return new EntityReference("bit_marketingactivitytype", new Guid("442bbb66-9df8-ed11-8849-6045bd8f9db0"));
                case "bit_easytrinkwasser": // "easyTrinkwasser":
                    return new EntityReference("bit_marketingactivitytype", new Guid("3e508c4c-2f09-ee11-8f6e-6045bd8c5c4a"));
                case "bit_easyvmpos":// "easyVMS POS":
                    return new EntityReference("bit_marketingactivitytype", new Guid("49dcdd8a-9df8-ed11-8849-6045bd8f9db0"));
                case "bit_schokokalender":// "Schokokalender":
                    return new EntityReference("bit_marketingactivitytype", new Guid("4b646181-e307-ee11-8f6e-6045bd8f9db0"));
                case "bit_taschenkalender":// "Taschenkalender":
                    return new EntityReference("bit_marketingactivitytype", new Guid("ccdbd172-9df8-ed11-8849-6045bd8f9db0"));
                //return new EntityReference("bit_marketingactivitytype", new Guid("e3aad378-9df8-ed11-8849-6045bd8f9db0"));
                default:
                    return null;// code block
            }
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

