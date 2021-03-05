using Newtonsoft.Json;
using PluginTodoist.Serialization.Resolvers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Todoist.Net;
using Todoist.Net.Models;

namespace TodoistPersister
{

    public class TodoistPersister
    {

        private static readonly JsonSerializerSettings SerializerSettings =
         new JsonSerializerSettings
         {
             NullValueHandling = NullValueHandling.Ignore,
             ContractResolver = new ConverterContractResolver()
         };

        private T DeserializeResponse<T>(string responseContent)
        {
            return JsonConvert.DeserializeObject<T>(responseContent, SerializerSettings);
        }


        private static TodoistPersister _instance;
        static HttpClient client = new HttpClient();
        static TodoistClient TDClient;
        public static TodoistPersister instance
        {
            get
            {

                if (_instance == null)
                {
                    _instance = new TodoistPersister();
                }

                return _instance;
            }
        }


        public string Token;
        private DateTime LastResourceRefresh;
        public TodoistPersister()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }
        private Resources res;
        private DateTime lastUpdate = DateTime.MinValue;
        public string getFilterAsString(string filterName)
        {
            string parameters = getFilterQueryParameter(filterName);
            return getTasksAsOneString(parameters);

        }
        internal string getFilterAsString(string filterName, int itemIndex)
        {
            string parameters = getFilterQueryParameter(filterName);
            return getTaskByIndexAsString(parameters, itemIndex);
        }

        private string getTaskByIndexAsString(string parameters, int itemIndex)
        {
            var resultItems = GetItemsByParams(parameters);

            string result = "";
            if (resultItems.Count() >= itemIndex + 1)
                result = resultItems[itemIndex].Content;


            return result;
        }

        string getFilterQueryParameter(string filterName)
        {
            if (TDClient == null)
            {

                TDClient = new TodoistClient(Token);
            }
            if (filterName == "")
                return "FilternameName is missing";
            if (lastUpdate < DateTime.Now.AddMinutes(-1))
            {
                res = TDClient.GetResourcesAsync().GetAwaiter().GetResult();
                lastUpdate = DateTime.Now;
            }
            Filter f = null;
            foreach (Filter filter in res.Filters)
            {
                if (filter.Name == filterName)
                    f = filter;

            }
            if (f == null)
                return "could not find filter: " + filterName;

            return "?filter=" + HttpUtility.UrlEncode(f.Query);
        }


        public string getProjectAsString(string projectName)
        {
            if (TDClient == null)
            {

                TDClient = new TodoistClient(Token);
            }

            if (projectName == "")
                return "ProjectName is missing";
            if (lastUpdate < DateTime.Now.AddMinutes(-1))
            {
                res = TDClient.GetResourcesAsync().GetAwaiter().GetResult();
                lastUpdate = DateTime.Now;

            }
            Project p = null;
            foreach (Project project in res.Projects)
            {
                if (project.Name == projectName)
                    p = project;

            }
            if (p == null)
                return "could not find project: " + projectName;

            return getTasksAsOneString("?project_id=" + p.Id);


        }


        internal string getProjectAsString(string projectName, int itemIndex)
        {
            throw new NotImplementedException();
        }
        public List<Item> GetItemsByParams(string parameters)
        {
            if (Token == "")
                return new List<Item>(); //todo: Throw exception, log, show result from last successful request

            client.DefaultRequestHeaders.Authorization
             = new AuthenticationHeaderValue("Bearer", Token);


            HttpResponseMessage response = client.GetAsync("https://api.todoist.com/rest/v1/tasks" + parameters).Result;
            if (response.IsSuccessStatusCode)
            {
                string projectResponse = response.Content.ReadAsStringAsync().Result;
                var items = DeserializeResponse<IEnumerable<Item>>(projectResponse);
                List<Item> resultitems = new List<Item>();
                foreach (var item in items)
                {
                    Item realItem = res.Items.Where(i => i.Id == item.Id).FirstOrDefault();
                    if (realItem == null)
                    {
                        item.Priority = Priority.Priority4;
                        resultitems.Add(item);
                    }
                    else
                        resultitems.Add(realItem);

                }
                resultitems = resultitems.OrderByDescending(o => o.Priority).ToList();
                return resultitems;
            }

            else
                return new List<Item>(); //todo: Throw exception, log, show result from last successful request


        }
        public string getTasksAsOneString(string parameters)
        {
            var resultItems = GetItemsByParams(parameters);
           
                string result = "";
                foreach (var item in resultItems)
                {
                    result = result + " • " + item.Content + Environment.NewLine;

                }

                return result;
        }


    }
}
