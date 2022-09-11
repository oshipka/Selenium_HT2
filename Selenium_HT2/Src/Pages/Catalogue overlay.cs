using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;
using SeleniumExtras.WaitHelpers;

namespace Selenium_HT2.Pages
{
	public class CatalogueOverlay
	{
		
		private IWebDriver _driver;
		private WebDriverWait _wait;
		private const string SidebarBtnXpath = @"//span[@class = 'sidebar-item']";

		public CatalogueOverlay(IWebDriver driver)
		{
			_driver = driver;
			_wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
			PageFactory.InitElements(driver, this);
		}

		[FindsBy(How.XPath, SidebarBtnXpath)]
		[CacheLookup]
		private IWebElement _catalogueBtn;


		public void WaitUntilVisible()
		{
			var elt = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(SidebarBtnXpath)));
		}

		public void ClickCatalogueBtn()
		{
			_catalogueBtn.Click();
		}

		public bool CatalogueBtn_IsClickable()
		{
			return _catalogueBtn.Displayed && _catalogueBtn.Enabled;
		}

		private void FindChildrenInCatalogueTree(int[] layerElementNumbers, out IWebElement firstChild,
			out IWebElement secondChild,
			out IWebElement thirdChild)
		{

			var first =
				$@"//div[contains(@class, 'first-level')]/ul/li[contains(@class, 'js_sidebar-item')][{layerElementNumbers[0]}]";
			var second =
				$@"/div[contains(@class, 'second-level')]/ul/li[contains(@class, 'js_sidebar-item')][{layerElementNumbers[1]}]";
			var third = $@"/div/ul[contains(@class, 'third-level-item')]/li[{layerElementNumbers[2]}]";

			Console.WriteLine($"First: {first}\nSecond: {second}\nThird: {third}");
			firstChild = _driver.FindElement(By.XPath(first));
			secondChild = firstChild.FindElement(By.XPath(first+second));
			thirdChild = secondChild.FindElement(By.XPath(first+second+third));
		}

		public bool HoverChain_ElementIsDisplayed(int[] layerElementNumbers)
		{
			var actionBuilder = new Actions(_driver);

			FindChildrenInCatalogueTree(layerElementNumbers, out var firstChild, out var secondChild,
				out var thirdChild);

			actionBuilder.MoveToElement(firstChild).MoveByOffset(20, 0).MoveToElement(secondChild).MoveByOffset(20, 0).MoveToElement(thirdChild).Perform(); 
			Thread.Sleep(1000);
			return thirdChild.Displayed;
		}

		public string HoverChain_GetLink(int[] layerElementNumbers)
		{
			FindChildrenInCatalogueTree(layerElementNumbers, out var firstChild, out var secondChild,
				out var thirdChild);
			return thirdChild.FindElement(By.TagName("a")).GetAttribute("href");
		}

		public void HoverChain_Click(int[] layerElementNumbers)
		{
			FindChildrenInCatalogueTree(layerElementNumbers, out var firstChild, out var secondChild,
				out var thirdChild);

			var actionBuilder = new Actions(_driver);
			actionBuilder.MoveToElement(firstChild).MoveByOffset(20, 0).MoveToElement(secondChild).MoveByOffset(20, 0).MoveToElement(thirdChild);
			Thread.Sleep(1000);
			actionBuilder.Click().Build().Perform();
		}

		public string GetCurrentUrl()
		{
			return _driver.Url;
		}

		public void RefreshPage()
		{
			_driver.Navigate().Refresh();
		}
	}
}