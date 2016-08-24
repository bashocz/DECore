using System;
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
            Func<InClass, OutClass> mapFunc = src =>
            {
                var dest = new OutClass();
                return dest;
            };

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
            Func<InClass, OutClass> mapFunc = src =>
            {
                var dest = outInstance;
                return dest;
            };
            var mapper = new Mapper<InClass, OutClass>(mapFunc);

            //Act
            var result = mapper.Map(new InClass());

            //Assert
            Assert.Equal(outInstance, result);
        }

        [Fact]
        public void Map_ReturnsInstance_Ctor_WithBuilder()
        {
            //Arrange
            var outInstance = new OutClass();
            Func<InClass, OutClass> mapFunc = src =>
            {
                var dest = outInstance;
                return dest;
            };
            var builder = new Mock<IMapFuncBuilder>();
            builder.Setup(m => m.CreateMapFunc<InClass, OutClass>()).Returns(mapFunc);
            var mapper = new Mapper<InClass, OutClass>(builder.Object);

            //Act
            var result = mapper.Map(new InClass());

            //Assert
            Assert.Equal(outInstance, result);
        }

        class InClass { }
        class OutClass { }
    }
}
