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
using Weighted_Randomizer;

namespace OkCupidBot.Services
{
    public class MessageService : BaseService
    {
        private MessageSettings settings;
        Random rand = new Random();

        private Profile Profile { get; set; }
        private DateTime DateTime { get; set; }


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
            #region Properties
            PropertyInfo prop = obj.GetType().GetProperty(key, BindingFlags.NonPublic | BindingFlags.Instance);

            if(prop == null)
            {
                prop = obj.GetType().GetProperty(key);
            }

            // If we successfully have a property return the value.
            if(prop != null)
            {
                return prop.GetValue(obj);
            }
            #endregion

            #region Field
            FieldInfo field = obj.GetType().GetField(key);

            if (field != null)
            {
                return field.GetValue(obj);
            }
            #endregion

            throw new InvalidDataException(string.Format("No property named \"{0}\".", key));
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
                        if (propValue.ToString().TryParseToInt() < condition.Value.TryParseToInt())
                            return false;
                        break;
                    case Operator.GreaterThanOrEqualTo:
                        if (propValue.ToString().TryParseToInt() < condition.Value.TryParseToInt())
                            return false;
                        break;
                    case Operator.LessThan:
                        if (propValue.ToString().TryParseToInt() > condition.Value.TryParseToInt())
                            return false;
                        break;
                    case Operator.LessThanOrEqualTo:
                        if (propValue.ToString().TryParseToInt() > condition.Value.TryParseToInt())
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
            DynamicWeightedRandomizer<string> allAvailableMessages = new DynamicWeightedRandomizer<string>();
            foreach (MessageGroup group in settings.Groups)
            {
                // If the conditions aren't met go to the next group.
                if (!this.ConditionsMet(group))
                    continue;

                foreach(Message messageObj in group.Messages)
                {
                    allAvailableMessages.Add(messageObj.Value, group.Conditions.Count);
                }
                
            }

            // If weight is 0 set weight to 1.
            if(allAvailableMessages.TotalWeight == 0)
            {
                foreach(string item in allAvailableMessages)
                {
                    allAvailableMessages.SetWeight(item, 1);
                }
            }

            if(allAvailableMessages.Count <= 0)
            {
                return string.Empty;
            }

            string message = GetParsedMessage(allAvailableMessages);
            return message;
        }

        private string GetParsedMessage(DynamicWeightedRandomizer<string> messages)
        {
            string message = messages.NextWithReplacement();

            MatchCollection nspace = new Regex(@"{(\w+(?:\.\w+)*)\.{0,1}}").Matches(message);

            foreach(Match match in nspace)
            {
                if (match.Success)
                {
                    string value = GetNameSpaceValue(match.Groups[1].Value);
                    message = message.Replace(match.Captures[0].Value, value);
                }
            }

            return message;
        }
    }
}
