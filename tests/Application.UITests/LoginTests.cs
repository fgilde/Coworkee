using Application.UITests.Base;
using Coworkee.Shared.Constants.Application;

namespace Application.UITests
{
    public class LoginTests : UiTestBase
    {
        public LoginTests(UITestFixture fixture)
            : base(fixture)
        { }

        [Fact]
        public async Task Test_LoginFlow()
        {
            var user = ApplicationConstants.Defaults.Users.Administrators[0];
            await Page.GotoAsync($"{FrontendUrl}/login");
            
            await Page.FillAsync("#email", user.Email);
            await Page.FillAsync("#password", user.Password);
            await Page.Keyboard.PressAsync("Tab");
            await Page.ClickAsync(".btn-login");

            await Page.WaitForSelectorAsync("h2");
            
            // Assertion
            var welcomeText = await Page.InnerTextAsync("h2");
            Assert.Contains(ApplicationConstants.ApplicationName, welcomeText);
        }
    }


}
