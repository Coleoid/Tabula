using NUnit.Framework;
using System;

namespace LibraryHoldingTestWorkflows
{
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

        public void Verify_that_my_friend_is_named__(string name)
        {
            Assert.That(name, Is.EqualTo(friendName));
        }

        public void Verify_that_my_friend_is_age__(int age)
        {
            Assert.That(age, Is.EqualTo(friendAge));
        }

        public void Verify_that_my_friend_has_birthday__(DateTime birthday)
        {
            Assert.That(birthday, Is.EqualTo(friendBirthday));
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

        public void There_should_be_eight_of__(int theseGuys)
        {
            Assert.That(theseGuys, Is.EqualTo(8), "failed as expected");
        }

        public void My_favorite_day_is__(DateTime favoriteDay)
        { }

        public void Always_explode()
        {
            int zero = 0;
            int x = 34268 / zero;
        }

        public string GetRange()
        {
            return range;
        }
    }
}
