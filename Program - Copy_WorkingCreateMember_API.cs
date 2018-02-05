using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Web.Http.Cors;
using VersionOne;
using VersionOne.SDK.APIClient;

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
               // return "https://v1dev.healthfirst.org/VersionOne_Dev";
            }
        }
        static string getXML(string xmlType)
        {
            string returnXML = "";
            if (xmlType == "MemberXML")
            {
                
                returnXML = @"<Asset href='/api-examples/rest-1.v1/New/Member'><Attribute name='Name' act='set'>Joe Agile</Attribute><Attribute name='Nickname' act='set'>Joe</Attribute><Attribute name='Username' act='set'>Joe.agile</Attribute><Attribute name='Email' act='set'>Joe.agile@mailinator.com</Attribute><Attribute name='IsCollaborator' act='set'>false</Attribute><Attribute name='NotifyViaEmail' act='set'>false</Attribute><Attribute name='SendConversationEmails' act='set'>false</Attribute><Relation name='DefaultRole' act='set'> <Asset href='/api-examples/rest-1.v1/Data/Role/4' idref='Role:4' /></Relation></Asset>";

            }
            return returnXML;
        }

        static void Main(string[] args)
        {
            

            Console.WriteLine("strart");
            //string url = string.Format("{0}/name?PrimaryName={1}", System.Configuration.ConfigurationManager.AppSettings["URLREST"], "yournmae");
            // string details = CallRestMethod(Program.baseURL);
            string CreateMember = postXMLData(Program.baseURL, getXML("MemberXML"));
            //Console.WriteLine(details);
            //getAllStories();
            // getMemberDetails();
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
        public static string CallRestMethod(string url)
        {
            try
            {
                Console.WriteLine("in CallREST");
                HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url + "/rest.v1/Data/Story?sel=Name,Number");
                webrequest.Method = "GET";
                webrequest.ContentType = "application/x-www-form-urlencoded";
                webrequest.Headers.Add("Username", "admin");
                webrequest.Headers.Add("Password", "admin");
                
                HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
                Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
                string result = string.Empty;
                result = responseStream.ReadToEnd();
                webresponse.Close();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "error";
            }
        }

        static string postXMLData(string destinationUrl, string requestXml)
        {
            destinationUrl = @"https://www16.v1host.com/api-examples/rest-1.v1/Data/Member";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);
            request.Credentials = new NetworkCredential("admin", "admin");
            //request.Headers.Add("Username", "admin");
            //request.Headers.Add("Password", "admin");
            //request.UseDefaultCredentials = true;
           // request.PreAuthenticate = true;
           // request.Credentials = CredentialCache.DefaultCredentials;
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
                return responseStr;
            }
            return null;
        }
    }
}


