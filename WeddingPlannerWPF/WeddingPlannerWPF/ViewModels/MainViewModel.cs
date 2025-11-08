using System.Collections.ObjectModel;
using System.Windows.Input;
using WeddingPlannerWPF.Commands;
using WeddingPlannerWPF.Models;

namespace WeddingPlannerWPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _guestName;
        private string _familyId;
        private string _banFamily1;
        private string _banFamily2;
        private string _log;
        private string _notificationLog;

        // =============== FIX: PROPER PROPERTIES WITH BACKING FIELDS ===============
        private Guest _selectedGuest;
        private Table _selectedTable;

        public ObservableCollection<Guest> AvailableGuests { get; } = new ObservableCollection<Guest>();
        public ObservableCollection<Table> Tables { get; } = new ObservableCollection<Table>();

        // Observer Pattern
        public WeddingPlanner Planner { get; private set; }

        public string NotificationLog
        {
            get => _notificationLog;
            set { _notificationLog = value; OnPropertyChanged(); }
        }

        // =============== FIX: PROPER SELECTED GUEST PROPERTY ===============
        public Guest SelectedGuest
        {
            get => _selectedGuest;
            set
            {
                _selectedGuest = value;
                OnPropertyChanged();
                // Refresh command states
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // =============== FIX: PROPER SELECTED TABLE PROPERTY ===============
        public Table SelectedTable
        {
            get => _selectedTable;
            set
            {
                _selectedTable = value;
                OnPropertyChanged();
                // Refresh command states
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

        // Commands
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
            Planner = new WeddingPlanner("Main Wedding Planner");

            // =============== FIX: BETTER COMMAND VALIDATION ===============
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
            // Add sample guests
            AvailableGuests.Add(new Guest("Ivan Petrov", "Petrov"));
            AvailableGuests.Add(new Guest("Maria Petrova", "Petrov"));
            AvailableGuests.Add(new Guest("Georgi Ivanov", "Ivanov"));
            AvailableGuests.Add(new Guest("Elena Ivanova", "Ivanov"));
            AvailableGuests.Add(new Guest("Stoyan Dimitrov", "Dimitrov"));
            AvailableGuests.Add(new Guest("Albena Dimitrova", "Dimitrov"));

            AddLog("Sample data loaded");
        }

        private void AddGuest()
        {
            try
            {
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
                // =============== FIX: BETTER NULL CHECKING ===============
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

                AddLog($"🔄 Assigning {guest.Name} to {table.Name}...");

                // Create family component for the guest
                var family = new Family(guest.FamilyId);
                family.Add(guest);

                if (table.CanAdd(family))
                {
                    table.Add(family);
                    AvailableGuests.Remove(guest);
                    AddLog($"✅ Successfully assigned {guest.Name} to {table.Name}!");

                    // Clear selections
                    SelectedGuest = null;
                    UpdateNotificationDisplay();
                }
                else
                {
                    AddLog($"❌ Cannot assign {guest.Name} to {table.Name} - constraints violated");
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

                var family = new Family(FamilyId);
                var familyGuests = AvailableGuests.Where(g => g.FamilyId == FamilyId).ToList();

                if (!familyGuests.Any())
                {
                    AddLog($"No guests found for family {FamilyId}");
                    return;
                }

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
            Log = $"{DateTime.Now:HH:mm:ss} - {message}\n" + Log;
        }
    }
}
