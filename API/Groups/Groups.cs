using FellowshipOne.Api;
using FellowshipOne.Api.Groups.QueryObject;
using MbUnit.Framework;

namespace API.Groups {

    [TestFixture]
    internal class Groups : Base {

        private string groupID { get; set; }

        [Test(Order = 1)]
        [Author("Tracy Mazelin")]
        [Description("Verifies the API returns a list of groups")]
        public void List() {
            var groupTypes = RestClient.GroupsRealm.GroupTypes.FindAll();
            var groups = RestClient.GroupsRealm.Groups.FindAll(groupTypes.Items[0].ID.Value.ToString());
            this.groupID = groups.Items[0].ID.Value.ToString();
            Assert.IsTrue(groups.Items.Count > 0);
        }

        [Test, DependsOn("List")]
        [Author("Tracy Mazelin")]
        [Description("Verifies the API returns a single group")]
        public void Show() {
            var group = RestClient.GroupsRealm.Groups.Get(groupID);
            Assert.IsNotEmpty(group.uri);
        }

        [Test]
        [Author("Tracy Mazelin")]
        [Description("Verifies passing a query object for group search")]
        public void Search() {
            var qo = new GroupQO();
            qo.IsSearchable = true;
            var groups = RestClient.GroupsRealm.Groups.FindBy(qo);
            Assert.IsTrue(groups.Items.Count > 0);
        }

        [Test, DependsOn("List")]
        [Author("Tracy Mazelin")]
        [Description("Verifies editing single group")]
        public void Edit() {
            //var group = RestClient.GroupsRealm.Groups.Get(groupID);
            //group.Description = "test";
            //var update = RestClient.GroupsRealm.Groups.Update(group, groupID);
            //Assert.IsTrue(update.Description == group.Description);
        }

        [Test]
        [Author("Tracy Mazelin")]
        [Description("Verifies creating single group")]
        public void Create() {
            
        }

    }
}