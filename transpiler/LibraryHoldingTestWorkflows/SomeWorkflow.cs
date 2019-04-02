using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryHoldingTestWorkflows
{
    public class SomeWorkflow
    {
        public void Fail_if__is_odd(int evenInput)
        {
            Assert.That(evenInput % 2 == 0, $"Input [{evenInput}] should have been even.");
        }
    }

    public class GreetingWorkflow : Workflow
    {
        public string friendName;
        public int friendAge;
        public DateTime friendBirthday;

        public void My_friend__turned__on__(string name, int age, DateTime birthday)
        {
            friendName = name;
            friendAge = age;
            friendBirthday = birthday;
        }


        public string range;
        public void Hello_World()
        {
            range = "all of us";
        }

        public void Hello_America()
        {
            range = "how are you?";
        }
    }
}
