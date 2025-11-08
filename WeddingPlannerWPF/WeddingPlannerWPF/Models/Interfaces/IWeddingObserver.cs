namespace WeddingPlannerWPF.Models.Interfaces
{
    public interface IWeddingObserver
    {
        void OnGuestAdded(Guest guest, Table table);
        void OnGuestRemoved(Guest guest, Table table);
        void OnFamilyBanned(string family1, string family2, Table table);
        void OnRuleViolation(string message);
    }
}
