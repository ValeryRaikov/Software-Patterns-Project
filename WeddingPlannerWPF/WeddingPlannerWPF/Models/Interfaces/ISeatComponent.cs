namespace WeddingPlannerWPF.Models.Interfaces
{
    public interface ISeatComponent
    {
        string Name { get; }
        int GetGuestCount();
        IEnumerable<Guest> GetGuests(); // Iterator pattern
        bool CanAdd(ISeatComponent component);
        void Add(ISeatComponent component);
        void Remove(ISeatComponent component);
        List<string> GetFamilies();
    }
}
