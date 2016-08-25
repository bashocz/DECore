using System;
using System.Linq;
using Moq;
using Xunit;

namespace Decore
{
    public class MapperTests
    {
        [Fact]
        public void Ctor_WithMapFunc()
        {
            //Arrange
            Func<InClass, OutClass> mapFunc = src => null;

            //Act
            var sut = new Mapper<InClass, OutClass>(mapFunc);

            //Assert
            Assert.NotNull(sut);
        }

        [Fact]
        public void Ctor_NoMapFunc_DoNotCreate()
        {
            //Act
            Assert.Throws<ArgumentNullException>(() =>
            {
                var sut = new Mapper<InClass, OutClass>((Func<InClass, OutClass>)null);
            });
        }

        [Fact]
        public void Ctor_WithFactory()
        {
            //Arrange
            var mockFactory = new Mock<IMapFuncFactory>();

            //Act
            var sut = new Mapper<InClass, OutClass>(mockFactory.Object);

            //Assert
            Assert.NotNull(sut);
        }

        [Fact]
        public void Ctor_NoFactory_DoNotCreate()
        {
            //Act
            Assert.Throws<ArgumentNullException>(() =>
            {
                var sut = new Mapper<InClass, OutClass>((IMapFuncFactory)null);
            });
        }

        [Fact]
        public void Map_ReturnsInstance_Ctor_WithMapFunc()
        {
            //Arrange
            var outInstance = new OutClass();
            Func<InClass, OutClass> mapFunc = src => outInstance;
            var sut = new Mapper<InClass, OutClass>(mapFunc);

            //Act
            var result = sut.Map(null);

            //Assert
            Assert.Equal(outInstance, result);
        }

        [Fact]
        public void Map_ReturnsInstance_Ctor_WithFactory()
        {
            //Arrange
            var outInstance = new OutClass();
            Func<InClass, OutClass> mapFunc = src => outInstance;
            var mockFactory = new Mock<IMapFuncFactory>();
            mockFactory.Setup(m => m.Get<InClass, OutClass>()).Returns(mapFunc);
            var sut = new Mapper<InClass, OutClass>(mockFactory.Object);

            //Act
            var result = sut.Map(null);

            //Assert
            Assert.Equal(outInstance, result);
        }

        [Fact]
        public void Map_ReturnsEnumerable_Empty()
        {
            //Arrange
            var outInstance = new OutClass();
            Func<InClass, OutClass> mapFunc = src => outInstance;
            var sut = new Mapper<InClass, OutClass>(mapFunc);

            //Act
            var result = sut.MapAll(new InClass[0]).ToArray();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Length);
        }

        [Fact]
        public void Map_ReturnsEnumerable_Single()
        {
            //Arrange
            var outInstance = new OutClass();
            Func<InClass, OutClass> mapFunc = src => outInstance;
            var sut = new Mapper<InClass, OutClass>(mapFunc);

            //Act
            var result = sut.MapAll(new InClass[] { null }).ToArray();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Length);
            Assert.Equal(outInstance, result[0]);
        }

        [Fact]
        public void Map_ReturnsEnumerable_Multiple()
        {
            //Arrange
            var outInstances = new[] { new OutClass(), new OutClass() };
            Func<InClass, OutClass> mapFunc = src =>
            {
                if (src == null)
                    return outInstances[0];
                return outInstances[1];
            };
            var sut = new Mapper<InClass, OutClass>(mapFunc);

            //Act
            var result = sut.MapAll(new[] { null, new InClass(), null }).ToArray();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
            Assert.Equal(outInstances[0], result[0]);
            Assert.Equal(outInstances[1], result[1]);
            Assert.Equal(outInstances[0], result[2]);
        }

        private class InClass { }
        private class OutClass { }
    }
}
