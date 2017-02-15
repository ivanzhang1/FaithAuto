using FellowshipOne.Api;
using FellowshipOne.Api.People.Model;
using FellowshipOne.Api.People.QueryObject;
using MbUnit.Framework;
using System;

namespace API.People {

    internal class Households : Base {
        private int? householdID;

        [Test(Order = 1)]
        [Author("Tracy Mazelin")]
        [Description("Verifies the API returns a list of households based on a query object")]
        public void Search() {
            var qo = new HouseholdQO();
            qo.LastUpdatedDate = Convert.ToDateTime("2015-01-01");
            var hh = RestClient.PeopleRealm.Households.Search<HouseholdCollection>(qo);
            this.householdID = hh[0].ID.Value;
            Assert.GreaterThan(hh.Count, 0);
        }

        [Test(Order = 2)]
        [Author("Tracy Mazelin")]
        [Description("Verifies the API returns a single household")]
        public void Show() {
            var hh = RestClient.PeopleRealm.Households.Get(this.householdID.ToString());
            Assert.IsNotNull(hh);
            Assert.IsNotNull(hh.ID);
        }

        [Test(Order = 3)]
        [Author("Tracy Mazelin")]
        [Description("Verifies the creation of a household")]
        public void Create() {
            var hh = new Household {
                HouseholdName = "API Test",
                HouseholdFirstName = "API",
                HouseholdSortName = "Test"
            };
            Household hhCreated = null;

            try {
                hhCreated = RestClient.PeopleRealm.Households.Create(hh);
            } catch (Exception e) {
                Assert.Fail(e.Message);
            }
            this.householdID = hhCreated.ID;
            Assert.IsNotNull(hhCreated);
            Assert.IsNotNull(hhCreated.ID);
        }

        [Test(Order = 4)]
        [Author("Tracy Mazelin")]
        [Description("Verifies the update of a hh")]
        public void Update() {
            var hh = RestClient.PeopleRealm.Households.Get(this.householdID.ToString());
            hh.HouseholdSortName = "Test Update";
            var hhUpdated = RestClient.PeopleRealm.Households.Update(hh, hh.ID.ToString());
            Assert.AreEqual(hhUpdated.HouseholdSortName, hh.HouseholdSortName);
        }
    }
}