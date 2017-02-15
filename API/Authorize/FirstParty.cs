using FellowshipOne.Api;
using MbUnit.Framework;

namespace API.Authorize {

    [TestFixture]
    internal class FirstParty : Base {
    
        [Test]
        [Author("Tracy Mazelin")]
        [Description("Verifies an access token is retrieved using 1st party oauth")]
        public void AccessToken() {
            Assert.IsNotEmpty(Ticket.AccessToken);
        }

        
    }
}