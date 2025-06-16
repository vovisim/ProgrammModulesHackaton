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
        private static AttributeService _attributeService;
        private static ObjectAttributeService _objectAttributeService;
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
            _objectAttributeService = new ObjectAttributeService();
            _decisionService = new DecisionService();
            _fileService = new FileService();
            _roleService = new RoleService();
            _xmlImportService = new XmlImportService();
            _xmlUploadService = new XmlUploadService(_objectService, _attributeService, _decisionService);


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
                Console.Clear();
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
                Console.Clear();
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

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управление объектами ===");
                Console.WriteLine("1. Список объектов");
                Console.WriteLine("2. Поиск по атрибутам");
                Console.WriteLine("3. Добавить объект");
                Console.WriteLine("4. Редактировать объект");
                Console.WriteLine("5. Удалить объект");
                Console.WriteLine("6. Назначить атрибут объекту");
                Console.WriteLine("7. Удалить атрибуты объекта");
                Console.WriteLine("8. Экспортировать объект");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        // Показать список объектов
                        var allObjects = _objectService.GetAll();
                        Console.Clear();
                        Console.WriteLine("=== Список объектов ===\n");
                        ObjectHelper.WriteObjectList(_objectAttributeService, allObjects);

                        // Спросить, хотим ли мы посмотреть детали
                        Console.Write("\nВведите ID объекта для подробного просмотра (или пусто для возврата): ");
                        var idInput = Console.ReadLine();
                        if (int.TryParse(idInput, out int detailId))
                        {
                            var obj = _objectService.GetById(detailId);
                            if (obj == null)
                            {
                                Console.WriteLine("Объект с таким ID не найден.");
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine($"=== Подробности объекта #{obj.Id} ===\n");
                                Console.WriteLine($"Название     : {obj.Name}");
                                Console.WriteLine($"Адрес        : {obj.Address}");
                                Console.WriteLine($"Описание     : {obj.Description}");
                                Console.WriteLine($"Создан       : {obj.CreatedAt:yyyy-MM-dd HH:mm}\n");

                                // Атрибуты
                                var attributes = _attributeService.GetAttributesByObjectId(obj.Id);
                                Console.WriteLine("Атрибуты:");
                                if (attributes.Count == 0)
                                {
                                    Console.WriteLine("  (нет атрибутов)");
                                }
                                else
                                {
                                    foreach (var a in attributes)
                                        Console.WriteLine($"  - [{a.Id}] {_attributeService.GetAttributeById(a.AttributeId)?.Name ?? a.AttributeId.ToString()}");
                                }
                                Console.WriteLine();

                                // Файлы
                                var files = _fileService.GetFilesForObject(obj.Id);
                                Console.WriteLine("Файлы и фото:");
                                if (files.Count == 0)
                                {
                                    Console.WriteLine("  (нет файлов)");
                                }
                                else
                                {
                                    foreach (var f in files)
                                        Console.WriteLine($"  - [{f.Id}] {f.FileName} ({f.FileType}), загружен {f.UploadedAt:yyyy-MM-dd}");
                                }
                                Console.WriteLine();

                                // Решения
                                var decs = _decisionService.GetDecisionsForObject(obj.Id);
                                Console.WriteLine("Поручения:");
                                if (decs.Count == 0)
                                {
                                    Console.WriteLine("  (нет поручений)");
                                }
                                else
                                {
                                    foreach (var d in decs)
                                    {
                                        Console.WriteLine($"""
                                          - [{d.Id}] {d.Text}
                                            Срок: {d.DueDate:yyyy-MM-dd}, Статус: {d.Status}, Ответственный: {d.Responsible}
                                        """);
                                    }
                                }
                            }
                        }
                        break;



                    case "2":
                        Console.Write("\nВведите имя атрибута: ");
                        var attrName = Console.ReadLine();

                        var matchedObjects = _objectService.FindByAttribute(attrName);
                        Console.WriteLine("\nРезультаты поиска:");
                        ObjectHelper.WriteObjectList(_objectAttributeService, matchedObjects);
                        break;

                    case "3":
                        
                        Console.Write("\nВведите название нового объекта: ");
                        var name = Console.ReadLine();
                        Console.Write("\nВведите адрес нового объекта: ");
                        var address = Console.ReadLine();
                        Console.Write("\nВведите описание нового объекта: ");
                        var description = Console.ReadLine();

                        var newObjectData = new ControlObject
                        {
                            Name = name!,
                            Address = address ?? "Адрес не указан",
                            Description = description ?? "",
                        };

                        if (newObjectData.Name != null)
                        {
                            _objectService.Add(newObjectData);
                            Console.WriteLine($"Объект {newObjectData.Name} добавлен.");
                        }
                        else
                        {
                            Console.WriteLine("Название не может быть пустым.");
                        }
                        Console.WriteLine("\nНажмите Enter для продолжения...");
                        Console.ReadLine();
                        break;

                    case "4":
                        Console.WriteLine("=== Список объектов ===\n");
                        ObjectHelper.WriteObjectList(_objectAttributeService, _objectService.GetAll());
                        Console.Write("\nВведите ID объекта для редактирования: ");

                        if (int.TryParse(Console.ReadLine(), out int editId))
                        {
                            var obj = _objectService.GetById(editId);
                            if (obj == null)
                            {
                                Console.WriteLine("Объект не найден.");
                                break;
                            }

                            Console.Write($"Текущее имя: {obj.Name}. Введите новое имя: ");
                            var newName = Console.ReadLine();
                            Console.Write($"Текущий адрес: {obj.Address}. Введите новый адрес: ");
                            var newAddress = Console.ReadLine();
                            Console.Write($"Текущее описание: {obj.Description}. Введите новое описание: ");
                            var newDescription = Console.ReadLine();

                            var newObjData = new ControlObject
                            {
                                Id = editId,
                                Name = string.IsNullOrWhiteSpace(newName) ? obj.Name : newName,
                                Address = string.IsNullOrWhiteSpace(newAddress) ? obj.Address : newAddress,
                                Description = string.IsNullOrWhiteSpace(newDescription) ? obj.Description : newDescription,
                                CreatedAt = obj.CreatedAt // сохраняем дату создания
                            };
                            
                            _objectService.Update(editId, newObjData);
                            Console.WriteLine("Объект обновлён.");
                            
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
                            _objectService.Delete(deleteId);
                            _objectAttributeService.DeleteAttributesByObjectId(deleteId); // Удаляем атрибуты объекта
                            Console.WriteLine("Объект удалён.");
                        }
                        else
                        {
                            Console.WriteLine("Неверный ID.");
                        }
                        break;

                    case "6":

                        ObjectHelper.WriteObjectList(_objectAttributeService, _objectService.GetAll());
                        Console.Write("Выберите объект которому присваивать атрибут: ");
                        int objectId = int.Parse(Console.ReadLine()!);
                        var objectAttributes = new List<ObjectAttribute>();

                        Console.Write("\nНапишите атрибуты через пробел\nПример: 1 2 3\n");
                        _attributeService.GetAllAttributes().ForEach((a) => {
                            Console.WriteLine($"ID: {a.Id}, Название: {a.Name}");
                        });
                        var attrIds = Console.ReadLine()?.Split(' ').Select(int.Parse).ToList();
                        if (attrIds != null)
                        {
                            foreach (var attrId in attrIds)
                            {
                                var attr = _attributeService.GetAttributeById(attrId);
                                if (attr != null)
                                {
                                    objectAttributes.Add(new ObjectAttribute
                                    {
                                        ObjectId = objectId,
                                        AttributeId = attr.Id
                                    });
                                }
                                else
                                {
                                    Console.WriteLine($"Атрибут с ID {attrId} не найден.");
                                }
                            }
                        }
                        _objectAttributeService.AssignAttributes(objectAttributes);
                        break;

                    case "7":
                        Console.WriteLine("=== Список объектов ===\n");
                        ObjectHelper.WriteObjectList(_objectAttributeService, _objectService.GetAll());
                        Console.Write("\nВыберите объект: ");
                        int deleteObjectId = int.Parse(Console.ReadLine()!);
                        Console.WriteLine("\nВы уверены что хотите удалить аттрибуты?]\n1.Да\n2.Нет");
                        if(Console.ReadLine() == "1")
                        {
                            _objectAttributeService.DeleteAttributesByObjectId(deleteObjectId);
                            Console.WriteLine("Атрибуты удалены.");
                        }
                        else
                        {
                            Console.WriteLine("Удаление отменено.");
                        }

                        break;

                    case "8":
                        Console.WriteLine("=== Список объектов ===\n");
                        ObjectHelper.WriteObjectList(_objectAttributeService, _objectService.GetAll());
                        Console.Write("\nВведите ID объекта для экспорта в XML: ");
                        if (!int.TryParse(Console.ReadLine(), out int exportId))
                        {
                            Console.WriteLine("Неверный формат ID.");
                            break;
                        }

                        // Предложим дефолтное имя файла, например object_{ID}.xml
                        var defaultFileName = $"object_{exportId}.xml";
                        Console.Write($"Введите путь и имя файла для сохранения (Enter для \"{defaultFileName}\"): ");
                        var inputPath = Console.ReadLine();
                        var filePath = string.IsNullOrWhiteSpace(inputPath)
                            ? Path.Combine(AppConfig.DataDir, defaultFileName)
                            : inputPath;

                        try
                        {
                            _xmlUploadService.ExportToXml(exportId, filePath);
                            Console.WriteLine($"\nОбъект #{exportId} успешно экспортирован в файл:\n{filePath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nОшибка при экспорте: {ex.Message}");
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
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управление решениями ===");
                Console.WriteLine("1. Список решений");
                Console.WriteLine("2. Добавить решение");
                Console.WriteLine("3. Редактировать решение");
                Console.WriteLine("4. Удалить решение");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("\nСписок решений:\n");
                        DecisionHelper.WriteDecisionList(_decisionService.GetAll());
                        break;

                    case "2":
                        ObjectHelper.WriteObjectList(_objectAttributeService, _objectService.GetAll());
                        Console.Write("Введите ID объекта: ");
                        if (!int.TryParse(Console.ReadLine(), out int controlObjectId))
                        {
                            Console.WriteLine("Неверный ID.");
                            break;
                        }

                        Console.Write("Введите текст поручения: ");
                        var text = Console.ReadLine() ?? "";

                        Console.Write("Введите дату исполнения (дд.мм.гггг): ");
                        if (!DateTime.TryParse(Console.ReadLine(), out DateTime dueDate))
                        {
                            Console.WriteLine("Неверная дата.");
                            break;
                        }

                        Console.Write("Введите ответственного: ");
                        var responsible = Console.ReadLine() ?? "";

                        _decisionService.AddDecision(new Decision
                        {
                            ControlObjectId = controlObjectId,
                            Text = text,
                            DueDate = dueDate,
                            Responsible = responsible,
                            Status = "Ожидает"
                        });

                        Console.WriteLine("Решение добавлено.");
                        break;

                    case "3":
                        DecisionHelper.WriteDecisionList(_decisionService.GetAll());
                        Console.Write("Введите ID решения для редактирования: ");
                        if (!int.TryParse(Console.ReadLine(), out int editId))
                        {
                            Console.WriteLine("Неверный ID.");
                            break;
                        }

                        var existing = _decisionService.GetById(editId);
                        if (existing == null)
                        {
                            Console.WriteLine("Решение не найдено.");
                            break;
                        }

                        Console.WriteLine($"Текущий текст: {existing.Text}");
                        Console.Write("Новый текст: ");
                        var newText = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newText)) existing.Text = newText;

                        Console.WriteLine($"Текущая дата: {existing.DueDate:dd.MM.yyyy}");
                        Console.Write("Новая дата (формат: дд.мм.гггг): ");
                        var dateInput = Console.ReadLine();
                        if (DateTime.TryParse(dateInput, out var newDate)) existing.DueDate = newDate;

                        Console.WriteLine($"Текущий статус: {existing.Status}");
                        Console.Write("Новый статус (Enter — оставить): ");
                        var newStatus = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newStatus)) existing.Status = newStatus;

                        Console.WriteLine($"Ответственный: {existing.Responsible}");
                        Console.Write("Новый ответственный (Enter — оставить): ");
                        var newResp = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newResp)) existing.Responsible = newResp;

                        _decisionService.UpdateDecision(existing);
                        Console.WriteLine("Решение обновлено.");
                        break;

                    case "4":
                        DecisionHelper.WriteDecisionList(_decisionService.GetAll());
                        Console.Write("Введите ID решения для удаления: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteId))
                        {
                            _decisionService.DeleteDecision(deleteId);
                            Console.WriteLine("Решение удалено.");
                        }
                        else
                        {
                            Console.WriteLine("Неверный ID.");
                        }
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }

                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();
            }
        }


        static void ManageFilesAndPhotos()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управление файлами и фотографиями ===");
                Console.WriteLine("1. Показать все файлы объекта");
                Console.WriteLine("2. Загрузить файл для объекта");
                Console.WriteLine("3. Скачать файл на диск");
                Console.WriteLine("4. Удалить файл");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ObjectHelper.WriteObjectList(_objectAttributeService, _objectService.GetAll());
                        Console.Write("Введите ID объекта: ");
                        if (!int.TryParse(Console.ReadLine(), out int objId1))
                        {
                            Console.WriteLine("Неверный ID.");
                            break;
                        }
                        Console.Clear();
                        Console.WriteLine($"\nФайлы для объекта ID={objId1}:\n");
                        FileHealper.WriteFileList(_fileService.GetFilesForObject(objId1));
                        break;

                    case "2":
                        ObjectHelper.WriteObjectList(_objectAttributeService, _objectService.GetAll());
                        Console.Write("Введите ID объекта: ");
                        if (!int.TryParse(Console.ReadLine(), out int objId2))
                        {
                            Console.WriteLine("Неверный ID.");
                            break;
                        }
                        Console.Clear();
                        Console.Write("Введите полный путь к файлу для загрузки: ");
                        var path = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                        {
                            Console.WriteLine("Файл не найден.");
                            break;
                        }
                        int newId = _fileService.UploadFile(objId2, path!);
                        Console.WriteLine($"Файл загружен под ID={newId}.");
                        break;

                    case "3":
                        ObjectHelper.WriteObjectList(_objectAttributeService, _objectService.GetAll());
                        Console.Write("Введите ID объекта для скачивания его файлов: ");
                        var objId = int.Parse(Console.ReadLine()!);
                        var fileToSave = _fileService.GetFilesForObject(objId);
                        if (fileToSave.Count == 0)
                        {
                            Console.WriteLine("Файл не найден.");
                            break;
                        }
                        Console.Write("Введите папку для сохранения: ");
                        var dest = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(dest))
                        {
                            Console.WriteLine("Путь не указан.");
                            break;
                        }

                        fileToSave.ForEach(item =>
                        {
                            _fileService.SaveFileToDisk(item, dest!);

                        });
                        Console.WriteLine("Все файлы успешно сохраннены!");
                        break;

                    case "4":
                        FileHealper.WriteFileList(_fileService.GetAllFiles());
                        Console.Write("Введите ID файла для удаления: ");
                        if (!int.TryParse(Console.ReadLine(), out int fileId4))
                        {
                            Console.WriteLine("Неверный ID.");
                            break;
                        }
                        _fileService.DeleteFile(fileId4);
                        Console.WriteLine("Файл удалён.");
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


        static void ImportFromXml()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Импорт объектов из XML ===");
                Console.Write("Введите полный путь к XML‑файлу (или пустую строку для отмены): ");
                var path = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(path))
                {
                    // Отмена
                    Console.WriteLine("Импорт отменён.");
                    break;
                }

                if (!File.Exists(path))
                {
                    Console.WriteLine("Файл не найден. Проверьте путь и повторите.");
                }
                else
                {
                    try
                    {
                        _xmlImportService.ImportFromXml(path);
                        Console.WriteLine("Импорт из XML успешно завершён.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при импорте: {ex.Message}");
                    }
                    break;
                }

                Console.WriteLine("\nНажмите Enter для повторной попытки или введите пустую строку для отмены...");
                var retry = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(retry))
                {
                    Console.WriteLine("Импорт отменён.");
                    break;
                }
            }

            Console.WriteLine("\nНажмите Enter для возврата в главное меню...");
            Console.ReadLine();
        }


        static void ShowAgenda()
        {
            Console.Clear();
            Console.WriteLine("=== Повестка ===\n");

            // 1) Новые объекты с решениями в статусе "Новое"
            var newObjs = _agendaService.GetObjectsWithNewDecisions();
            Console.WriteLine("Новые объекты с новыми поручениями:");
            if (newObjs.Count == 0)
            {
                Console.WriteLine("  (нет новых объектов)");
            }
            else
            {
                foreach (var obj in newObjs)
                {
                    Console.WriteLine($"  • ID: {obj.Id}, Название: {obj.Name}, Адрес: {obj.Address}");
                }
            }

            Console.WriteLine();

            // 2) Объекты с просроченными решениями
            var overdue = _agendaService.GetObjectsWithOverdueDecisions();
            Console.WriteLine("Объекты с просроченными поручениями:");
            if (overdue.Count == 0)
            {
                Console.WriteLine("  (нет просроченных поручений)");
            }
            else
            {
                foreach (var obj in overdue)
                {
                    Console.WriteLine($"  • ID: {obj.Id}, Название: {obj.Name}, Адрес: {obj.Address}");
                }
            }

            Console.WriteLine();

            // 3) Все объекты с активными (не выполненными) поручениями
            var active = _agendaService.GetActiveAgenda();
            Console.WriteLine("Объекты с активными поручениями:");
            if (active.Count == 0)
            {
                Console.WriteLine("  (нет активных поручений)");
            }
            else
            {
                foreach (var obj in active)
                {
                    Console.WriteLine($"  • ID: {obj.Id}, Название: {obj.Name}, Адрес: {obj.Address}");
                }
            }

            Console.WriteLine("\nНажмите Enter для возврата в меню...");
            Console.ReadLine();
        }

    }

}
