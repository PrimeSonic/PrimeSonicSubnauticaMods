#if DEBUG
namespace MoreCyclopsUpgrades.Tests
{
    using NUnit.Framework;
    using MoreCyclopsUpgrades;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    [TestFixture]
    public class TestScenarios
    {
        [Test]
        public void SortedCyclopsModules_CorrectOrder()
        {
            var modulesToPatch = new SortedCyclopsModules(7)
            {
                new DepletedNuclearModule(),
                new NuclearCharger(),
                new PowerUpgradeMk3(),
                new PowerUpgradeMk2(),
                new ThermalChargerMk2(),
                new SolarChargerMk2(),
                new SolarCharger(false),
            };

            IEnumerable<CyclopsModules> cyModsEnum = Enum.GetValues(typeof(CyclopsModules)).Cast<CyclopsModules>();

            CyclopsModules first = cyModsEnum.Min();
            CyclopsModules last = cyModsEnum.Max();

            CyclopsModules i = first;

            foreach (CyclopsModule module in modulesToPatch.Values)
            {
                Console.WriteLine($"Expected:{(int)i}:{i}");
                Assert.AreEqual(i, module.ModuleID);
                i++;
            }

            Assert.IsTrue(i > last);

        }
    }
}
#endif
