namespace WeddingPlannerWPF.Models.Interfaces
{
    public interface IWeddingSubject
    {
        void Attach(IWeddingObserver observer);
        void Detach(IWeddingObserver observer);
        void NotifyGuestAdded(Guest guest, Table table);
        void NotifyGuestRemoved(Guest guest, Table table);
        void NotifyFamilyBanned(string family1, string family2, Table table);
        void NotifyRuleViolation(string message);
    }
}
