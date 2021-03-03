using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TodoistPersister
{
    public class TodoistPersister
    {
        private static TodoistPersister _instance;
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


            LastResourceRefresh = DateTime.Now;

        }

        public string getProjectAsString(string projectName)
        {
            Debugger.Launch();
            Debugger.Break();
                return "";
        }
    }
}
