namespace KachnaOnline.Data.Entities.BoardGames
{
    public enum ReservationEventType
    {
        Created = 0,
        Cancelled = 1,
        Assigned = 2,
        HandedOver = 3,
        ExtensionRequested = 4,
        ExtensionGranted = 5,
        ExtensionRefused = 6,
        Returned = 7
    }
}
