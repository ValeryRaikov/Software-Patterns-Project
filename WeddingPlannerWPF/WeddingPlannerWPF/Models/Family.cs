using WeddingPlannerWPF.Models.Interfaces;

namespace WeddingPlannerWPF.Models
{
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

        public int GetGuestCount() => Members.Count;

        public IEnumerable<Guest> GetGuests() // Iterator pattern
        {
            foreach (var guest in Members)
            {
                yield return guest;
            }
        }

        public bool CanAdd(ISeatComponent component)
        {
            return component is Guest guest && guest.FamilyId == FamilyId;
        }

        public void Add(ISeatComponent component)
        {
            if (component is Guest guest)
            {
                if (guest.FamilyId != FamilyId)
                    throw new ArgumentException("Guest does not belong to this family");
                Members.Add(guest);
            }
        }

        public void Remove(ISeatComponent component)
        {
            if (component is Guest guest)
                Members.Remove(guest);
        }

        public List<string> GetFamilies() => new List<string> { FamilyId };

        public override string ToString() => $"{Name} ({Members.Count} members)";
    }
}
