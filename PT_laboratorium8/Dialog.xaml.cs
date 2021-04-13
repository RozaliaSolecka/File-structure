using System;
using System.Collections.Generic;
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
using System.Text.RegularExpressions;
using System.IO;

namespace PT_laboratorium8
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class Dialog : Window
    {
        private string path;
        private bool ifDirectory = false;
        public Dialog(string path)
        {
            InitializeComponent();
            this.path = path;
        }

        private void cancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void okButtonClick(object sender, RoutedEventArgs e)
        {
            if ((bool)fileButton.IsChecked && !Regex.IsMatch(nameBox.Text, "^[a-zA-Z0-9_~-]{1,8}\\.(txt|php|html)$"))
            {
                System.Windows.MessageBox.Show("Error. Incorrect type of file or name");
            }
            else if (!(bool)fileButton.IsChecked && !Regex.IsMatch(nameBox.Text, "^[a-zA-Z0-9_~-]{1,8}$"))
            {
                System.Windows.MessageBox.Show("Error. Incorrect type of file or name");
            }
            else if (!(bool)fileButton.IsChecked && !(bool)directoryButton.IsChecked)
            {
                System.Windows.MessageBox.Show("No type");
            }
            else
            {
                path += "\\";
                path += nameBox.Text;
                FileAttributes fileAttributes = FileAttributes.Normal;

                if ((bool)readOnly.IsChecked)
                {
                    fileAttributes |= FileAttributes.ReadOnly;
                }
                if ((bool)archive.IsChecked)
                {
                    fileAttributes |= FileAttributes.Archive;
                }
                if ((bool)hidden.IsChecked)
                {
                    fileAttributes |= FileAttributes.Hidden;
                }
                if ((bool)system.IsChecked)
                {
                    fileAttributes |= FileAttributes.System;
                }
                if ((bool)fileButton.IsChecked)
                {
                    File.Create(path);
                }
                else if ((bool)directoryButton.IsChecked)
                {
                    Directory.CreateDirectory(path);
                }
                File.SetAttributes(path, fileAttributes);
                Close();
            }
            if ((bool)directoryButton.IsChecked)
            {
                ifDirectory = true;
            }
            else
            {
                ifDirectory = false;
            }
        }

        public string getPath()
        {
            return path;
        }

        public bool ifDir()
        {
            return ifDirectory;
        }
    }
}