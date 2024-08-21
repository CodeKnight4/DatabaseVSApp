using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;
namespace CRUDTests
{
    [TestClass]
    public class BasicTests
    {
        string[] actorColumnList = ["First_Name", "Last_Name", "Date_Of_Birth", "Country_Of_Birth"];
        string[] movieColumnList = ["Title", "Description", "Release_Year", "Running_Length_Mins", "IMDb_Rating", "Reviews", "Director"];

        [TestMethod]
        public void AppExitsCorrectly()
        {
            try
            {
                Program.Menu();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Method didn't correctly run, Exception: {ex.Message}");
            }

        }

        [TestMethod]
        public void CorrectTableIsEntered()
        {
            string table = "Actor";
            Assert.AreEqual(Program.ValidateTable(table), true);
        }

        [TestMethod]
        public void IncorrectTableIsEntered()
        {
            string table = "Fireman";
            Assert.AreEqual(Program.ValidateTable(table), false);
        }

        [TestMethod]
        public void TestConnectionToDatabase()
        {
            Assert.IsInstanceOfType(Program.InitApp(), typeof(SqlConnection));
        }

        [TestMethod]
        public void DoesCreateMethodRunWithoutException()
        {
            try
            {
                Program.CreateMethod(Program.InitApp());
            }
            catch (Exception ex)
            {
                Assert.Fail($"Method didn't correctly run, Exception: {ex.Message}");
            }
        }

        [TestMethod]
        public void DoesReadMethodRunWithoutException()
        {
            try
            {
                Program.ReadMethod(Program.InitApp());
            }
            catch (Exception ex)
            {
                Assert.Fail($"Method didn't correctly run, Exception: {ex.Message}");
            }
        }

        [TestMethod]
        public void DoesUpdateMethodRunWithoutException()
        {
            //var test = Program.InitApp();
            try
            {
                Program.UpdateMethod(Program.InitApp());
            }
            catch (Exception ex)
            {
                Assert.Fail($"Method didn't correctly run, Exception: {ex.Message}");
            }
        }

        [TestMethod]
        public void DoesDeleteMethodRunWithoutException()
        {
            //var test = Program.InitApp();
            try
            {
                Program.DeleteMethod(Program.InitApp());
            }
            catch (Exception ex)
            {
                Assert.Fail($"Method didn't correctly run, Exception: {ex.Message}");
            }
        }

        [TestMethod]
        public void EnterValidColumn()
        {
        }

        [TestMethod]
        public void EnterInvalidColumn()
        {
        }

        [TestMethod]
        public void EnterValidDate()
        {
        }

        [TestMethod]
        public void EnterInvalidDate()
        {
        }

        [TestMethod]
        public void EnterValidYear()
        {
        }

        [TestMethod]
        public void EnterInvalidYear()
        {
        }

        [TestMethod]
        public void EnterValidLength()
        {
        }

        [TestMethod]
        public void EnterInvalidLength()
        {
        }

        [TestMethod]
        public void EnterValidRating()
        {
        }

        [TestMethod]
        public void EnterInvalidRating()
        {
        }
    }
}