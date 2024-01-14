using System.Diagnostics;

Process process = new();

process.StartInfo.FileName = "cmd.exe";
process.StartInfo.Arguments = "/K init.cmd";
process.StartInfo.UseShellExecute = true;

process.Start();