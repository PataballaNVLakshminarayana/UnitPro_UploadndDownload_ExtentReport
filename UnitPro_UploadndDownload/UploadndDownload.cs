using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System.IO;
using OpenQA.Selenium.Support.UI;
using AventStack.ExtentReports;
using OpenQA.Selenium.DevTools.V143.DOM;
using AventStack.ExtentReports.Reporter;

namespace UnitPro_UploadndDownload
{
    [TestFixture]
    public class UploadndDownload
    {
        private static IWebDriver _driver;
        private static ChromeOptions _options;
        public static ExtentReports _EReports;
        public static ExtentTest _ETest;
        public TestContext instance;
        public TestContext TestContext
        {
            set { instance = value; }
            get { return instance; }
        }
        [OneTimeSetUp]
        public void CreateReport()
        {
            var reportpath = Path.Combine(Directory.GetCurrentDirectory(), "TestReport");
            Directory.CreateDirectory(reportpath);
            var filepath = Path.Combine(reportpath, "TestReport.html");
            _EReports = new ExtentReports();
            var sparkReporter = new ExtentSparkReporter(filepath);
            _EReports.AttachReporter(sparkReporter);
        }
        [SetUp]
        public void DriverInit()
        {
            _options = new ChromeOptions();
            // configure download preferences before creating the driver so the created
            // Chrome instance picks them up. This also avoids creating multiple
            // ChromeDriver instances which left orphaned processes.
            string downloadpath = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
            _options.AddUserProfilePreference("download.default_directory", downloadpath);
            _options.AddUserProfilePreference("download.prompt_for_download", false);
            _options.AddUserProfilePreference("disable-popup-blocking", true);

            _driver = new ChromeDriver(_options);
            _driver.Manage().Window.Maximize();
            _driver.Navigate().GoToUrl("https://demoqa.com/upload-download");
            _ETest = _EReports.CreateTest(TestContext.CurrentContext.Test.Name);
        }
        [Test]
        public void TC_UploadFile()
        {
            WebDriverWait _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            By _file = By.XPath("//input[@id='uploadFile']");
            IWebElement _filepath=_wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(_file));
            string filedomine = AppDomain.CurrentDomain.BaseDirectory;
            string filepath = Path.Combine(filedomine, "DataSets", "Credentials_valied.txt");
            filepath=Path.GetFullPath(filepath);
            _filepath.SendKeys(filepath);
            _ETest.Log(Status.Pass, "File uploaded successfully");
        }
        [Test]
        public void TC_DownloadFile()
        {
            WebDriverWait _wait=new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            By _downloadfile=By.XPath("//a[@id='downloadButton']");
            IWebElement _download=_wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(_downloadfile));
            _download.Click();
            // do not close the driver here; TearDown will quit and clean up the browser
            _ETest.Log(Status.Pass, "File uploaded successfully");
        }
        [TearDown]
        public void DriverQuit()
        {
            if (_driver!=null)
            {
                _driver.Quit();
                _driver = null;
            }
        }
        [OneTimeTearDown]
        public void EndReport()
        {
            _EReports.Flush(); // ✅ correct place
        }
    }
}
