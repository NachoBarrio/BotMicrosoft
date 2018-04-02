using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application.Bot_To_CRM
{
    public static class EmailDialogToCRM
    {
        private static IOrganizationService _service = null;
        private static Entity _entity = null;
        public static void createLead(string name, string email)
        {
            try
            {
                _service = UtilsCRM.getConnection();
                _entity = UtilsCRM.createEntityTextFields("lead", new List<string> {"fullname","emailaddress1" }, new List<string> {name,email});

                _service.Create(_entity);
            }
            catch(Exception ex)
            {
                //TODO write in log
            }
            
        }
    }
}