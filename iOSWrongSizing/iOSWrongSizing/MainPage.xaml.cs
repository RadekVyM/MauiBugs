namespace iOSWrongSizing;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void ButtonClicked(object sender, EventArgs e)
    {
        if (variableLabel.WidthRequest == 100)
            variableLabel.WidthRequest = 200;
        else
            variableLabel.WidthRequest = 100;
    }
}

