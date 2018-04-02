using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application.Bot_To_CRM
{
    public static class UtilsCRM
    {

        private static IOrganizationService _service = null;
        private static System.IO.StreamWriter _logFile = null;
        private static Entity _entity = null;


        #region Connections
        public static IOrganizationService getConnection()
        {
            CrmServiceClient _conn = new CrmServiceClient(Properties.Settings.Default.UrlCrm);
            IOrganizationService _service = _conn.OrganizationWebProxyClient != null ?
                    (IOrganizationService)_conn.OrganizationWebProxyClient : (IOrganizationService)_conn.OrganizationServiceProxy;
            return _service;
        }
        #endregion
        public static Entity createEntityTextFields(string entityName,List<string> fields,List<string> values,Entity entity = null)
        {
            if (entity == null)
                _entity = new Entity(entityName);
            else
                _entity = entity;
            // create attributes
            try
            {
                if(fields.Count() != values.Count())
                {
                    throw new Exception("fields and values not matching");
                }
                else
                {
                    _entity.Attributes = new AttributeCollection();
                    var att = fields.Zip(values, (f, v) => new { Field = f, Value = v });
                    foreach(var fv in att)
                    {
                        _entity.Attributes.Add(fv.Field, fv.Value);
                    }

                    return _entity;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error due to: " + ex.Message.ToString());
            }

           
        }

        
    }
}