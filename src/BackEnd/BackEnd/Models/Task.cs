using Autofac;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackEnd.Models
{
    /// <summary>
    /// Task class to execute some action at a certain time
    /// </summary>
    public class Task
    {
        public enum Operations { TurnOn, TurnOff };
        public enum TaskTypes { OneTime, Repeated };

        public int TaskId { get; set; }
        public string Description { get; set; }
        public Operations Operation { get; set; }
        public string DeviceMac { get; set; }
        public TaskTypes TaskType { get; set; }
        public int RepeatEvery { get; set; }
        public DateTime StartDate { get; set; }
        public string JobId { get; set; }

        public Task() { }

        public Task(Operations op)
        {
            Operation = op;
        }

        public static void Execute(Operations op, string mac)
        {
            // when entering this function we need to execute the task
            Plug device;
            using (ILifetimeScope scope = Program.Container.BeginLifetimeScope())
            {
                SmartSwitchDbContext context = scope.Resolve<SmartSwitchDbContext>();
                device = context.Plugs.Find(mac);
                Execute(op, device, context);
            }
        }

        public static void Execute(Operations op, Plug device)
        {
            switch (op)
            {
                case Operations.TurnOn:
                    device.TurnOn();
                    break;
                case Operations.TurnOff:
                    device.TurnOff();
                    break;
            }
        }

        public static void Execute(Operations op, Plug device, SmartSwitchDbContext context)
        {
            switch (op)
            {
                case Operations.TurnOn:
                    device.TurnOn(context);
                    break;
                case Operations.TurnOff:
                    device.TurnOff(context);
                    break;
            }
        }

        public void Schedule()
        {
            switch (TaskType)
            {
                case TaskTypes.OneTime:
                    JobId = BackgroundJob.Schedule(() => Execute(Operation, DeviceMac), StartDate - DateTime.Now);
                    break;
                case TaskTypes.Repeated:
                    JobId = BackgroundJob.Schedule(() => ExecuteAndScheduleNextExecution(Operation, DeviceMac, RepeatEvery, TaskId), StartDate - DateTime.Now);
                    break;
            }
        }

        public void Delete() => BackgroundJob.Delete(JobId);

        public static void ExecuteAndScheduleNextExecution(Operations operation, string mac, int repeatEvery, int taskId)
        {
            using (ILifetimeScope scope = Program.Container.BeginLifetimeScope())
            {
                SmartSwitchDbContext context = scope.Resolve<SmartSwitchDbContext>();
                var tasks = context.Tasks;
                var task = tasks.Find(taskId);
                if (task == null) throw new Exception("task is null");
                task.JobId = BackgroundJob.Schedule(() => ExecuteAndScheduleNextExecution(operation, mac, repeatEvery, taskId), TimeSpan.FromMinutes(repeatEvery));
                context.SaveChanges();
                Execute(operation, context.Plugs.Find(mac), context);
            }
        }
    }
}