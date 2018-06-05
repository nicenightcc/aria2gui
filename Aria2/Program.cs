using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

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

            bool first;
            Mutex m = new Mutex(true, "aria2gui", out first);
            if (!first)
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
                }
                catch { }
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
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = new FileInfo(filename).DirectoryName;
                startInfo.FileName = filename;
                //设置启动动作,确保以管理员身份运行
                startInfo.Verb = "runas";
                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                }
                catch { }
                //退出
            }
            Environment.Exit(0);
            return;
        }
    }
}
