using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace DS3_Save_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string dsSaveFolder;

        Button selected;
        
        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists(".\\config.txt")) File.Create(".\\config.txt").Close();
            if (File.ReadAllText(".\\config.txt").Length != 0 && File.ReadAllLines(".\\config.txt")[0].Length != 0)
            {
                dsSaveFolder = File.ReadAllLines(".\\config.txt")[0];
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("You do not have a config set up yet!\nThis contains the name of your steam directory, unique to your steam account.\nBy default, I will pick the last used one. If you want to choose a different one, cancel this message.\nBe aware this program wont work unless you set it yourself or press 'OK'",
                "No config!", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    string[] dirs = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DarkSoulsIII");
                    if (dirs.Length == 0) 
                    {

                    }
                    else if (dirs.Length == 1)
                    {
                        dsSaveFolder = dirs[0];
                        File.WriteAllText(".\\config.txt", dsSaveFolder);
                    }
                    else
                    {
                        int newestIndex = 0;
                        for(int i = 0; i < dirs.Length; ++i)
                        {
                            if (File.GetLastWriteTimeUtc(Directory.GetFiles(dirs[i])[0]).ToFileTimeUtc() > 
                            File.GetLastWriteTimeUtc(Directory.GetFiles(dirs[newestIndex])[0]).ToFileTimeUtc()) newestIndex = i;
                        }
                        dsSaveFolder = dirs[newestIndex];
                        File.WriteAllText(".\\config.txt", dsSaveFolder);
                    }

                    Directory.CreateDirectory(dsSaveFolder + "\\DS3Manager");
                }
            }
            List<Button> saves = new List<Button>();
            string[] saveDirs  = Directory.GetDirectories(dsSaveFolder + "\\DS3Manager");

            for(int i = 0; i < saveDirs.Length; ++i)
            {
                Button button = new Button();
                button.Height = 100;
                button.Background = Brushes.LightGray;
                button.Click += saveSelect;
                button.Content = saveDirs[i].Remove(0, dsSaveFolder.Length + 12);
                saveList.Children.Add(button);
            }
        }
        void onConfirm(object sender, RoutedEventArgs e)
        {
            if (input.Text.Length == 0) return;
            if (Directory.GetDirectories(dsSaveFolder + "\\DS3Manager").Any(x => x == dsSaveFolder + "\\DS3Manager\\" + input.Text)) return;
            Button button = new Button();
            button.Height = 100;
            button.Background = Brushes.LightGray;
            button.Click += saveSelect;
            button.Content = input.Text;
            saveList.Children.Add(button);
            Directory.CreateDirectory(dsSaveFolder + "\\DS3Manager\\" + input.Text);
        }
        void onDelete(object sender, RoutedEventArgs e)
        {
            if (selected.Content.ToString().Length == 0) return;
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this save? This action is irreversible.", "Deleting save", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) 
            {
                Directory.Delete(dsSaveFolder + "\\DS3Manager\\" + selected.Content.ToString(), true);
                saveList.Children.Remove(selected as UIElement);
                MessageBox.Show("Your save was deleted successfully", "Success!", MessageBoxButton.OK);
            }
        }
        void saveSelect(object sender, RoutedEventArgs e)
        {
            foreach(UIElement el in saveList.Children) (el as Button).Background = Brushes.LightGray;
            Button button = sender as Button;
            button.Background = Brushes.DarkRed;
            selected = button;
        }
        void onClear(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to clear your current save file? This action is irreversible.", "Deleting save", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) 
            {
                File.Delete(dsSaveFolder + "\\DS30000.sl2");
                MessageBox.Show("Your save was deleted successfully", "Success!", MessageBoxButton.OK);
            }
        }
        void onSave(object sender, RoutedEventArgs e)
        {
            File.Copy(dsSaveFolder + "\\DS30000.sl2", dsSaveFolder + "\\DS3Manager\\" + selected.Content.ToString() + "\\DS30000.sl2", true);
            MessageBox.Show("Saved current save file to \"" + selected.Content.ToString() + "\"", "Success!", MessageBoxButton.OK);
        }
        void onLoad(object sender, RoutedEventArgs e)
        {
            File.Copy(dsSaveFolder + "\\DS3Manager\\" + selected.Content.ToString() + "\\DS30000.sl2", dsSaveFolder + "\\DS30000.sl2", true);
            MessageBox.Show("Loaded save file from \"" + selected.Content.ToString() + "\"", "Success!", MessageBoxButton.OK);
        }

        string getLines(string[] lines)
        {
            string build = lines[0];
            for(int i = 1; i < lines.Length; ++i) build += "\n" + lines[i];
            return build;
        }
    }
}
