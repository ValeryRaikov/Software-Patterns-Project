using WeddingPlannerWPF.Models.Interfaces;

namespace WeddingPlannerWPF.Models
{
    public class WeddingPlanner : IWeddingObserver
    {
        public string PlannerName { get; set; }
        public List<string> Notifications { get; private set; }

        public WeddingPlanner(string name)
        {
            PlannerName = name;
            Notifications = new List<string>();
        }

        // =============== OBSERVER PATTERN IMPLEMENTATION ===============
        public void OnGuestAdded(Guest guest, Table table)
        {
            string message = $"[{DateTime.Now:HH:mm:ss}] {guest.GuestName} was seated at {table.Name}";
            Notifications.Add(message);
            Console.WriteLine($"{PlannerName} notified: {message}");
        }

        public void OnGuestRemoved(Guest guest, Table table)
        {
            string message = $"[{DateTime.Now:HH:mm:ss}] {guest.GuestName} was removed from {table.Name}";
            Notifications.Add(message);
            Console.WriteLine($"{PlannerName} notified: {message}");
        }

        public void OnFamilyBanned(string family1, string family2, Table table)
        {
            string message = $"[{DateTime.Now:HH:mm:ss}] Families {family1} and {family2} banned from sitting together at {table.Name}";
            Notifications.Add(message);
            Console.WriteLine($"{PlannerName} notified: {message}");
        }

        public void OnRuleViolation(string message)
        {
            string fullMessage = $"[{DateTime.Now:HH:mm:ss}] RULE VIOLATION: {message}";
            Notifications.Add(fullMessage);
            Console.WriteLine($"{PlannerName} notified: {fullMessage}");
        }

        public void ClearNotifications()
        {
            Notifications.Clear();
        }

        public string GetLatestNotification()
        {
            return Notifications.Count > 0 ? Notifications[^1] : "No notifications";
        }
    }
}
