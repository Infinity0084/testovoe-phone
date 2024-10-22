using PhoneApp.Domain.Attributes;
using PhoneApp.Domain.DTO;
using PhoneApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EmployeesLoaderNetworkPlugin
{
    [Author(Name = "Yura Pahomovich")]
    public class Plugin : IPluggable
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();        

        public IEnumerable<DataTransferObject> Run(IEnumerable<DataTransferObject> args)
        {
            logger.Info("Loading employees from network");

            var url = EmployeesLoaderNetworkPlugin.Properties.Resource1.UserUrl;
            string responseBody = "";

            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            responseBody = reader.ReadToEnd();
                        }
                    }
                    else
                    {
                        logger.Info($"Ошибка: {response.StatusCode}");
                    }
                }
            } catch (Exception ex)
            {
                logger.Info($"Ошибка при запросе: {ex.Message}");
            }


           var usersList = Newtonsoft.Json.JsonConvert.DeserializeObject<UsersResponse>(responseBody);
           var employeesList = usersList.Users;

            logger.Info($"Loaded {employeesList.Count()} employees");
            return employeesList.Cast<DataTransferObject>();
        }
    }

    class UsersResponse
    {
        public List<EmployeesDTO> Users { get; set; }
    }
}
