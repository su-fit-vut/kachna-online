namespace KachnaOnline.Business.Constants
{
    public static class AuthConstants
    {
        // Name of role for managers of states.
        public const string StatesManager = "StatesManager";

        // Name of role for managers of events.
        public const string EventsManager = "EventsManager";

        // Name of role for managers of board games.
        public const string BoardGamesManager = "BoardGamesManager";

        // Name of role for admins of the whole system.
        public const string Admin = "Admin";

        // Name of policy that allows access to any kind of manager or administrator.
        public const string AnyManagerPolicy = nameof(AnyManagerPolicy);

        public const string AdminOrBoardGamesManagerPolicy = nameof(AdminOrBoardGamesManagerPolicy);
    }
}
