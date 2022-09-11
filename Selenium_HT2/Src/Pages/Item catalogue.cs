using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;

namespace Selenium_HT2.Pages
{
	public class Item_catalogue
	{
		private Dictionary<string, string> Categories = new Dictionary<string, string>
		{
			{ "Mice", "https://avic.ua/ua/myishi" },
			{ "Ipad", "https://avic.ua/ua/ipad" },
			{"Acoustics", "https://avic.ua/ua/kolonki/vid--multimedijnaya-akustika"},
			{"Monitors", "https://avic.ua/ua/monitoryi/tip-monitora--igrovoj"},
			{"SmartKitchen", "https://avic.ua/ua/umnyij-dom/type--smart-kukhnya"},
			{"IPhone", "https://avic.ua/ua/iphone"}
		};

		private IWebDriver _driver;
		private WebDriverWait _wait;

		public Item_catalogue(IWebDriver driver)
		{
			_driver = driver;
			_wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			PageFactory.InitElements(driver, this);
		}

		public void GoToPage(string key)
		{
			if (!Categories.ContainsKey(key))
			{
				key = "IPhone";
			}
			_driver.Navigate().GoToUrl(Categories[key]);
		}
		public void GoToPage(int key)
		{
			if (Categories.Count<key)
			{
				key = Categories.Count-1;
			}
			_driver.Navigate().GoToUrl(Categories[Categories.Keys.ToList()[key]]);
		}

		
		
		[FindsBy(How.XPath, @"//div[contains(@class, 'item-prod')]/div/a[contains(@class, 'prod-cart__buy')][1]")]
		public IList<IWebElement> AddToCartButtons;

		[CacheLookup] [FindsBy(How.XPath, @"//div[contains(@class, '__descr')]")]
		public IList<IWebElement> ItemNames;

		public bool AddToCartIsVisible(int numberOnPage)
		{
			var actionBuilder = new Actions(_driver);
			actionBuilder.MoveToElement(AddToCartButtons[numberOnPage], 5, 5).Perform();
			return AddToCartButtons[numberOnPage].Displayed;
		}

		public void ClickAddToCartBtn(int numberOnPage)
		{
			var actionBuilder = new Actions(_driver);
			actionBuilder.MoveToElement(AddToCartButtons[numberOnPage], 5, 5).Perform();
			AddToCartButtons[numberOnPage].Click();
		}

		public string GetName(int numberOnPage)
		{
			return ItemNames[numberOnPage].GetAttribute("innerHTML");
		}
	}
}