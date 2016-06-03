# OkCupidBot
There actually have been 2 other versions of this kind of program, and this is the cleanest and most advanced one which I decided to upload and document.

### First Version
The first one was made using a different library. It would crash all the time and would fail to load the browser because one already existed and you would have to end process a hidden internet explorer. All in all it wasn't the best library and caused all kinds of issues.

### Second Version
Second one was using the same library as this but it was a complete mess of code and didn't have proper encapsulation and had things all in one big method for the most part. So it was just this uploaded version, but a huge mess.

### Third Version ( this repo )
Now we are on this one that I will probably stick with, the library is solid and allows for a lot of customization and allows me to use any type of browser (however it is hardcoded right now to use firefox).

# Arguments
```csharp
// Pauses after every 'action'
/Debug

// Username to be used when logging into okcupid.
/Username="{username}"

// Password to be used when logging into okcupid.
/Password="{password}"

// The settings file to be used.
// File must be inside /Config folder
/ProfileSettingsFileName="DefaultProfileSettings.xml"
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

#region 

# Libraries
1. [Selenium WebDriver](https://www.nuget.org/packages/Selenium.WebDriver/)
2. [Selenium WebDriver Support Classes](https://www.nuget.org/packages/Selenium.Support/)

# Disclaimer
1. If you use this there is a chance your account can be banned. Use at your own risk, I have put in safe measures to not send mass messages to quickly in succession. However if you run it non stop I am sure OkCupid would recognize that something is going on.
2. You accept all risk when running OkCupidBot and any liability.
