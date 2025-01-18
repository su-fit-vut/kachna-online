namespace KachnaOnline.Data.Entities.ClubStates
{
    public enum StateType
    {
        OpenBar = 0,
        OpenEvent = 1,
        Private = 2,
        // Closed = 3  -- this value is not saved in the database but it is used in the DTOs
        OpenTearoom = 4,
        OpenAll = 5
    }
}
