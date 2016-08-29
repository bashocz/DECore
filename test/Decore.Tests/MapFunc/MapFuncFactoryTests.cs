using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Decore.MapFunc
{
    public class MapFuncFactoryTests
    {
        [Fact]
        public void Default()
        {
            //Act
            var sut = MapFuncFactory.Default;

            //Assert
            Assert.NotNull(sut);
        }

        [Fact]
        public void Ctor_WithBuilderFactoryMethod()
        {
            //Arrange
            var mockBuilder = new Mock<IMapFuncBuilder>();
            Func<Type, Type, IMapFuncBuilder> factory = (tin, tout) => mockBuilder.Object;

            //Act
            var sut = new MapFuncFactory(factory);

            //Arrange
            Assert.NotNull(sut);
        }

        [Fact]
        public void Ctor_WithoutBuilderFactoryMethod()
        {
            //Act
            var sut = new MapFuncFactory(null);

            //Arrange
            Assert.NotNull(sut);
        }

        [Fact]
        public void DefaultFunc()
        {
            //Arrange
            var sut = new MapFuncFactory(null);

            //Assert
            Assert.Equal(0, sut.DefaultFunc<object, int>(null));
            Assert.Equal(0, sut.DefaultFunc<object, double>(null));
            Assert.Equal(0, sut.DefaultFunc<object, decimal>(null));
            Assert.Equal(default(DateTime), sut.DefaultFunc<object, DateTime>(null));
            Assert.Null(sut.DefaultFunc<object, string>(null));
            Assert.Null(sut.DefaultFunc<object, OutClass>(null));
        }

        [Fact]
        public void DefaultBuilderFactoryMethod_Null()
        {
            //Arrange
            var sut = new MapFuncFactory(null);

            //Act
            var builder = sut.DefaultBuilderFactoryMethod(null, null);

            //Assert
            Assert.Null(builder);
        }

        [Fact]
        public void BuildFunc_NoFactoryMethod()
        {
            //Arrange
            var sut = new MapFuncFactory(null);

            //Act
            var mapFunc = sut.BuildFunc<object, object>(typeof(object), typeof(object));

            //Assert
            Assert.Equal(sut.DefaultFunc<object, object>, mapFunc);
        }

        [Fact]
        public void BuildFunc_WithFactoryMethod()
        {
            //Arrange
            Func<object, object> func = (src) => src;
            var mockBuilder = new Mock<IMapFuncBuilder>();
            mockBuilder.Setup(m => m.Build<object, object>()).Returns(func);
            Func<Type, Type, IMapFuncBuilder> factory = (tin, tout) => mockBuilder.Object;
            var sut = new MapFuncFactory(factory);

            //Act
            var mapFunc = sut.BuildFunc<object, object>(typeof(object), typeof(object));

            //Assert
            Assert.Equal(func, mapFunc);
        }

        [Fact]
        public void Get_SingleFunc()
        {
            //Assert
            Func<object, object> func = (src) => src;
            var mockBuilder = new Mock<IMapFuncBuilder>();
            mockBuilder.Setup(m => m.Build<object, object>()).Returns(func);
            Func<Type, Type, IMapFuncBuilder> factory = (tin, tout) => mockBuilder.Object;
            var sut = new MapFuncFactory(factory);

            //Act
            var mapFunc1 = sut.Get<object, object>();
            var mapFunc2 = sut.Get<object, object>();

            //Assert
            Assert.Equal(func, mapFunc1);
            Assert.Equal(mapFunc1, mapFunc2);
        }

        [Fact]
        public void Get_MultipleFuncs()
        {
            //Assert
            Func<object, object> func1 = (src) => src;
            Func<int, int> func2 = (src) => src;
            var mockBuilder = new Mock<IMapFuncBuilder>();
            mockBuilder.Setup(m => m.Build<object, object>()).Returns(func1);
            mockBuilder.Setup(m => m.Build<int, int>()).Returns(func2);
            Func<Type, Type, IMapFuncBuilder> factory = (tin, tout) => mockBuilder.Object;
            var sut = new MapFuncFactory(factory);

            //Act
            var mapFunc11 = sut.Get<object, object>();
            var mapFunc12 = sut.Get<int, int>();
            var mapFunc21 = sut.Get<object, object>();
            var mapFunc22 = sut.Get<int, int>();

            //Assert
            Assert.Equal(func1, mapFunc11);
            Assert.Equal(func2, mapFunc12);
            Assert.Equal(mapFunc11, mapFunc21);
            Assert.Equal(mapFunc12, mapFunc22);
        }

        [Fact]
        public void Get_NoFactory()
        {
            //Arrange
            var sut = new MapFuncFactory(null);

            //Act
            var mapFunc = sut.Get<object, object>();

            //Assert
            Assert.Equal(sut.DefaultFunc<object, object>, mapFunc);
        }

        [Fact]
        public void Get_Concurent()
        {
            //Assert
            var buildCount = 0;
            Func<object, object> func = (src) => src;
            var mockBuilder = new Mock<IMapFuncBuilder>();
            mockBuilder.Setup(m => m.Build<object, object>()).Returns(() =>
            {
                buildCount++;
                return func;
            });
            Func<Type, Type, IMapFuncBuilder> factory = (tin, tout) => mockBuilder.Object;
            var sut = new MapFuncFactory(factory);

            //Act
            Task<Func<object, object>>[] tasks =
            {
                Task<Func<object, object>>.Factory.StartNew(() => sut.Get<object, object>()),
                Task<Func<object, object>>.Factory.StartNew(() => sut.Get<object, object>()),
                Task<Func<object, object>>.Factory.StartNew(() => sut.Get<object, object>()),
            };
            Task.WaitAll(tasks);

            //Assert
            Assert.Equal(1, buildCount);
            Assert.NotNull(tasks[0].Result);
            Assert.NotNull(tasks[1].Result);
            Assert.NotNull(tasks[2].Result);
        }

        private class OutClass { }
    }
}
