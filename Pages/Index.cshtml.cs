using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameCurationFramework.Pages {
    public class IndexModel: PageModel {
        [BindProperty]
        public List<string> RolledTags { get; set; } = [];

        private readonly List<string> _tags =
        [
            "Action",
            "RPG",
            "Puzzle",
            "Narrative",
            "Horror",
            "Shooter",
            "Platformer",
            "Strategy"
        ];

        public void OnPostRoll() {
            RolledTags = [.. _tags
                .OrderBy(x => Random.Shared.Next())
                .Take(3)];
        }

        public void OnPostReroll(string tag) {
            var replacement = _tags
                .Where(x => !RolledTags.Contains(x))
                .OrderBy(x => Random.Shared.Next())
                .First();

            var index = RolledTags.IndexOf(tag);

            if(index >= 0) {
                RolledTags[index] = replacement;
            }
        }

    }
}
