using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace SauceDemoTests.Tests
{
    public class JsonFileTests
    {
        [Test]
        public void ReadAndLogJsonFile()
        {
            // กำหนด path ของไฟล์ JSON
            string jsonPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestData\keywords.json"));

            // เช็คว่าไฟล์มีอยู่หรือไม่
            Assert.IsTrue(File.Exists(jsonPath), $"File not found at {jsonPath}");

            // อ่านไฟล์ JSON ทั้งหมดเป็น string
            string jsonContent = File.ReadAllText(jsonPath);

   

            // แปลง JSON string เป็น JObject
            JObject jsonObject = JObject.Parse(jsonContent);

            // ดึง array products เป็น string[]
            string[] wantedItems = jsonObject["products"]?.ToObject<string[]>();

            // เช็คว่าได้ข้อมูลจริงหรือไม่
            Assert.IsNotNull(wantedItems, "Products array is null or missing");

            // Log ค่าใน wantedItems ทีละตัว
            TestContext.WriteLine("----- Products Array -----");
            foreach (var item in wantedItems)
            {
                TestContext.WriteLine(item);
            }
            TestContext.WriteLine("--------------------------");
        }
    }
}
