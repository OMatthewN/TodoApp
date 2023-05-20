using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using CsvHelper;
using CsvHelper.Configuration;

namespace DailyPlanner
{
    public partial class MainWindow : Window
    {
        private List<TaskItem> tasks;
        private string dataFilePath = "tasks.csv";

        public MainWindow()
        {
            InitializeComponent();
            LoadTasksFromDataFile();
            tasksListBox.ItemsSource = tasks;
        }

        private void LoadTasksFromDataFile()
        {
            tasks = new List<TaskItem>();

            if (File.Exists(dataFilePath))
            {
                using (var reader = new StreamReader(dataFilePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    tasks = csv.GetRecords<TaskItem>().ToList();
                }
            }
        }

        private void SaveTasksToDataFile()
        {
            using (var writer = new StreamWriter(dataFilePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(tasks);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string description = descriptionTextBox.Text.Trim();
            DateTime dateTime = GetSelectedDateTime();

            if (string.IsNullOrEmpty(description) || dateTime == default)
            {
                MessageBox.Show("Введите описание и выберите дату/время.", "Ошибка");
                return;
            }

            TaskItem task = new TaskItem(description, dateTime);
            tasks.Add(task);
            SaveTasksToDataFile();
            ClearInputs();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            TaskItem selectedTask = GetSelectedTask();

            if (selectedTask != null)
            {
                string newDescription = descriptionTextBox.Text.Trim();
                DateTime newDateTime = GetSelectedDateTime();

                if (string.IsNullOrEmpty(newDescription) || newDateTime == default)
                {
                    MessageBox.Show("Введите новое описание и выберите новую дату/время.", "Ошибка");
                    return;
                }

                selectedTask.Description = newDescription;
                selectedTask.DateTime = newDateTime;
                SaveTasksToDataFile();
                ClearInputs();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            TaskItem selectedTask = GetSelectedTask();

            if (selectedTask != null)
            {
                tasks.Remove(selectedTask);
                SaveTasksToDataFile();
                ClearInputs();
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = descriptionTextBox.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Введите краткое описание для поиска.", "Ошибка");
                return;
            }

            List<TaskItem> searchResults = tasks.Where(task => task.Description.Contains(searchTerm)).ToList();

            if (searchResults.Count == 0)
            {
                MessageBox.Show("Ничего не найдено.", "Результаты поиска");
            }
            else
            {
                tasksListBox.ItemsSource = searchResults;
            }
        }

        private void FilterComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string selectedFilter = ((ComboBoxItem)filterComboBox.SelectedItem).Content.ToString();

            if (selectedFilter == "Все")
            {
                tasksListBox.ItemsSource = tasks;
            }
            else
            {
                List<TaskItem> filteredTasks = tasks.Where(task => task.Category == selectedFilter).ToList();
                tasksListBox.ItemsSource = filteredTasks;
            }
        }

        private void ClearInputs()
        {
            descriptionTextBox.Text = string.Empty;
            datePicker.SelectedDate = null;
            timeTextBox.Text = string.Empty;
        }

        private TaskItem GetSelectedTask()
        {
            return tasksListBox.SelectedItem as TaskItem;
        }

        private DateTime GetSelectedDateTime()
        {
            DateTime selectedDate = datePicker.SelectedDate ?? default;
            DateTime selectedTime;

            if (DateTime.TryParse(timeTextBox.Text, out selectedTime))
            {
                return selectedDate.Date + selectedTime.TimeOfDay;
            }

            return default;
        }
    }
}
