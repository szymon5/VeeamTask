using System.Diagnostics;

namespace Monitor.Model
{
    public class MonitorProcess
    {
        private const int MILLISECONDS_IN_ONE_MINUTE = 60000;
        private string processName;
        private double maxProcessLifeTime;
        private int frequency;
        private System.Timers.Timer timerForKillTheProcesses;
        private string fullPath;
        private string logFilePath;
        private List<Process> processes;
        private List<ProcessModel> processesModels;
        private EqualityComparer equalityComparer;

        public int GetProcessesQuantity => processesModels.Count;
        public bool IsTimerEnabled => timerForKillTheProcesses.Enabled;

        public MonitorProcess(string[] input) 
        {
            //index [0] contains path to the file so indexes are being parsed from index [1]
            processName = input[1];
            maxProcessLifeTime = Convert.ToDouble(input[2]);
            frequency = Convert.ToInt32(input[3]) + MILLISECONDS_IN_ONE_MINUTE;

            fullPath = Path.GetFullPath("Monitor");
            logFilePath = fullPath.Remove(fullPath.IndexOf(@"bin\")) + @"Log\logs.txt";
            
            timerForKillTheProcesses = new System.Timers.Timer(frequency);
            processes = new List<Process>();
            processesModels = new List<ProcessModel>();
            equalityComparer = new EqualityComparer();

            timerForKillTheProcesses.Elapsed += Timer_Elapsed;
            timerForKillTheProcesses.Start();
        }

        public void GetProcesses()
        {
            //if list is empty, get processes to list
            if (processes.Count == 0) processes = Process.GetProcesses().ToList();
            else
            {
                //update the processes list. Get and assign processes to list.
                processes.AddRange(Process.GetProcesses().ToList());
                processes = processes.Distinct(equalityComparer).ToList();//elimination of duplikates using IEqualityComparer
            }

            //assign processes to list
            for(int i=0;i< processes.Count;i++)
            {
                processesModels.Add(new ProcessModel()
                {
                    Process = processes[i],
                    StartTimeOfExistance = DateTime.Now,//this variable contains the start date of the process.
                });
            }
        }

        private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            var curDate = DateTime.Now;

            //if start time in minutes is bigger than max lifetime allowed, the prepare list to delete
            var processesToKill = processesModels.Where(p => (curDate - p.StartTimeOfExistance).TotalMinutes > maxProcessLifeTime && 
                                                           p.Process.ProcessName.ToLower().Contains(processName.ToLower())).ToList();

            if (processesToKill.Count > 0)
            {
                for (int i = 0; i < processesToKill.Count; i++)
                {
                    processesToKill[i].Process.Kill();

                    //add to log.txt file which process has been killed and at which time
                    await File.AppendAllTextAsync(logFilePath,
                        $"Log from {DateTime.Now}: the process: " +
                        $"[{processesToKill[i].Process.ProcessName} - " +
                        $"{processesToKill[i].Process.Id} - " +
                        $"{processesToKill[i].Process.MainWindowTitle}] has been killed." +
                        $"{Environment.NewLine}");

                    //remove process from the list
                    processesModels.Remove(processesToKill[i]);
                }
            }

            GetProcesses();//update list of processes
        }

        public void TerminateApp()
        {
            timerForKillTheProcesses.Stop();
            timerForKillTheProcesses.Elapsed -= Timer_Elapsed;
        }
    }
}
