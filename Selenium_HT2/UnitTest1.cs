using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Selenium_HT2.Pages;

namespace Selenium_HT2
{
	public class Tests
	{
		private const string DestinationUrl = @"https://avic.ua/";
		private const string MonitorsUrl = @"https://avic.ua/ua/monitoryi";
		private const string HeatersUrl = @"https://avic.ua/ua/obogrevateli";
		private IWebDriver _driver;

		[SetUp]
		public void SetUp()
		{
			_driver = new ChromeDriver();
			_driver.Manage().Window.Maximize();
		}
		
		[Test]
		[TestCase(1, false)]
		[TestCase(5, false)]
		[TestCase(10, false)]
		[TestCase(20, true)]
		[TestCase(30, true)]
		public void PopupAppears(int seconds, bool shouldAppear)
		{
			var mainPage = new MainPage(_driver);
			mainPage.GoToPage();
			mainPage.WaitForPopup(seconds);
			if (shouldAppear)
			{
				const string expectedLink = @"https://avic.ua/ua/back-to-school-skidki-do-50";
             			
				Assert.True(mainPage.PromoPopUpIsVisible());
				Assert.AreEqual(expectedLink, mainPage.GetPromoLink());
			}
			else
			{
				Assert.False(mainPage.PromoPopUpIsVisible());
			}
		}

		[Test]
		public void PopupCanClose()
		{
			var mainPage = new MainPage(_driver);
			mainPage.GoToPage();
			mainPage.WaitUntilPopupVisible(60);
			Assert.True(mainPage.PromoPopUpCanClose());
			mainPage.ClosePopUp();
			Assert.False(mainPage.PromoPopUpIsVisible());
		}

		[Test]
		[TestCase(@"https://avic.ua/ua/iphone/seriya--iphone-13-promax", new []{1, 1, 1} )]
		[TestCase(@"https://avic.ua/ua/ipad/seriya--ipad-mini-2021", new []{1, 4, 1} )]
		[TestCase(@"https://avic.ua/ua/gotovyie-pk/proizvoditel--hp", new []{3, 1, 1} )]
		[TestCase(@"https://avic.ua/ua/chekhol-klaviatura-apple-smart-keyboard-folio-for-ipad-pro-11-mu8g2-item", new []{1, 8, 3} )]
		[TestCase(@"https://avic.ua/ua/monobloki/proizvoditel--lenovo", new []{3, 3, 3} )]
		[TestCase(@"https://avic.ua/ua/girobordyi-i-giroskuteryi/vid--elektrosamokat", new []{13, 2, 1} )]
		public void NavigateThroughMenuLeadsToCorrectPages(string expectedUrl, int[] childElements)
		{
			var mainPage = new MainPage(_driver);
			var navigationMenu = new CatalogueOverlay(_driver);
			mainPage.GoToPage();
			
			navigationMenu.WaitUntilVisible();
			Assert.True(navigationMenu.CatalogueBtn_IsClickable());
			navigationMenu.ClickCatalogueBtn();
			Assert.True(navigationMenu.HoverChain_ElementIsDisplayed(childElements));
			var linkOnElement = navigationMenu.HoverChain_GetLink(childElements);
			navigationMenu.HoverChain_Click(childElements);
			Assert.AreEqual(expectedUrl, linkOnElement);
			Assert.AreEqual(expectedUrl, navigationMenu.GetCurrentUrl());
		}
		
		[Test]
		[TestCase("Mice", 0)]
		[TestCase("Mice", 5)]
		[TestCase("Mice", 11)]
		[TestCase("Monitors", 0)]
		[TestCase("Monitors", 11)]
		[TestCase("IPhone", 0)]
		[TestCase("IPhone", 11)]
		public void AddToCartOneItem(string category, int numberOnPage)
		{
			var catalogue = new Item_catalogue(_driver);
			catalogue.GoToPage(category);
			Assert.True(catalogue.AddToCartIsVisible(numberOnPage));
			catalogue.ClickAddToCartBtn(numberOnPage);
			var itemNameOnPage = catalogue.GetName(numberOnPage);

			var cart = new Cart_modal(_driver);
			cart.WaitForModal();
			Assert.True(cart.ContinueBtn_IsVisible());
			
			Assert.AreEqual(itemNameOnPage, cart.GetName(0));
		}

