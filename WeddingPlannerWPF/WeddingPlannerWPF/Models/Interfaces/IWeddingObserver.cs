namespace WeddingPlannerWPF.Models.Interfaces
{
    // Observer Pattern (Наблюдател) - допълнителен патерн
    // Този интерфейс позволява на различни компоненти да се абонират и реагират на промени в разпределението на гостите
    public interface IWeddingObserver
    {
        // Извиква се при успешно добавяне на гост към маса
        // Употреба: Актуализация на UI, запис в лог, изпращане на нотификация
        void OnGuestAdded(Guest guest, Table table);

        // Извиква се при премахване на гост от маса
        void OnGuestRemoved(Guest guest, Table table);

        // Извиква се при откриване на конфликт между семейства на една маса (при забрана за сядане на 2 определени семейства заедно)
        void OnFamilyBanned(string family1, string family2, Table table);

        // Извиква се при нарушаване на правило за разпределение на гостите
        // Употреба: Показване на предупреждения, запис на грешки, уведомяване на потребителя
        void OnRuleViolation(string message);
    }
}
