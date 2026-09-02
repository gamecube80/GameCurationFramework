namespace GameCurationFramework.Model {
    public class Game {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? CoverArtUrl { get; set; }

        public decimal Price { get; set; }

        public decimal? UserRating { get; set; }
    }
}
