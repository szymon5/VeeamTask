using Monitor.Model;

namespace Monitor.Tests
{
    public class MonitorTest
    {
        private MonitorProcess _monitorProcess;

        [SetUp]
        public void SetUp()
        {
            _monitorProcess = new MonitorProcess(new string[] { "Monitor.exe", "notepad", "5", "1" });
        }

        [Test]
        public void GettingTheListOfCurrenltyActiveProcesses()
        {
            _monitorProcess.GetProcesses();

            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(_monitorProcess.GetProcessesQuantity.ToString());
                Assert.IsNotNull(_monitorProcess.GetProcessesQuantity);
                Assert.Greater(_monitorProcess.GetProcessesQuantity, 0);
            });
        }

        [Test]
        public void TerminateMonitoringWhenQKeyHasBeenPressed()
        {
            _monitorProcess.TerminateApp();

            Assert.IsFalse(_monitorProcess.IsTimerEnabled);
        }
    }
}