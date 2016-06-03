using OkCupidBot.Common;
using OpenQA.Selenium;
using System;
using System.Collections;
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

        public void OpenMessageInput()
        {
            IWebElement messageButton = this.Services.WebService.Browser.FindElementByPartialClass("chat");
            messageButton.Click();
            this.Services.WebService.WaitForPageReady();
        }

        public List<Profile> ScanMatches()
        {
            List<Profile> profiles = new List<Profile>();

            this.Services.WebService.NavigateTo(Urls.OkCupidMatches);
            this.Services.WebService.ScrollTo(By.ClassName("blank_state"));
            this.Services.LogService.WriteHeader("Scanning Available Matches");

            ReadOnlyCollection<IWebElement> matches = this.Services.WebService.Browser.FindElementsByClassName("matchcard-user");

            // After it changes pages the matches are no longer found, so lets save them in a temp dictionary.
            List<Tuple<string, string>> cachedMatches = new List<Tuple<string, string>>();
            foreach(IWebElement element in matches)
            {
                cachedMatches.Add(new Tuple<string, string>(element.FindElement(By.ClassName("name")).Text, element.FindElement(By.ClassName("name")).GetAttribute("href")));
            }


            for(int i = 0; i <= cachedMatches.Count; i++)
            {
                string username = cachedMatches[i].Item1;
                Uri uri = new Uri(cachedMatches[i].Item2);

                // TODO add database check if already visited profile.

                this.Services.LogService.WriteLine("{0}. Parsing \"{1}\" ...", i + 1, username);
                profiles.Add(this.GetProfile(username, uri));
            }

            this.Services.LogService.WriteLine("Found {0} matches.", profiles.Count);
            return profiles;
        }

        private Profile GetProfile(string username, Uri profilePage)
        {
            Profile prof = new Profile(username, profilePage);
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


            prof.SendMessage();
            return prof;
        }

        private void ParseDetails(ref Profile prof, List<string> details)
        {
            // Get all types in the entire assembly with attribute of ProfileDetailAttribute
            foreach(string detail in details)
            {
                #region Non Enum
                Match heightMatch = Regex.Match(detail, @"(\d’) (\d”)");
                if (heightMatch.Success)
                {
                    Height height = new Height();
                    int feet;
                    if (int.TryParse(Regex.Match(heightMatch.Groups[1].Value, @"\d+").Value, out feet))
                    {
                        height.Feet = feet;
                    }

                    int inches;
                    if (int.TryParse(Regex.Match(heightMatch.Groups[2].Value, @"\d+").Value, out inches))
                    {
                        height.Inches = inches;
                    }
                    prof.Height = height;
                }
                #endregion

                #region Enums
                object propertyValue = detail.ToEnum();
                if (propertyValue == null) continue;

                foreach(PropertyInfo property in prof.GetType().GetProperties())
                {

                    if (property.PropertyType == propertyValue.GetType())
                    {
                        property.SetValue(prof, propertyValue);
                        break;
                    }
                    else if (property.PropertyType.GenericTypeArguments.Length > 0 && property.PropertyType.GenericTypeArguments[0] == propertyValue.GetType())
                    {
                        var list = (IList)property.GetValue(prof);
                        if (list == null) list = (IList)Activator.CreateInstance(property.PropertyType);

                        if (!list.Contains(propertyValue))
                        {
                            list.Add(propertyValue);
                        }

                        property.SetValue(prof, list);
                        break;
                    }
                }
                #endregion
            }
        }

        public void Dispose()
        {
            // nothing
        }
    }
}
