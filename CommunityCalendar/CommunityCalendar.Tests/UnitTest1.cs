using System;
using System.IO;
using CommunityCalendar;

namespace CommunityCalendar.Tests {
    public class OrganizersFileTests
    {
        const string TestFile = "test-events.txt";

        [Fact]
        public void CreateNewUser() {
        if(File.Exists(TestFile)) File.Delete(TestFile);

        UserFunctions.CreateUser("Jim", TestFile);
        Assert.True(UserFunctions.UserExists("Jim", TestFile));

        File.Delete(TestFile);
        }

        [Fact]

        public void UserExists_False_IfEmpty() {
            if(File.Exists(TestFile)) File.Delete(TestFile);

            Assert.False(UserFunctions.UserExists("Tim", TestFile));
            
            File.Delete(TestFile);
        }

        [Fact]

        public void NewEventAdds() {
            if(File.Exists(TestFile)) File.Delete(TestFile);

            EventFunctions.NewEvent("Tom", "market", "05/15/2025", "10:00 AM", "a community market.", TestFile);

            var events = EventFunctions.GetUserEvents("Tom", TestFile);
            Assert.Single(events);
            Assert.StartsWith("Tom|market|05/15/2025|10:00 AM|a community market", events[0]);
        }

        [Fact]

        public void ReturnUserEvents() {
            if(File.Exists(TestFile)) File.Delete(TestFile);

            EventFunctions.NewEvent("Gary","event 1", "05/16/2025", "12:00 PM", "Test Event One", TestFile);
            EventFunctions.NewEvent("Gary","event 2", "05/17/2025", "1:00 PM", "Test Event Two", TestFile);

            var eventlist = EventFunctions.GetUserEvents("Gary", TestFile);
            Assert.Equal(2, eventlist.Count);
            Assert.Contains(eventlist, e => e.StartsWith("Gary|event 1|05/16/2025|12:00 PM|Test Event One"));
            Assert.Contains(eventlist, e => e.StartsWith("Gary|event 2|05/17/2025|1:00 PM|Test Event Two"));
        }

        [Fact]
        public void ReturnEventsforDate() {
            if(File.Exists(TestFile)) File.Delete(TestFile);

            EventFunctions.NewEvent("John", "event 1", "05/16/2025", "12:00 PM", "Test Event One", TestFile);
            EventFunctions.NewEvent("John","event 2", "05/17/2025", "1:00 PM", "Test Event Two", TestFile);

            var may16 = EventFunctions.GetEventsforDate("05/16/2025", TestFile);
            Assert.Single(may16);
            Assert.StartsWith("John|event 1|05/16/2025|12:00 PM|Test Event One", may16[0]);

            var may17 = EventFunctions.GetEventsforDate("05/17/2025", TestFile);
            Assert.Single(may17);
            Assert.StartsWith("John|event 2|05/17/2025|1:00 PM|Test Event Two", may17[0]);
        }
    }
}