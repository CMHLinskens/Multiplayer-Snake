using System;
using Xunit;

namespace Server.Tests
{
    public class GameTests
    {
        [Theory]
        [InlineData(10, 10, 10, 10)]
        [InlineData(16, 1, 0, 1)]
        [InlineData(5, -1, 5, 15)]
        public void CheckOutOfBounds_Test(int x, int y, int expectedX, int expectedY)
        {
            // Arrange
            Game game = new Game(new Lobby("Test lobby", "Test player", 3, Utils.MapSize.size16x16));
            game.InitializeGameField();

            // Act
            (int y, int x) actual = game.CheckOutOfBounds((y, x));

            // Assert
            Assert.Equal((actual.y, actual.x), (expectedY, expectedX));
        }

        [Theory]
        [InlineData(5,5,5,5,true)]
        [InlineData(2,2,2,3,false)]
        [InlineData(3,3,7,7,false)]
        public void CollisionWithFood_Test(int foodX, int foodY, int playerX, int playerY, bool expected)
        {
            // Arrange
            Game game = new Game(new Lobby("Test lobby", "Test player", 3, Utils.MapSize.size16x16));
            game.InitializeGameField();
            game.SetFoodPos(foodX, foodY);

            // Act
            bool actual = game.CollisionWithFood((playerY, playerX));

            // Assert
            Assert.Equal(actual, expected);
        }
    }
}
