using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OkCupidBot.Services;
using OkCupidBot.Models;
using System.Reflection;
using System.Collections;

namespace OkCupidBot.Common
{
    public class Profile
    {
        public string Username { get; set; }
        public Uri ProfilePage { get; set; }
        public int Age { get; set; }
        public string Location { get; set; }
        public int MatchPercent { get; set; }
        public List<Orientation> Orientation { get; set; }
        public RelationshipStatus RelationshipStatus { get; set; }
        public BodyType BodyType { get; set; }
        public List<Ethnicity> Ethnicity { get; set; }
        public Height Height { get; set; }
        public Smoking Smoking { get; set; }
        public Drinking Drinking { get; set; }
        public Drugs Drugs { get; set; }
        public Sex Sex { get; set; }
        public bool IsOnline { get; set; }

        public string SelfSummary { get; set; }

        public Profile()
        {
            Orientation = new List<Orientation>();
            Ethnicity = new List<Ethnicity>();
        }

        public Profile(string username, Uri profile)
        {
            this.Username = username;
            this.ProfilePage = profile;
            
            Orientation = new List<Orientation>();
            Ethnicity = new List<Ethnicity>();
        }

        public bool HasSentMessage()
        {
            bool result = false;

            // If we aren't currently on the profiles page go to it and open the message box.
            if (!ServiceManager.Services.WebService.AlreadyOnPage(this.ProfilePage))
            {
                ServiceManager.Services.WebService.NavigateTo(this.ProfilePage);
                ServiceManager.Services.OkCupidService.OpenMessageInput();
            }

            result = ServiceManager.Services.WebService.Browser.FindElementsByClassName("yours").Count > 0;

            if (result)
            {
                ServiceManager.Services.LogService.WriteLine("Message has already been sent to \"{0}\".", ConsoleColor.Yellow, this.Username);
            }

            return result;
        }

        public bool ShouldSendMessage()
        {
            ProfileSettings settings = Load.ProfileSettings();
            List<bool> valid = new List<bool>();
            // Loop over all the profile settings.
            foreach (ProfileSetting setting in settings.Settings)
            {
                valid.Clear();
                bool conditionsMet = true;

                // Check the conditions.
                foreach (ProfileCondition condition in setting.Conditions)
                {
                    PropertyInfo property = this.GetType().GetProperty(condition.Field);

                    // If it does not contain the property throw an exception.
                    if (property == null) throw new NullReferenceException(string.Format("\"{0}\" field does not exist in {1}. Profile setting condition \"Field\" is invalid.", condition.Field, this.GetType()));

                    object propValue = property.GetValue(this);

                    // If the value is greater than to and less than from.
                    if ((int)propValue < condition.From || (int)propValue > condition.To)
                    {
                        conditionsMet = false;
                        break;
                    }
                }

                // If all the conditions were not met continue to next setting.
                if (!conditionsMet) continue;

                
                // Now check all profile values.
                foreach(Requirement value in setting.Requirements)
                {
                    PropertyInfo property = this.GetType().GetProperty(value.Key);
                    object propValue = property.GetValue(this);

                    switch (value.Operator)
                    {
                        case Operator.Equals:
                            if (propValue.ToString() == value.Value)
                            {
                                ServiceManager.Services.LogService.WriteLine("{0}: \"{1}\" equals \"{2}\".", ConsoleColor.Green, value.Key, propValue.ToString(), value.Value);
                                valid.Add(true);
                            }
                            else
                            {
                                ServiceManager.Services.LogService.WriteLine("{0}: \"{1}\" does not equal \"{2}\".", ConsoleColor.Red, value.Key, propValue.ToString(), value.Value);
                                valid.Add(false);
                            }
                            break;
                        case Operator.GreaterThan:
                            if ((int)propValue > value.Value.TryParseToInt())
                            {
                                ServiceManager.Services.LogService.WriteLine("{0}: \"{1}\" is greater than \"{2}\".", ConsoleColor.Green, value.Key, propValue.ToString(), value.Value);
                                valid.Add(true);
                            }
                            else
                            {
                                ServiceManager.Services.LogService.WriteLine("{0}: \"{1}\" is not greater \"{2}\".", ConsoleColor.Red, value.Key, propValue.ToString(), value.Value);
                                valid.Add(false);
                            }
                            break;
                        case Operator.GreaterThanOrEqualTo:
                            if ((int)propValue >= value.Value.TryParseToInt())
                            {
                                ServiceManager.Services.LogService.WriteLine("{0}: \"{1}\" greater than or equal to \"{2}\".", ConsoleColor.Green, value.Key, propValue.ToString(), value.Value);
                                valid.Add(true);
                            }
                            else
                            {
                                ServiceManager.Services.LogService.WriteLine("{0}: \"{1}\" is not greater or equal to \"{2}\".", ConsoleColor.Red, value.Key, propValue.ToString(), value.Value);
                                valid.Add(false);
                            }
                            break;
                        case Operator.LessThan:
                            if ((int)propValue < value.Value.TryParseToInt())
                            {
                                ServiceManager.Services.LogService.WriteLine("{0}: \"{1}\" less than \"{2}\".", ConsoleColor.Green, value.Key, propValue.ToString(), value.Value);
                                valid.Add(true);
                            }
                            else
                            {
                                ServiceManager.Services.LogService.WriteLine("{0}: \"{1}\" is not less than \"{2}\".", ConsoleColor.Red, value.Key, propValue.ToString(), value.Value);
                                valid.Add(false);
                            }
                            break;
                        case Operator.LessThanOrEqualTo:
                            if ((int)propValue <= value.Value.TryParseToInt())
                            {
                                ServiceManager.Services.LogService.WriteLine("{0}: \"{1}\" less than or equal to \"{2}\".", ConsoleColor.Green, value.Key, propValue.ToString(), value.Value);
                                valid.Add(true);
                            }
                            else
                            {
                                ServiceManager.Services.LogService.WriteLine("{0}: \"{1}\" is not less than or equal to \"{2}\".", ConsoleColor.Red, value.Key, propValue.ToString(), value.Value);
                                valid.Add(false);
                            }
                            break;
                        case Operator.Contains:
                            if (((IList)propValue).SpecialContains(value.Value))
                            {
                                ServiceManager.Services.LogService.WriteLine("{0}: \"{1}\" contains \"{2}\".", ConsoleColor.Green, value.Key, propValue.ToString(), value.Value);
                                valid.Add(true);
                            }
                            else
                            {
                                ServiceManager.Services.LogService.WriteLine("{0}: \"{1}\" does not contain \"{2}\".", ConsoleColor.Red, value.Key, propValue.ToString(), value.Value);
                                valid.Add(false);
                            }
                            break;
                        default:
                            throw new InvalidOperationException("Operator is invalid.");
                    }
                }

                // Is there any with false?
                if (!valid.Any(i => !i))
                {
                    break;
                }
            }

            return !valid.Any(i => !i);
        }

        public void SendMessage()
        {
            ServiceManager.Services.LogService.WriteSubHeader("Sending Message to \"{0}\".", this.Username);

            // If already sent message do not send another.
            if (this.HasSentMessage())
            {
                ServiceManager.Services.LogService.WriteLine("Already sent message.");
                return;
            }
                

            // Should we send a message to the user?
            if (!this.ShouldSendMessage())
            {
                ServiceManager.Services.LogService.WriteLine("Invalid Requirements...");
                return;
            }
            else
            {
                ServiceManager.Services.LogService.WriteLine("Valid Requirements...");
            }




        }


    }
}
