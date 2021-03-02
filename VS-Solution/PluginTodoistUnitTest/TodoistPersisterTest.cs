using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginTodoistUnitTest
{
    [TestClass]
    public class TodoistPersisterTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            TodoistPersister.TodoistPersister.instance.Token = "313037e74f776fecac3733c9ddb6d72b1331739f";
            var tasks = TodoistPersister.TodoistPersister.instance.getProjectAsString("BV");
            Assert.IsNotNull(tasks);
        }
    }
}
