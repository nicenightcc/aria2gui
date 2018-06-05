using System;
using System.IO;
using System.Linq;

namespace Aria2
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            var filename = process.MainModule.FileName;

            var existed = System.Diagnostics.Process.GetProcessesByName(process.ProcessName).Any(p => p.Id != process.Id);
            if (existed)
            {
                try
                {
                    var client = new IPCHelper().Client("ipc://aria2channel/aria2gui");
                    if (args.Length > 0)
                    {
                        client.Send(string.Join("\r", args));
                    }
                    else
                    {
                        client.Send("show");
                    }
                    Environment.Exit(0);
                    return;
                }
                catch (Exception e)
                {
                    if (args.Length > 0) { }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("检测到有旧版本或其他Aria2程序正在运行，\r\n\r\n请将它们退出后再运行本程序！", "Aria2", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }
                }
                Environment.Exit(0);
                return;
            }

            /**
           * 当前用户是管理员的时候，直接启动应用程序
           * 如果不是管理员，则使用启动对象启动程序，以确保使用管理员身份运行
           */
            //获得当前登录的Windows用户标示
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
            //判断当前登录用户是否为管理员
            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                //如果是管理员，则直接运行
                Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, "lib");
                new StartUp().Start();
            }
            else
            {
                //创建启动对象
                var p = new System.Diagnostics.Process();
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.WorkingDirectory = new FileInfo(filename).DirectoryName;
                p.StartInfo.FileName = filename;
                //设置启动动作,确保以管理员身份运行
                p.StartInfo.Verb = "runas";
                try
                {
                    p.Start();
                }
                catch { }
                //退出
            }
            Environment.Exit(0);
            return;
        }
    }
}
