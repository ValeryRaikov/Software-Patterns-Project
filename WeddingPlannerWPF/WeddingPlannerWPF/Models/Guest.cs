using WeddingPlannerWPF.Models.Interfaces;

namespace WeddingPlannerWPF.Models
{
    public class Guest : ISeatComponent
    {
        public string GuestName { get; }  // Changed from Name to GuestName
        public string FamilyId { get; }
        public string DisplayName => $"{GuestName} ({FamilyId})";

        // ISeatComponent implementation
        public string Name => DisplayName;  // This satisfies the interface

        public Guest(string name, string familyId)
        {
            GuestName = name;
            FamilyId = familyId;
        }

        public int GetGuestCount() => 1;

        public IEnumerable<Guest> GetGuests() // Iterator pattern
        {
            if (this != null)
            {
                yield return this;
            }
        }

        public bool CanAdd(ISeatComponent component) => false;

        public void Add(ISeatComponent component)
        {
            throw new System.InvalidOperationException("Cannot add to a guest");
        }

        public void Remove(ISeatComponent component)
        {
            throw new System.InvalidOperationException("Cannot remove from a guest");
        }

        public List<string> GetFamilies() => new List<string> { FamilyId };

        public override string ToString() => DisplayName;
    }
}
