using BattleShip;
using BattleShip.Exceptions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BattleShipTest
{
    public class BoardStateTrackerTest
    {
        const int DefaultBoardWidth = 10;
        const int DefaultBoardHeight = 10;

        const int LargeBoardWidth = 10000;
        const int LargeBoardHeight = 10000;

        #region Ctor

        [Theory]
        [InlineData(1, 10)]
        [InlineData(DefaultBoardWidth, DefaultBoardHeight)]
        [InlineData(Int32.MaxValue, Int32.MaxValue)]
        public void Ctor_WhenDimensionsValid_CreatesTheBoardWithTheRightSize(int width, int height)
        {

            // Act
            var sutStateTracker = new BoardStateTracker(width, height);

            // Assert
            sutStateTracker.Dimensions.Width.Should().Be(width);
            sutStateTracker.Dimensions.Height.Should().Be(height);
        }

        [Theory]
        [InlineData(1, -10)]
        [InlineData(-3, 4)]
        [InlineData(-5, -6)]
        [InlineData(0, 0)]
        public void Ctor_WhenDimensionsInvalid_ThrowsArgumentException(int width, int height)
        {

            // Act
            Action action = () => new BoardStateTracker(width, height);

            // Assert
            action.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        #endregion

        #region AddShip

        [Fact]
        public void AddShip_WhenShipIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var sutStateTracker = new BoardStateTracker(DefaultBoardWidth, DefaultBoardHeight);
            Ship ship = null;

            // Act
            Action action = () => sutStateTracker.AddShip(ship);

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(GetShipsWithNullOrEmptyCoordinates))]
        public void AddShip_WhenShipCoordinatesIsNullOrEmpty_ThrowsArgumentException(Ship ship)
        {
            // Arrange
            var sutStateTracker = new BoardStateTracker(DefaultBoardWidth, DefaultBoardHeight);

            // Act
            Action action = () => sutStateTracker.AddShip(ship);

            // Assert
            action.Should()
                  .ThrowExactly<ArgumentException>()
                  .And.Message.Should().Be("Ship coordinates cannot be null or empty. (Parameter 'ship')");
        }

        [Theory]
        [MemberData(nameof(GetOutOfBoundariesShips))]
        public void AddShip_WhenShipCoordinatesAreOutOfBoundaries_ThrowsShipOutOfBoundariesException(HashSet<Position> shipCoordinates)
        {
            // Arrange
            var sutStateTracker = new BoardStateTracker(DefaultBoardWidth, DefaultBoardHeight);
            var ship = new Ship() { Coordinates = shipCoordinates };

            // Act
            Action action = () => sutStateTracker.AddShip(ship);

            // Assert
            action.Should().ThrowExactly<ShipOutOfBoundariesException>();
        }

        [Theory]
        [MemberData(nameof(GetOverlappedShips))]
        public void AddShip_WhenShipCoordinatesOverlapsWithExistingShips_ThrowsShipOverlapException(HashSet<Position> shipCoordinates, HashSet<Position> existingShipCoordinates)
        {
            // Arrange
            var sutStateTracker = new BoardStateTracker(DefaultBoardWidth, DefaultBoardHeight);

            var existingShip = new Ship() { Coordinates = existingShipCoordinates };
            var newShip = new Ship() { Coordinates = shipCoordinates };

            // Act
            sutStateTracker.AddShip(existingShip);
            Action action = () => sutStateTracker.AddShip(newShip);

            // Assert
            action.Should().ThrowExactly<ShipOverlapException>();
        }

        #endregion

        #region TakeAttack

        [Theory]
        [MemberData(nameof(GetAttacksWithExpectedHit))]
        public void TakeAttack_WhenShipIsInPosition_ReturnsHit(List<Ship> existingShips, Position attackPosition)
        {
            // Arrange
            var sutStateTracker = new BoardStateTracker(LargeBoardWidth, LargeBoardHeight);
            existingShips.ForEach((ship) => sutStateTracker.AddShip(ship));

            // Act
            var result = sutStateTracker.TakeAttack(attackPosition);

            // Assert
            result.Should().Be(AttackResult.Hit);
        }

        [Theory]
        [MemberData(nameof(GetAttacksWithExpectedMiss))]
        public void TakeAttack_WhenShipIsNotInPosition_ReturnsMiss(List<Ship> existingShips, Position attackPosition)
        {
            // Arrange
            var sutStateTracker = new BoardStateTracker(LargeBoardWidth, LargeBoardHeight);
            existingShips.ForEach((ship) => sutStateTracker.AddShip(ship));

            // Act
            var result = sutStateTracker.TakeAttack(attackPosition);

            // Assert
            result.Should().Be(AttackResult.Miss);
        }

        #endregion

        #region GetBoardStatus

        [Fact]
        public void GetBoardStatus_WhenBoardHasNoShips_ReturnsEmpty()
        {
            // Arrange
            var sutStateTracker = new BoardStateTracker(LargeBoardWidth, LargeBoardHeight);

            // Act
            var result = sutStateTracker.GetBoardStatus();

            // Assert
            result.Should().Be(BoardStatus.Empty);
        }

        [Theory]
        [MemberData(nameof(GetAllShipsDestroyedList))]
        public void GetBoardStatus_WhenAllShipsHit_ReturnsAllShipsDestroyed(List<Ship> shipsInBoard)
        {
            // Arrange
            var sutStateTracker = new BoardStateTracker(LargeBoardWidth, LargeBoardHeight);
            shipsInBoard.ForEach(ship => sutStateTracker.AddShip(ship));

            // Act
            var result = sutStateTracker.GetBoardStatus();

            // Assert
            result.Should().Be(BoardStatus.AllShipsDestroyed);
        }

        [Theory]
        [MemberData(nameof(GetNotAllShipsDestroyedList))]
        public void GetBoardStatus_WhenSomeShipsAreNotFullyDestroyed_ReturnsShipsAvailable(List<Ship> shipsInBoard)
        {
            // Arrange
            var sutStateTracker = new BoardStateTracker(LargeBoardWidth, LargeBoardHeight);
            shipsInBoard.ForEach(ship => sutStateTracker.AddShip(ship));

            // Act
            var result = sutStateTracker.GetBoardStatus();

            // Assert
            result.Should().Be(BoardStatus.ShipsAvailable);
        }

        [Theory]
        [MemberData(nameof(GetIntactShipListCollection))]
        public void GetBoardStatus_WhenAllShipsAreFullyAttacked_ReturnsAllShipsDestroyedStatus(List<Ship> intactShips)
        {
            // Arrange
            var sutStateTracker = new BoardStateTracker(LargeBoardWidth, LargeBoardHeight);
            intactShips.ForEach(ship => sutStateTracker.AddShip(ship));
            intactShips.ForEach(ship => ship.Coordinates.ToList().ForEach(coordinates => sutStateTracker.TakeAttack(coordinates)));

            // Act
            var result = sutStateTracker.GetBoardStatus();

            // Assert
            result.Should().Be(BoardStatus.AllShipsDestroyed);
        }

        [Theory]
        [MemberData(nameof(GetIntactShipListCollection))]
        public void GetBoardStatus_WhenSomeShipsAreNotFullyAttacked_ReturnsShipsAvailableStatus(List<Ship> intactShips)
        {
            // Arrange
            var sutStateTracker = new BoardStateTracker(LargeBoardWidth, LargeBoardHeight);
            intactShips.ForEach(ship => sutStateTracker.AddShip(ship));
            // Take Attack on some positions (every other position)
            int i = 1;
            intactShips.ForEach(ship => ship.Coordinates.ToList().ForEach(coordinate =>
            {
                if (i % 2 == 0)
                {
                    sutStateTracker.TakeAttack(coordinate);
                }
                i++;
            }));

            // Act
            var result = sutStateTracker.GetBoardStatus();

            // Assert
            result.Should().Be(BoardStatus.ShipsAvailable);
        }

        #endregion

        public static IEnumerable<object[]> GetIntactShipListCollection()
        {
            yield return new object[]
           {
                new List<Ship>()
                {
                    new Ship(){Coordinates = new HashSet<Position>(){ GetPosition(1,1)}},
                    new Ship(){Coordinates = new HashSet<Position>(){ GetPosition(2,2)}}
                }
           };
            yield return new object[]
            {
                new List<Ship>()
                {
                    new Ship(){Coordinates = new HashSet<Position>(){ GetPosition(1,1), GetPosition(1,2), GetPosition(1,3)}},
                    new Ship(){Coordinates = new HashSet<Position>(){ GetPosition(2,1), GetPosition(2,2), GetPosition(2,3)}}
                }
            };
        }

        public static IEnumerable<object[]> GetAllShipsDestroyedList()
        {
            yield return new object[]
            {
                new List<Ship>()
                {
                    new Ship(){
                        Coordinates = new HashSet<Position>(){ GetPosition(1,1) },
                        HitCoordinates= new HashSet<Position>(){ GetPosition(1,1) }
                    }
                }
            };
            yield return new object[]
            {
                new List<Ship>()
                {
                    new Ship()
                    {
                        Coordinates = new HashSet<Position>(){ GetPosition(1,1) },
                        HitCoordinates= new HashSet<Position>(){ GetPosition(1,1) }
                    },
                    new Ship()
                    {
                        Coordinates = new HashSet<Position>(){ GetPosition(1,2), GetPosition(1,3) },
                        HitCoordinates= new HashSet<Position>(){ GetPosition(1,2), GetPosition(1,3) }
                    }
                }
            };
            yield return new object[]
            {
                new List<Ship>()
                {
                    new Ship()
                    {
                        Coordinates = Enumerable.Range(4,10).Select(num => GetPosition(2, num)).ToHashSet(),
                        HitCoordinates = Enumerable.Range(4,10).Select(num => GetPosition(2, num)).ToHashSet()
                    }
                }
            };
            yield return new object[]
            {
                 new List<Ship>()
                {
                    new Ship()
                    {
                        Coordinates = Enumerable.Range(4,10).Select(num => GetPosition(5, num)).ToHashSet(),
                        HitCoordinates = Enumerable.Range(4,10).Select(num => GetPosition(5, num)).ToHashSet()
                    }
                }
            };
            yield return new object[]
           {
                 new List<Ship>()
                {
                    new Ship()
                    {
                        Coordinates = Enumerable.Range(4,10).Select(num => GetPosition(num, num)).ToHashSet(),
                        HitCoordinates = Enumerable.Range(4,10).Select(num => GetPosition(num, num)).ToHashSet()
                    }
                }
           };
        }

        public static IEnumerable<object[]> GetNotAllShipsDestroyedList()
        {
            yield return new object[]
            {
                new List<Ship>()
                {
                    new Ship(){
                        Coordinates = new HashSet<Position>(){ GetPosition(1,1) }
                    }
                }
            };
            yield return new object[]
            {
                new List<Ship>()
                {
                    new Ship()
                    {
                        Coordinates = new HashSet<Position>(){ GetPosition(1,1) },
                        HitCoordinates= new HashSet<Position>(){ GetPosition(1,1) }
                    },
                    new Ship()
                    {
                        Coordinates = new HashSet<Position>(){ GetPosition(1,2), GetPosition(1,3) },
                        HitCoordinates= new HashSet<Position>(){ GetPosition(1,2) }
                    }
                }
            };
            yield return new object[]
            {
                new List<Ship>()
                {
                    new Ship()
                    {
                        Coordinates = Enumerable.Range(4,10).Select(num => GetPosition(2, num)).ToHashSet(),
                        HitCoordinates = Enumerable.Range(4,8).Select(num => GetPosition(2, num)).ToHashSet()
                    }
                }
            };
            yield return new object[]
            {
                 new List<Ship>()
                {
                    new Ship()
                    {
                        Coordinates = Enumerable.Range(6,10).Select(num => GetPosition(5, num)).ToHashSet(),
                        HitCoordinates = Enumerable.Range(6,8).Select(num => GetPosition(5, num)).ToHashSet()
                    }
                }
            };
            yield return new object[]
            {
                 new List<Ship>()
                {
                    new Ship()
                    {
                        Coordinates = Enumerable.Range(6,10).Select(num => GetPosition(num, num)).ToHashSet(),
                        HitCoordinates = Enumerable.Range(6,8).Select(num => GetPosition(num, num)).ToHashSet()
                    }
                }
            };
        }

        public static IEnumerable<object[]> GetAttacksWithExpectedHit()
        {
            yield return new object[]
            {
                new List<Ship>(){ new Ship() { Coordinates = new HashSet<Position>() { new Position(0,0) } } },
                new Position(0,0)
            };
            yield return new object[]
            {
                new List<Ship>(){ new Ship() { Coordinates = new HashSet<Position>() { new Position(1,0), new Position(2,0) } } },
                new Position(2,0)
            };
            yield return new object[]
            {
                new List<Ship>(){
                        new Ship() { Coordinates = new HashSet<Position>() { new Position(1,0), new Position(2,0) } },
                        new Ship() { Coordinates = new HashSet<Position>() { new Position(5,5), new Position(5,6), new Position(5, 7) } }
                },
                new Position(5,6)
            };
            yield return new object[]
            {
                new List<Ship>(){ new Ship() { Coordinates =  Enumerable.Range(50,10).Select(col => new Position(7, col)).ToHashSet() } },
                new Position(7,50)
            };
            yield return new object[]
            {
                new List<Ship>(){ new Ship() { Coordinates =  Enumerable.Range(50,10).Select(col => new Position(7, col)).ToHashSet() } },
                new Position(7,59)
            };
        }

        public static IEnumerable<object[]> GetAttacksWithExpectedMiss()
        {
            yield return new object[]
            {
                new List<Ship>(){ new Ship() { Coordinates = new HashSet<Position>() { new Position(0,0) } } },
                new Position(1,0)
            };
            yield return new object[]
            {
                new List<Ship>(){ new Ship() { Coordinates = new HashSet<Position>() { new Position(1,0), new Position(2,0) } } },
                new Position(0,3)
            };
            yield return new object[]
            {
                new List<Ship>(){
                        new Ship() { Coordinates = new HashSet<Position>() { new Position(1,0), new Position(2,0) } },
                        new Ship() { Coordinates = new HashSet<Position>() { new Position(5,5), new Position(5,6), new Position(5, 7) } }
                },
                new Position(0,0)
            };
            yield return new object[]
            {
                new List<Ship>(){ new Ship() { Coordinates =  Enumerable.Range(50,10).Select(col => new Position(7, col)).ToHashSet() } },
                new Position(7,49)
            };
            yield return new object[]
            {
                new List<Ship>(){ new Ship() { Coordinates =  Enumerable.Range(50,10).Select(col => new Position(7, col)).ToHashSet() } },
                new Position(7,60)
            };
        }

        public static IEnumerable<object[]> GetOutOfBoundariesShips()
        {
            yield return new object[] { new HashSet<Position>() { GetPosition(0, DefaultBoardHeight) } };
            yield return new object[] { new HashSet<Position>() { GetPosition(DefaultBoardWidth, 0) } };
            yield return new object[] { new HashSet<Position>() { GetPosition(0, -1), GetPosition(0, 0), GetPosition(0, 1) } };
            yield return new object[] { new HashSet<Position>() { GetPosition(-1, 0), GetPosition(0, 0), GetPosition(2, 0) } };
            yield return new object[] { new HashSet<Position>() { GetPosition(0, 1), GetPosition(0, 2), GetPosition(DefaultBoardWidth, 0) } };
            yield return new object[] { new HashSet<Position>() { GetPosition(1, 0), GetPosition(2, 0), GetPosition(0, DefaultBoardHeight) } };
            yield return new object[] { new HashSet<Position>() { GetPosition(DefaultBoardWidth + 5, DefaultBoardHeight + 10) } };
        }

        public static IEnumerable<object[]> GetOverlappedShips()
        {
            yield return new object[] { new HashSet<Position>() { GetPosition(0, 0) }, new HashSet<Position>() { GetPosition(0, 0) } };
            yield return new object[] { new HashSet<Position>() { GetPosition(3, 3) }, new HashSet<Position>() { GetPosition(3, 3) } };
            yield return new object[] {
                new HashSet<Position>() { GetPosition(DefaultBoardWidth - 1, DefaultBoardHeight - 1) },
                new HashSet<Position>() { GetPosition(DefaultBoardWidth - 1, DefaultBoardHeight - 1) }
            };
            yield return new object[] {
                new HashSet<Position>() { GetPosition(0, 0), GetPosition(0, 1), GetPosition(0, 2) },
                new HashSet<Position>() { GetPosition(0, 0), GetPosition(0, 1), GetPosition(0, 2), GetPosition(0, 3) }
            };
            yield return new object[] {
                new HashSet<Position>() { GetPosition(0, 0), GetPosition(0, 1), GetPosition(0, 2) },
                new HashSet<Position>() { GetPosition(0, 1), GetPosition(1, 1), GetPosition(2, 1) }
            };
            yield return new object[] {
                new HashSet<Position>() {GetPosition(0, DefaultBoardHeight - 2), GetPosition(1, DefaultBoardHeight - 2), GetPosition(2, DefaultBoardHeight - 2) },
                new HashSet<Position>() { GetPosition(1, DefaultBoardHeight - 3), GetPosition(1, DefaultBoardHeight - 2), GetPosition(1, DefaultBoardWidth - 1) }
            };
        }

        public static IEnumerable<object[]> GetShipsWithNullOrEmptyCoordinates()
        {
            yield return new object[] { new Ship() { Coordinates = null } };
            yield return new object[] { new Ship() { Coordinates = new HashSet<Position>() } };
        }

        private static Position GetPosition(int column, int row)
        {
            return new Position(column, row);
        }
    }
}
