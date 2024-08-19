using static System.Net.Mime.MediaTypeNames;
namespace CRUDTests
{
    [TestClass]
    public class BasicTests
    {
        //private Program test;

        [TestMethod]
        public void AppExitsCorrectly()
        {
            Program.Menu();
           
        }
    }
}