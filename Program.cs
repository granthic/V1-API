using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using VersionOne.SDK.APIClient;
using System.Xml;
using System.Configuration;

namespace V1API_ConsoleApp
{
    ///https://community.versionone.com/VersionOne_Connect/Developer_Library/Get_an_SDK/.NET_SDK/Querying_Assets
    class Program
    {


        /// <summary>
        /// Get the base url of the VersionOne application.. 
        /// </summary>
        public static string baseURL
        {
            get
            {
               return "https://www16.v1host.com/api-examples/";

            }
        }
        static string userName = "admin";
        static string pwd = "admin";
        static string domain = "v1";
        
        static string getXML(string xmlType,string i, string[] user)
        {
            string returnXML = "";
            if (xmlType == "MemberXML")
            {
                returnXML = @"<Asset href='/VersionOne_Prod/rest-1.v1/New/Member'><Attribute name='Name' act='set'>" + user[0] +
                                "</Attribute><Attribute name='Nickname' act='set'>" + user[1] +
                                "</Attribute><Attribute name='Username' act='set'>" + domain+@"\" + user[2] + // update as required. 
                                "</Attribute><Attribute name='Email' act='set'>" + user[3] +
                                "</Attribute><Attribute name='IsCollaborator' act='set'>" + "false" +
                                "</Attribute><Attribute name='NotifyViaEmail' act='set'>" + "false" +
                                "</Attribute><Attribute name='SendConversationEmails' act='set'>" + "false" +
                                "</Attribute><Relation name='DefaultRole' act='set'>" +
                                     "<Asset href='/VersionOne_Prod/rest-1.v1/Data/Role/4' idref='Role:4' /></Relation>" + //Team Member Role
                                "</Asset>";
            }
            else if (xmlType == "InactivateMemberXML")
            {
                returnXML = @"";
            }
                return returnXML;
        }
        static string getXML(string xmlType, string i)
        {
            string returnXML = "";
            if (xmlType == "MemberXML")
            {

                returnXML = @"<Asset href='/api-examples/rest-1.v1/New/Member'><Attribute name='Name' act='set'>" + "Sam Agile" +
                    "</Attribute><Attribute name='Nickname' act='set'>" + "Sam" + i.ToString() +
                    "</Attribute><Attribute name='Username' act='set'>" + "Sam.agile" + i.ToString() +
                    "</Attribute><Attribute name='Email' act='set'>" + "Sam.agile" + i.ToString() + "@mailinator.com" +
                    "</Attribute><Attribute name='IsCollaborator' act='set'>" + "false" +
                    "</Attribute><Attribute name='NotifyViaEmail' act='set'>" + "false" +
                    "</Attribute><Attribute name='SendConversationEmails' act='set'>" + "false" +
                    "</Attribute><Relation name='DefaultRole' act='set'>" +
                         "<Asset href='/api-examples/rest-1.v1/Data/Role/4' idref='Role:4' /></Relation>" + //Team Member Role
                    "</Asset>";
            }
            else if (xmlType == "InactivateMemberXML")
            {
                returnXML = @"";
            }
            return returnXML;
        }

        static void getUserCredentials()
        {
            Console.WriteLine("\renter UserName");
           userName = Console.ReadLine();
           // Console.WriteLine("enter Passworf");
           // pwd= Console.ReadLine();
        }
        static void Main(string[] args)
        {
            // byte[] data = System.Text.Encoding.ASCII.GetBytes("pmarathe");
            //  data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            // String hash = System.Text.Encoding.ASCII.GetString(data);

            bool runNewUsers = false;
            bool runInactivateUsers = false;

            foreach (string arg in args)
            {
                if (arg.ToLower() == "runNewUsers".ToLower())
                {
                    runNewUsers = true;
                }
                else if (arg.ToLower() == "runInactivateUsers".ToLower())
                {
                    runInactivateUsers = true;
                }
                Console.WriteLine(args);
            }


            Console.WriteLine(args[0]);
            getUserCredentials();

            Console.WriteLine("start");
            //string url = string.Format("{0}/name?PrimaryName={1}", System.Configuration.ConfigurationManager.AppSettings["URLREST"], "yournmae");
            // string details = CallRestMethod(Program.baseURL);


            if (runNewUsers)
            { 
                var result = ReadCsvSimple(@"InactiveUserList.csv", ',')
                .Skip(1); // Skip first Row
                foreach (var items in result)
                {
                    string InactivateMember = getXMLData(Program.baseURL, "/rest-1.v1/Data/Member?sel=Name,Username,Email&where=Email='" + items[1] + "'");
                    // Inactivate user  rest-1.v1/Data/Member/175026?op=Inactivate 
                    postXMLData(Program.baseURL, InactivateMember + "?op=Inactivate", "");

                }
            }
            if (runInactivateUsers)
            {
                var users = ReadCsvSimple(@"NewUsersList.csv", ';')
            .Skip(1);
                foreach (var user in users)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Creating User " + user[0].ToString());
                    string CreateMember = postXMLData(Program.baseURL, "/rest-1.v1/Data/Member", getXML("MemberXML", 0.ToString(), user));

                }
            }

