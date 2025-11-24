using WeddingPlannerWPF.Models.Interfaces;

namespace WeddingPlannerWPF.Models
{
    // Composite pattern -> Leaf компонент
    // Като Leaf компонент, този клас е терминален елемент, който няма деца
    public class Guest : ISeatComponent
    {
        public string GuestName { get; } 
        public string FamilyId { get; }
        public string DisplayName => $"{GuestName} ({FamilyId})";

        public string Name => DisplayName;  // Interface имплементация (Имплементация на ISeatComponent.Name)

        public Guest(string name, string familyId)
        {
            GuestName = name;
            FamilyId = familyId;
        }

        // Връща брой гости - за отделен гост винаги е 1
        public int GetGuestCount() => 1;

        // Iterator pattern -> връща само себе си като единствен елемент
        // Тъй като това е Leaf компонент, итераторът връща само текущия обект
        // Използва yield return за съвместимост с IEnumerable
        public IEnumerable<Guest> GetGuests()
        {
                yield return this; // Гостът е сам свой собствен итератор
        }

        // Проверява дали може да се добави компонент към гост (винаги false заради Leaf component)
        public bool CanAdd(ISeatComponent component) => false;

        // Опит за добавяне на компонент към гост - не е позволено за Leaf
        public void Add(ISeatComponent component)
        {
            throw new InvalidOperationException("Cannot add to a guest");
        }

        // Опит за премахване на компонент от гост - не е позволено за Leaf
        public void Remove(ISeatComponent component)
        {
            throw new InvalidOperationException("Cannot remove from a guest");
        }

        // Връща списък с фамилни имена - за гост винаги връща неговото FamilyId
        public List<string> GetFamilies() => new List<string> { FamilyId };

        public override string ToString() => DisplayName;
    }
}
