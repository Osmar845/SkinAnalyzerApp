namespace SkinAnalyzerApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent(); // Muy importante
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///RegisterPage");
    }
}
