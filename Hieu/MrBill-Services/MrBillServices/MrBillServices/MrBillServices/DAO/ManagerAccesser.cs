using MrBillServices.Entities;
using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace MrBillServices.DAO
{
    public class ManagerAccesser
    {
        public List<UserProfile> GettAllUser()
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                return mrbillDbEntities.UserProfiles.Include("CompanyInfo").ToList();
            }
        }

        public List<UserSupplierInfo> GetAllUserSupplierInfo()
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                return mrbillDbEntities.UserSupplierInfoes.ToList();
            }
        }

        public List<Object> GetAllMessage()
        {
            using (Pop3Client client = new Pop3Client())
            {
                // Connect to the server
                client.Connect("imap.tma.com.vn", 995, true);

                // Authenticate ourselves towards the server
                client.Authenticate("bchieu", "Hieubui1@#", AuthenticationMethod.UsernameAndPassword);

                // Get the number of messages in the inbox
                int messageCount = client.GetMessageCount();

                // We want to download all messages
                List<Object> allMessages = new List<Object>(messageCount);

                // Messages are numbered in the interval: [1, messageCount]
                // Ergo: message numbers are 1-based.
                // Most servers give the latest message the highest number
                for (int i = messageCount; i > 0; i--)
                {
                    allMessages.Add(client.GetMessage(i));
                }
                return allMessages;
            }
        }
    }
}
