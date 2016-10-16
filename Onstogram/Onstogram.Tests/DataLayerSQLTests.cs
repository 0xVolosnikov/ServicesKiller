using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Onstogram.Model;
using Onstogram.DataLayer.SQL;

namespace Onstogram.Tests
{
    [TestClass]
    public class DataLayerSQLTests
    {
        private const string _connectionString = "Data Source=I-PC\\SQLSERVER14;Initial Catalog=Onstogram;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        [TestMethod]
        public void ShouldAddUser()
        {
            // arrange
            var user = new User
            {
                NickName = "Test"
            };
            var dataLayer = new DataLayer.SQL.DataLayer(_connectionString);
            // act
            user = dataLayer.AddUser(user);
            // asserts
            var resultUser = dataLayer.GetUser(user.Id);
            Assert.AreEqual(user.NickName, resultUser.NickName);
        }

        [TestMethod]
        public void ShouldAddImage()
        {
            // arrange
            var image = new Image
            {
                Picture = new byte[1],
                UserId = Guid.Parse("cd83ff57-33c7-4fe2-b743-5af6bdd154c1"),
                Time = DateTime.Now

            };
            image.Time = new DateTime(image.Time.Year, image.Time.Month, image.Time.Day, image.Time.Hour, image.Time.Minute, image.Time.Second, image.Time.Kind); 

            var dataLayer = new DataLayer.SQL.DataLayer(_connectionString);
            // act
            image = dataLayer.AddImage(image);
            // asserts
            var resultImage = dataLayer.GetImage(image.Id);
            Assert.AreEqual(image.Time, resultImage.Time);
        }
    }
}
