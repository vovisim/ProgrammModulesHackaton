using ProgrammModulesHackaton.Helpers;
using ProgrammModulesHackaton.Models;
using ProgrammModulesHackaton.Services;



namespace ProgrammModulesHackaton
{
    class Program
    {
        private static AuthService _authService;
        private static UserService _userService;
        private static ObjectService _objectService;
        private static ControlObjectService _controlObjectService;
        private static AttributeService _attributeService;
        private static DecisionService _decisionService;
        private static FileService _fileService;
        private static AgendaService _agendaService;
        private static XmlImportService _xmlImportService;
        private static XmlUploadService _xmlUploadService;
        private static RoleService _roleService;

        static void Main(string[] args)
        {
            // 1. Путь к базе данных
            var projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
            var dataDir = Path.Combine(projectDir!, "Data");
            var dbPath = Path.Combine(dataDir, "database.db");

            // 2. Инициализация базы
            DatabaseService.InitializeDatabase(); // ← тут внутри можно оставить получение пути тоже, если нужно

            // 3. Создание сервисов с передачей dbPath
            _agendaService = new AgendaService();
            _attributeService = new AttributeService();
            _authService = new AuthService();
            _userService = new UserService();
            _controlObjectService = new ControlObjectService();
            _objectService = new ObjectService();
            _decisionService = new DecisionService();
            _fileService = new FileService();
            _roleService = new RoleService();
            _xmlImportService = new XmlImportService();
            _xmlUploadService = new XmlUploadService(_xmlImportService);


            //// 3. Авторизация / вход
            Console.WriteLine("=== Контроль объектов. Пожалуйста, войдите в систему ===");
            bool loggedIn = false;
            while (!loggedIn)
            {
                Console.Write("Логин: ");
                var username = Console.ReadLine();
                Console.Write("Пароль: ");
                var password = ConsoleHelper.ReadPassword(); // метод, скрывающий ввод
                _authService.CurrentUser = _authService.Authenticate(username, password);
                if (_authService.CurrentUser != null)
                {
                    Console.WriteLine($"Добро пожаловать, {_authService.CurrentUser.FullName}!");
                    loggedIn = true;
                }
                else
                {
                    Console.WriteLine("Неверный логин или пароль. Повторите попытку.\n");
                }
            }

            // 4. Основное меню
            while (true)
            {
                Console.WriteLine("\n=== Главное меню ===");
                Console.WriteLine("1. Управление пользователями (Admin только)");
                Console.WriteLine("2. Управление настраиваемыми полями (Admin только)");
                Console.WriteLine("3. Просмотр / редактирование объектов контроля");
                Console.WriteLine("4. Управление решениями по объектам");
                Console.WriteLine("5. Файлы / фотографии");
                Console.WriteLine("6. Импорт объектов из XML");
                Console.WriteLine("7. Показать повестку (новые/просроченные)");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");
                var choice = Console.ReadLine();
                Console.WriteLine(_authService.CurrentUser.Role);
                switch (choice)
                {
                    case "1":
                        if (_authService.CurrentUser.Role == "Admin")
                            ManageUsers();
                        else
                            Console.WriteLine("Доступ закрыт. Требуется роль Admin.");
                        break;
                    case "2":
                        if (_authService.CurrentUser.Role == "Admin")
                            ManageAttributes();
                        else
                            Console.WriteLine("Доступ закрыт. Требуется роль Admin.");
                        break;
                    case "3":
                        ManageControlObjects();
                        break;
                    case "4":
                        ManageDecisions();
                        break;
                    case "5":
                        ManageFilesAndPhotos();
                        break;
                    case "6":
                        ImportFromXml();
                        break;
                    case "7":
                        ShowAgenda();
                        break;
                    case "0":
                        Console.WriteLine("Выход из приложения. До свидания!");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        static void ManageUsers()
        {
            while (true)
            {
                Console.WriteLine("\n=== Управление пользователями ===");
                Console.WriteLine("1. Список пользователей");
                Console.WriteLine("2. Добавить пользователя");
                Console.WriteLine("3. Редактировать пользователя");
                Console.WriteLine("4. Удалить пользователя");
                Console.WriteLine("0. Назад в главное меню");
                Console.Write("Выберите действие: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        var users = _userService.GetAllUsers();
                        Console.WriteLine("\n--- Пользователи ---");
                        foreach (var user in users)
                        {
                            Console.WriteLine($"ID: {user.Id}, Логин: {user.Username}, ФИО: {user.FullName}, Роль: {user.Role}");
                        }
                        break;

                    case "2":
                        Console.Write("Логин: ");
                        var username = Console.ReadLine();
                        Console.Write("Пароль: ");
                        var password = ConsoleHelper.ReadPassword();
                        Console.Write("ФИО: ");
                        var fullName = Console.ReadLine();
                        Console.Write("Роль (например, Admin или User): ");
                        var role = Console.ReadLine();

                        var newUser = new User
                        {
                            Username = username!,
                            PasswordHash = PasswordHelper.HashPassword(password!),
                            FullName = fullName!,
                            Role = role!
                        };

                        _userService.AddUser(newUser);
                        Console.WriteLine("Пользователь добавлен.");
                        break;

                    case "3":
                        Console.Write("Введите ID пользователя для редактирования: ");
                        if (int.TryParse(Console.ReadLine(), out int editId))
                        {
                            var userToEdit = _userService.GetUserById(editId);
                            if (userToEdit != null)
                            {
                                Console.Write($"Новый логин ({userToEdit.Username}): ");
                                var newUsername = Console.ReadLine();
                                Console.Write($"Новый ФИО ({userToEdit.FullName}): ");
                                var newFullName = Console.ReadLine();
                                Console.Write($"Новая роль ({userToEdit.Role}): ");
                                var newRole = Console.ReadLine();

                                userToEdit.Username = string.IsNullOrWhiteSpace(newUsername) ? userToEdit.Username : newUsername;
                                userToEdit.FullName = string.IsNullOrWhiteSpace(newFullName) ? userToEdit.FullName : newFullName;
                                userToEdit.Role = string.IsNullOrWhiteSpace(newRole) ? userToEdit.Role : newRole;

                                _userService.UpdateUser(userToEdit);
                                Console.WriteLine("Пользователь обновлён.");
                            }
                            else
                            {
                                Console.WriteLine("Пользователь не найден.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Некорректный ID.");
                        }
                        break;

                    case "4":
                        Console.Write("Введите ID пользователя для удаления: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteId))
                        {
                            _userService.DeleteUser(deleteId);
                            Console.WriteLine("Пользователь удалён.");
                        }
                        else
                        {
                            Console.WriteLine("Некорректный ID.");
                        }
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }
        }


        static void ManageAttributes()
        {

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управление атрибутами ===");
                Console.WriteLine("1. Показать все атрибуты");
                Console.WriteLine("2. Добавить атрибут");
                Console.WriteLine("3. Обновить атрибут");
                Console.WriteLine("4. Удалить атрибут");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        var attributes = _attributeService.GetAllAttributes();
                        Console.WriteLine("\nСписок атрибутов:");
                        foreach (var attr in attributes)
                        {
                            Console.WriteLine($"ID: {attr.Id}, Название: {attr.Name}");
                        }
                        break;

                    case "2":
                        Console.Write("\nВведите название нового атрибута: ");
                        var name = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            _attributeService.AddAttribute(name);
                            Console.WriteLine("Атрибут добавлен.");
                        }
                        else
                        {
                            Console.WriteLine("Название не может быть пустым.");
                        }
                        break;

                    case "3":
                        Console.Write("\nВведите ID атрибута для обновления: ");
                        if (int.TryParse(Console.ReadLine(), out int updateId))
                        {
                            var attr = _attributeService.GetAttributeById(updateId);
                            if (attr == null)
                            {
                                Console.WriteLine("Атрибут не найден.");
                                break;
                            }

                            Console.Write($"Введите новое название для '{attr.Name}': ");
                            var newName = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(newName))
                            {
                                _attributeService.DeleteAttribute(updateId); // удаляем старый
                                _attributeService.AddAttribute(newName);     // создаем новый
                                Console.WriteLine("Атрибут обновлён.");
                            }
                            else
                            {
                                Console.WriteLine("Название не может быть пустым.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Неверный формат ID.");
                        }
                        break;

                    case "4":
                        Console.Write("\nВведите ID атрибута для удаления: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteId))
                        {
                            _attributeService.DeleteAttribute(deleteId);
                            Console.WriteLine("Атрибут удалён.");
                        }
                        else
                        {
                            Console.WriteLine("Неверный формат ID.");
                        }
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }

                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();
            }
        }




        static void ManageControlObjects()
        {
            var controlObjectService = new ControlObjectService();
            var objectAttributeService = new ObjectAttributeService();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управление объектами ===");
                Console.WriteLine("1. Список объектов");
                Console.WriteLine("2. Поиск по атрибутам");
                Console.WriteLine("3. Добавить объект");
                Console.WriteLine("4. Редактировать объект");
                Console.WriteLine("5. Удалить объект");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        var allObjects = controlObjectService.GetAll();
                        Console.WriteLine("\nСписок объектов:");
                        foreach (var obj in allObjects)
                        {
                            Console.WriteLine($"ID: {obj.Id}, Название: {obj.Name}");
                        }
                        break;

                    case "2":
                        Console.Write("\nВведите имя атрибута: ");
                        var attrName = Console.ReadLine();
                        Console.Write("Введите значение атрибута: ");
                        var attrValue = Console.ReadLine();

                        var matchedObjects = controlObjectService.FindByAttribute(attrName, attrValue);
                        Console.WriteLine("\nРезультаты поиска:");
                        foreach (var obj in matchedObjects)
                        {
                            Console.WriteLine($"ID: {obj.Id}, Название: {obj.Name}");
                        }
                        break;

                    case "3":
                        Console.Write("\nВведите название нового объекта: ");
                        var name = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            controlObjectService.Add(name);
                            Console.WriteLine("Объект добавлен.");
                        }
                        else
                        {
                            Console.WriteLine("Название не может быть пустым.");
                        }
                        break;

                    case "4":
                        Console.Write("\nВведите ID объекта для редактирования: ");
                        if (int.TryParse(Console.ReadLine(), out int editId))
                        {
                            var obj = controlObjectService.GetById(editId);
                            if (obj == null)
                            {
                                Console.WriteLine("Объект не найден.");
                                break;
                            }

                            Console.Write($"Текущее имя: {obj.Name}. Введите новое имя: ");
                            var newName = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(newName))
                            {
                                controlObjectService.Update(editId, newName);
                                Console.WriteLine("Объект обновлён.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Неверный ID.");
                        }
                        break;

                    case "5":
                        Console.Write("\nВведите ID объекта для удаления: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteId))
                        {
                            controlObjectService.Delete(deleteId);
                            Console.WriteLine("Объект удалён.");
                        }
                        else
                        {
                            Console.WriteLine("Неверный ID.");
                        }
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }

                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();
            }
        }






        static void ManageDecisions()
        {
            // CRUD для решений (DecisionService)
        }

        static void ManageFilesAndPhotos()
        {
            // Загрузка/удаление документов и фотографий (FileService)
        }

        static void ImportFromXml()
        {
            // Выбираем путь к XML, запускаем XmlImportService.Import(path)
        }

        static void ShowAgenda()
        {
            // _agendaService.GetAgenda() → выводим в консоль
        }
    }

}
