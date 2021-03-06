﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OkCupidBot.Services;
using OkCupidBot.Models;
using System.Reflection;
using System.Collections;
using OpenQA.Selenium;
using static OkCupidBot.Services.DiscordService;

namespace OkCupidBot.Common
{
    public class Essays
    {
        public string SelfSummary { get; set; }
        public string DoingWithMyLife { get; set; }
        public string ReallyGoodAt { get; set; }
        public string FirstNoticeAboutMe { get; set; }
        public string FavoriteItems { get; set; }
        public string NeverDoWithout { get; set; }
        public string ThinkingAbout { get; set; }
        public string TypicalFridayNight { get; set; }
        public string MostPrivateThing { get; set; }
        public string MessageMeIf { get; set; }
    }

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
        public string ProfileImageUrl { get; set; }

        public Essays Essays { get; set; }


        public Profile()
        {
            Essays = new Essays();
            Orientation = new List<Orientation>();
            Ethnicity = new List<Ethnicity>();
        }

        public Profile(string username, Uri profile)
        {
            this.Username = username;
            this.ProfilePage = profile;
            Essays = new Essays();

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
            else
            {
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
            Dictionary<string, bool> requirementPassed = new Dictionary<string, bool>();

            // Loop over all the profile settings.
            foreach (ProfileSetting setting in settings.Settings)
            {
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
                    if(requirementPassed.ContainsKey(value.Key) && requirementPassed[value.Key])
                    {
                        continue;
                    }

                    PropertyInfo property = this.GetType().GetProperty(value.Key);
                    object propValue = property.GetValue(this);

                    if (!requirementPassed.ContainsKey(value.Key))
                    {
                        requirementPassed.Add(value.Key, false);
                    }

                    switch (value.Operator)
                    {
                        case Operator.Equals:
                            if (propValue.ToString() == value.Value)
                                requirementPassed[value.Key] = true;
                            break;
                        case Operator.GreaterThan:
                            if ((int)propValue > value.Value.TryParseToInt())
                                requirementPassed[value.Key] = true;
                            break;
                        case Operator.GreaterThanOrEqualTo:
                            if ((int)propValue >= value.Value.TryParseToInt())
                                requirementPassed[value.Key] = true;
                            break;
                        case Operator.LessThan:
                            if ((int)propValue < value.Value.TryParseToInt())
                                requirementPassed[value.Key] = true;
                            break;
                        case Operator.LessThanOrEqualTo:
                            if ((int)propValue <= value.Value.TryParseToInt())
                                requirementPassed[value.Key] = true;
                            else
                                requirementPassed[value.Key] = true;
                            break;
                        case Operator.Contains:
                            if (((IList)propValue).SpecialContains(value.Value))
                                requirementPassed[value.Key] = true;
                            break;
                        case Operator.DoesNotContain:
                            if (!((IList)propValue).SpecialContains(value.Value))
                                requirementPassed[value.Key] = true;
                            break;
                        default:
                            throw new InvalidOperationException("Operator is invalid.");
                    }
                }
            }

            foreach(KeyValuePair<string, bool> pair in requirementPassed.Where(i => !i.Value))
            {
                ServiceManager.Services.LogService.WriteLine("Failed Validation Because: \"{0}\"", ConsoleColor.Red, pair.Key);
            }

            return !requirementPassed.Any(i => !i.Value);
        }