            //Console.WriteLine(details);
            //getAllStories();
            //getMemberDetails();
            //CreateNewStory();
            //CreateNewMember();
            //getStoryDetails();

            Console.ReadLine();
        }
        #region "V1Connector API"
        #region "Authentications"
        static V1Connector BasicAuthentication()
            {
                V1Connector connector = V1Connector
                .WithInstanceUrl(Program.baseURL)
                .WithUserAgentHeader("AppName", "1.0")
                .WithUsernameAndPassword("admin", "admin")
                .Build();
            
                return connector;
           
            }
            static V1Connector windowsIntegratedAuthentication()
            {
                // windows Integrated Authentication 
               V1Connector connector = V1Connector
                   .WithInstanceUrl(Program.baseURL)   
                   .WithUserAgentHeader("AppName", "1.0")  
                   .WithWindowsIntegrated()
                   .Build();
                return connector;
            }
        #endregion
        static void getAllStories()
        {

            IServices services = new Services(BasicAuthentication());
           // IServices services = new Services(windowsIntegratedAuthentication());
            IAssetType storyType = services.Meta.GetAssetType("Story");
            Query query = new Query(storyType);

            IAttributeDefinition nameAttribute = storyType.GetAttributeDefinition("Name");
            IAttributeDefinition estimateAttribute = storyType.GetAttributeDefinition("Estimate");
            query.Selection.Add(nameAttribute);
            query.Selection.Add(estimateAttribute);
            QueryResult result = services.Retrieve(query);

            foreach (Asset story in result.Assets)
            {
                Console.WriteLine(story.Oid.Token);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(story.GetAttribute(nameAttribute).Value);
                Console.WriteLine(story.GetAttribute(estimateAttribute).Value);
                Console.WriteLine();
            }

        }
        static void getStoryDetails()
        {

        }
        static void getMemberDetails()
        {
            IServices services = new Services(BasicAuthentication());
            Oid memberId = services.GetOid("Member:1213");
            Query query = new Query(memberId);
            
            IAttributeDefinition nameAttribute = services.Meta.GetAttributeDefinition("Member.Name");
            IAttributeDefinition userNameAttribute = services.Meta.GetAttributeDefinition("Member.Username");
            IAttributeDefinition nickNameAttribute = services.Meta.GetAttributeDefinition("Member.Nickname");
            IAttributeDefinition emailAttribute = services.Meta.GetAttributeDefinition("Member.Email");
            IAttributeDefinition RoleAttribute = services.Meta.GetAttributeDefinition("Member.DefaultRole");
            IAttributeDefinition NotifyViaEmailAttribute = services.Meta.GetAttributeDefinition("Member.NotifyViaEmail");
            IAttributeDefinition SendConversationEmailsAttribute = services.Meta.GetAttributeDefinition("Member.SendConversationEmails");
            query.Selection.Add(nameAttribute);
            query.Selection.Add(userNameAttribute);
            query.Selection.Add(nickNameAttribute);
            query.Selection.Add(emailAttribute);
            query.Selection.Add(NotifyViaEmailAttribute);
            query.Selection.Add(RoleAttribute);
            query.Selection.Add(SendConversationEmailsAttribute);
            
            QueryResult result = services.Retrieve(query);
            Asset member = result.Assets[0];

            Console.WriteLine(member.Oid.Token);
            Console.WriteLine(member.GetAttribute(nameAttribute).Value);
            Console.WriteLine(member.GetAttribute(nickNameAttribute).Value);
            Console.WriteLine(member.GetAttribute(emailAttribute).Value);
            Console.WriteLine(member.GetAttribute(NotifyViaEmailAttribute).Value);
            Console.WriteLine(member.GetAttribute(RoleAttribute).Value);
            Console.WriteLine(member.GetAttribute(SendConversationEmailsAttribute).Value);



        }
        static void CreateNewStory()
        {
             IServices services = new Services(BasicAuthentication());
            Oid projectId = services.GetOid("Scope:0");
            IAssetType storyType = services.Meta.GetAssetType("Story");
            Asset newStory = services.New(storyType, projectId);
            IAttributeDefinition nameAttribute = storyType.GetAttributeDefinition("Name");
            IAttributeDefinition DescrAttribute = storyType.GetAttributeDefinition("Description");
            newStory.SetAttributeValue(nameAttribute, "Description ConsoleAPI");
            newStory.SetAttributeValue(nameAttribute, "ConsoleAPI With Description");
            services.Save(newStory);

            Console.WriteLine(newStory.Oid.Token);
            Console.WriteLine(newStory.GetAttribute(storyType.GetAttributeDefinition("Scope")).Value);
            Console.WriteLine(newStory.GetAttribute(nameAttribute).Value);
        }

