using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;
using SeleniumExtras.WaitHelpers;

namespace Selenium_HT2.Pages
{
	public class MainPage
	{
		private IWebDriver _driver;
		private WebDriverWait _wait;
		
		private string _pageUrl = @"https://avic.ua/";
		private const string PromoXpath = @"//div[@id='js_popUp']";
		public MainPage(IWebDriver driver)
		{
			this._driver = driver;
			_wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			PageFactory.InitElements(driver, this);
		}

		[FindsBy(How = How.XPath, Using = PromoXpath)]
		[CacheLookup]
		public IWebElement PromoPopup;
		
		[FindsBy(How.XPath, @"//div[@id='js_popUp']/a")]
		[CacheLookup]
		public IWebElement PromoPopupLink;
		
		[FindsBy(How.XPath, @"//div[@id='js_popUp']/button")]
		[CacheLookup]
		public IWebElement PromoPopupCloseBtn;

		public void GoToPage()
		{
			_driver.Navigate().GoToUrl(_pageUrl);
		}

		public void WaitUntilPopupVisible(int timeout)
		{
			_wait.Timeout = TimeSpan.FromSeconds(timeout);
			var elt = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(PromoXpath)));
			_wait.Timeout = TimeSpan.FromSeconds(10);
		}
		
		public void WaitForPopup(int seconds)
		{
			Thread.Sleep(TimeSpan.FromSeconds(seconds));
		}
		
		public bool PromoPopUpIsVisible()
		{
			return PromoPopup.Displayed;
		}

		public bool PromoPopUpCanClose()
		{
			return PromoPopupCloseBtn.Displayed && PromoPopupCloseBtn.Enabled;
		}

		public void ClosePopUp()
		{
			PromoPopupCloseBtn.Click();
		}

		public string GetPromoLink()
		{
			return PromoPopupLink.GetAttribute("href");
		}
	}
}