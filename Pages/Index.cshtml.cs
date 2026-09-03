using GameCurationFramework.Helpers;
using GameCurationFramework.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameCurationFramework.Pages {
    public class IndexModel(GameDataService gameDataService): PageModel {
        private readonly GameDataService _gameDataService = gameDataService;

        [BindProperty]
        public List<string> RolledTags { get; set; } = [];

        [BindProperty]
        public Dictionary<string, string> SortByTag { get; set; } = [];

        [BindProperty]
        public Dictionary<string, bool> SortDescendingByTag { get; set; } = [];

        public Dictionary<string, List<Game>> MatchingGames { get; set; } = [];

        public async Task OnPostRollAsync() {
            var tags = await _gameDataService.GetTagsAsync();

            RolledTags = [.. tags
                .OrderBy(x => Random.Shared.Next())
                .Take(3)
                .Select(x => x.Name)];

            await LoadMatchingGamesAsync();
        }

        public async Task OnPostRerollAsync(string tag) {
            var tags = await _gameDataService.GetTagsAsync();

            var replacement = tags
                .Where(x => !RolledTags.Contains(x.Name))
                .OrderBy(x => Random.Shared.Next())
                .First();

            var index = RolledTags.IndexOf(tag);

            if(index >= 0) {
                RolledTags[index] = replacement.Name;
            }

            SortByTag.Remove(tag);
            SortDescendingByTag.Remove(tag);

            await LoadMatchingGamesAsync();
        }

        private async Task LoadMatchingGamesAsync() {
            var games = await _gameDataService.GetGamesAsync();

            MatchingGames = RolledTags.ToDictionary(
                tag => tag,
                tag => {
                    IEnumerable<Game> matches =
                        games.Where(game => game.Tags.Contains(tag));

                    var sortBy = SortByTag.GetValueOrDefault(tag);
                    var descending =
                        SortDescendingByTag.GetValueOrDefault(tag);

                    matches = sortBy switch {
                        "Name" => descending
                            ? matches.OrderByDescending(g => GetGameSortNumber(g.Name))
                            : matches.OrderBy(g => GetGameSortNumber(g.Name)),

                        "Price" => descending
                            ? matches.OrderByDescending(g => g.Price)
                            : matches.OrderBy(g => g.Price),

                        "Rating" => descending
                            ? matches.OrderByDescending(g => g.UserRating)
                            : matches.OrderBy(g => g.UserRating),

                        _ => matches
                    };

                    return matches.ToList();
                });
        }

        private static int GetGameSortNumber(string name) {
            var lastPart = name.Split(' ').Last();

            return int.TryParse(lastPart, out var number)
                ? number
                : int.MaxValue;
        }

        public async Task OnPostSortAsync(string tag, string sortColumn) {
            if(SortByTag.TryGetValue(tag, out var currentSort) &&
                currentSort == sortColumn) {
                SortDescendingByTag[tag] =
                    !SortDescendingByTag.GetValueOrDefault(tag);
            }
            else {
                SortByTag[tag] = sortColumn;
                SortDescendingByTag[tag] = false;
            }

            await LoadMatchingGamesAsync();
        }

    }
}
