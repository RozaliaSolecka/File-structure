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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;


namespace PT_laboratorium8
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog()
            {
                Description = "Select directory to open"
            };

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                treeView.Items.Clear();
                DirectoryInfo directoryPath = new DirectoryInfo(dlg.SelectedPath);
                var root = DirectoryTree(directoryPath);
                treeView.Items.Add(root);

            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void deleteDirectory(string path)
        {
            DirectoryInfo directories = new DirectoryInfo(path);

            FileAttributes attributes = File.GetAttributes(path);
            FileAttributes newAttributes = attributes & ~FileAttributes.ReadOnly;
            File.SetAttributes(path, newAttributes);

            foreach (var directory in directories.GetDirectories())
            {
                deleteDirectory(directory.FullName);
            }
            foreach (var file in directories.GetFiles())
            {
                string filePath = (string)file.FullName;
                FileAttributes fileAttributes = File.GetAttributes(filePath);
                FileAttributes newFileAttributes = fileAttributes & ~FileAttributes.ReadOnly;
                File.SetAttributes(filePath, newFileAttributes);

                File.Delete(file.FullName);
            }

            Directory.Delete(path);
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (TreeViewItem)treeView.SelectedItem;

            string path = (string)selectedItem.Tag;
            FileAttributes attributes = File.GetAttributes(path);
            FileAttributes newAttributes = attributes & ~FileAttributes.ReadOnly;
            File.SetAttributes(path, newAttributes);
            TreeViewItem TWI = (TreeViewItem)treeView.Items[0];

            // usuwanie z drzewa
            if ((TreeViewItem)treeView.Items[0] != selectedItem)    // jezeli wybrany element nie jest korzeniem
            {
                TreeViewItem parent = (TreeViewItem)selectedItem.Parent;
                parent.Items.Remove(selectedItem);

            }
            else
            {
                treeView.Items.Clear();
            }

            // usuwanie z dysku
            if (Directory.Exists(path))
            {
                deleteDirectory(path);
            }
            else if(File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private void CreateClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (TreeViewItem)treeView.SelectedItem;
            string path = (string)selectedItem.Tag;
            Dialog dialog = new Dialog(path);
            dialog.ShowDialog();
            if (path != dialog.getPath())
            {
                if (dialog.ifDir())   // folder
                {
                    DirectoryInfo directory = new DirectoryInfo(dialog.getPath());
                    selectedItem.Items.Add(DirectoryTree(directory));
                }
                else
                {
                    FileInfo file = new FileInfo(dialog.getPath());
                    selectedItem.Items.Add(FileTree(file));
                }
            }
        }

        private void OpenClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (TreeViewItem)treeView.SelectedItem;
            string path = (string)selectedItem.Tag;
            string text = File.ReadAllText(path);
            textBlockforContent.Text = text;

        }

        private void addFileContextMenu(TreeViewItem item)
        {
            item.ContextMenu = new System.Windows.Controls.ContextMenu();
            var delete = new System.Windows.Controls.MenuItem();
            delete.Header = "Delete";
            delete.Click += new RoutedEventHandler(this.DeleteClick);
            item.ContextMenu.Items.Add(delete);

            var open = new System.Windows.Controls.MenuItem();
            open.Header = "Open";
            open.Click += new RoutedEventHandler(this.OpenClick);
            item.ContextMenu.Items.Add(open);

        }
        private void addDirectoryContextMenu(TreeViewItem item)
        {
            item.ContextMenu = new System.Windows.Controls.ContextMenu();
            var delete = new System.Windows.Controls.MenuItem();
            delete.Header = "Delete";
            delete.Click += new RoutedEventHandler(this.DeleteClick);
            item.ContextMenu.Items.Add(delete);

            var create = new System.Windows.Controls.MenuItem();
            create.Header = "Create";
            create.Click += new RoutedEventHandler(this.CreateClick);
            item.ContextMenu.Items.Add(create);

        }

        private TreeViewItem FileTree(FileInfo path)
        {
            var item = new TreeViewItem
            {
                Header = path.Name,
                Tag = path.FullName
            };

            addFileContextMenu(item);   // dodanie menu
            item.Selected += new RoutedEventHandler(this.dosAttribute);  // atrybuty dosowe

            return item;

        }

        private TreeViewItem DirectoryTree(DirectoryInfo path)
        {
            var root = new TreeViewItem
            {
                Header = path.Name,
                Tag = path.FullName
            };

            addDirectoryContextMenu(root);   //dodanie menu

            foreach (DirectoryInfo subDirectory in path.GetDirectories())
            {
                root.Items.Add(DirectoryTree(subDirectory));
            }
            foreach (FileInfo file in path.GetFiles())
            {
                root.Items.Add(FileTree(file));
            }
            root.Selected += new RoutedEventHandler(this.dosAttribute);
            return root;
        }


        private void dosAttribute(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)treeView.SelectedItem;
            FileAttributes attributes = File.GetAttributes((string)item.Tag);
            DOS.Text = "";
            string rahs = "";

            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                rahs += 'r';
            }
            else
            {
                rahs += '-';
            }
            if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
            {
                rahs += 'a';
            }
            else
            {
                rahs += '-';
            }
            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                rahs += 'h';
            }
            else
            {
                rahs += '-';
            }
            if ((attributes & FileAttributes.System) == FileAttributes.System)
            {
                rahs += 's';
            }
            else
            {
                rahs += '-';
            }
            DOS.Text += rahs;

        }
    }
}