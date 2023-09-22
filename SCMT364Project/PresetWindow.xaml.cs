using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace SCMT364Project
{
    /// <summary>
    /// Interaction logic for PresetWindow.xaml
    /// </summary>
    public partial class PresetWindow : Window
    {
        public PresetWindow(Button button, string text)
        {
            //    MessageBox.Show("Text: " + text);
            //if (text.Trim() == String.Empty)
            //{
            //    MessageBox.Show("No data to save, returning to main window");
            //    Close();

            //}
            reload = button;
            fileText = text;

            // On load, we need to populate lbxPresets with the names of the files in the Load/preset_list file
            InitializeComponent();
            loadPresets();
        }
        string fileText = string.Empty;
        string presetListPath = @"Load/preset_list.txt";
        Button reload = new Button();
      

        /// <summary>
        /// Both close the form and communicate with the main window that it needs to refresh its presets
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to exit the Preset Window?", "Preset Window", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Close();
            }

        }
        /// <summary>
        /// fileText should be like:
        /// "A\n15\n_\nB\n20\nA\N
        /// </summary>
        /// <param name="filePath"> ex. @"Presets/presetNew.txt"</param>
        private void createNewFile(string newFilePath)
        {
            using StreamWriter sw = new StreamWriter(newFilePath);

            sw.Write(fileText);
            sw.Close();
        }

        /// <summary>
        /// Add normal fileName to list of all presets
        /// </summary>
        /// <param name="newFilePath"> ex. @"Load/</param>
        private bool addFileNameToList(string newFileName)
        {
            // Form array of current filenames
            List<string> currFiles = File.ReadAllLines(presetListPath).ToList();
            // Add new filename to array (no .txt)
            if (currFiles.Contains(newFileName))
            {
                MessageBox.Show("Similar filename was already in list, please delete file before adding it");
                txtTitle.Focus();
                return false;
            }
            currFiles.Add(newFileName);
            // Copy new array to current filenames
            File.WriteAllText(presetListPath, String.Empty);
            using StreamWriter presetListFile = new StreamWriter(presetListPath);
            foreach (string fileName in currFiles)
            {
                if (fileName.Length != 0)
                    presetListFile.Write(fileName + '\n');
            }
            presetListFile.Close();
            return true;
        }

        /// <summary>
        /// delete the file
        /// </summary>
        /// <param name="filePath"> ex. @"Presets/preset1.txt"</param>
        private void deleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        /// <summary>
        /// Remove the fileName from the list of presets
        /// </summary>
        /// <param name="deleteFileName"> ex. "preset1" </param>
        private void removeFileFromList(string deleteFileName)
        {
            // Form array of current filenames
            List<string> currFiles = File.ReadAllLines(presetListPath).ToList();
            // Add new filename to array (no .txt)
            currFiles.Remove(deleteFileName);
            // Copy new array to current filenames
            File.WriteAllText(presetListPath, String.Empty);
            using StreamWriter presetListFile = new StreamWriter(presetListPath);
            foreach (string fileName in currFiles)
            {
                if (fileName.Length != 0)
                    presetListFile.Write(fileName + '\n');
            }
            presetListFile.Close();
        }
        private void loadPresets()
        {
            lbxPresets.Items.Clear();
            string[] presets = File.ReadAllLines(presetListPath);
            foreach (string preset in presets)
            {
                //MessageBox.Show("Adding: " + preset + "(" + preset.Length + ")");
                if (preset.Length != 0)
                    lbxPresets.Items.Add(preset);
            }

        }
        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            title.Visibility = Visibility.Visible;
            Title.Visibility = Visibility.Visible;
            save.Visibility = Visibility.Visible;
            txtTitle.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            /**
             * Write the fileText to 
             * 1) [filename].txt
             * 2) preset_list.txt
             */
            Validator v = new Validator();
            if (txtTitle.Text == String.Empty)
            {
                MessageBox.Show("Please input a file name");
                return;
            }
            if (!v.isLength(15, txtTitle))
            {
                MessageBox.Show("Please input less than 16 characters");
                return;
            }
            if (!v.isAlphaNumeric(txtTitle))
            {
                MessageBox.Show("Please input only alphanumeric characters");
                return;
            }

            // Saves the Preset file
            string pathPresetFolder = @$"Presets/{txtTitle.Text.ToLower()}.txt";

            if (addFileNameToList(txtTitle.Text.Trim().ToLower()))
            {
                createNewFile(pathPresetFolder);
                reload.Background = Brushes.PaleVioletRed;
                loadPresets();
            }
            //addFileNameToList(txtTitle.Text.ToLower());

            //MessageBox.Show($"{pathPresetFolder} was added to folder");

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lbxPresets.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a preset from the list", "Input Error");
                lbxPresets.Focus();
                return;
            }
            if (lbxPresets.SelectedItem == null)
            {
                MessageBox.Show("SelectedIitem was null");
                return;
                //MessageBox.Show($"{deleteFile} will be deleted");

            }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string deleteFileName = lbxPresets.SelectedItem.ToString();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            if (MessageBox.Show($"Are you sure you want to delete: {deleteFileName} permanently from the list?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // Delete actual file from Preset Folder
                string deletePath = $@"Presets/{deleteFileName}.txt";
                deleteFile(deletePath);
#pragma warning disable CS8604 // Possible null reference argument.
                removeFileFromList(deleteFileName);
#pragma warning restore CS8604 // Possible null reference argument.
                // Delete file mention in Load Folder
                loadPresets();
                reload.Background = Brushes.PaleVioletRed;
            }
        }
    }
}
