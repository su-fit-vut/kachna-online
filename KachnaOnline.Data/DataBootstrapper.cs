// DataBootstrapper.cs
// Author: Ondřej Ondryáš

using KachnaOnline.Data.Entities.BoardGames;
using KachnaOnline.Data.Entities.Users;

namespace KachnaOnline.Data
{
    /// <summary>
    /// A DataBootstrapper instance is used to fill the application's database with initial data
    /// about existing board game categories, board games etc.
    /// </summary>
    public class DataBootstrapper
    {
        private readonly AppDbContext _dbContext;

        public DataBootstrapper(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Fills the database with initial data.
        /// </summary>
        public void BootstrapDatabase()
        {
            var cats = this.MakeCategories();
            this.MakeGames(cats);
            this.MakeRoles();
        }

        /// <summary>
        /// Creates <see cref="Category"/> objects describing the board games categories
        /// and saves them to the database.
        /// </summary>
        /// <returns>An array of created <see cref="Category"/> entities.</returns>
        private Category[] MakeCategories()
        {
            foreach (var category in _dbContext.BoardGameCategories)
            {
                _dbContext.BoardGameCategories.Remove(category);
            }

            var categories = new[]
            {
                new Category
                {
                    Name = "Časově náročnější hry",
                    ColourHex = "0000ff"
                },
                new Category
                {
                    Name = "Komplexní hry",
                    ColourHex = "0000ff"
                },
                new Category
                {
                    Name = "Karetní hry",
                    ColourHex = "7f6000"
                },
                new Category
                {
                    Name = "Menší hry",
                    ColourHex = "7f6000"
                },
                new Category
                {
                    Name = "Pexeso",
                    ColourHex = "7f6000"
                },
                new Category
                {
                    Name = "Hry na hodinku",
                    ColourHex = "ff0000"
                },
                new Category
                {
                    Name = "Párty hry",
                    ColourHex = "ff0000"
                },
                new Category
                {
                    Name = "Hry pro dva",
                    ColourHex = "38761d"
                },
                new Category
                {
                    Name = "Vědomostní hry",
                    ColourHex = "666666"
                },
                new Category
                {
                    Name = "Věčné klasiky",
                    ColourHex = "741b47"
                }
            };

            _dbContext.BoardGameCategories.AddRange(categories);
            _dbContext.SaveChanges();
            return categories;
        }

        /// <summary>
        /// Creates <see cref="BoardGame"/> objects describing the available board games
        /// and saves them to the database.
        /// </summary>
        /// <returns>An array of created <see cref="BoardGame"/> entities.</returns>
        private void MakeGames(Category[] cats)
        {
            var games = new[]
            {
                new BoardGame
                {
                    Category = cats[0], Name = "Last Night on Earth", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 6
                },
                new BoardGame
                {
                    Category = cats[0], Name = "Na Křídlech", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 1, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[1], Name = "Scythe", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 1, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[1], Name = "Kmotr impérium Corleonů", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[1], Name = "Mars Teraformace", Unavailable = 0, Visible = true,
                    InStock = 1,
                    PlayersMin = 1, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[1], Name = "Vládci podzemí", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[1], Name = "Zaklínač", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[1], Name = "Zima mrtvých cesty osudu", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[1], Name = "Zombicide Černý mor", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 1, PlayersMax = 6
                },
                new BoardGame
                {
                    Category = cats[0], Name = "Ostrov Skye", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[0], Name = "WarhammerQuest", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 1, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[0], Name = "Robinson Crusoe", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 1, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[1], Name = "Mafia city", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 3, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[1], Name = "Temné znamení", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 1, PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[1], Name = "Temné znamení brány arkhamu", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 1, PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[1], Name = "Fiasco", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 3, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[2], Name = "Vzhůru do podzemí", Unavailable = 0, Visible = true,
                    InStock = 1,
                    PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[2], Name = "Munchkin", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 3, PlayersMax = 6
                },
                new BoardGame
                {
                    Category = cats[2], Name = "Munchkin Loot letter ", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[2], Name = "Opráski sčeskí historije", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Karty mrtvého muže", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Red7", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2,
                    PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Illegal", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 5,
                    PlayersMax = 9
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Ovce boj o pastviny", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Dobble", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 1,
                    PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Duch", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2,
                    PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Duchová v koupelně", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Bang! Kostky", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 3, PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Malá velká království", Unavailable = 0, Visible = true,
                    InStock = 1,
                    PlayersMin = 2, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[4], Name = "Pexeso Hello Kitty", Unavailable = 0, Visible = true,
                    InStock = 1,
                    PlayersMin = 2, PlayersMax = 2
                },
                new BoardGame
                {
                    Category = cats[4], Name = "Pexeso Hovna (2x)", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 2
                },
                new BoardGame
                {
                    Category = cats[4], Name = "Pexeso Kočky", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 2
                },
                new BoardGame
                {
                    Category = cats[4], Name = "Pexeso Moje první zvířátka", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 2
                },
                new BoardGame
                {
                    Category = cats[4], Name = "Pexeso Pat a Mat", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 2
                },
                new BoardGame
                {
                    Category = cats[4], Name = "Pexeso Šachy", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 2
                },
                new BoardGame
                {
                    Category = cats[2], Name = "Bang (2x)", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 4, PlayersMax = 15
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Guillotine", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[2], Name = "Citadela", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 7
                },
                new BoardGame
                {
                    Category = cats[2], Name = "Exploding Kittens + Imploding Kittens", Unavailable = 0,
                    Visible = true, InStock = 1, PlayersMin = 2, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[2], Name = "Sabotér", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 3, PlayersMax = 10
                },
                new BoardGame
                {
                    Category = cats[2], Name = "Levá & Pravá", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[2], Name = "Uno", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2,
                    PlayersMax = 10
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Boží zboží", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[3], Name = "VUT Dobble", Unavailable = 0, Visible = true, InStock = 2,
                    PlayersMin = 2, PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Chňapni pejska", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 1, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Pět okurek", Unavailable = 0, Visible = true, InStock = 2,
                    PlayersMin = 2, PlayersMax = 6
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Střelené kachny", Unavailable = 0, Visible = true,
                    InStock = 3,
                    PlayersMin = 3, PlayersMax = 6
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Blafuj", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2,
                    PlayersMax = 6
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Hanabi", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2,
                    PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[3], Name = "Add-On: Imploding Kittens - Cone of Shame", Unavailable = 0,
                    Visible = true, InStock = 1
                },
                new BoardGame
                {
                    Category = cats[8], Name = "AZ kvíz", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 2
                },
                new BoardGame
                {
                    Category = cats[8],
                    Name = "Koncept",
                    Unavailable = 0,
                    Visible = true, InStock = 1, PlayersMin = 4
                },
                new BoardGame
                {
                    Category = cats[8], Name = "Matematika V Kostce", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 1, PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[8], Name = "Naše století", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 6
                },
                new BoardGame
                {
                    Category = cats[8], Name = "Svět otázky a odpovědi", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 6
                },
                new BoardGame
                {
                    Category = cats[8], Name = "Timeline Historical Events", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[8], Name = "Timeline Inventions", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[8], Name = "Timeline Music & Cinema", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[8], Name = "Timeline Science & Discoveries", Unavailable = 0,
                    Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[8], Name = "Timeline Star Wars", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[8], Name = "Quoridor", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[6], Name = "Activity original legend", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 3, PlayersMax = 16
                },
                new BoardGame
                {
                    Category = cats[6], Name = "Scrabble", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[6], Name = "Escape: The Curse of the Temple", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 1, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Origin počátek lidstva", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Labyrinth", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[0], Name = "Semafory", Unavailable = 0, Visible = true,
                    InStock = 1,
                    PlayersMin = 1, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[0], Name = "Settlers zrod impéria Atlantida", Unavailable = 0,
                    Visible = true, InStock = 1, PlayersMin = 1, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[0], Name = "Settlers zrod impéria Aztékové", Unavailable = 0,
                    Visible = true, InStock = 1, PlayersMin = 1, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[0], Name = "Dominion", Unavailable = 0, Visible = true,
                    InStock = 1,
                    PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[0], Name = "Dominion intriky", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[6], Name = "Dixit", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 3,
                    PlayersMax = 12
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Mysterium", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 7
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Takenoko", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Takenoko panďátka", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Ticket to ride Evropa", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Tash-kalar aréna legend", Unavailable = 0,
                    Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[6], Name = "Ubongo", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2,
                    PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Vládce Tokia", Unavailable = 0, Visible = true,
                    InStock = 1,
                    PlayersMin = 2, PlayersMax = 6
                },
                new BoardGame
                {
                    Category = cats[6], Name = "Jungle speed", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 10
                },
                new BoardGame
                {
                    Category = cats[6], Name = "Krycí jména", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 2
                },
                new BoardGame
                {
                    Category = cats[6], Name = "Krycí jména obrázky", Unavailable = 0, Visible = true,
                    InStock = 2, PlayersMin = 2, PlayersMax = 2
                },
                new BoardGame
                {
                    Category = cats[6], Name = "Párty věž", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 2
                },
                new BoardGame
                {
                    Category = cats[6], Name = "Vúdú", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 3,
                    PlayersMax = 6
                },
                new BoardGame
                {
                    Category = cats[6], Name = "Vúdú ninžové a lidojedi", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[6], Name = "Karty proti lidskosti", Unavailable = 0, Visible = true,
                    InStock = 1,
                    PlayersMin = 3, PlayersMax = 10
                },
                new BoardGame
                {
                    Category = cats[6], Name = "KFC Twister ", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 3
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Brno Šalinou", Unavailable = 0, Visible = true,
                    InStock = 1,
                    PlayersMin = 2, PlayersMax = 6
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Vše nej!", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 3, PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[9], Name = "Černý petr Pat a Mat", Unavailable = 0, Visible = true,
                    InStock = 1,
                    PlayersMin = 3, PlayersMax = 3
                },
                new BoardGame
                {
                    Category = cats[9], Name = "Kvarteto", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 3, PlayersMax = 3
                },
                new BoardGame
                {
                    Category = cats[9], Name = "Dostihy a sázky", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 6
                },
                new BoardGame
                {
                    Category = cats[9], Name = "Člověče nezlob se", Unavailable = 0, Visible = true,
                    InStock = 1,
                    PlayersMin = 2, PlayersMax = 6
                },
                new BoardGame
                {
                    Category = cats[9], Name = "Domino", Unavailable = 0, Visible = true, InStock = 1
                },
                new BoardGame
                {
                    Category = cats[9], Name = "Karty dvouhlavé", Unavailable = 0,
                    Visible = true, InStock = 2
                },
                new BoardGame
                {
                    Category = cats[9], Name = "Karty kanastové", Unavailable = 0,
                    Visible = true, InStock = 2
                },
                new BoardGame
                {
                    Category = cats[9], Name = "Karty pokrové", Unavailable = 0,
                    Visible = true, InStock = 2
                },
                new BoardGame
                {
                    Category = cats[9], Name = "Kostky (set)", Unavailable = 0,
                    Visible = true, InStock = 1
                },
                new BoardGame
                {
                    Category = cats[9], Name = "Šach", Unavailable = 0, Visible = true, InStock = 2,
                    PlayersMin = 2, PlayersMax = 2
                },
                new BoardGame
                {
                    Category = cats[9], Name = "Hrací kostky", Unavailable = 0, Visible = true,
                    InStock = 2
                },
                new BoardGame
                {
                    Category = cats[9], Name = "Hlavolamy", Unavailable = 0, Visible = true, InStock = 2
                },
                new BoardGame
                {
                    Category = cats[9], Name = "Monopoly", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 8
                },
                new BoardGame
                {
                    Category = cats[6], Name = "McJohny's", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 3, PlayersMax = 6
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Velká kniha strašlivých čar", Unavailable = 0,
                    Visible = true, InStock = 1, PlayersMin = 2, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Cacao", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Carcasone princezna a drak", Unavailable = 0,
                    Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 6
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Kana gawa", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Pandemic", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Pandemic nové hrozby", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[5], Name = "Carcasone", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 5
                },
                new BoardGame
                {
                    Category = cats[7], Name = "7 divů světa Duel", Unavailable = 0, Visible = true,
                    InStock = 1,
                    PlayersMin = 2, PlayersMax = 2
                },
                new BoardGame
                {
                    Category = cats[7], Name = "Krycí jména duet", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 2
                },
                new BoardGame
                {
                    Category = cats[7], Name = "Patchwork", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 2
                },
                new BoardGame
                {
                    Category = cats[7], Name = "Pentago", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2, PlayersMax = 2
                },
                new BoardGame
                {
                    Category = cats[7], Name = "Lodě", Unavailable = 0, Visible = true, InStock = 1,
                    PlayersMin = 2,
                    PlayersMax = 2
                },
                new BoardGame
                {
                    Category = cats[0], Name = "7 divů světa", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 2, PlayersMax = 7
                },
                new BoardGame
                {
                    Category = cats[0], Name = "Čarodějky", Unavailable = 0, Visible = true,
                    InStock = 1,
                    PlayersMin = 1, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[0], Name = "Osadníci z Katanu", Unavailable = 0, Visible = true,
                    InStock = 1, PlayersMin = 3, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[0], Name = "Settlers zrod impéria + Proč se nepřátelit",
                    Unavailable = 0, Visible = true, InStock = 1, PlayersMin = 1, PlayersMax = 4
                },
                new BoardGame
                {
                    Category = cats[0], Name = "Zámky šíleného krále Ludvíka", Unavailable = 0,
                    Visible = true, InStock = 1, PlayersMin = 1, PlayersMax = 4
                }
            };

            _dbContext.BoardGames.AddRange(games);
            _dbContext.SaveChanges();
        }

        private void MakeRoles()
        {
            foreach (var role in _dbContext.Roles)
            {
                _dbContext.Roles.Remove(role);
            }

            var roles = new[]
            {
                new Role { Name = "StatesManager" },
                new Role { Name = "EventsManager" },
                new Role { Name = "BoardGamesManager" },
                new Role { Name = "Admin" }
            };

            _dbContext.Roles.AddRange(roles);
            _dbContext.SaveChanges();
        }
    }
}
