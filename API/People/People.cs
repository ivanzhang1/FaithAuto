using FellowshipOne.Api;
using FellowshipOne.Api.People.Model;
using FellowshipOne.Api.People.QueryObject;
using MbUnit.Framework;
using Gallio.Framework;
using System;

namespace API.People {

    internal class People : Base {
        private int? personID;
        private int? householdID;

        [Test(Order = 1)]
        [Author("Tracy Mazelin")]
        [Description("Verifies the API returns a list of people based on a query object")]
        public void Search() {
            var qo = new PeopleQO();
            qo.Name = "sm";
            qo.RecordsPerPage = 5;
            var people = RestClient.PeopleRealm.People.Search<PersonCollection>(qo);
            this.personID = people[0].ID.Value;
            this.householdID = people[0].HouseholdID.Value;
            Assert.GreaterThan(people.Count, 0);
        }

        [Test(Order = 2)]
        [Author("Tracy Mazelin")]
        [Description("Verifies the API returns a single person")]
        public void Show() {
            var person = RestClient.PeopleRealm.People.Get(this.personID.ToString());
            Assert.IsNotNull(person);
            Assert.IsNotNull(person.HouseholdID);
        }

        [Test(Order = 3)]
        [Author("Tracy Mazelin")]
        [Description("Verifies the creation of a person")]
        public void Create() {
            var person = new Person {
                HouseholdID = this.householdID,
                Status = {
                    ID = 110,
                    Date = DateTime.Now
                },
                FirstName = "API",
                LastName = "Test",
                HouseholdMemberType = {
                    ID = (int)FellowshipOne.Api.People.Enum.HouseholdMemberTypes.Head
                }
            };
            Person personCreated = null;

            try {
                personCreated = RestClient.PeopleRealm.People.Create(person);
            } catch (Exception e) {
                Assert.Fail(e.Message);
            }
            this.personID = personCreated.ID;
            Assert.IsNotNull(personCreated);
            Assert.IsNotNull(personCreated.ID);
        }

        [Test(Order = 4)]
        [Author("Tracy Mazelin")]
        [Description("Verifies the update of a person")]
        public void Update() {
            var person = RestClient.PeopleRealm.People.Get(this.personID.ToString());
            person.LastName = "Test Update";
            var personUpdated = RestClient.PeopleRealm.People.Update(person, person.ID.ToString());
            Assert.AreEqual(personUpdated.LastName, person.LastName);
        }

        [Test(Order = 5)]
        [Author("Tracy Mazelin")]
        [Description("Tests Fix for FO-4689 Stored Procedure [API_PeopleSearch_ResultsByLastUpdated]. Count returned is incorrect as well as payload")]
        public void Issue_FO_4689() {
            var qo = new PeopleQO();
            qo.LastUpdatedDate = Convert.ToDateTime("2015-08-18");
            var r = RestClient.PeopleRealm.People.Search<PersonCollection>(qo);
            // The results count should be less than or equal to the count muliplied by additional pages plus 1 for the current page.
            // Fix deployed to Production and test now passes. 9-1-2015.  
            Assert.LessThanOrEqualTo(r.TotalRecords, (r.Count * (r.AdditionalPages + 1)));
        }

        [Test]
        [Author("Mady Kou")]
        [Description("FO-4973 API: People Merge Losers API (FO-4075) Not Filtering By ChurchCode")]
        public void SearchMergers()
        {
            TestLog.WriteLine("Generate the URL");

            var qo = new MergersQO();
            qo.LastUpdatedDate = Convert.ToDateTime("2015-08-31");

            string winnerName = "";
            var mergers = RestClient.PeopleRealm.PeopleMerges.Search<MergerCollection>(qo);
            foreach (var merger in mergers) {
                winnerName = "";

                TestLog.WriteLine("Check the winner id: " + merger.WinnerId.id);
                winnerName = base._sql.People_Individual_FetchColumnValue(15, merger.WinnerId.id, "first_Name");
                TestLog.WriteLine("The winner name found from DB is: " + winnerName);
                Assert.IsFalse("".Equals(winnerName), "The winner name found is not correct in current DC church");

                TestLog.WriteLine("Check the loser id: " + merger.LoserId.id);
                Assert.IsFalse("".Equals(merger.LoserId.id.ToString()), "The loser id found should not be empty in API resposne");
            }

        }
    }
}