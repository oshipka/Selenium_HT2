using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;
using SeleniumExtras.WaitHelpers;

namespace Selenium_HT2.Pages
{
	public class Cart_modal
	{
		
		private IWebDriver _driver;
		private WebDriverWait _wait;


		public Cart_modal(IWebDriver driver)
		{
			_driver = driver;
			_wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			PageFactory.InitElements(driver, this);
		}

		public void WaitForModal()
		{
			var el = _wait.Until(ExpectedConditions.ElementIsVisible(
				By.XPath(@"//div[contains(@class,'current')]/div[contains(@class, 'cartModal')]")));
		}
		
		[FindsBy(How.XPath, @"//a/div/div[contains(@class, 'active-cart')]/../..")]
		public IWebElement CartBtn;
		
		[FindsBy(How.XPath, @"//div/a[ contains(@class, 'orange')or contains(text(), 'Продовжити покупки')]")]
		public IWebElement ContinueShoppingBtn;

		[FindsBy(How.XPath, @"//span[@class = 'js_plus btn-count btn-count--plus ']")]
		public IList<IWebElement> PlusButtons;

		[FindsBy(How.XPath, @"//span[@class = 'js_minus btn-count btn-count--minus ']")]
		public IList<IWebElement> MinusButtons;

		[FindsBy(How.XPath, @"//div[@class = 'total-h']/span[@class = 'prise']")]
		public IList<IWebElement> Prices;

		[FindsBy(How.XPath, @"//input[contains(@class, 'count')]")]
		public IList<IWebElement> Quantities;

		[FindsBy(How.XPath, @"//span[@class='name']")]
		public IList<IWebElement> ItemNames;

		[FindsBy(How.XPath, @"//div[@class = 'item-total']/span[@class = 'prise']")]
		public IWebElement TotalDisplayed;

		public bool ContinueBtn_IsVisible()
		{
			return ContinueShoppingBtn.Displayed;
		}

		public void ClickContinueBtn()
		{
			ContinueShoppingBtn.Click();
		}

		public string GetName(int numberInCart)
		{
			return ItemNames[numberInCart].GetAttribute("innerHTML");
		}

		public void ClickPlus(int i)
		{
			PlusButtons[i].Click();
		}

		public void ClickMinus(int i)
		{
			MinusButtons[i].Click();
		}

		private List<int> GetQuantities()
		{
			return Quantities.Select(element => int.Parse(element.GetAttribute("value"))).ToList();
		}

		private List<int> GetPrices()
		{
			return Prices.Select(element => int.Parse(element.GetAttribute("innerHTML").Split(" ")[0])).ToList();
		}

		public int CalculateTotalPrice()
		{
			var prices = GetPrices();
			var quantities = GetQuantities();
			if (prices.Count != quantities.Count)
			{
				Assert.Fail();
			}

			var iMax = prices.Count;
			var total = 0;
			for (var i = 0; i < iMax; i++)
			{
				total += prices[i] * quantities[i];
			}

			return total;
		}

		public int GetDisplayedTotal()
		{
			return int.Parse(TotalDisplayed.GetAttribute("innerHTML").Split(" ")[0]);
		}

		public bool CartBtnIsVisible()
		{
			return CartBtn.Displayed;
		}

		public void ClickCartBtn()
		{
			CartBtn.Click();
		}
	}
}