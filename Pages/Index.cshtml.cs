using GameCurationFramework.Helpers;
using GameCurationFramework.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameCurationFramework.Pages {
    public class IndexModel(GameDataService gameDataService): PageModel {
        private readonly GameDataService _gameDataService = gameDataService;

        [BindProperty]
        public List<string> RolledTags { get; set; } = [];

        public async Task OnPostRollAsync() {
            var tags = await _gameDataService.GetTagsAsync();

            RolledTags = [.. tags
                .OrderBy(x => Random.Shared.Next())
                .Take(3)
                .Select(x => x.Name)];
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
        }

    }
}
