using Microsoft.EntityFrameworkCore;
using RepoAnalyzer.Core.Services;
using RepoAnalyzer.Data;
using RepoAnalyzer.Models;

namespace RepoAnalyzer.Tests
{
    public class FavoriteServiceTests
    {
        private AnalyzerDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AnalyzerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AnalyzerDbContext(options);
        }

        [Fact]
        public async Task GetUserFavorites()
        {
            // Arrange
            var db = CreateDbContext();
            var analysis1 = new RepoAnalysis() { Id = Guid.NewGuid(), RepoId = "repo1", UserId = "user1", AnalyzedAt = DateTime.UtcNow };
            var analysis2 = new RepoAnalysis() { Id = Guid.NewGuid(), RepoId = "repo2", UserId = "user1", AnalyzedAt = DateTime.UtcNow };
            var analysis3 = new RepoAnalysis() { Id = Guid.NewGuid(), RepoId = "repo3", UserId = "user2", AnalyzedAt = DateTime.UtcNow };
            var entity1 = new RepoFavoritedEvent
            {
                UserId = analysis1.UserId,
                RepoId = analysis1.RepoId,
                Name = analysis1.Name,
                Owner = analysis1.Owner,
                Stars = 4,
                UpdatedAt = DateTime.UtcNow
            };
            var entity2 = new RepoFavoritedEvent
            {
                UserId = analysis2.UserId,
                RepoId = analysis2.RepoId,
                Name = analysis2.Name,
                Owner = analysis2.Owner,
                Stars = 4,
                UpdatedAt = DateTime.UtcNow
            };
            var entity3 = new RepoFavoritedEvent
            {
                UserId = analysis3.UserId,
                RepoId = analysis3.RepoId,
                Name = analysis3.Name,
                Owner = analysis3.Owner,
                Stars = 4,
                UpdatedAt = DateTime.UtcNow
            };

            var service = new FavoritesService(db);

            await service.AddFavoriteAsync(entity1, analysis1);
            await service.AddFavoriteAsync(entity2, analysis2);
            await service.AddFavoriteAsync(entity3, analysis3);

            // Act
            var favorites = (await service.GetFavoritesAsync("user1")).ToList();

            // Assert
            Assert.Equal(2, favorites.Count);
            Assert.All(favorites, f => Assert.Equal("user1", f.UserId));
            Assert.Contains(favorites, f => f.RepoId == "repo1");
            Assert.Contains(favorites, f => f.RepoId == "repo2");
        }

        [Fact]
        public async Task CreateFavorite()
        {
            // Arrange
            var db = CreateDbContext();
            var analysis1 = new RepoAnalysis() {  Id = Guid.NewGuid(), RepoId = "repo1", UserId = "user1", AnalyzedAt = DateTime.UtcNow };
            var entity = new RepoFavoritedEvent
            {
                UserId = analysis1.UserId,
                RepoId = analysis1.RepoId,
                Name = analysis1.Name,
                Owner = analysis1.Owner,
                Stars = 4,
                UpdatedAt = DateTime.UtcNow
            };

            var service = new FavoritesService(db);

            await service.AddFavoriteAsync(entity, analysis1);

            // Act
            var favorites = (await service.GetFavoritesAsync("user1")).ToList();

            // Assert
            Assert.Equal(1, favorites.Count);
            Assert.Equal(favorites[0].AnalysisId, analysis1.Id);
        }

        [Fact]
        public async Task GetUserFavorites_NoFavorites_ReturnsEmpty()
        {
            // Arrange
            var db = CreateDbContext();
            var service = new FavoritesService(db);

            // Act
            var favorites = (await service.GetFavoritesAsync("ghost-user")).ToList();

            // Assert
            Assert.Empty(favorites);
        }

        [Fact]
        public async Task AddFavorite_DuplicateFavorite_DoesNotCreateDuplicate()
        {
            // Arrange
            var db = CreateDbContext();
            var service = new FavoritesService(db);

            var analysis = new RepoAnalysis
            {
                Id = Guid.NewGuid(),
                RepoId = "repo-dup",
                UserId = "user1",
                AnalyzedAt = DateTime.UtcNow
            };

            var entity = new RepoFavoritedEvent
            {
                UserId = analysis.UserId,
                RepoId = analysis.RepoId,
                Name = "Duplicate Repo",
                Owner = "Someone",
                Stars = 10,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            await service.AddFavoriteAsync(entity, analysis);
            await service.AddFavoriteAsync(entity, analysis); // try adding again
            var favorites = (await service.GetFavoritesAsync("user1")).ToList();

            // Assert
            Assert.Single(favorites); // only one should exist
            Assert.Equal("repo-dup", favorites[0].RepoId);
        }
    }
}
