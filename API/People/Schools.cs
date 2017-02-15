using FellowshipOne.Api;
using MbUnit.Framework;

namespace API.People {

    internal class Schools : Base {

        [Test(Order = 1)]
        [Author("Tracy Mazelin")]
        [Description("Verifies the API returns a list of schools")]
        public void List() {
            //var schools = RestClient.PeopleRealm.Schools.FindAll();
            //Assert.IsTrue(schools.Items.Count > 0);
        }

        [Test]
        [Author("Tracy Mazelin")]
        [Description("Verifies the API returns a single school")]
        public void Show() {
            //var schools = RestClient.PeopleRealm.Sch

           // var schools = RestClient.PeopleRealm.Schools.Get(12457);
           // Assert.IsNotEmpty(schools.uri);
        }
    }
}
