using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // Не забудь добавить это пространство имён
using Newtonsoft.Json;

namespace ConsoleApp3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var taskList = new TaskList();

            // Автоматическая загрузка задач при запуске программы
            taskList.LoadTasksFromFile("tasks.json");

            while (true)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1 - Добавить задачу");
                Console.WriteLine("2 - Просмотреть все задачи");
                Console.WriteLine("3 - Изменить статус задачи");
                Console.WriteLine("4 - Удалить задачу");
                Console.WriteLine("5 - Сохранить задачи в файл");
                Console.WriteLine("6 - Загрузить задачи из файла");
                Console.WriteLine("7 - Выйти");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        taskList.AddNewTask();
                        break;
                    case "2":
                        taskList.ShowAllTasksInfo();
                        break;
                    case "3":
                        taskList.ChangeTaskStatus();
                        break;
                    case "4":
                        taskList.DeleteTask();
                        break;
                    case "5":
                        taskList.SaveTasksToFile("tasks.json");
                        break;
                    case "6":
                        taskList.LoadTasksFromFile("tasks.json");
                        break;
                    case "7":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
                Console.WriteLine();
            }
        }
    }

    public class Task
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime CreationDate { get; set; }

        public Task(string name, string description, TaskStatus status, DateTime creationDate)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Имя не может быть пустым или null", nameof(name));
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException("Описание не может быть пустым или null", nameof(description));
            }

            Name = name;
            Description = description;
            Status = status;
            CreationDate = creationDate;
        }
    }

    public class TaskList
    {
        public List<Task> Tasks { get; set; }

        public TaskList()
        {
            Tasks = new List<Task>();
        }

        public void AddNewTask()
        {
            Console.WriteLine("Введите название задачи: ");
            string name = Console.ReadLine();
            Console.WriteLine("Введите описание задачи: ");
            string description = Console.ReadLine();
            Console.WriteLine("Укажите статус задачи (нажмите на соответствующую цифру):");
            Console.WriteLine("1 - Запланирована");
            Console.WriteLine("2 - В процессе");
            Console.WriteLine("3 - Выполнена");
            string statusInput = Console.ReadLine();

            TaskStatus status;
            switch (statusInput)
            {
                case "1":
                    status = TaskStatus.Planned;
                    break;
                case "2":
                    status = TaskStatus.InProgress;
                    break;
                case "3":
                    status = TaskStatus.Done;
                    break;
                default:
                    throw new ArgumentException("Неверный статус задачи.");
            }

            Tasks.Add(new Task(name, description, status, DateTime.Now));
            Console.WriteLine("Задача добавлена!");
        }

        public void ShowAllTasksInfo()
        {
            if (Tasks.Count == 0)
            {
                Console.WriteLine("Нет задач.");
                return;
            }

            Console.WriteLine("Какие задачи показать? (нажмите соответствующую цифру для фильтрации)");
            Console.WriteLine("1 - запланированные задачи");
            Console.WriteLine("2 - в процессе");
            Console.WriteLine("3 - выполнено");
            Console.WriteLine("4 - показать все");
            Console.WriteLine("5 - назад в меню");

            string input = Console.ReadLine();

            if (input == "5")
            {
                return; // Возврат в меню
            }

            Console.WriteLine("Сортировать задачи по дате создания? (нажмите соответствующую цифру):");
            Console.WriteLine("1 - По возрастанию (от старых к новым)");
            Console.WriteLine("2 - По убыванию (от новых к старым)");
            Console.WriteLine("3 - Без сортировки");

            string sortInput = Console.ReadLine();

            IEnumerable<Task> tasksToDisplay = Tasks;

            // Фильтрация задач
            switch (input)
            {
                case "1":
                    tasksToDisplay = Tasks.Where(t => t.Status == TaskStatus.Planned);
                    break;
                case "2":
                    tasksToDisplay = Tasks.Where(t => t.Status == TaskStatus.InProgress);
                    break;
                case "3":
                    tasksToDisplay = Tasks.Where(t => t.Status == TaskStatus.Done);
                    break;
                case "4":
                    tasksToDisplay = Tasks;
                    break;
                default:
                    Console.WriteLine("Неверный выбор фильтрации.");
                    return;
            }

            // Сортировка задач
            switch (sortInput)
            {
                case "1":
                    tasksToDisplay = tasksToDisplay.OrderBy(t => t.CreationDate);
                    break;
                case "2":
                    tasksToDisplay = tasksToDisplay.OrderByDescending(t => t.CreationDate);
                    break;
                case "3":
                    // Без сортировки
                    break;
                default:
                    Console.WriteLine("Неверный выбор сортировки.");
                    return;
            }

            // Вывод задач
            foreach (var task in tasksToDisplay)
            {
                Console.WriteLine($"Название: {task.Name}, " +
                                 $"Описание: {task.Description}, " +
                                 $"Статус: {task.Status}, " +
                                 $"Дата создания: {task.CreationDate}");
            }
        }

        public void ChangeTaskStatus()
        {
            Console.WriteLine("Введите название задачи, статус которой хотите изменить:");
            string taskName = Console.ReadLine();

            var task = Tasks.Find(t => t.Name == taskName);
            if (task == null)
            {
                Console.WriteLine("Задача не найдена.");
                return;
            }

            Console.WriteLine("Укажите новый статус задачи (нажмите на соответствующую цифру):");
            Console.WriteLine("1 - Запланирована");
            Console.WriteLine("2 - В процессе");
            Console.WriteLine("3 - Выполнена");
            string statusInput = Console.ReadLine();

            switch (statusInput)
            {
                case "1":
                    task.Status = TaskStatus.Planned;
                    break;
                case "2":
                    task.Status = TaskStatus.InProgress;
                    break;
                case "3":
                    task.Status = TaskStatus.Done;
                    break;
                default:
                    throw new ArgumentException("Неверный статус задачи.");
            }

            Console.WriteLine("Статус задачи изменён!");
        }

        public void DeleteTask()
        {
            Console.WriteLine("Введите название задачи, которую вы хотите удалить: ");
            string deleteTaskInput = Console.ReadLine();

            var task = Tasks.Find(t => t.Name == deleteTaskInput);

            if (task == null)
            {
                Console.WriteLine("Такой задачи нет...");
            }
            else
            {
                Tasks.Remove(task);
                Console.WriteLine("Задача удалена!");
            }
        }

        public void SaveTasksToFile(string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(Tasks, Formatting.Indented);
                File.WriteAllText(filePath, json);
                Console.WriteLine("Задачи сохранены в файл.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении задач: {ex.Message}");
            }
        }

        public void LoadTasksFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    Tasks = JsonConvert.DeserializeObject<List<Task>>(json);
                    Console.WriteLine("Задачи загружены из файла.");
                }
                else
                {
                    Console.WriteLine("Файл с задачами не найден.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке задач: {ex.Message}");
            }
        }
    }

    public enum TaskStatus
    {
        Planned,
        InProgress,
        Done
    }
}