using BattleShip;
using BattleShip.Exceptions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BattleShipTest
{
    public class ShipFactoryTest
    {
        [Fact]
        public void CreateShip_WhenOneDimensionShipIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var sutShipFactory = new ShipFactory();
            OneDimensionShip oneDimensionShip = null;

            // Act
            Action action = () => sutShipFactory.CreateShip(oneDimensionShip);

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void CreateShip_WhenOneDimensionShipLengthIsZeroOrNegative_ThrowsArgumentException(int shipLength)
        {
            // Arrange
            var sutShipFactory = new ShipFactory();
            var oneDimensionShip = new OneDimensionShip() { Length = shipLength };

            // Act
            Action action = () => sutShipFactory.CreateShip(oneDimensionShip);

            // Assert
            var expectedErrorMessage = $"The {nameof(OneDimensionShip.Length)} of {nameof(OneDimensionShip)} argument is zero or negative.";
            action.Should().ThrowExactly<ArgumentException>()
                  .And.Message.Should().Be(expectedErrorMessage);
        }

        [Theory]
        [MemberData(nameof(GetPositionsWithNegativeComponent))]
        public void CreateShip_WhenOneDimensionShipLengthHasANegativeComponent_ThrowsArgumentException(Position position)
        {
            // Arrange
            var sutShipFactory = new ShipFactory();
            var oneDimensionShip = new OneDimensionShip() { Length = 1, StartPosition = position };

            // Act
            Action action = () => sutShipFactory.CreateShip(oneDimensionShip);

            // Assert
            var expectedErrorMessage = $"{nameof(OneDimensionShip.StartPosition)} of {nameof(OneDimensionShip)} argument cannot have a negative coordinate.";
            action.Should().ThrowExactly<ArgumentException>()
                  .And.Message.Should().Be(expectedErrorMessage);
        }

        [Theory]
        [MemberData(nameof(GetValidOneDimensionShipAndExpectedShipPairs))]
        public void CreateShip_WhenOneDimensionShipIsValid_ReturnsCorrectShip(OneDimensionShip oneDimensionShip, Ship expectedShip)
        {
            // Arrange
            var sutShipFactory = new ShipFactory();

            // Act
            var result = sutShipFactory.CreateShip(oneDimensionShip);

            // Assert
            result.Should().Be(expectedShip);
        }

        public static IEnumerable<object[]> GetValidOneDimensionShipAndExpectedShipPairs()
        {
            yield return new object[] {
                new OneDimensionShip() { Length = 1, Orientation = ShipOrientation.Horizontal, StartPosition = new Position(5, 5) },
                new Ship() { Coordinates = new HashSet<Position>(){ GetPosition(5,5) }  }
            };
            yield return new object[] {
                new OneDimensionShip() { Length = 1, Orientation = ShipOrientation.Vertical, StartPosition = new Position(3, 3) },
                new Ship() { Coordinates = new HashSet<Position>(){ GetPosition(3,3) }  }
            };
            yield return new object[] {
                new OneDimensionShip() { Length = 3, Orientation = ShipOrientation.Horizontal, StartPosition = new Position(0, 1) },
                new Ship() { Coordinates = new HashSet<Position>(){ GetPosition(0,1), GetPosition(1, 1), GetPosition(2, 1) }  }
            };
            yield return new object[] {
                new OneDimensionShip() { Length = 20, Orientation = ShipOrientation.Vertical, StartPosition = new Position(1, 10) },
                new Ship() { Coordinates = Enumerable.Range(10, 20).Select(col => new Position(1, col)).ToHashSet() }
            };
            yield return new object[] {
                new OneDimensionShip() { Length = 100, Orientation = ShipOrientation.Horizontal, StartPosition = new Position(5, 10) },
                new Ship() { Coordinates = Enumerable.Range(5, 100).Select(row => new Position(row, 10)).ToHashSet() }
            };
        }

        public static IEnumerable<object[]> GetPositionsWithNegativeComponent()
        {
            yield return new object[] { new Position(-1, 0) };
            yield return new object[] { new Position(0, -1) };
            yield return new object[] { new Position(-1, 3) };
            yield return new object[] { new Position(3, -2) };
            yield return new object[] { new Position(-3, -5) };
        }

        private static Position GetPosition(int column, int row)
        {
            return new Position(column, row);
        }
    }
}
