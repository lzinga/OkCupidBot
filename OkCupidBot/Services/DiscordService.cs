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

namespace OkCupidBot.Services
{
    public class DiscordService : BaseService
    {
        public struct UserMessage
        {
            public string User;
            public string Message;
        }

        private DiscordClient _client;
        private Channel channel;
        public bool ReceiveMessages = false;
        public List<UserMessage> UserMessages { get; private set; }

        public DiscordService()
        {
            _client = new DiscordClient();
            _client.Connect(ServiceManager.Services.ArgumentService.Arguments.DiscordUsername, ServiceManager.Services.ArgumentService.Arguments.DiscordPassword);

            while(_client.State != ConnectionState.Connecting)
            {
                Services.LogService.WriteLine("Connecting to Discord");
            }

            channel = _client.GetChannel(167049576006680576);
            UserMessages = new List<UserMessage>();
        }

        public void SendMessage(string message)
        {
            if (_client.State != ConnectionState.Connected)
            {
                return;
            }

            channel.SendMessage(message);
        }


        public void GetUserMessages()
        {
            ReceiveMessages = true;
            UserMessages.Clear();

            
            _client.MessageReceived += _client_MessageReceived;
        }

        private void _client_MessageReceived(object sender, MessageEventArgs e)
        {
            if(!ReceiveMessages)
            {
                return;
            }

            Match rgx = Regex.Match(e.Message.Text, @"\/\bOkCupidMessage +(.*)", RegexOptions.IgnoreCase);

            if (rgx.Success)
            {
                string message = rgx.Groups[1].Value;
                UserMessages.Add(new UserMessage() { User = e.User.Name, Message = message.Trim() });
            }

            
        }
    }
}
