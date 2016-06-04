using OkCupidBot.Common;
using OkCupidBot.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OkCupidBot.Services
{
    public class MessageService : BaseService
    {
        private MessageSettings settings;
        Random rand = new Random();

        private Profile Profile { get; set; }

        public MessageService()
        {
            settings = Load.MessageSettings();
        }

        private string GetNameSpaceValue(string key)
        {
            // If it contains a . it means its a namespace.
            if (key.Contains("."))
            {
                object value = null;
                foreach (string item in key.Split('.'))
                {
                    if(value == null)
                    {
                        value = GetPropertyValue(item, this);
                        continue;
                    }

                    if(key.Split('.').Last() == value.GetType().Name)
                    {
                        return value.ToString();
                    }
                    else
                    {
                        value = GetPropertyValue(item, value);
                    }
                }

                return value.ToString();
            }
            else
            {
                return GetPropertyValue(key, this).ToString();
            }
        }

        private object GetPropertyValue(string key, object obj)
        {
            PropertyInfo prop = obj.GetType().GetProperty(key, BindingFlags.NonPublic | BindingFlags.Instance);

            if(prop == null)
            {
                prop = obj.GetType().GetProperty(key);
            }

            if (prop == null)
                throw new InvalidDataException(string.Format("No property named \"{0}\".", key));

            return prop.GetValue(obj);
        }

        private bool ConditionsMet(MessageGroup group)
        {
            List<bool> conditions = new List<bool>();
            foreach (MessageCondition condition in group.Conditions)
            {
                object propValue = GetNameSpaceValue(condition.Key);

                switch (condition.Operator)
                {
                    case Operator.Equals:
                        if (!string.Equals(propValue.ToString(), condition.Value, StringComparison.OrdinalIgnoreCase))
                            return false;
                        break;
                    case Operator.GreaterThan:
                        if ((int)propValue < condition.Value.TryParseToInt())
                            return false;
                        break;
                    case Operator.GreaterThanOrEqualTo:
                        if ((int)propValue < condition.Value.TryParseToInt())
                            return false;
                        break;
                    case Operator.LessThan:
                        if ((int)propValue > condition.Value.TryParseToInt())
                            return false;
                        break;
                    case Operator.LessThanOrEqualTo:
                        if ((int)propValue > condition.Value.TryParseToInt())
                            return false;
                        break;
                    case Operator.Contains:
                        if (!((IList)propValue).SpecialContains(condition.Value))
                            return false;
                        break;
                    case Operator.DoesNotContain:
                        if (((IList)propValue).SpecialContains(condition.Value))
                            return false;
                        break;
                    default:
                        throw new InvalidOperationException("Operator is invalid.");
                }
            }

            return true;
        }

        public string GetMessage(Profile prof)
        {
            this.Profile = prof;
            List<string> allAvailableMessages = new List<string>();
            foreach(MessageGroup group in settings.Groups)
            {
                // If the conditions aren't met go to the next group.
                if (!this.ConditionsMet(group))
                    continue;

                allAvailableMessages.AddRange(group.Messages.Select(i => i.Value));
            }

            if(allAvailableMessages.Count <= 0)
            {
                return string.Empty;
            }

            string message = GetParsedMessage(allAvailableMessages.ToList());
            return message;
        }

        private string GetParsedMessage(List<string> messages)
        {
            List<string> innerMessages = messages;
            string message = messages[rand.Next(0, messages.Count - 1)];

            MatchCollection nspace = new Regex(@"{(\w+(?:\.\w+)*)\.{0,1}}").Matches(message);

            foreach(Match match in nspace)
            {
                if (match.Success)
                {
                    string value = GetNameSpaceValue(match.Groups[1].Value);
                    message = Regex.Replace(message, @"{(\w+(?:\.\w+)*)\.{0,1}}", value);
                }
            }

            return message;
        }
    }
}
