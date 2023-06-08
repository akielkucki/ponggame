using System.Windows;

namespace PongGame
{
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
            
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            
        new MainWindow().Show();
        Close();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the settings window at the same position as the current window
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Left = Left + (Width - settingsWindow.Width) / 2;
            settingsWindow.Top = Top + (Height - settingsWindow.Height) / 2;
            settingsWindow.ShowDialog();
        }


    }
}