using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OkCupidBot.Common
{
    public static class Extensions
    {
        /// <summary>
        /// Serialize the object into a string.
        /// </summary>
        public static string Serialize<T>(this T obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, obj);
                return textWriter.ToString();
            }
        }

        public static bool SpecialContains(this IList list, string value)
        {
            foreach(var item in list)
            {
                if(item.ToString() == value)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Deserializes string into object.
        /// </summary>
        public static T Deserialize<T>(this string data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(data))
            {
                return (T)serializer.Deserialize(reader);
            }
        }


        public static SecureString ToSecureString(this string strPassword)
        {
            var secureStr = new SecureString();
            if (strPassword.Length > 0)
            {
                foreach (var c in strPassword.ToCharArray()) secureStr.AppendChar(c);
            }
            return secureStr;
        }

        public static IWebElement FindElementByPartialClass(this IWebDriver driver, string partialClass)
        {
            return driver.FindElement(By.XPath(string.Format(@"//*[@class[contains(., '{0}')]]", partialClass)));
        }

        public static IWebElement FindElementByPartialClass(this IWebElement element, string partialClass)
        {
            return element.FindElement(By.XPath(string.Format(@"//*[@class[contains(., '{0}')]]", partialClass)));
        }

        public static bool ElementExists(this IWebElement element, By by)
        {
            return element.FindElements(by).Count > 0;
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9 ]", "").Trim();
        }

        public static object ToEnum(this string str)
        {
            string original = str.Trim().RemoveSpecialCharacters();
            str = str.Trim().RemoveSpecialCharacters().Replace(" ", "");

            Type[] types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(i => i.FullName.Contains("OkCupidBot"))
                .SelectMany(i => i.GetTypes().Where(x => Attribute.IsDefined(x, typeof(ProfileDetailAttribute))).Select(y => y)).ToArray();

            object result = null;
            foreach (Type type in types)
            {
                // Try to parse it directly to the enum.
                try
                {
                    result = Enum.Parse(type, str);

                    // For some reason the height say "55" would actually be turned into the enum.
                    if(!Enum.IsDefined(type, result))
                    {
                        result = null;
                    }

                    break;
                }
                catch (Exception ex)
                {
                    // best we can do.
                }

                foreach (FieldInfo field in type.GetFields().Where(i => i.GetCustomAttribute<RelationToAttribute>() != null))
                {
                    RelationToAttribute[] relation = field.GetCustomAttributes<RelationToAttribute>().ToArray();
                    if (relation == null || relation.Length <= 0) continue;

                    foreach(RelationToAttribute attr in relation)
                    {
                        // Check the original value not the trimmed/no-space one.
                        if (string.Equals(attr.Name, original, StringComparison.OrdinalIgnoreCase))
                        {
                            result = Enum.Parse(type, field.Name);

                            // For some reason the height say "55" would actually be turned into the enum.
                            if (!Enum.IsDefined(type, result))
                            {
                                result = null;
                                continue;
                            }

                            return result;
                        }
                    }
                }

            }


            return result;
        }

        public static int TryParseToInt(this string str)
        {
            int val;
            if(int.TryParse(str, out val))
            {
                return val;
            }

            return -1;
        }
    }
}
