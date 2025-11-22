namespace WeddingPlannerWPF.Models.Interfaces
{
    // Subject (Наблюдаван обект) от Observer Pattern шаблона
    // Този интерфейс дефинира договора за обекти, които могат да бъдат наблюдавани
    // Отговаря за управлението на списък с наблюдатели и уведомяването им при промени
    public interface IWeddingSubject
    {
        // Регистрира (абонира) наблюдател за получаване на уведомления
        void Attach(IWeddingObserver observer);

        // Премахва (деабонира) наблюдател от списъка
        void Detach(IWeddingObserver observer);

        // Уведомява всички регистрирани наблюдатели за добавяне на гост
        void NotifyGuestAdded(Guest guest, Table table);

        // Уведомява всички регистрирани наблюдатели за премахване на гост
        void NotifyGuestRemoved(Guest guest, Table table);

        // Уведомява за конфликт между две семейства на дадена маса
        void NotifyFamilyBanned(string family1, string family2, Table table);

        // Уведомява за нарушение на правило
        void NotifyRuleViolation(string message);
    }
}
