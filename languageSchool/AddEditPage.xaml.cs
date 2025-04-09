using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace languageSchool
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {

        private Client _currentClient = new Client();

        private bool isNew = true;

        public AddEditPage(Client client)
        {
            InitializeComponent();

            if (client != null)
            {
                _currentClient = client;
                isNew = false;

                if (client.GenderCode == "м")
                {
                    MaleRB.IsChecked = true;
                }
                else
                {
                    FemaleRB.IsChecked = true;
                }

            }


            if (client == null)
            {
                IdTB.Visibility = Visibility.Hidden;
                IdTBox.Visibility = Visibility.Hidden;
            }

            DataContext = _currentClient;



        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            Regex regMail = new Regex(@"[a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9_-][a-zA-Z0-9_-]+");


            if (string.IsNullOrEmpty(_currentClient.FirstName))
            {
                errors.AppendLine("Поле Имя не заполнено");
            }

            if (string.IsNullOrEmpty(_currentClient.LastName))
            {
                errors.AppendLine("Поле фамилия не заполнено");
            }

            if (string.IsNullOrEmpty(_currentClient.Email))
            {
                errors.AppendLine("Поле email не заполнено");
            }

            if (string.IsNullOrEmpty(_currentClient.Phone))
            {
                errors.AppendLine("Поле телефона не заполнено");
            }

            if (string.IsNullOrEmpty(BirthdayDP.Text))
            {
                errors.AppendLine("Поле даты рождения не заполнено");
            }

            if (MaleRB.IsChecked == false && FemaleRB.IsChecked == false)
            {
                errors.AppendLine("Пол не выбран");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // проверка на правильность

            errors = new StringBuilder();

            if (_currentClient.FirstName.Length > 50 || !isValidFio(_currentClient.FirstName))
            {
                errors.AppendLine("Что-то не так с полем имени");
            }

            if (!string.IsNullOrEmpty(_currentClient.Patronymic))
            {
                if (_currentClient.Patronymic.Length > 50 || !isValidFio(_currentClient.Patronymic))
                {
                    errors.AppendLine("Поле отчество заполнено некорректно");
                }
            }

            if (_currentClient.LastName.Length > 50 || !isValidFio(_currentClient.LastName))
            {
                errors.AppendLine("Поле Фамилии заполнено некорректно");

            }

            if (!regMail.IsMatch(_currentClient.Email))
            {
                errors.AppendLine("Поле email заполнено некорректно");
            }

            if (!isValidPhone(_currentClient.Phone))
            {
                errors.AppendLine("Поле Телефон заполнено некорректно");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }



            if (MaleRB.IsChecked == true)
            {
                _currentClient.GenderCode = "м";
            }
            else
            {
                _currentClient.GenderCode = "ж";
            }

            _currentClient.Birthday = Convert.ToDateTime(BirthdayDP.Text);
            _currentClient.RegistrationDate = DateTime.Now;


            if (isNew)
            {
                try
                {
                    IbragimovDLanguageEntities.GetContext().Client.Add(_currentClient);
                    IbragimovDLanguageEntities.GetContext().SaveChanges();
                    MessageBox.Show("Информация сохранена");
                    Manager.MainFrame.GoBack();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

            }
            else
            {
                try
                {
                    IbragimovDLanguageEntities.GetContext().SaveChanges();
                    MessageBox.Show("Информация сохранена");
                    Manager.MainFrame.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }



        }

        private void PhotoPathButton_Click(object sender, RoutedEventArgs e)
        {

            string projectDirectory = GetProjectRootDirectory();
            string clientsFolderPath = System.IO.Path.Combine(projectDirectory, "Клиенты");

            // Создаем папку, если её нет
            if (!Directory.Exists(clientsFolderPath))
            {
                Directory.CreateDirectory(clientsFolderPath);
            }

            OpenFileDialog myOpenFileDialog = new OpenFileDialog
            {
                InitialDirectory = clientsFolderPath
            };

            if (myOpenFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = myOpenFileDialog.FileName;

                // Сохраняем относительный путь ОТНОСИТЕЛЬНО КОРНЯ ПРОЕКТА
                _currentClient.PhotoPath = System.IO.Path.Combine("Клиенты", System.IO.Path.GetFileName(selectedFilePath));

                // Загружаем изображение по полному пути
                ClientImage.Source = new BitmapImage(new Uri(selectedFilePath));
            }
        }

        // Метод для получения корня проекта
        private string GetProjectRootDirectory()
        {
            // Путь к исполняемому файлу (bin/Debug)
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            // Поднимаемся на 3 уровня: bin/Debug → bin → Корень проекта
            return System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(exePath)));
        }

        bool isValidFio(string str)
        {
            bool isValid = true;
            foreach (char character in str)
            {
                if (!char.IsLetter(character) && character != ' ' && character != '-') 
                {
                    isValid = false;
                }
            }
            return isValid;
        }
        bool isValidPhone(string str)
        {
            bool isValid = true;
            foreach (char character in str.Trim())
            {
                if (!char.IsDigit(character) && character != ' ' && character != '-' && character != '+' && character != '(' && character != ')')
                {
                    isValid = false;
                }
            }

            return isValid;
        }
    }
}
