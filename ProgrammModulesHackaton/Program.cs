using ProgrammModulesHackaton.Helpers;
using ProgrammModulesHackaton.Services;



namespace ProgrammModulesHackaton
{
    class Program
    {
        private static AuthService _authService;
        private static UserService _userService;
        private static ObjectService _objectService;
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
                var user = _authService.Authenticate(username, password);
                if (user != null)
                {
                    Console.WriteLine($"Добро пожаловать, {user.FullName}!");
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

                switch (choice)
                {
                    case "1":
                        if (_authService.CurrentUser.RoleName == "Admin")
                            ManageUsers();
                        else
                            Console.WriteLine("Доступ закрыт. Требуется роль Admin.");
                        break;
                    case "2":
                        if (_authService.CurrentUser.RoleName == "Admin")
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
            // Вызову меню CRUD для пользователей (UserService)
        }

        static void ManageAttributes()
        {
            // CRUD для AttributeDefinitions
        }

        static void ManageControlObjects()
        {
            // 1. Список объектов
            // 2. Поиск по атрибутам
            // 3. Добавление (ручное)
            // 4. Редактирование
            // 5. Удаление
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
