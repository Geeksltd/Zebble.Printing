[logo]: https://raw.githubusercontent.com/Geeksltd/Zebble.Printing/master/icon.png "Zebble.Printing"

## Zebble

![logo]

A Zebble plugin for signing with Firebase Auth.

[![NuGet](https://img.shields.io/nuget/v/Zebble.Printing.svg?label=NuGet)](https://www.nuget.org/packages/Zebble.Printing/)

> With this plugin you can get information from the user of Firebase Auth like email in your Zebble application and it is available on all platforms.

<br />

### Setup
* Available on NuGet: [https://www.nuget.org/packages/Zebble.Printing/](https://www.nuget.org/packages/Zebble.Printing/)
* Install in your platform client projects.
* Available for iOS, Android and UWP.

<br />

### Initialization
First of all, you need to initialize this plugin by calling `Printing.Current.Initialize` method. To do so, make a call to it on your project startup, and feed it by your Firebase Web API key. It's recommended to store the key in your Zebble's `\Resources\Config.xml` file.

```csharp
class StartUp
{
    public static Task Run()
    {
        Printing.Current.Initialize(Config.Get("Firebase.ApiToken"));

        // TODO: Any required init
        Zebble.Mvvm.ViewModel.Go<WelcomePage>();
        return Task.CompletedTask;
    }
}
```

#### How can I acquire my Firebase Auth key?
You need to set up a Google Firebase Auth app in your console. To create a new project, open your [Firebase console](https://console.firebase.google.com/), click on Add project, enter a name, and then click on the Create Project button. After that, you've to add an Authentication app to your project. To do so, on the right side of the project's overview page, head to `Develop > Authentication > Get Started`. In this step, you can configure your app to use one or more of the available sign-in providers. Click on Email/Password, turn the Enable switch on, and click on the Save button. Finally, you have to copy your API token by heading to `Project settings > General > Web API Key` and then add it to your `Config.xml` file as follow:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<settings>
  ...
  <Firebase.ApiToken value="<YOUR_API_TOKEN>" />
</settings>
```

<br />

### API Usage
After initializing the plugin, you can use the following APIs.

#### Register
This will create a new user account under your Firebase Auth app. For calling this async method, you need to provide a pair of email and password.

```csharp
class LoginPage : FullScreen
{
    public readonly Bindable<string> Email = new Bindable<string>();
    public readonly Bindable<string> Password = new Bindable<string>();

    public async Task TapLogin()
    {
        var result = await Printing.Current.Register(Email.Value, Password.Value);

        if (result.Succeeded)
            Go<WelcomePage>();
        else
            Dialog.Alert($"Register failed: {result.Message} ({result.Code})");
    }
}
```

#### Login
This will send the provided credentials to Firebase and asks it to get an auth token. For calling this async method, you need to provide a pair of valid and existing email and password.

```csharp
class LoginPage : FullScreen
{
    public readonly Bindable<string> Email = new Bindable<string>();
    public readonly Bindable<string> Password = new Bindable<string>();

    public async Task TapLogin()
    {
        var result = await Printing.Current.Login(Email.Value, Password.Value);

        if (result.Succeeded)
            Go<WelcomePage>();
        else
            Dialog.Alert($"Login failed: {result.Message} ({result.Code})");
    }
}
```

##### Note

Both of above methods will store the created or authenticated user info internally for further usage. 

#### RefreshTokenExpiry
It's obvious that the persisted user session will be expired after a period of time. So you should use this method to validate the persisted auth token. This will automatically revoke the token if it's already expired, otherwise the token expiration will be expanded. It's recommended to call this method prior to  your app launch. 

```csharp
class SplashPage : FullScreen
{
    protected async override Task NavigationStartedAsync()
    {
        var isValid = await Printing.Current.RefreshTokenExpiry();

        if (isValid)
            Go<HomePage>();
        else
            Go<LoginPage>();
    }
}
```

#### GetUser
To get user details, call this method.

```csharp
class HomePage : FullScreen
{
    public async Task TapShowEmail()
    {
        var user = await Printing.Current.GetUser();

        Dialog.Alert(`User email: {user.Email}`);
    }
}
```

#### IsAnonymous
To determine if user is anonymous, call this method.

```csharp
class HomePage : FullScreen
{
    public async Task TapGoToLoginIfRequired()
    {
        if (await Printing.Current.IsAnonymous())
            Go<LoginPage>();
    }
}
```

#### IsAuthenticated
To determine if user is logged in, call this method.

```csharp
class HomePage : FullScreen
{
    public async Task TapSecurePage()
    {
        if (await Printing.Current.IsAuthenticated())
            Go<SecurePage>();
    }
}
```

#### Logout
To log out the user, call this method. It will purge the internally stored state of the user.

```csharp
class HomePage : FullScreen
{
    public async Task TapLogout()
    {
        await Printing.Current.Logout();
        
        Go<LoginPage>(PageTransition.SlideBack);
    }
}
```