		private static readonly object[] CalculateTestCaseData =
		{
			new object[]
			{
				new List<Tuple<int, int>>
				{
					Tuple.Create(0, 0)
				},
				new List<int> { 1 },
				new List<int> { 0 }
			},
			new object[]
			{
				new List<Tuple<int, int>>
				{
					Tuple.Create(1, 0)
				},
				new List<int> { 3 },
				new List<int> { 0 }
			},
			new object[]
			{
				new List<Tuple<int, int>>
				{
					Tuple.Create(2, 0)
				},
				new List<int> { 2 },
				new List<int> { 1 }
			},
			new object[]
			{
				new List<Tuple<int, int>>
				{
					Tuple.Create(4, 0),
					Tuple.Create(5, 0),
				},
				new List<int> { 1, 1 },
				new List<int> { 0, 0 }
			},
			new object[]
			{
				new List<Tuple<int, int>>
				{
					Tuple.Create(5, 0),
					Tuple.Create(0, 0),
					Tuple.Create(1, 0)
				},
				new List<int> { 5, 1, 1 },
				new List<int> { 0, 0, 0 }
			},
			new object[]
			{
				new List<Tuple<int, int>>
				{
					Tuple.Create(2, 0),
					Tuple.Create(3, 0),
					Tuple.Create(4, 0)
				},
				new List<int> { 3, 1, 1 },
				new List<int> { 2, 0, 0 }
			},
			new object[]
			{
				new List<Tuple<int, int>>
				{
					Tuple.Create(2, 0),
					Tuple.Create(3, 0),
					Tuple.Create(4, 0)
				},
				new List<int> { 3, 4, 2 },
				new List<int> { 0, 0, 0 }
			},
			new object[]
			{
				new List<Tuple<int, int>>
				{
					Tuple.Create(2, 0),
					Tuple.Create(3, 0),
					Tuple.Create(4, 0)
				},
				new List<int> { 3, 2, 2 },
				new List<int> { 2, 0, 0 }
			},
			new object[]
			{
				new List<Tuple<int, int>>
				{
					Tuple.Create(2, 0),
					Tuple.Create(3, 0),
					Tuple.Create(4, 0)
				},
				new List<int> { 3, 3, 3 },
				new List<int> { 2, 1, 2 }
			}
		};

		[Test]
		[TestCaseSource(nameof(CalculateTestCaseData))]
		public void PriceCalculatedProperly(List<Tuple<int, int>> itemsToAdd, List<int> itemsCountEach, List<int> extraItems)
		{
			var catalogue = new Item_catalogue(_driver);
			var cart = new Cart_modal(_driver);
			
			//ads items to cart
			foreach (var tuple in itemsToAdd)
			{
				catalogue.GoToPage(tuple.Item1);
				Assert.True(catalogue.AddToCartIsVisible(tuple.Item2));
				catalogue.ClickAddToCartBtn(tuple.Item2);
				cart.WaitForModal();
				Assert.True(cart.ContinueBtn_IsVisible());
				cart.ClickContinueBtn();
			}
			
			Assert.True(cart.CartBtnIsVisible());
			cart.ClickCartBtn();
			var count = itemsCountEach.Count;

			//click + for each item
			for (var i = 0; i < count; i++)
			{
				var k = itemsCountEach[i] + extraItems[i]-1;
				for (var j = 0; j < k; j++)
				{
					cart.ClickPlus(i);
					Thread.Sleep(1000);
				}
			}
			
			//click - for each item
			for (var i = 0; i < count; i++)
			{
				for (var j = 0; j < extraItems[i]; j++)
				{
					cart.ClickMinus(i);
					Thread.Sleep(1000);
				}
			}

			Assert.AreEqual(cart.CalculateTotalPrice(), cart.GetDisplayedTotal());
			
			/*
			_driver.Navigate().GoToUrl(MonitorsUrl);
			var buyButton =
				new WebDriverWait(_driver, TimeSpan.FromSeconds(10)).Until(wd =>
					_driver.FindElement(By.ClassName("prod-cart__buy")));
			var buttonData = buyButton.GetDomAttribute("data-ecomm-cart");
			var addToCartItemJson = (JObject)JsonConvert.DeserializeObject(buttonData);
			Debug.Assert(addToCartItemJson != null, nameof(addToCartItemJson) + " != null");
			var price = int.Parse(addToCartItemJson.GetValue("price").ToString());
			buyButton.Click();
			
			var wait =
				new WebDriverWait(_driver, TimeSpan.FromSeconds(10))
				{
					PollingInterval = TimeSpan.FromSeconds(2),
				};
			wait.IgnoreExceptionTypes( typeof(NoSuchElementException), typeof(StaleElementReferenceException));
			
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
				wait.Until(w =>
					w.FindElement(By.XPath(@"//div[@class = 'item-total']/span[@class = 'prise']")).Enabled);
				var newSpanText = wait.Until(w =>
					w.FindElement(By.XPath(@"//div[@class = 'item-total']/span[@class = 'prise']"))
						.GetAttribute("innerHTML"));

				while (newSpanText == prevSpanText)
				{
					newSpanText = wait.Until(w =>
						w.FindElement(By.XPath(@"//div[@class = 'item-total']/span[@class = 'prise']"))
							.GetAttribute("innerHTML"));
				}

				prevSpanText = newSpanText;


				var ntotalSpan = wait.Until(w =>
					w.FindElement(By.XPath(@"//div[@class = 'item-total']/span[@class = 'prise']"))
						.GetAttribute("innerHTML").Split(" ")[0]);

				totalExpected -= price;
				Assert.True(int.Parse(ntotalSpan) == totalExpected);
			}*/
		}

		[TearDown]
		public void TearDown()
		{
			_driver.Quit();
		}
	}
}