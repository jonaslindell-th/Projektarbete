using Microsoft.VisualStudio.TestTools.UnitTesting;
using Projektarbete;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace ProjektarbeteTest
{
    [TestClass()]
    public class MainWindowTests
    {
        [TestMethod()]
        public void InvalidCouponTest()
        {
            Assert.AreEqual(false, Coupon.IsValid("invalidCode"));
        }

        public void SaveCartTest()
        {

        }
    }
}