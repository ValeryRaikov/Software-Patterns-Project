using System.Collections.ObjectModel;
using System.Windows.Input;
using WeddingPlannerWPF.Commands;
using WeddingPlannerWPF.Models;

namespace WeddingPlannerWPF.ViewModels
{
    // Основната логика и връзка между View и Models в MVVM архитектурата
    // Този клас е централният ViewModel, който координира всички операции по сватбеното планиране
    public class MainViewModel : ViewModelBase
    {
        private string _guestName;
        private string _familyId;
        private string _banFamily1;
        private string _banFamily2;
        private string _log;
        private string _notificationLog;

        private Guest _selectedGuest;
        private Table _selectedTable;

        // Колекции за данните - ObservableCollection автоматично нотифицира UI за промени
        public ObservableCollection<Guest> AvailableGuests { get; } = new ObservableCollection<Guest>();
        public ObservableCollection<Table> Tables { get; } = new ObservableCollection<Table>();

        // Observer Pattern -> наблюдател за събития от таблиците
        // WeddingPlanner служи като централен наблюдател и логър на всички събития
        public WeddingPlanner Planner { get; private set; }

        public string NotificationLog
        {
            get => _notificationLog;
            set { _notificationLog = value; OnPropertyChanged(); }
        }

        public Guest SelectedGuest
        {
            get => _selectedGuest;
            set
            {
                _selectedGuest = value;
                OnPropertyChanged();
                // Рефрешване на състоянието на командите
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public Table SelectedTable
        {
            get => _selectedTable;
            set
            {
                _selectedTable = value;
                OnPropertyChanged();
                // Рефрешване на състоянието на командите
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string GuestName
        {
            get => _guestName;
            set { _guestName = value; OnPropertyChanged(); }
        }

        public string FamilyId
        {
            get => _familyId;
            set { _familyId = value; OnPropertyChanged(); }
        }

        public string BanFamily1
        {
            get => _banFamily1;
            set { _banFamily1 = value; OnPropertyChanged(); }
        }

        public string BanFamily2
        {
            get => _banFamily2;
            set { _banFamily2 = value; OnPropertyChanged(); }
        }

        public string Log
        {
            get => _log;
            set { _log = value; OnPropertyChanged(); }
        }

        // // Команди за UI взаимодействия - всяка команда свързва бутон с конкретна логика
        public ICommand AddGuestCommand { get; }
        public ICommand CreateTableCommand { get; }
        public ICommand AssignGuestCommand { get; }
        public ICommand AssignFamilyCommand { get; }
        public ICommand BanFamilyCommand { get; }
        public ICommand ShowTableDetailsCommand { get; }
        public ICommand ShowNotificationsCommand { get; }
        public ICommand ClearNotificationsCommand { get; }

        public MainViewModel()
        {
            // Инициализация на планировчика
            Planner = new WeddingPlanner("Main Wedding Planner");

            // Инициализация на командите с соответствующи методи и условия за изпълнение
            AddGuestCommand = new RelayCommand(AddGuest, () => !string.IsNullOrWhiteSpace(GuestName) && !string.IsNullOrWhiteSpace(FamilyId));
            CreateTableCommand = new RelayCommand(CreateTable);
            AssignGuestCommand = new RelayCommand(AssignGuest, () => SelectedGuest != null && SelectedTable != null);
            AssignFamilyCommand = new RelayCommand(AssignFamily, () => !string.IsNullOrWhiteSpace(FamilyId) && SelectedTable != null);
            BanFamilyCommand = new RelayCommand(BanFamily, () => !string.IsNullOrWhiteSpace(BanFamily1) && !string.IsNullOrWhiteSpace(BanFamily2) && SelectedTable != null);
            ShowTableDetailsCommand = new RelayCommand(ShowTableDetails, () => SelectedTable != null);
            ShowNotificationsCommand = new RelayCommand(ShowNotifications);
            ClearNotificationsCommand = new RelayCommand(ClearNotifications);

            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // Добавяне на примерни гости в системата
            AvailableGuests.Add(new Guest("Ivan Petrov", "Petrovi"));
            AvailableGuests.Add(new Guest("Maria Petrova", "Petrovi"));
            AvailableGuests.Add(new Guest("Georgi Ivanov", "Ivanovi"));
            AvailableGuests.Add(new Guest("Elena Ivanova", "Ivanovi"));
            AvailableGuests.Add(new Guest("Stoyan Dimitrov", "Dimitrovi"));
            AvailableGuests.Add(new Guest("Albena Dimitrova", "Dimitrovi"));
            AvailableGuests.Add(new Guest("Yoanna Todorova", "Todorovi"));
            AvailableGuests.Add(new Guest("Stefan Todorov", "Todorovi"));
            AvailableGuests.Add(new Guest("Ivailo Todorov", "Todorovi"));

            AddLog("Sample data loaded");
        }

        private void AddGuest()
        {
            try
            {
                // Създаване на нов гост и добавяне към списъка с налични гости
                var guest = new Guest(GuestName, FamilyId);
                AvailableGuests.Add(guest);
                AddLog($"Added guest: {guest.Name}");

                GuestName = string.Empty;
                FamilyId = string.Empty;
            }
            catch (Exception ex)
            {
                AddLog($"Error adding guest: {ex.Message}");
            }
        }

        private void CreateTable()
        {
            try
            {
                // Създаване на нова маса с автоматично генериран ID
                var table = new Table(Tables.Count + 1);
                table.Attach(Planner);
                Tables.Add(table);
                AddLog($"Created {table.Name}");
            }
            catch (Exception ex)
            {
                AddLog($"Error creating table: {ex.Message}");
            }
        }

        private void AssignGuest()
        {
            try
            {
                // Валидация на входните данни
                if (SelectedGuest == null)
                {
                    AddLog("Error: No guest selected");
                    return;
                }

                if (SelectedTable == null)
                {
                    AddLog("Error: No table selected");
                    return;
                }

                Guest guest = SelectedGuest;
                Table table = SelectedTable;

                AddLog($"Assigning {guest.Name} to {table.Name}...");

                // Създаване на family компонент за госта и добавяне към масата
                var family = new Family(guest.FamilyId);
                family.Add(guest);

                if (table.CanAdd(family))
                {
                    table.Add(family);
                    AvailableGuests.Remove(guest);
                    AddLog($"Successfully assigned {guest.Name} to {table.Name}!");

                    // Изчистване на селектирания гост
                    SelectedGuest = null;
                    UpdateNotificationDisplay();
                }
                else
                {
                    AddLog($"Cannot assign {guest.Name} to {table.Name} - constraints violated");
                }
            }
            catch (Exception ex)
            {
                AddLog($"Error assigning guest: {ex.Message}");
            }
        }

        private void AssignFamily()
        {
            try
            {
                if (SelectedTable == null)
                {
                    AddLog("Error: No table selected");
                    return;
                }

                // Създаване на семейство и намиране на всички гости от това семейство
                var family = new Family(FamilyId);
                var familyGuests = AvailableGuests.Where(g => g.FamilyId == FamilyId).ToList();

                if (!familyGuests.Any())
                {
                    AddLog($"No guests found for family {FamilyId}");
                    return;
                }

                // Добавяне на всички гости от семейството към family компонента
                foreach (var guest in familyGuests)
                {
                    family.Add(guest);
                }

                if (SelectedTable.CanAdd(family))
                {
                    SelectedTable.Add(family);

                    foreach (var guest in familyGuests)
                    {
                        AvailableGuests.Remove(guest);
                    }

                    AddLog($"Assigned {family.Name} to {SelectedTable.Name}");
                    UpdateNotificationDisplay();
                }
                else
                {
                    AddLog($"Cannot assign {family.Name} to {SelectedTable.Name} - violates constraints");
                }
            }
            catch (Exception ex)
            {
                AddLog($"Error assigning family: {ex.Message}");
            }
        }

        private void BanFamily()
        {
            try
            {
                if (SelectedTable == null)
                {
                    AddLog("Error: No table selected");
                    return;
                }

                // Забраняване на две семейства да седят заедно на избраната маса
                SelectedTable.BanFamilyPair(BanFamily1, BanFamily2);
                AddLog($"Banned {BanFamily1} and {BanFamily2} from sitting together at {SelectedTable.Name}");

                BanFamily1 = string.Empty;
                BanFamily2 = string.Empty;

                UpdateNotificationDisplay();
            }
            catch (Exception ex)
            {
                AddLog($"Error banning families: {ex.Message}");
            }
        }

        private void ShowTableDetails()
        {
            try
            {
                if (SelectedTable == null)
                {
                    AddLog("Error: No table selected");
                    return;
                }

                var table = SelectedTable;
                var guests = table.GetGuests().ToList();
                var families = table.GetFamilies();
                var bannedPairs = table.GetBannedPairs();

                // Генериране на детайлен отчет за масата
                var details = $"{table.Name} Details:\n";
                details += $"Total Guests: {guests.Count}/{table.MaxGuests}\n";
                details += $"Families: {string.Join(", ", families)} ({families.Count}/{table.MaxFamilies})\n";

                if (guests.Any())
                {
                    details += $"Guests: {string.Join(", ", guests.Select(g => g.Name))}\n";
                }
                else
                {
                    details += "Guests: None\n";
                }

                if (bannedPairs.Any())
                {
                    details += $"Banned pairs: {string.Join("; ", bannedPairs)}";
                }
                else
                {
                    details += "Banned pairs: None";
                }

                AddLog(details);
            }
            catch (Exception ex)
            {
                AddLog($"Error showing table details: {ex.Message}");
            }
        }

        // Показване на всички нотификации от планировчика
        private void ShowNotifications()
        {
            try
            {
                var notifications = Planner.Notifications;
                if (notifications.Any())
                {
                    NotificationLog = "=== WEDDING PLANNER NOTIFICATIONS ===\n" +
                                     string.Join("\n", notifications) +
                                     $"\n\nTotal: {notifications.Count} notifications";
                }
                else
                {
                    NotificationLog = "No notifications yet";
                }
            }
            catch (Exception ex)
            {
                NotificationLog = $"Error showing notifications: {ex.Message}";
            }
        }

        // Почистване на всички нотификации от планировчика
        private void ClearNotifications()
        {
            try
            {
                Planner.ClearNotifications();
                NotificationLog = "Notifications cleared";
            }
            catch (Exception ex)
            {
                NotificationLog = $"Error clearing notifications: {ex.Message}";
            }
        }

        // Обновяване на дисплея с най-новата нотификация
        private void UpdateNotificationDisplay()
        {
            try
            {
                var latest = Planner.GetLatestNotification();
                if (latest != "No notifications")
                {
                    NotificationLog = $"Latest: {latest}";
                }
            }
            catch (Exception ex)
            {
                NotificationLog = $"Error updating notifications: {ex.Message}";
            }
        }

        private void AddLog(string message)
        {
            // Добавяне на съобщение в лога с timestamp
            // Новото съобщение се добавя отгоре за по-лесно четене
            Log = $"{DateTime.Now:HH:mm:ss} - {message}\n" + Log;
        }
    }
}
