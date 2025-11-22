using WeddingPlannerWPF.Models.Interfaces;

namespace WeddingPlannerWPF.Models
{
    // Composite pattern - Composite компонент, съдържащ гости
    // Този клас представлява семейство като група от гости, които споделят едно фамилно име
    public class Family : ISeatComponent
    {
        public string FamilyId { get; }
        public string Name => $"Family {FamilyId}";
        private List<Guest> Members { get; }

        public Family(string familyId)
        {
            FamilyId = familyId;
            Members = new List<Guest>();
        }

        // Връща общия брой членове в семейството
        // Имплементация на ISeatComponent.GetGuestCount()
        public int GetGuestCount() => Members.Count;

        // Връща последователност от всички гости в това семейство
        // Използва yield return за ефективно итериране
        public IEnumerable<Guest> GetGuests() // Iterator pattern -> обхожда всички гости в семейството
        {
            foreach (var guest in Members)
            {
                yield return guest;
            }
        }

        // Проверява дали даден компонент може да бъде добавен към това семейство
        public bool CanAdd(ISeatComponent component)
        {
            return component is Guest guest && guest.FamilyId == FamilyId;
        }

        // Добавя компонент към семейството
        public void Add(ISeatComponent component)
        {
            if (component is Guest guest)
            {
                if (guest.FamilyId != FamilyId)
                    throw new ArgumentException("Guest does not belong to this family");
                Members.Add(guest);
            }
        }

        // Премахва компонент от семейството
        public void Remove(ISeatComponent component)
        {
            if (component is Guest guest)
                Members.Remove(guest);
        }

        // Връща списък с фамилни имена в този компонент
        // За семейството винаги връща само неговия FamilyId
        public List<string> GetFamilies() => new List<string> { FamilyId };

        public override string ToString() => $"{Name} ({Members.Count} members)";
    }
}
