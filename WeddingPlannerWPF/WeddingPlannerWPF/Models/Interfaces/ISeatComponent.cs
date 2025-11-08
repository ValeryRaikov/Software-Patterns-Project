namespace WeddingPlannerWPF.Models.Interfaces
{
    // Composite pattern -> общ интерфейс за всички компоненти
    public interface ISeatComponent
    {
        public string Name { get; }
        public int GetGuestCount();
        public IEnumerable<Guest> GetGuests(); // Iterator pattern -> метод за обхождане
        protected bool CanAdd(ISeatComponent component);
        protected void Add(ISeatComponent component);
        protected void Remove(ISeatComponent component);
        public List<string> GetFamilies();
    }
}
