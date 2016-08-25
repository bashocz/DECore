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

        public void Ctor_WithoutFuncStore()
        {
            //Act
            var sut = new MapFuncFactory(null);

            //Arrange
            Assert.NotNull(sut);
        }

        [Fact]
        public void Ctor_WithFuncStore()
        {
            //Arrange
            var funcStore = new Dictionary<Tuple<Type, Type>, object>();

            //Act
            var sut = new MapFuncFactory(funcStore);

            //Arrange
            Assert.NotNull(sut);
        }

        [Fact]
        public void Ctor_WithBuilderFactoryMethod()
        {
            //Arrange
            var funcStore = new Dictionary<Tuple<Type, Type>, object>();
            var mockBuilder = new Mock<IMapFuncBuilder>();
            Func<IMapFuncBuilder> factory = () => mockBuilder.Object;

            //Act
            var sut = new MapFuncFactory(funcStore, factory);

            //Arrange
            Assert.NotNull(sut);
        }

        [Fact]
        public void Ctor_WithoutBuilderFactoryMethod()
        {
            //Arrange
            var funcStore = new Dictionary<Tuple<Type, Type>, object>();

            //Act
            var sut = new MapFuncFactory(funcStore, null);

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
        public void GetBuilder_NoFactoryMethod()
        {
            //Arrange
            var sut = new MapFuncFactory(null);

            //Act
            var builder = sut.GetBuilder();

            //Assert
            Assert.Null(builder);
        }

        [Fact]
        public void GetBuilder_WithFactoryMethod()
        {
            //Arrange
            var funcStore = new Dictionary<Tuple<Type, Type>, object>();
            var mockBuilder = new Mock<IMapFuncBuilder>();
            Func<IMapFuncBuilder> factory = () => mockBuilder.Object;
            var sut = new MapFuncFactory(funcStore, factory);

            //Act
            var builder = sut.GetBuilder();

            //Assert
            Assert.Equal(mockBuilder.Object, builder);
        }

        [Fact]
        public void BuildFunc_NoFactoryMethod()
        {
            //Arrange
            var sut = new MapFuncFactory(null);

            //Act
            var mapFunc = sut.BuildFunc<object, object>();

            //Assert
            Assert.Equal(sut.DefaultFunc<object, object>, mapFunc);
        }

        [Fact]
        public void BuildFunc_WithFactoryMethod()
        {
            //Arrange
            Func<object, object> func = (src) => src;
            var funcStore = new Dictionary<Tuple<Type, Type>, object>();
            var mockBuilder = new Mock<IMapFuncBuilder>();
            mockBuilder.Setup(m => m.Build<object, object>()).Returns(func);
            Func<IMapFuncBuilder> factory = () => mockBuilder.Object;
            var sut = new MapFuncFactory(funcStore, factory);

            //Act
            var mapFunc = sut.BuildFunc<object, object>();

            //Assert
            Assert.Equal(func, mapFunc);
        }

        [Fact]
        public void Get_SingleFuncInStore()
        {
            //Assert
            Func<object, object> func = (src) => src;
            var funcStore = new Dictionary<Tuple<Type, Type>, object>
            {
                { new Tuple<Type, Type>(typeof(object), typeof(object)), func }
            };
            var sut = new MapFuncFactory(funcStore);

            //Act
            var result = sut.Get<object, object>();

            //Assert
            Assert.Equal(func, result);
        }

        [Fact]
        public void Get_MultipleFuncInStore()
        {
            //Assert
            Func<object, object> func1 = (src) => src;
            Func<int, int> func2 = (src) => src;
            var funcStore = new Dictionary<Tuple<Type, Type>, object>
            {
                { new Tuple<Type, Type>(typeof(object), typeof(object)), func1 },
                { new Tuple<Type, Type>(typeof(int), typeof(int)), func2 }
            };
            var sut = new MapFuncFactory(funcStore);

            //Act
            var result1 = sut.Get<object, object>();
            var result2 = sut.Get<int, int>();

            //Assert
            Assert.Equal(func1, result1);
            Assert.Equal(func2, result2);
        }

        [Fact]
        public void Get_NoFuncInStore()
        {
            //Arrange
            var sut = new MapFuncFactory(null);

            //Act
            var mapFunc = sut.Get<object, object>();

            //Assert
            Assert.Equal(sut.DefaultFunc<object, object>, mapFunc);
        }

        private class OutClass { }
    }
}
