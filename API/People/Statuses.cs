using FellowshipOne.Api;
using MbUnit.Framework;

namespace API.People {

    internal class Statuses : Base {

        [Test(Order = 1)]
        [Author("Tracy Mazelin")]
        [Description("Verifies the API returns a list of statuses")]
        public void List() {
            var statuses = RestClient.PeopleRealm.Statuses.FindAll();
            Assert.IsTrue(statuses.Items.Count > 0);
        }

        [Test, DependsOn("List")]
        [Author("Tracy Mazelin")]
        [Description("Verifies the API returns a single status")]
        public void Show() {
            var status = RestClient.PeopleRealm.Statuses.Get("110");
            Assert.IsNotEmpty(status.uri);
        }
    }
}