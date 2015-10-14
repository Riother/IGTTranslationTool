using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IGTLocalizer.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using IGTLocalizer;
namespace Tests
{
   

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestSerialization()
        {
            ClientLibrary cl = new ClientLibrary(new List<Client>(){
                new Client(new Dictionary<String, String>(){
                    {"testKey", "testValue"},
                    {"Default", "demo"},
                }), 
                new Client(new Dictionary<String, String>(){
                    {"testKeyClient2", "testValue"},
                    {"DefaultClient2", "demo"},
                })
            });
            string serializedJson = JsonConvert.SerializeObject(cl);

            ClientJsonParser jsonParser = new ClientJsonParser(serializedJson);
            Assert.IsTrue(serializedJson.Equals(jsonParser.serializeClients()));
        }
    }
}


//{
//  "Default":
//    {
//      "demo":"Demo", 
//      "instructions":"How To Play"
//    },
//   "client2":
//    {
//      "demo":"Demo2", 
//      "instructions":"How Not To Play"
//    },
//}

//should parse this into a client library with an array of 2 clients
//each client should have 2 text vars