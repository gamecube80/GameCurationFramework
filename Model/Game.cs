namespace GameCurationFramework.Model {
    public class Game {
        public string Id { get; set; } = "";

        public string Type { get; set; } = "game";

        public string Name { get; set; } = "";

        public string? CoverArtUrl { get; set; }

        public decimal Price { get; set; }

        public decimal? UserRating { get; set; }

        public List<string> Tags { get; set; } = [];
    }
}
