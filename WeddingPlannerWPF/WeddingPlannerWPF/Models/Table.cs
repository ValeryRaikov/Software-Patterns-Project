using WeddingPlannerWPF.Models.Interfaces;

namespace WeddingPlannerWPF.Models
{
    // Composite pattern - Composite компонент, съдържащ гости/семейства
    public class Table : ISeatComponent, IWeddingSubject
    {
        public int TableId { get; }
        public string Name => $"Table {TableId}";
        public int MaxGuests { get; set; } = 10;
        public int MaxFamilies { get; set; } = 2;

        private List<ISeatComponent> Components { get; set; }
        private HashSet<Tuple<string, string>> BannedFamilyPairs { get; set; }  

        private List<IWeddingObserver> _observers = new List<IWeddingObserver>();

        public void Attach(IWeddingObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IWeddingObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyGuestAdded(Guest guest, Table table)
        {
            foreach (var observer in _observers)
            {
                observer.OnGuestAdded(guest, table);
            }
        }

        public void NotifyGuestRemoved(Guest guest, Table table)
        {
            foreach (var observer in _observers)
            {
                observer.OnGuestRemoved(guest, table);
            }
        }

        public void NotifyFamilyBanned(string family1, string family2, Table table)
        {
            foreach (var observer in _observers)
            {
                observer.OnFamilyBanned(family1, family2, table);
            }
        }

        public void NotifyRuleViolation(string message)
        {
            foreach (var observer in _observers)
            {
                observer.OnRuleViolation(message);
            }
        }

        public Table(int tableId)
        {
            TableId = tableId;
            Components = new List<ISeatComponent>();
            BannedFamilyPairs = new HashSet<Tuple<string, string>>();
        }

        public int GetGuestCount() => Components.Sum(c => c.GetGuestCount());

        public IEnumerable<Guest> GetGuests() // Iterator pattern -> обхожда всички гости на масата
        {
            foreach (var component in Components)
            {
                foreach (var guest in component.GetGuests())
                {
                    yield return guest;
                }
            }
        }

        public bool CanAdd(ISeatComponent component)
        {
            // Проверка за лимит на гости на масата
            if (GetGuestCount() + component.GetGuestCount() > MaxGuests)
            {
                NotifyRuleViolation($"Cannot add {component.Name} to {Name}: Exceeds maximum of {MaxGuests} guests");

                return false;
            }

            // Проверка за лимит на семейства на масата
            var currentFamilies = GetFamilies();
            var newFamilies = component.GetFamilies();
            var combinedFamilies = currentFamilies.Union(newFamilies).ToList();

            if (combinedFamilies.Count > MaxFamilies)
            {
                NotifyRuleViolation($"Cannot add {component.Name} to {Name}: Exceeds maximum of {MaxFamilies} families");

                return false;
            }

            // Проверка за забранени семейни двойки
            foreach (var currentFamily in currentFamilies)
            {
                foreach (var newFamily in newFamilies)
                {
                    // Създаване на сортирана двойка за консистентност
                    var family1 = string.Compare(currentFamily, newFamily) < 0 ? currentFamily : newFamily;
                    var family2 = string.Compare(currentFamily, newFamily) < 0 ? newFamily : currentFamily;
                    var pair = Tuple.Create(family1, family2);

                    if (BannedFamilyPairs.Contains(pair))
                    {

                        NotifyRuleViolation($"Cannot add {component.Name} to {Name}: Families {family1} and {family2} are banned from sitting together");

                        return false;
                    }
                }
            }

            return true;
        }

        public void Add(ISeatComponent component)
        {
            if (!CanAdd(component))
                throw new InvalidOperationException($"Cannot add {component.Name} to {Name}");

            Components.Add(component);

            // Observer pattern -> нотифициране за добавените гости
            foreach (var guest in component.GetGuests())
            {
                NotifyGuestAdded(guest, this);
            }
        }

        public void Remove(ISeatComponent component)
        {
            Components.Remove(component);

            // Observer pattern -> нотифициране за премахнатите гости
            foreach (var guest in component.GetGuests())
            {
                NotifyGuestRemoved(guest, this);
            }
        }

        public List<string> GetFamilies()
        {
            var families = new HashSet<string>();
            foreach (var component in Components)
            {
                families.UnionWith(component.GetFamilies());
            }
            return families.ToList();
        }

        public void BanFamilyPair(string family1, string family2)
        {
            // Създаване на сортирана двойка за консистентност
            var sortedFamily1 = string.Compare(family1, family2) < 0 ? family1 : family2;
            var sortedFamily2 = string.Compare(family1, family2) < 0 ? family2 : family1;
            var pair = Tuple.Create(sortedFamily1, sortedFamily2);

            BannedFamilyPairs.Add(pair);

            NotifyFamilyBanned(family1, family2, this);
        }

        public List<string> GetBannedPairs()
        {
            return BannedFamilyPairs.Select(p => $"{p.Item1} - {p.Item2}").ToList();
        }

        public override string ToString()
        {
            var families = GetFamilies();
            return $"{Name} - {GetGuestCount()}/{MaxGuests} guests, {families.Count}/{MaxFamilies} families";
        }
    }
}
