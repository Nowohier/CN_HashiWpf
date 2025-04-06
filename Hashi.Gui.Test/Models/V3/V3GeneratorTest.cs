//using CNHashiWpf.Models.V3.Generator;
//using FluentAssertions;

//namespace CNHashiWpf.Test.Models.V3
//{
//    [TestFixture]
//    public class V3GeneratorTests
//    {
//        [Test]
//        public void CreateHash_ShouldGenerateCorrectNumberOfNodes()
//        {
//            // Arrange
//            var n = 10;
//            var sizeLength = 10;
//            var sizeWidth = 10;
//            var difficulty = 5;
//            var beta = 20;
//            var checkDiff = false;
//            var generator = new V3Generator(n, sizeLength, sizeWidth, difficulty, beta, checkDiff);

//            // Act
//            var field = generator.CreateHash(n, sizeLength, sizeWidth, difficulty, beta, checkDiff);

//            // Assert
//            generator.GetNodes().Count.Should().Be(n);
//        }

//        [Test]
//        public void CreateNewEdges_ShouldAddEdges()
//        {
//            // Arrange
//            var n = 10;
//            var sizeLength = 10;
//            var sizeWidth = 10;
//            var difficulty = 5;
//            var beta = 20;
//            var checkDiff = false;
//            var generator = new V3Generator(n, sizeLength, sizeWidth, difficulty, beta, checkDiff);
//            var field = generator.CreateHash(n, sizeLength, sizeWidth, difficulty, beta, checkDiff);
//            var initialEdgeCount = generator.GetEdges().Count;

//            // Act
//            generator.CreateNewEdges(field, 50);

//            // Assert
//            generator.GetEdges().Count.Should().Be(initialEdgeCount);
//        }

//        [Test]
//        public void SetBeta_ShouldIncreaseDoubleEdges()
//        {
//            // Arrange
//            var n = 10;
//            var sizeLength = 10;
//            var sizeWidth = 10;
//            var difficulty = 5;
//            var beta = 100;
//            var checkDiff = false;
//            var generator = new V3Generator(n, sizeLength, sizeWidth, difficulty, beta, checkDiff);
//            var field = generator.CreateHash(n, sizeLength, sizeWidth, difficulty, beta, checkDiff);
//            generator.CreateNewEdges(field, 50);
//            var initialDoubleEdgeCount = generator.GetEdges().FindAll(e => e.Count == 2).Count;

//            // Act
//            generator.SetBeta(field, beta);

//            // Assert
//            var newDoubleEdgeCount = generator.GetEdges().FindAll(e => e.Count == 2).Count;
//            newDoubleEdgeCount.Should().BeGreaterThan(initialDoubleEdgeCount);
//        }

//        [Test]
//        public void CreateNode_ShouldAddNode()
//        {
//            // Arrange
//            var n = 10;
//            var sizeLength = 10;
//            var sizeWidth = 10;
//            var difficulty = 5;
//            var beta = 20;
//            var checkDiff = false;
//            var generator = new V3Generator(n, sizeLength, sizeWidth, difficulty, beta, checkDiff);
//            var field = generator.CreateHash(n, sizeLength, sizeWidth, difficulty, beta, checkDiff);
//            var initialNodeCount = generator.GetNodes().Count;

//            // Act
//            generator.CreateNode(field, generator.GetNodes()[0]);

//            // Assert
//            generator.GetNodes().Count.Should().BeGreaterThan(initialNodeCount);
//        }

//        [Test]
//        public void CheckSurroundingFields_ShouldReturnTrueForSurroundedNode()
//        {
//            // Arrange
//            var n = 10;
//            var sizeLength = 10;
//            var sizeWidth = 10;
//            var difficulty = 5;
//            var beta = 20;
//            var checkDiff = false;
//            var generator = new V3Generator(n, sizeLength, sizeWidth, difficulty, beta, checkDiff);
//            var field = generator.CreateHash(n, sizeLength, sizeWidth, difficulty, beta, checkDiff);
//            var node = generator.GetNodes()[0];

//            // Act
//            var result = generator.CheckSurroundingFields(node.Y, node.X, field);

//            // Assert
//            result.Should().BeTrue();
//        }
//    }
//}
