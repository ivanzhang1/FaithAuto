using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace FTTests.Portal.WebLink {
	[TestFixture]
    [Importance(Importance.Critical)]
	public class Portal_WebLink_VolunteerApplication : FixtureBase {
		#region Manage Forms

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies you can associate and remove a portal user from an opportunity.")]
        public void Portal_WebLink_VolunteerApplication_ManageForms_AddRemoveAssociatedUser() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Select volunteer form and add a portal user to an opportunity
            test.Portal.WebLink_FormNames_AddAssociatedUser("Auto Test Volunteer Form", "FT Tester");

            // Remove portal user from the opportunity
            test.Portal.WebLink_FormNames_RemoveAssociatedUser("Auto Test Volunteer Form", "FT Tester");

            // Logout of Portal
            test.Portal.Logout();

        }
		#endregion Manage Forms
	}
}
