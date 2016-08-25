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
            var mapper = new Mapper<InClass, OutClass>(mapFunc);

            //Assert
            Assert.NotNull(mapper);
        }

        [Fact]
        public void Ctor_NoMapFunc_DoNotCreate()
        {
            //Act
            Assert.Throws<ArgumentNullException>(() =>
            {
                var mapper = new Mapper<InClass, OutClass>((Func<InClass, OutClass>)null);
            });
        }

        [Fact]
        public void Ctor_WithBuilder()
        {
            //Arrange
            var builder = new Mock<IMapFuncBuilder>();

            //Act
            var mapper = new Mapper<InClass, OutClass>(builder.Object);

            //Assert
            Assert.NotNull(mapper);
        }

        [Fact]
        public void Ctor_NoBuilder_DoNotCreate()
        {
            //Act
            Assert.Throws<ArgumentNullException>(() =>
            {
                var mapper = new Mapper<InClass, OutClass>((IMapFuncBuilder)null);
            });
        }

        [Fact]
        public void Map_ReturnsInstance_Ctor_WithMapFunc()
        {
            //Arrange
            var outInstance = new OutClass();
            Func<InClass, OutClass> mapFunc = src => outInstance;
            var mapper = new Mapper<InClass, OutClass>(mapFunc);

            //Act
            var result = mapper.Map(null);

            //Assert
            Assert.Equal(outInstance, result);
        }

        [Fact]
        public void Map_ReturnsInstance_Ctor_WithBuilder()
        {
            //Arrange
            var outInstance = new OutClass();
            Func<InClass, OutClass> mapFunc = src => outInstance;
            var builder = new Mock<IMapFuncBuilder>();
            builder.Setup(m => m.CreateMapFunc<InClass, OutClass>()).Returns(mapFunc);
            var mapper = new Mapper<InClass, OutClass>(builder.Object);

            //Act
            var result = mapper.Map(null);

            //Assert
            Assert.Equal(outInstance, result);
        }

        [Fact]
        public void Map_ReturnsEnumerable_Empty()
        {
            //Arrange
            var outInstance = new OutClass();
            Func<InClass, OutClass> mapFunc = src => outInstance;
            var mapper = new Mapper<InClass, OutClass>(mapFunc);

            //Act
            var result = mapper.MapAll(new InClass[0]).ToArray();

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
            var mapper = new Mapper<InClass, OutClass>(mapFunc);

            //Act
            var result = mapper.MapAll(new InClass[] { null }).ToArray();

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
            var mapper = new Mapper<InClass, OutClass>(mapFunc);

            //Act
            var result = mapper.MapAll(new[] { null, new InClass(), null }).ToArray();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
            Assert.Equal(outInstances[0], result[0]);
            Assert.Equal(outInstances[1], result[1]);
            Assert.Equal(outInstances[0], result[2]);
        }

        class InClass { }
        class OutClass { }
    }
}