        static void CreateNewMember()
        {
            try
            {
                IServices services = new Services(BasicAuthentication());
                Oid roleId = services.GetOid("Role:4");
                IAssetType memberType = services.Meta.GetAssetType("Member");
                Asset newMember = services.New(memberType, roleId);
                IAttributeDefinition nameAttribute = memberType.GetAttributeDefinition("Name");
                IAttributeDefinition nickNameAttribute = memberType.GetAttributeDefinition("Nickname");
                //IAttributeDefinition userNameAttribute = memberType.GetAttributeDefinition("Username");
                IAttributeDefinition emailAttribute = memberType.GetAttributeDefinition("Email");
                IAttributeDefinition roleAttribute = memberType.GetAttributeDefinition("DefaultRole");
               IAttributeDefinition IsCollaboratorAttribute = memberType.GetAttributeDefinition("IsCollaborator");
                IAttributeDefinition IsNotifyViaMailAttribute = memberType.GetAttributeDefinition("NotifyViaEmail ");
                IAttributeDefinition sendConversationEmailsAttribute = memberType.GetAttributeDefinition("SendConversationEmails");
                //IAttributeDefinition IsRoleAttribute = memberType.GetAttributeDefinition("Role");
                newMember.SetAttributeValue(nameAttribute, "Console Agile");
                newMember.SetAttributeValue(nickNameAttribute, "JoshC");
                //newMember.SetAttributeValue(userNameAttribute, "Josh.Agile");
                newMember.SetAttributeValue(emailAttribute, "Console.Agile@mailinglist.com");
                newMember.SetAttributeValue(roleAttribute, 4);
                newMember.SetAttributeValue(IsCollaboratorAttribute, true);
                newMember.SetAttributeValue(IsNotifyViaMailAttribute, true);
                newMember.SetAttributeValue(sendConversationEmailsAttribute, true);
                //newMember.SetAttributeValue(IsRoleAttribute, "4");
                services.Save(newMember);

                Console.WriteLine(newMember.Oid.Token);
                //Console.WriteLine(newMember.GetAttribute(memberType.GetAttributeDefinition("Role")).Value);
                Console.WriteLine(newMember.GetAttribute(nameAttribute).Value);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(ex.Message);
            }
        }

        #endregion
        #region "REST API Calls"
        public static string getXMLData(string destinationUrl, string APIUrl)
        {
            try
            {
                Console.WriteLine("in CallREST");
                destinationUrl = Program.baseURL + APIUrl;
                HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(destinationUrl);
                webrequest.Method = "GET";
                webrequest.ContentType = "application/x-www-form-urlencoded";
                webrequest.Credentials = new NetworkCredential(userName, pwd, domain);

                HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
                Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
                string result = string.Empty;
                result = responseStream.ReadToEnd();
                webresponse.Close();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                XmlNodeList elemList = doc.GetElementsByTagName("Asset");
                for (int i = 0; i < elemList.Count; i++)
                {
                    string attrVal = elemList[i].Attributes["href"].Value;
                    return attrVal;
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "error";
            }
        }

        static string postXMLData(string destinationUrl,string APIUrl, string requestXml)
        {
            destinationUrl = Program.baseURL + APIUrl;//  @"/rest-1.v1/Data/Member";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);
            request.Credentials = new NetworkCredential(userName,pwd,domain);
            //request.Credentials = new NetworkCredential("pmarathe", "2018", "Healthfirst");
            //request.Credentials = new NetworkCredential("svc_v1_dev", "Ver$1oN1_88","Healthfirst");

            //request.Headers.Add("Username", "admin");
            //request.Headers.Add("Password", "admin");
            request.ContentType = "text/xml; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                string responseStr = new StreamReader(responseStream).ReadToEnd();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(responseStr);
                return responseStr;
            }
            return null;
        }
        #endregion

        #region "Helpers"
        // Simple: quotation has not been implemented
        // Disclamer: demo only, do not use your own CSV readers
        public static IEnumerable<string[]> ReadCsvSimple(string file, char delimiter)
        {
            return File
              .ReadLines(file)
              .Where(line => !string.IsNullOrEmpty(line)) // skip empty lines if any
              .Select(line => line.Split(delimiter));
        }
        #endregion
    }
}


