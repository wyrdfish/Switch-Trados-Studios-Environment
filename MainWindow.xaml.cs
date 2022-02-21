using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Switch_Trados_Studios_Environment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        private readonly OverwriteStudioEnvironment _overwriteStudioEnvironment = new OverwriteStudioEnvironment();

        public MainWindow()
        {
            InitializeComponent();
            _overwriteStudioEnvironment.PopulateEnvironmentDictionaries();
            _overwriteStudioEnvironment.PopulateStudioDictionaries();

            var availableStudioBuilds = _overwriteStudioEnvironment.GetListOfInstalledTradosStudioBuilds();
            
            foreach (string build in availableStudioBuilds)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Background = Brushes.White;
                comboBoxItem.Content = build;
                StudioBuildType.Items.Add(comboBoxItem);
            }
            if (availableStudioBuilds.Count == 1)
            {
                StudioBuildType.SelectedIndex = 0;
            }
            else if (availableStudioBuilds.Count == 0)
            {
                const string message = "There isn't any Trados Studio Build installed\n\nPlease Install one and restart the tool or add a custom path";
                Status.Visibility = Visibility.Visible;
                Status.Text = message;
                Status.Foreground = Brushes.DarkRed;
                SwitchEnvironmentButton.IsEnabled = false;
            }

            string environmentFolderPath = _overwriteStudioEnvironment.GetEnvironmentFolder();
            var environmentConfigFiles = Directory.GetFiles(environmentFolderPath);

            foreach (string environmentFile in environmentConfigFiles)
            {
                string fileName = new FileInfo(environmentFile).Name;
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Background = Brushes.White;
                comboBoxItem.Content = System.IO.Path.GetFileNameWithoutExtension(fileName);
                EnvironmentType.Items.Add(comboBoxItem);
            }
        }

        private void ChangeEnvironment_Click(object sender, RoutedEventArgs e)
        {
            var studioIndex = StudioBuildType.SelectedIndex;
            var environmentIndex = EnvironmentType.SelectedIndex;
            var studioProcesses = _overwriteStudioEnvironment.GetStudioProcesses();

            if (studioIndex == -1 || environmentIndex == -1)
            {
                const string message = "Select which Trados Studio build you want to change \n\nAnd what environment type you want to change to";
                Status.Visibility = Visibility.Visible;
                Status.Text = message;
                Status.Foreground = Brushes.Black;
                return;
            }

            if (studioProcesses.Length != 0)
            {
                const string message = "Trados Studio needs to be closed before changing the environment. \n\nDo you want to close it?";
                const string caption = "Close Studio Confirmation";
                var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                _overwriteStudioEnvironment.CloseStudio(studioProcesses);
            }

            _overwriteStudioEnvironment.DeleteLanguageCloudLoggedInCofig(studioIndex);

            SwitchEnvironmentButton.IsEnabled = false;
            try
            {
                string message = string.Format("Success:\n\n{0} has been changed to the {1}", StudioBuildType.Text, EnvironmentType.Text);
                _overwriteStudioEnvironment.SwitchStudiosLcEnvironment(studioIndex, environmentIndex);
                Status.Visibility = Visibility.Visible;
                Status.Text = message;
                Status.Foreground = Brushes.Green;
            }
            catch(Exception exception)
            {
                Status.Visibility = Visibility.Visible;
                Status.Text = String.Format("Failed:\n\n{0}", exception.Message);
                Status.Foreground = Brushes.OrangeRed;
            }
            SwitchEnvironmentButton.IsEnabled = true;
        }

        private void CustomLocation_Click(object sender, RoutedEventArgs e)
        {
            string customLocation = _overwriteStudioEnvironment.ChooseCustomLocation();
            if (customLocation != string.Empty)
            {
                bool customPathAdded = _overwriteStudioEnvironment.AddCustomBuildPathToDictionary(customLocation);
                if (customPathAdded)
                {
                    StudioBuildType.Items.Add(customLocation);
                }
                StudioBuildType.SelectedItem = customLocation;
                SwitchEnvironmentButton.IsEnabled = true;
            }
        }
    }
}
