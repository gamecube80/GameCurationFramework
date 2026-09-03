using GameCurationFramework.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Caching.Memory;

namespace GameCurationFramework.Helpers {
    public class GameDataService {
        private readonly Container _container;
        private readonly IMemoryCache _cache;

        private const string TagsCacheKey = "all-tags";
        private const string GamesCacheKey = "all-games";

        public GameDataService(
            CosmosClient cosmosClient,
            IConfiguration configuration,
            IMemoryCache cache) {
            var databaseName = configuration["CosmosDb:DatabaseName"];
            var containerName = configuration["CosmosDb:ContainerName"];

            _container = cosmosClient.GetContainer(
                databaseName,
                containerName);

            _cache = cache;
        }

        public async Task<List<Tag>> GetTagsAsync() {
            if(_cache.TryGetValue(TagsCacheKey, out List<Tag>? tags)) {
                return tags!;
            }

            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.type = @type")
                .WithParameter("@type", "tag");

            var iterator = _container.GetItemQueryIterator<Tag>(query);

            tags = [];

            while(iterator.HasMoreResults) {
                var response = await iterator.ReadNextAsync();
                tags.AddRange(response);
            }

            _cache.Set(
                TagsCacheKey,
                tags,
                TimeSpan.FromMinutes(30));

            return tags;
        }

        public async Task<List<Game>> GetGamesAsync() {
            if(_cache.TryGetValue(GamesCacheKey, out List<Game>? games)) {
                return games!;
            }

            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.type = @type")
                .WithParameter("@type", "game");

            var iterator = _container.GetItemQueryIterator<Game>(query);

            games = [];

            while(iterator.HasMoreResults) {
                var response = await iterator.ReadNextAsync();
                games.AddRange(response);
            }

            _cache.Set(
                GamesCacheKey,
                games,
                TimeSpan.FromMinutes(30));

            return games;
        }

    }
}