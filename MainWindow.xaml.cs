using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;

namespace ModernDesktopApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private int _actionCounter = 0;

    public MainWindow()
    {
        InitializeComponent();
        AddAction("Application started");
    }

    private void SayHelloButton_Click(object sender, RoutedEventArgs e)
    {
        string name = NameInput.Text.Trim();
        
        if (string.IsNullOrEmpty(name))
        {
            OutputText.Text = "Please enter your name first!";
            OutputText.Foreground = new SolidColorBrush(Color.FromRgb(220, 53, 69)); // Red color
            AddAction("Validation error: Empty name field");
        }
        else
        {
            OutputText.Text = $"Hello, {name}! Welcome to .NET 8 WPF! 👋";
            OutputText.Foreground = new SolidColorBrush(Color.FromRgb(0, 120, 212)); // Blue color
            AddAction($"Greeted user: {name}");
        }
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        NameInput.Text = string.Empty;
        OutputText.Text = string.Empty;
        AddAction("Form cleared");
        NameInput.Focus();
    }

    private void AddAction(string action)
    {
        _actionCounter++;
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        string logEntry = $"[{timestamp}] #{_actionCounter}: {action}";
        
        ActionsList.Items.Insert(0, logEntry);
        
        // Keep only the last 10 actions
        while (ActionsList.Items.Count > 10)
        {
            ActionsList.Items.RemoveAt(ActionsList.Items.Count - 1);
        }
    }

    private void OpenDatabaseButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var dbWindow = new DatabaseWindow();
            dbWindow.Show();
            AddAction("Opened FlexDesk Database Manager");
        }
        catch (Exception ex)
        {
            OutputText.Text = $"Error opening database: {ex.Message}";
            OutputText.Foreground = new SolidColorBrush(Color.FromRgb(220, 53, 69));
            AddAction($"Database error: {ex.Message}");
        }
    }
}
