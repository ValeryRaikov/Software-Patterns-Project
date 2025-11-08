using WeddingPlannerWPF.Models.Interfaces;

namespace WeddingPlannerWPF.Models
{
    // Composite pattern -> Leaf компонент
    public class Guest : ISeatComponent
    {
        public string GuestName { get; } 
        public string FamilyId { get; }
        public string DisplayName => $"{GuestName} ({FamilyId})";

        public string Name => DisplayName;  // Interface имплементация

        public Guest(string name, string familyId)
        {
            GuestName = name;
            FamilyId = familyId;
        }

        public int GetGuestCount() => 1;

        public IEnumerable<Guest> GetGuests() // Iterator pattern -> връща само себе си
        {
            if (this != null)
            {
                yield return this; // Гостът е сам свой собствен итератор
            }
        }

        public bool CanAdd(ISeatComponent component) => false;

        public void Add(ISeatComponent component)
        {
            throw new InvalidOperationException("Cannot add to a guest");
        }

        public void Remove(ISeatComponent component)
        {
            throw new InvalidOperationException("Cannot remove from a guest");
        }

        public List<string> GetFamilies() => new List<string> { FamilyId };

        public override string ToString() => DisplayName;
    }
}
