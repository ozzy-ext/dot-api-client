using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MyLab.ApiClient;
using TestServer;
using TestServer.Models;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace IntegrationTests
{
    public class RespContentApiClientBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly ITestOutputHelper _output;
        private readonly ApiClient<ITestServer> _client;
        private readonly ITestServer _proxy;

        /// <summary>
        /// Initializes a new instance of <see cref="RespContentApiClientBehavior"/>
        /// </summary>
        public RespContentApiClientBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _output = output;
           var clientProvider = new DelegateHttpClientProvider(webApplicationFactory.CreateClient);
           _client = new ApiClient<ITestServer>(clientProvider);
           _proxy = ApiProxy<ITestServer>.Create(clientProvider);
        }

        [Fact]
        public async Task ShouldProvideXmlResponse()
        {
            //Act
            var resp = await _client
                .Call(s => s.GetXmlObj())
                .GetResult();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Fact]
        public async Task ShouldProvideJsonResponse()
        {
            //Act
            var resp = await _client
                .Call(s => s.GetJsonObj())
                .GetResult();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Fact]
        public async Task ShouldProvideEnumerableResponse()
        {
            //Act
            var resp = await _client
                .Call(s => s.GetEnumerable())
                .GetResult();
            var respArr = resp.ToArray();

            //Assert
            Assert.Equal(2, respArr.Length);
            Assert.Contains("foo", respArr);
            Assert.Contains("bar", respArr);
        }

        [Fact]
        public async Task ShouldProvideArrayResponse()
        {
            //Act
            var resp = await _client
                .Call(s => s.GetArray())
                .GetResult();
            var respArr = resp.ToArray();

            //Assert
            Assert.Equal(2, respArr.Length);
            Assert.Contains("foo", respArr);
            Assert.Contains("bar", respArr);
        }

        [Fact]
        public async Task ShouldProvideXmlResponseWithProxy()
        {
            //Act
            var resp = await _proxy.GetXmlObj();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Fact]
        public async Task ShouldProvideJsonResponseWithProxy()
        {
            //Act
            var resp = await _proxy.GetJsonObj();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Fact]
        public async Task ShouldProvideEnumerableResponseWithProxy()
        {
            //Arrange

            //Act
            var resp = await _proxy.GetEnumerable();
            var respArr = resp.ToArray();

            //Assert
            Assert.Equal(2, respArr.Length);
            Assert.Contains("foo", respArr);
            Assert.Contains("bar", respArr);
        }

        [Fact]
        public async Task ShouldProvideArrayResponseWithProxy()
        {
            //Arrange

            //Act
            var resp = await _proxy.GetArray();
            var respArr = resp.ToArray();

            //Assert
            Assert.Equal(2, respArr.Length);
            Assert.Contains("foo", respArr);
            Assert.Contains("bar", respArr);
        }

        [Theory]
        [MemberData(nameof(GetDigitContentProvidingTestCases))]
        public async Task ShouldProvideDigitValue(string title, Func<ApiClient<ITestServer>, Task<object>> testProvider, object expected)
        {
            //Act
            var resp = await testProvider(_client);

            //Assert
            Assert.Equal(expected, resp);
        }

        [Theory]
        [MemberData(nameof(GetDigitContentProvidingTestCasesForProxy))]
        public async Task ShouldProvideDigitValueWithProxy(string title, Func<ITestServer, Task<object>> testProvider, object expected)
        {
            //Act
            var resp = await testProvider(_proxy);

            //Assert
            Assert.Equal(expected, resp);
        }

        public static IEnumerable<object[]> GetDigitContentProvidingTestCases()
        {
            yield return new object[]{ "short", (Func<ApiClient<ITestServer>, Task<object>>) (async c => (object)await c.Call(s => s.GetShort()).GetResult()), (short)10 };
            yield return new object[]{ "ushort", (Func<ApiClient<ITestServer>, Task<object>>) (async c => (object)await c.Call(s => s.GetUShort()).GetResult()), (ushort)10 };
            yield return new object[]{ "int", (Func<ApiClient<ITestServer>, Task<object>>) (async c => (object)await c.Call(s => s.GetInt()).GetResult()), 10 };
            yield return new object[]{ "uint", (Func<ApiClient<ITestServer>, Task<object>>) (async c => (object)await c.Call(s => s.GetUInt()).GetResult()), 10U };
            yield return new object[]{ "long", (Func<ApiClient<ITestServer>, Task<object>>) (async c => (object)await c.Call(s => s.GetLong()).GetResult()), 10L };
            yield return new object[]{ "ulong", (Func<ApiClient<ITestServer>, Task<object>>) (async c => (object)await c.Call(s => s.GetULong()).GetResult()), 10UL };
            yield return new object[]{ "double", (Func<ApiClient<ITestServer>, Task<object>>) (async c => (object)await c.Call(s => s.GetDouble()).GetResult()), 10.1D };
            yield return new object[]{ "float", (Func<ApiClient<ITestServer>, Task<object>>) (async c => (object)await c.Call(s => s.GetFloat()).GetResult()), 10.1F };
            yield return new object[]{ "decimal", (Func<ApiClient<ITestServer>, Task<object>>) (async c => (object)await c.Call(s => s.GetDecimal()).GetResult()), (decimal)10.1 };
        }

        public static IEnumerable<object[]> GetDigitContentProvidingTestCasesForProxy()
        {
            yield return new object[] { "short", (Func<ITestServer, Task<object>>)(async c => (object)await c.GetShort()), (short)10 };
            yield return new object[] { "ushort", (Func<ITestServer, Task<object>>)(async c => (object)await c.GetUShort()), (ushort)10 };
            yield return new object[] { "int", (Func<ITestServer, Task<object>>)(async c => (object)await c.GetInt()), 10 };
            yield return new object[] { "uint", (Func<ITestServer, Task<object>>)(async c => (object)await c.GetUInt()), 10U };
            yield return new object[] { "long", (Func<ITestServer, Task<object>>)(async c => (object)await c.GetLong()), 10L };
            yield return new object[] { "ulong", (Func<ITestServer, Task<object>>)(async c => (object)await c.GetULong()), 10UL };
            yield return new object[] { "double",(Func<ITestServer, Task<object>>)(async c => (object)await c.GetDouble()), 10.1D };
            yield return new object[] { "float", (Func<ITestServer, Task<object>>)(async c => (object)await c.GetFloat()), 10.1F };
            yield return new object[] { "decimal", (Func<ITestServer, Task<object>>)(async c => (object)await c.GetDecimal()), (decimal)10.1 };
        }



        [Api("resp-content")]
        public interface ITestServer
        {

            [Get("data/xml")]
            Task<TestModel> GetXmlObj();

            [Get("data/json")]
            Task<TestModel> GetJsonObj();

            [Get("data/enumerable")]
            Task<IEnumerable<string>> GetEnumerable();

            [Get("data/enumerable")]
            Task<string[]> GetArray();

            [Get("data/int")]
            Task<short> GetShort();
            [Get("data/int")]
            Task<ushort> GetUShort();
            [Get("data/int")]
            Task<int> GetInt();
            [Get("data/int")]
            Task<uint> GetUInt();
            [Get("data/int")]
            Task<long> GetLong();
            [Get("data/int")]
            Task<ulong> GetULong();
            [Get("data/float")]
            Task<double> GetDouble();
            [Get("data/float")]
            Task<float> GetFloat();
            [Get("data/float")]
            Task<decimal> GetDecimal();
        }
    }
}