namespace WeddingPlannerWPF.Models.Interfaces
{
    // Composite pattern -> общ интерфейс за всички компоненти
    // Позволява третиране на отделни гости и групи от гости по единен начин
    public interface ISeatComponent
    {
        public string Name { get; }
        public int GetGuestCount();

        // Iterator pattern -> метод за обхождане
        // Връща последователност от всички Guest обекти в този компонент
        public IEnumerable<Guest> GetGuests();

        // Проверява дали може да се добави даден компонент към текущия
        protected bool CanAdd(ISeatComponent component);

        // Добавя подкомпонент към текущия компонент
        protected void Add(ISeatComponent component);

        // Премахва подкомпонент от текущия компонент
        protected void Remove(ISeatComponent component);

        // Връща списък с уникални фамилни имена на всички гости в компонента
        public List<string> GetFamilies();
    }
}
