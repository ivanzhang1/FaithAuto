using FellowshipOne.Api;
using MbUnit.Framework;

namespace API.Groups {

    [TestFixture]
    internal class GroupTypes : Base {

        private string groupTypeID { get; set; }

        [Test(Order = 1)]
        [Author("Tracy Mazelin")]
        [Description("Verifies the API returns a list of group types")]
        public void List() {
            var groupTypes = RestClient.GroupsRealm.GroupTypes.FindAll();
            this.groupTypeID = groupTypes.Items[0].ID.Value.ToString();
            Assert.IsTrue(groupTypes.Items.Count > 0);
        }

        [Test]
        [Author("Tracy Mazelin")]
        [Description("Verifies the API returns a single group type")]
        public void Show() {
            var group = RestClient.GroupsRealm.GroupTypes.Get(groupTypeID);
            Assert.IsNotEmpty(group.uri);
        }
    }
}