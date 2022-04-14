using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Ellipse_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text.Length == 0)
            {
                SearchBox.Text = "Поиск";
            }
        }

        private void CreateBoxFName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CreateBoxFName.Text.Length == 0)
            {
                CreateBoxFName.Text = "Имя";
            }
        }

        private void CreateBoxFName_GotFocus(object sender, RoutedEventArgs e)
        {
            CreateBoxFName.Text = "";
        }

        private void CreateBoxL_GotFocus(object sender, RoutedEventArgs e)
        {
            CreateBoxLName.Text = "";
        }

        private void CreateBoxL_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CreateBoxLName.Text.Length == 0)
            {
                CreateBoxLName.Text = "Фамилия";
            }
        }

        private void CreateBoxAge_LostFocus(object sender, RoutedEventArgs e)
        {


            if (CreateBoxAge.Text.Length == 0)
            {
                CreateBoxAge.Text = "Возраст";
            }
        }

        private void CreateBoxAge_GotFocus(object sender, RoutedEventArgs e)
        {
            CreateBoxAge.Text = "";
        }

        private async void RefreshItems()
        {
            var client = App.HttpClient;

            var response = await client.GetAsync("https://localhost:7207/api/User");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<IEnumerable<User>>(
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                ContentData.ItemsSource = content;

            }

            else
            {
                MessageBox.Show("Произошла ошибка. Попробуйте позже");
            }
        }

        private void LoadListUsers_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshItems();
        }

        private void OpenAddMenu(object sender, MouseButtonEventArgs e)
        {
            AddMenu.Visibility = Visibility.Visible;
        }

        private async void SaveChanges(object sender, MouseButtonEventArgs e)
        {
            var client = App.HttpClient;

            var newUser = new User
            {
                FirstName = CreateBoxFName.Text,
                LastName = CreateBoxLName.Text,
                Age = int.Parse(CreateBoxAge.Text)
            };

            var sendContent = JsonContent.Create(newUser);

            var response = await client.PostAsync("https://localhost:7207/api/User", sendContent);

            if (response.IsSuccessStatusCode)
            {
                RefreshItems();


            }

            else
            {
                MessageBox.Show("Произошла ошибка. Попробуйте позже");
            }
        }

        private void HideAddMenu(object sender, MouseButtonEventArgs e)
        {
            AddMenu.Visibility = Visibility.Hidden;
        }

        private void SetParams(object sender, MouseButtonEventArgs e)
        {
        }

        private void ContentData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var template = ContentData.SelectedItem == null ? "0 | Имя | Фамилия | Возраст" :ContentData.SelectedItem.ToString();
            IdUser.Text = template.Split(" | ")[0];
            FNameUser.Text = template.Split(" | ")[1];
            LNameUser.Text = template.Split(" | ")[2];
            AgeUser.Text = "Возраст: " + template.Split(" | ")[3];
        }

        private async void PutChanges(object sender, MouseButtonEventArgs e)
        {
            var client = App.HttpClient;

            var newUser = new User
            {
                FirstName = EditFName.Text,
                LastName = EditLName.Text,
                Age = int.Parse(EditAge.Text)
            };

            var sendContent = JsonContent.Create(newUser);

            var response = await client.PutAsync($"https://localhost:7207/api/User/{IdUser.Text}", sendContent);

            if (response.IsSuccessStatusCode)
            {
                EditFName.Visibility = Visibility.Hidden;
                EditLName.Visibility = Visibility.Hidden;
                EditAge.Visibility = Visibility.Hidden;

                IdUser.Visibility = Visibility.Visible;
                FNameUser.Visibility = Visibility.Visible;
                LNameUser.Visibility = Visibility.Visible;

                TextChangeuser.Text = "Изменить";

                ButtonChangeUser.MouseDown -= PutChanges;
                ButtonChangeUser.MouseDown += ChangeUser;

                RefreshItems();

            }

            else
            {
                MessageBox.Show("Произошла ошибка. Попробуйте позже");
            }
        }

        private void ChangeUser(object sender, MouseButtonEventArgs e)
        {
            if (IdUser.Text == "0")
            {
                
            }
            else
            {
                EditFName.Visibility = Visibility.Visible;
                EditLName.Visibility = Visibility.Visible;
                EditAge.Visibility = Visibility.Visible;

                FNameUser.Visibility = Visibility.Hidden;
                LNameUser.Visibility = Visibility.Hidden;

                TextChangeuser.Text = "Сохранить";
                ButtonChangeUser.MouseDown -= ChangeUser;
                ButtonChangeUser.MouseDown += PutChanges;
            }

            

           
        }

        private async void DeleteUser(object sender, MouseButtonEventArgs e)
        {
            var client = App.HttpClient;

            var response = await client.DeleteAsync($"https://localhost:7207/api/User/{IdUser.Text}");

            if (response.IsSuccessStatusCode)
            {
                RefreshItems();

            }

            else
            {
                MessageBox.Show("Произошла ошибка. Попробуйте позже");
            }
        }

        private async void FilterByFirstName(object sender, TextChangedEventArgs e)
        {
            if (SearchBox.Text.Length == 0)
            {
                RefreshItems();
            }
            else
            {
                var client = App.HttpClient;

                var response = await client.GetAsync($"https://localhost:7207/api/User/{SearchBox.Text}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadFromJsonAsync<IEnumerable<User>>(
                        new JsonSerializerOptions(JsonSerializerDefaults.Web));

                    ContentData.ItemsSource = content;

                }

                else
                {
                    MessageBox.Show("Произошла ошибка. Попробуйте позже");
                }
            }
            

        }
    }
}
