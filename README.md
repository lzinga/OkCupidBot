# OkCupidBot
I made this as an experiment a while back. It has been through quite a few iterations but this is probably the most solid one. Since things tend to change on OkCupid quite often it is bound to break every now and then.

1/14/2017 - I haven't updated this in a while so it is bound to be broken at this point. Just did some minor updates since I planned to make the repo public.

# Arguments
```csharp
// Pauses after every 'action'
/Debug

// Username to be used when logging into okcupid.
/Username="{username}"

// Password to be used when logging into okcupid.
/Password="{password}"

// The conditions and requirements for who to send a message to.
// File must be inside /Config folder
/ProfileSettingsFileName="DefaultProfileSettings.xml"

// Config file to determine which message should be sent based on special cases.
// File must be inside /Config folder
/MessageSettingsFileName="DefaultMessages.xml"

// The discord account to login as. (I believe there are special bot accounts that can be used,
// so this might not be needed anymore)
/DiscordUsername=""
/DiscordPassword=""

// I don't full remember what this one was for.
// I believe it is what is used to detect what the person wants to send from discord.
// For example if DiscordMessage was "/okm"
// anyone in discord would have to write "/okm Hello sexy, what is your favorite thing to do"
// It would then detect that as a message wanting to be sent to the person.
/DiscordMessage=""
```

# Profile Settings Xml
```xml
<?xml version="1.0" encoding="utf-16"?>
<ProfileSettings>
  
  <Setting>
    <!-- All conditions must be met for requirements to be checked. -->
    <Conditions>
      <Condition Field="MatchPercent" From="0" To="49" />
    </Conditions>
    
    <!-- Requirements are specific profile settings that must be true to send a message. -->
    <!-- In the below example it will not send a message if BodyType does not equal Curvy or Fit -->
    <Requirements>
      <Requirement Key="BodyType" Operator="Equals" Value="Curvy" />
      <Requirement Key="BodyType" Operator="Equals" Value="Fit" />
    </Requirements>
  </Setting>
  
  <!-- You can add another <Setting> to do different requirements if MatchPercent is from 50 to 100. -->
  
</ProfileSettings>
```

# Messages Xml
Most advanced conditions should be at the top and smallest conditions should be at the bottom. It will fall through to each MessageGroup below it and check all conditions. So it will add all messages as long as the conditions are met. Adding a MessageGroup with no conditions will make all messages in it be available.

Inside the messages you can access variables of the users Profile class (will be updating a wiki probably of all available properties. Eventually you will also be able to access Weather and so on. So if you wanted to access the users height you would do {Profile.Height.Feet} which would insert the value.

For the condition key if you wanted to make sure they were above 5 feet you could do the following:
```xml
<Condition Key="Profile.Height.Feet" Operator="GreaterThanOrEqualTo" Value="5" />
```

```xml
<?xml version="1.0" encoding="utf-16"?>
<MessageSettings>

  <MessageGroup>
    <Conditions>
      <Condition Key="Profile.IsOnline" Operator="Equals" Value="true" />
    </Conditions>
    <Message Value="Hello {Profile.Username}, how are you doing tonight." />
  </MessageGroup>

  <MessageGroup>
    <Conditions />
    <Message Value="Quick! the most recent movie you have watched and what you thought of it!" />
    <Message Value="If you had to survive on a desert island for 5 years and you could only bring one thing, what would you bring?: 1. Machete 2. A book (what book?) 3. A volleyball 4. Hatchet" />
    <Message Value="Hey! What are you doing right this second! Other than reading my message!" />
    <Message Value="I have been running out of movies to watch on Netflix. Would you have any good suggestions? How about your favorite?" />
  </MessageGroup>

</MessageSettings>
```



# Libraries
1. [Selenium WebDriver](https://www.nuget.org/packages/Selenium.WebDriver/)
2. [Selenium WebDriver Support Classes](https://www.nuget.org/packages/Selenium.Support/)
3. [Discord.Net](https://github.com/RogueException/Discord.Net)

# Disclaimer
1. If you use this there is a chance your account can be banned. Use at your own risk, I have put in safe measures to not send mass messages to quickly in succession. However if you run it non stop I am sure OkCupid would recognize that something is going on.
2. You accept all risk when running OkCupidBot and any liability.
