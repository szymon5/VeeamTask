using Monitor.Model;

var commandLineInput = Environment.GetCommandLineArgs();


MonitorProcess monitorProcess = new MonitorProcess(commandLineInput);
monitorProcess.GetProcesses();

var quit = Console.ReadKey();

if(quit.Equals("q")) monitorProcess.TerminateApp();