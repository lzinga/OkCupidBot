using OkCupidBot.Common;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OkCupidBot.Services
{
    public class OkCupidService : BaseService
    {

        public OkCupidService()
        {
            ServiceManager.Services.WebService.NavigateTo(Urls.OkCupid);
        }

        public void Login()
        {
            this.Services.WebService.NavigateTo(Urls.OkCupidLogin);
            var usernameField = this.Services.WebService.Browser.FindElementById("login_username");
            var passwordField = this.Services.WebService.Browser.FindElementById("login_password");
            var loginButton = this.Services.WebService.Browser.FindElementById("sign_in_button");

            usernameField.SendKeys(this.Services.ArgumentService.Arguments.Username);
            passwordField.SendKeys(this.Services.ArgumentService.Arguments.Password.ToString());
            loginButton.Click();

            this.Services.WebService.WaitForPageReady();
        }

        public List<Profile> ScanMatches()
        {
            List<Profile> profiles = new List<Profile>();

            this.Services.WebService.NavigateTo(Urls.OkCupidMatches);
            this.Services.WebService.ScrollTo(By.ClassName("blank_state"));
            this.Services.LogService.WriteHeader("Scanning Available Matches");
            Dictionary<string, Uri> baseMatches = new Dictionary<string, Uri>();
            ReadOnlyCollection<IWebElement> matches = this.Services.WebService.Browser.FindElementsByClassName("matchcard-user");
            for(int i = 0; i <= matches.Count; i++)
            {
                string username = matches[i].FindElement(By.ClassName("name")).Text;
                Uri uri = new Uri(matches[i].FindElement(By.ClassName("name")).GetAttribute("href"));

                // TODO add database check if already visited profile.

                Profile newProf = new Profile() { Username = username, ProfilePage = uri };
                this.GetProfile(newProf);

                this.Services.LogService.WriteLine("{0}. Parsing \"{1}\" ...", i + 1, username);
                baseMatches.Add(username, uri);
            }

            this.Services.LogService.WriteLine("Found {0} matches.", profiles.Count);
            return profiles;
        }

        private void GetProfile(Profile prof)
        {
            this.Services.WebService.NavigateTo(prof.ProfilePage);

            #region Main Region
            IWebElement main = this.Services.WebService.Browser.FindElementByClassName("profilesection");

            // Self Summary
            IWebElement selfSummary = main.FindElement(By.XPath("div[1]/div[2]"));
            prof.SelfSummary = selfSummary.Text;


            #endregion

            #region Age / Match / IsOnline
            IWebElement amo = this.Services.WebService.Browser.FindElementByPartialClass("basics-asl");
            prof.Age = amo.FindElementByPartialClass("asl-age").Text.TryParseToInt();
            prof.MatchPercent = Regex.Match(amo.FindElementByPartialClass("asl-match").FindElement(By.TagName("a")).Text, @"\d+").Value.TryParseToInt();
            prof.IsOnline = amo.ElementExists(By.XPath("//*[@data-tooltip='Online now']"));
            #endregion

            #region SideBar
            IWebElement sidebar = this.Services.WebService.Browser.FindElementByPartialClass("content-sidebar");

            List<string> details = new List<string>();
            if(sidebar.ElementExists(By.ClassName("basics")))
            {
                details.AddRange(sidebar.FindElement(By.ClassName("basics")).Text.Trim().Split(','));
            }

            if(sidebar.ElementExists(By.ClassName("background")))
            {
                details.AddRange(sidebar.FindElement(By.ClassName("background")).Text.Trim().Split(','));
            }

            if(sidebar.ElementExists(By.ClassName("misc")))
            {
                details.AddRange(sidebar.FindElement(By.ClassName("misc")).Text.Trim().Split(','));
            }
            #endregion

            // Turn the details list into their respective enums.
            this.ParseDetails(ref prof, details);

        }
        
        private void ParseDetails(ref Profile prof, List<string> details)
        {
            // Get all types in the entire assembly with attribute of ProfileDetailAttribute
            Type[] types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(i => i.FullName.Contains("OkCupidBot"))
                .SelectMany(i => i.GetTypes().Where(x => Attribute.IsDefined(x, typeof(ProfileDetailAttribute))).Select(y => y)).ToArray();

            foreach(Type profileDetail in types)
            {
                // TODO move below logic into here.
            }



            foreach(PropertyInfo property in prof.GetType().GetProperties())
            {
                foreach(string detail in details)
                {
                    // This will not work for a list property of enum.
                    if (property.PropertyType.IsEnum)
                    {
                        try
                        {
                            property.SetValue(prof, Enum.Parse(property.PropertyType, detail.Trim().Replace(" ", ""), true));
                        }
                        catch (Exception ex)
                        {
                            // Failed to get enum by regular name. Try by attribute.
                            RelationToAttribute attribute = property.GetCustomAttribute<RelationToAttribute>();
                            if (attribute == null) continue;

                            property.SetValue(prof, Enum.Parse(property.PropertyType, attribute.Name));
                        }

                    }
                }
            }
        }

        public void Dispose()
        {
            // nothing
        }
    }
}
