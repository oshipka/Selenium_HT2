using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Selenium_HT2
{
	public class Tests
	{
		private const string DestinationUrl = @"https://avic.ua/";
		private const string MonitorsUrl = @"https://avic.ua/ua/monitoryi";
		private const string HeatersUrl = @"https://avic.ua/ua/obogrevateli";
		
		[Test]
		public void PopupAppears()
		{
			var xpathtoclosepopup = @"//div[@id='js_popUp']/button[@title = 'Close']";
			var linkToPromo = @"/ua/back-to-school-skidki-do-50";
			
			IWebDriver driver = new ChromeDriver();
			driver.Navigate().GoToUrl(DestinationUrl);
			//Assert.IsTrue(driver.Url.Contains("avic"), "");
			var popup = (new WebDriverWait(driver, TimeSpan.FromMinutes(1))).Until(element => element.FindElement(By.XPath(@"//div[@id='js_popUp']")));
			Assert.NotNull(popup);
			Assert.AreEqual( driver.FindElement(By.XPath(@"//div[@id='js_popUp']/a[@href = '/ua/back-to-school-skidki-do-50']")), popup.FindElement(By.TagName("a")));
			var closeButton = (new WebDriverWait(driver, TimeSpan.FromSeconds(60))).Until(e=>popup.FindElement(By.TagName("button")));
			closeButton.Click();
			driver.Quit();
		}

		[Test]
		public void NavigateThroughMenuToHeaters()
		{
			IWebDriver driver = new ChromeDriver();
			driver.Manage().Window.Maximize();
			driver.Navigate().GoToUrl(DestinationUrl);
			var wait =
				new WebDriverWait(driver, TimeSpan.FromSeconds(20))
				{
					PollingInterval = TimeSpan.FromSeconds(2),
				};

			var toNewPage = wait.Until(w => w.FindElement(By.XPath(@"//img[@alt= 'Побутова техніка']/../..")));
			Assert.True(toNewPage.Displayed);
			toNewPage.Click();
			
			var toThirdPage = wait.Until(w => w.FindElement(By.XPath(@"//div[@class='brand-box__title']/a[text()='Кліматична техніка']")));
			Assert.True(toThirdPage.Displayed);
			toThirdPage.Click();

			var toFourthPage = wait.Until(w => w.FindElement(By.XPath(@"//div[@class='brand-box__title']/a[text() = 'Обігрівачі']")));
			Assert.True(toFourthPage.Displayed);
			toFourthPage.Click();

			Assert.AreEqual(HeatersUrl, driver.Url);
			driver.Quit();
		}
		
		[Test]
		public void AddToCartOneItem()
		{
			IWebDriver driver = new ChromeDriver();
			driver.Manage().Window.Maximize();
			driver.Navigate().GoToUrl(MonitorsUrl);
			var buyButton =
				new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(wd =>
					driver.FindElement(By.ClassName("prod-cart__buy")));
			var buttonData = buyButton.GetDomAttribute("data-ecomm-cart");
			var addToCartItemJson = (JObject)JsonConvert.DeserializeObject(buttonData);
			Debug.Assert(addToCartItemJson != null, nameof(addToCartItemJson) + " != null");
			var productName = addToCartItemJson.GetValue("name").ToString();
			buyButton.Click();
			var wait =
				new WebDriverWait(driver, TimeSpan.FromSeconds(10))
				{
					PollingInterval = TimeSpan.FromSeconds(2),
				};
			wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
			var singleItem =
				wait.Until( wd =>
					wd.FindElement(By.XPath(@"//div[@id='js_cart']/div/div[@class='cart-parent']/div/div[@class='item']")));
			var divData = singleItem.GetDomAttribute("data-ecomm-cart");
			var inCartItemJson = (JObject)JsonConvert.DeserializeObject(divData);
			Assert.AreEqual(addToCartItemJson, inCartItemJson);
			Assert.NotNull(inCartItemJson.GetValue("name"));
			Assert.AreEqual(productName, inCartItemJson.GetValue("name").ToString());
			driver.Quit();
		}

		[Test]
		public void PriceCalculatedProperly()
		{
			IWebDriver driver = new ChromeDriver();
			driver.Manage().Window.Maximize();
			driver.Navigate().GoToUrl(MonitorsUrl);
			var buyButton =
				new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(wd =>
					driver.FindElement(By.ClassName("prod-cart__buy")));
			var buttonData = buyButton.GetDomAttribute("data-ecomm-cart");
			var addToCartItemJson = (JObject)JsonConvert.DeserializeObject(buttonData);
			Debug.Assert(addToCartItemJson != null, nameof(addToCartItemJson) + " != null");
			var productName = addToCartItemJson.GetValue("name").ToString();
			var price = Int32.Parse(addToCartItemJson.GetValue("price").ToString());
			buyButton.Click();
			
			var wait =
				new WebDriverWait(driver, TimeSpan.FromSeconds(10))
				{
					PollingInterval = TimeSpan.FromSeconds(2),
				};
			wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
			
			var add = new Random().Next(2, 10);
			var sub = new Random().Next(1, add);
			var prevSpanText = wait.Until(w =>
				w.FindElement(By.XPath(@"//div[@class = 'item-total']/span[@class = 'prise']"))
					.GetAttribute("innerHTML"));
			var totalExpected = price;
			
			for (var i = 0; i < add; i++)
			{
				wait.Until(w =>
					w.FindElement(By.XPath("//span[@class = 'js_plus btn-count btn-count--plus ']"))).Click();

				var newSpanText = wait.Until(w =>
					w.FindElement(By.XPath(@"//div[@class = 'item-total']/span[@class = 'prise']"))
						.GetAttribute("innerHTML"));
				Console.WriteLine(newSpanText);
				while (newSpanText==prevSpanText)
				{
					newSpanText = wait.Until(w =>
						w.FindElement(By.XPath(@"//div[@class = 'item-total']/span[@class = 'prise']"))
							.GetAttribute("innerHTML"));
				}

				prevSpanText = newSpanText;
				var totalSpan = wait.Until(w =>
					w.FindElement(By.XPath(@"//div[@class = 'item-total']/span[@class = 'prise']")).GetAttribute("innerHTML").Split(" ")[0]);
				totalExpected += price;
				Assert.True(int.Parse(totalSpan)==totalExpected);
			}
			for (var i = 0; i < sub; i++)
			{
				var minusButton = wait.Until(w =>
					w.FindElement(By.XPath("//span[@class = 'js_minus btn-count btn-count--minus ']")));
				minusButton.Click();
				wait.Until(w=>w.FindElement(By.XPath(@"//div[@class = 'item-total']/span[@class = 'prise']")).Enabled);
				var newSpanText = wait.Until(w =>
					w.FindElement(By.XPath(@"//div[@class = 'item-total']/span[@class = 'prise']"))
						.GetAttribute("innerHTML"));
				while (newSpanText==prevSpanText)
				{
					newSpanText = wait.Until(w =>
						w.FindElement(By.XPath(@"//div[@class = 'item-total']/span[@class = 'prise']"))
							.GetAttribute("innerHTML"));
				}

				prevSpanText = newSpanText;
				
				
				var ntotalSpan = wait.Until(w =>
					w.FindElement(By.XPath(@"//div[@class = 'item-total']/span[@class = 'prise']")).GetAttribute("innerHTML").Split(" ")[0]);

				totalExpected -= price;
				Assert.True(int.Parse(ntotalSpan)==totalExpected);
			}
		} 
	}
}