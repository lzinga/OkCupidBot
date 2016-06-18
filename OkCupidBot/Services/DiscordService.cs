using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using Discord;
using System.Text.RegularExpressions;
using Discord.Commands;

namespace OkCupidBot.Services
{
    public class DiscordService : BaseService
    {
        public const long ChannelOne = 193453248336756736;
        public const long ChannelTwo = 167049576006680576;

        public struct UserMessage
        {
            public ulong Id;
            public string Mention;
            public string User;
            public string Message;
        }

        private DiscordClient _client;
        private Channel channel;
        public bool ReceiveMessages = false;
        public List<UserMessage> UserMessages { get; private set; }

        public DiscordService()
        {
            DiscordConfigBuilder config = new DiscordConfigBuilder();

            _client = new DiscordClient();
            _client.Connect(ServiceManager.Services.ArgumentService.Arguments.DiscordUsername, ServiceManager.Services.ArgumentService.Arguments.DiscordPassword);

            int i = 0;
            while(_client.State != ConnectionState.Connected)
            {
                if(i == 0)
                    Services.LogService.WriteLine("Connecting to Discord");

                i = 1;
            }

            // As long as channel is null keep trying to set it.
            while(channel == null)
            {
                channel = _client.GetChannel(ChannelOne);
            }

            Services.LogService.WriteLine("Connected to channel: {0}", channel.Name);

            UserMessages = new List<UserMessage>();

            this.SendMessage("OkCupid Bot Successfully Connected!");

            _client.MessageReceived += _client_MessageReceived;
        }

        public void SendMessage(string message)
        {
            if (_client.State != ConnectionState.Connected)
            {
                return;
            }

            channel.SendMessage(message);
        }
 

        public string UserMention(ulong id)
        {
            return channel.GetUser(id).Mention;
        }

        public void GetUserMessages()
        {
            ReceiveMessages = true;
            UserMessages.Clear();
        }

        private void _client_MessageReceived(object sender, MessageEventArgs e)
        {
            if(!ReceiveMessages)
            {
                return;
            }

            if(string.IsNullOrEmpty(e.Message.Text))
            {
                return;
            }

            if(e.Message.Text.Contains("Nigger") || e.Message.Text.Contains("nigger"))
            {
                return;
            }

            Match rgx = Regex.Match(e.Message.Text, @"\/\bokcm +(.*)", RegexOptions.IgnoreCase);
            if (rgx.Success)
            {
                string message = rgx.Groups[1].Value;
                
                // If the user already has a message inserted, don't allow another.
                if(UserMessages.Any(i => i.User == e.User.Name))
                {
                    return;
                }

                UserMessages.Add(new UserMessage() { Id = e.User.Id, User = e.User.Name, Message = message.Trim(), Mention = e.User.Mention });
            }

            
        }
    }
}
