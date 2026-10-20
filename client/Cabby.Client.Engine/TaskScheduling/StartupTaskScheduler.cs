using Cabby.Client.Engine.TaskScheduling.Logging;
using Cabby.Client.Shared.Contracts.TaskScheduling;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.TaskScheduler;

namespace Cabby.Client.Engine.TaskScheduling
{
    internal sealed class StartupTaskScheduler(ILogger<StartupTaskScheduler> logger) : IStartupTaskScheduler
    {
        private const string DefaultTaskName = "CabbyStartupTask";

        public TaskRegisterResult RegisterTaskIfNotExists(string taskName = DefaultTaskName)
        {
            if (TaskService.Instance.GetTask(taskName) != null)
            {
                logger.FailedToRegisterTask(taskName);
                return new TaskRegisterResult(
                    Success: false,
                    TaskName: taskName,
                    Message: "Task already exists.",
                    TaskAlreadyExists: true
                );
            }

            logger.RegisteringTask(taskName);

            TaskDefinition td = TaskService.Instance.NewTask();

            td.Principal.LogonType = TaskLogonType.InteractiveToken;
            td.Principal.RunLevel = TaskRunLevel.Highest;

            td.Triggers.Add(new LogonTrigger());

            td.Actions.Add(
                path: Environment.ProcessPath,
                workingDirectory: Path.GetDirectoryName(Environment.ProcessPath));

            TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, td);

            Microsoft.Win32.TaskScheduler.Task newTask = TaskService.Instance.GetTask(taskName);
            if (newTask == null)
            {
                logger.FailedToRegisterTask(taskName);
                return new TaskRegisterResult(
                    Success: false,
                    TaskName: taskName,
                    Message: "Failed to register task.",
                    TaskAlreadyExists: false
                );
            }

            return new TaskRegisterResult(
                Success: true,
                TaskName: taskName,
                Message: "Task registered successfully.",
                TaskAlreadyExists: false
            );
        }

        public TaskDeleteResult EnsureTaskIsDeleted(string taskName = DefaultTaskName)
        {
            logger.EnsuringTaskIsDeleted(taskName);
            TaskService.Instance.RootFolder.DeleteTask(taskName, false);

            Microsoft.Win32.TaskScheduler.Task nullTask = TaskService.Instance.GetTask(taskName);
            if (nullTask != null)
            {
                logger.FailedToDeleteTask(taskName);
                return new TaskDeleteResult(
                    Success: false,
                    TaskName: taskName,
                    Message: "Failed to delete task."
                );
            }

            return new TaskDeleteResult(
                Success: true,
                TaskName: taskName,
                Message: "Task deleted successfully."
            );
        }
    }
}
