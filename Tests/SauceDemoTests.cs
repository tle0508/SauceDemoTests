
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace SauceDemoTests
{
    public class SauceDemoTests
    {
        private ChromeDriver driver;
        private List<string> log;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            log = new List<string>();
        }

        [Test]
        public void SauceDemoFlow()
        {
            // Step 1: Login
            driver.Navigate().GoToUrl("https://www.saucedemo.com/");
            log.Add("Navigated to saucedemo.com");

            driver.FindElement(By.Id("user-name")).SendKeys("visual_user");
            driver.FindElement(By.Id("password")).SendKeys("secret_sauce");
            driver.FindElement(By.Id("login-button")).Click();
            log.Add("✅Login successful");
            Thread.Sleep(2000);
            // Step 2: Add items
            string[] wantedItems = { "Sauce Labs Bolt T-Shirt", "Sauce Labs Backpack", "Sauce Labs Bike Light" };           
            var items = driver.FindElements(By.ClassName("inventory_item"));

            foreach (var item in items)
            {
                var itemName = item.FindElement(By.ClassName("inventory_item_name")).Text;
                foreach (var wanted in wantedItems)
                {
                    if (itemName.Contains(wanted))
                    {
                        item.FindElement(By.CssSelector("button.btn_inventory")).Click();
                        log.Add($"✅ Added to cart: {itemName}");                    
                        Thread.Sleep(1000);
                    }
                }
            }

            foreach (var wanted in wantedItems)
            {
                bool found = false;
                foreach (var item in items)
                {
                    var name = item.FindElement(By.ClassName("inventory_item_name")).Text;
                    if (name.Contains(wanted))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    log.Add($"❌ Item not found: {wanted}");
                }
            }

            // Step 3: Remove Backpack and verify cart
            driver.FindElement(By.ClassName("shopping_cart_link")).Click();
            Thread.Sleep(1000);
            log.Add("Navigated to cart");


            var cartItems = driver.FindElements(By.ClassName("cart_item"));
            bool removed = false;

            foreach (var item in cartItems)
            {
                var itemName = item.FindElement(By.ClassName("inventory_item_name")).Text;
                if (itemName == "Sauce Labs Backpack")
                {
                    
                    item.FindElement(By.TagName("button")).Click();
                    log.Add("✅ Removed Backpack from cart");
                    removed = true;
                    break;
                }
            }

            if (!removed)
            {
                log.Add("❌ Backpack not found in cart");
            }

            Thread.Sleep(1000);
            // Step 4: Proceed to checkout
            driver.FindElement(By.Id("checkout")).Click();
            log.Add("Clicked Checkout");
            Thread.Sleep(1000);
            driver.FindElement(By.Id("first-name")).SendKeys("sirun");
            Thread.Sleep(1000);
            driver.FindElement(By.Id("last-name")).SendKeys("sununtasinn");
            Thread.Sleep(1000);
            driver.FindElement(By.Id("postal-code")).SendKeys("58/3");
            Thread.Sleep(1000);
            driver.FindElement(By.Id("continue")).Click();
            log.Add("add info success");

            // Step 5: Verify price + tax
            var itemPrices = driver.FindElements(By.ClassName("inventory_item_price"));
            double subtotal = 0;
            foreach (var price in itemPrices)
            {
                if (double.TryParse(price.Text.Replace("$", ""), out double p))
                {
                    subtotal += p;
                }
            }

            double tax = Math.Round(subtotal * 0.08, 2);
            double expectedTotal = Math.Round(subtotal + tax, 2);

            var taxText = driver.FindElement(By.ClassName("summary_tax_label")).Text;
            var totalText = driver.FindElement(By.ClassName("summary_total_label")).Text;

            double actualTax = double.Parse(taxText.Replace("Tax: $", ""));
            double actualTotal = double.Parse(totalText.Replace("Total: $", ""));

            log.Add($"Subtotal: ${subtotal}");
            log.Add($"Expected Tax (8%): ${tax} | Actual: ${actualTax}");
            log.Add($"Expected Total: ${expectedTotal} | Actual: ${actualTotal}");


            // Step 6: Finish order
            driver.FindElement(By.Id("finish")).Click();
            string thankYouText = driver.FindElement(By.ClassName("complete-header")).Text;

            Assert.IsTrue(thankYouText.Contains("Thank you"), "Order was not completed successfully");
            log.Add("✅ Order completed successfully");

            // Step 7: All done
            log.Add("🎉 Test flow completed successfully!");
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var line in log)
            {
                TestContext.WriteLine("[LOG] " + line);
            }
            driver.Dispose();
            
        }
    }
}