        public void SendMessage()
        {
            // If already sent message do not send another.
            if (this.HasSentMessage())
            {
                ServiceManager.Services.LogService.WriteLine("Already sent message.");
                return;
            }

            // If discord get messages from users.
            if (ServiceManager.Services.ArgumentService.Arguments.DiscordMessage)
            {
                ServiceManager.Services.LogService.WriteLine("Waiting for Discord...", ConsoleColor.Yellow);
                ServiceManager.Services.DiscordService.SendMessage($"=================================");
                ServiceManager.Services.DiscordService.SendMessage($"{this.Username}");
                ServiceManager.Services.DiscordService.SendMessage($"=================================");
                ServiceManager.Services.DiscordService.SendMessage($"Here is here information:");
                ServiceManager.Services.DiscordService.SendMessage($"Age: {this.Age}");
                ServiceManager.Services.DiscordService.SendMessage($"Body Type: {this.BodyType}");
                ServiceManager.Services.DiscordService.SendMessage($"{this.Height.Feet} Feet {this.Height.Inches} Inches");
                ServiceManager.Services.DiscordService.SendMessage($"Relationship Status: {this.RelationshipStatus}");
                ServiceManager.Services.DiscordService.SendMessage($"Orientation:  {{{string.Join(",", this.Orientation)}}}");
                ServiceManager.Services.DiscordService.SendMessage($"Profile Picture: {this.ProfileImageUrl}");

                List<UserMessage> messages = new List<UserMessage>();
                int waitTime = 40;
                
                ServiceManager.Services.DiscordService.GetUserMessages();
                ServiceManager.Services.LogService.WriteLine("Trying to get messages from discord...");
                ServiceManager.Services.DiscordService.SendMessage($"Waiting for {waitTime} seconds, compose your messages by using /okcm!");
                ServiceManager.Services.WebService.WaitForTime(waitTime);
                messages = ServiceManager.Services.DiscordService.UserMessages;
                

                bool infi = true;
                if (messages.Count <= 0)
                {
                    ServiceManager.Services.DiscordService.SendMessage($"No messages given, waiting for atleast 1 message.");

                    // Keep going until it gets a message.
                    while (ServiceManager.Services.DiscordService.UserMessages.Count <= 0 && infi)
                    {
                        if (ServiceManager.Services.DiscordService.UserMessages.Count > 0)
                        {
                            ServiceManager.Services.DiscordService.SendMessage($"Atleast 1 message has been given, waiting for {waitTime} seconds to continue.");
                            ServiceManager.Services.DiscordService.SendMessage($"Waiting for {waitTime} more seconds, compose your messages by using /okcm!");
                            ServiceManager.Services.WebService.WaitForTime(waitTime);
                            messages = ServiceManager.Services.DiscordService.UserMessages;

                            infi = false;
                            break;
                        }
                    }
                }

                ServiceManager.Services.DiscordService.ReceiveMessages = false;
                ServiceManager.Services.DiscordService.SendMessage($"{messages.Count} Messages Found:");

                for (int i = 0; i < messages.Count; i++)
                {
                    UserMessage curMessage = messages[i];
                    ServiceManager.Services.DiscordService.SendMessage($"{i + 1}. {curMessage.Mention}: {curMessage.Message}");
                }

                Random rand = new Random();
                UserMessage selectedMessage = messages[rand.Next(0, messages.Count - 1)];
                ServiceManager.Services.DiscordService.SendMessage($"Congratulations {selectedMessage.Mention} your message has been chosen - \"{selectedMessage.Message}\"");

                if (string.IsNullOrEmpty(selectedMessage.Message))
                {
                    ServiceManager.Services.LogService.WriteLine("The returned message was empty, something bad happened.", ConsoleColor.Yellow);
                    return;
                }

                ServiceManager.Services.LogService.WriteLine("Message: \"{0}\".", selectedMessage.Message);
                ServiceManager.Services.DiscordService.SendMessage("Message has been sent. Continuing to next profile.");
                //ServiceManager.Services.OkCupidService.SendMessage(selectedMessage.Message);
                //ServiceManager.Services.DatabaseService.AddUser(this, selectedMessage.Message);
            }
            else
            {
                // Should we send a message to the user?
                if (!this.ShouldSendMessage())
                {
                    ServiceManager.Services.LogService.WriteLine("Invalid Requirements...", ConsoleColor.DarkRed);
                    ServiceManager.Services.DatabaseService.AddUser(this, null);
                    return;
                }
                else
                {
                    ServiceManager.Services.LogService.WriteLine("Valid Requirements", ConsoleColor.DarkGreen);
                }


                string message = ServiceManager.Services.MessageService.GetMessage(this);

                if (string.IsNullOrEmpty(message))
                {
                    ServiceManager.Services.LogService.WriteLine("The returned message was empty, something bad happened.", ConsoleColor.Yellow);
                    return;
                }

                ServiceManager.Services.LogService.WriteLine("Message: \"{0}\".", message);
                ServiceManager.Services.OkCupidService.SendMessage(message);
                ServiceManager.Services.DatabaseService.AddUser(this, message);
            }

            
            ServiceManager.Services.LogService.WriteSubHeader("Sent Message to \"{0}\".", this.Username);
        }
    }
}
