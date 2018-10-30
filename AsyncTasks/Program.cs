using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AoCli;

namespace AsyncTasks
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    List<Task> tasksToWait = new List<Task>();

        //    new List<string>() { "https://www.baidu.com/", "https://www.alibaba.com/", "https://www.tencent.com/zh-cn/index.html" }
        //    .Select((url) =>
        //    {
        //        var taskLife = new TaskLife<byte[]>()
        //        {
        //            Url = url
        //        };
        //        var client = new WebClient();
        //        var task = client.DownloadDataTaskAsync(new Uri(taskLife.Url));
        //        taskLife.FirstTask = task;
        //        Console.WriteLine(String.Format("[{0}] {1} started download", DateTime.Now.ToFullTimeSecondString(), taskLife.Url));
        //        return taskLife;
        //    })
        //    .ToList()
        //    .ForEach((taskLife) =>
        //    {
        //        var task = Task.Factory.StartNew(() =>
        //        {
        //            taskLife.FirstTask.Wait();
        //            //Console.WriteLine(String.Format("[{0}] {1} length: {2}", DateTime.Now.ToFullTimeSecondString(), taskLife.Url, null));
        //            Console.WriteLine(String.Format("[{0}] {1} length: {2}", DateTime.Now.ToFullTimeSecondString(), taskLife.Url, taskLife.FirstTask.Result.Length));
        //        });
        //        tasksToWait.Add(taskLife.FirstTask);
        //        //tasksToWait.Add(task);
        //    });

        //    Task.Factory.StartNew(() =>
        //    {
        //        Task.WaitAll(tasksToWait.ToArray());
        //        Console.WriteLine(String.Format("[{0}] all downloaded", DateTime.Now.ToFullTimeSecondString()));
        //    });

        //    Console.WriteLine(String.Format("[{0}] all downloaded in main", DateTime.Now.ToFullTimeSecondString()));
        //    Console.ReadLine();
        //}


        static void Main(string[] args)
        {
            int invokedLimit = args.Length == 0 ? 4 : Convert.ToInt32(args[0]);

            InvokedTimes = InvokedTimes + 1;
            Console.WriteLine(String.Format("{0} times invoked", InvokedTimes));

            if (InvokedTimes > invokedLimit)
            {
                return;
            }

            var tasksToWaitInMain = new List<Task>();
            new List<string>() {
                "https://www.baidu.com/",
                "https://www.alibaba.com/",
                "https://www.tencent.com/zh-cn/index.html"
            }
            .ForEach((url) =>
            {
                tasksToWaitInMain.Add(Task.Factory.StartNew(() =>
                {
                    var task = new WebClient().DownloadDataTaskAsync(new Uri(url));
                    Console.WriteLine(String.Format("{0} {1} started download", DateTime.Now.ToFullTimeSecondString(), url));
                    task.ContinueWith((taskOfBytes) =>
                    {
                        Console.WriteLine(String.Format("{0} {1} downloaded, lenght: {2}", DateTime.Now.ToFullTimeSecondString(), url, taskOfBytes.Result.Length));
                        //Action action = () => { Console.WriteLine(); };

                    });
                    task.Wait();
                }));
            });

            Task.WaitAll(tasksToWaitInMain.ToArray());
            Console.WriteLine(String.Format("{0} Press Any Key To Exit", DateTime.Now.ToFullTimeSecondString()));

            Task.Factory.StartNew(() =>
            {
                Main(new List<string>() { invokedLimit.ToString() }.ToArray());
            });

            Console.ReadLine();

        }

        public static int InvokedTimes { get; set; } = 0;

        public class TaskLife<T>
        {
            public string Url { get; set; }
            public Task<T> FirstTask { get; set; }
        }
    }
}
