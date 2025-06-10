namespace SkinAnalyzerApp.Views;

public partial class MessageModalPage : ContentPage
{
    public MessageModalPage(string mensaje)
    {
        InitializeComponent();
        MessageLabel.Text = mensaje;
    }

    private async void OnAcceptClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
