﻿using Microsoft.Build.Tasks;
using System;
using System.IO;
using TaskScheduler;

namespace TaskSchelder
{
    /// <summary>
    /// Класс который помогает в получении задач из планировщика задач
    /// </summary>
    class TaskCharp
    {
        const int TASK_FLAG_HIDDEN = (int)TaskScheduler._TASK_ENUM_FLAGS.TASK_ENUM_HIDDEN;
        /// <summary>
        /// Директория текущего каталога
        /// </summary>
        public ITaskFolder current { get; set; }
        /// <summary>
        /// Родительская директория текущего каталога
        /// </summary>
        public ITaskFolder parent { get; set; }
        private bool init { get; set; } = false;
        private TaskScheduler.TaskScheduler taskService;

        public delegate void TaskSchelderTaskHandler(IRegisteredTask task);
        /// <summary>
        /// Событие действия при получении задачи
        /// </summary>
        public event TaskSchelderTaskHandler ActOnTask;
        public delegate void TaskSchelderFolderHandler(ITaskFolder task);
        /// <summary>
        /// Событие действия при получении папки
        /// </summary>
        public event TaskSchelderFolderHandler ActOnFolder;
        public delegate void TaskSchelderHandlerStart();
        /// <summary>
        /// Событие начала получения задач и папок
        /// </summary>
        public event TaskSchelderHandlerStart ActOnStart;
        public delegate void TaskSchelderHandler();
        /// <summary>
        /// Событие завершения получения задач 
        /// </summary>
        public event TaskSchelderHandler ActOnProgress;


        /// <summary>
        /// Получает информацию о задаче 
        /// </summary>
        /// <param name="name">Имя задачи</param>
        /// <param name="path">Путь к задаче</param>
        /// <returns>Экземпляр <see cref="TaskInfo"/></returns>
        public TaskInfo GetTask(string name,string path="\\")
        {
            ITaskFolder folder = taskService.GetFolder("\\");
            if (path!="\\")folder = taskService.GetFolder(path.Remove(path.LastIndexOf(name)-1));
            foreach (IRegisteredTask task in folder.GetTasks(TASK_FLAG_HIDDEN))
            {
                if (task.Name == name)
                {
                    return new TaskInfo(task.Name, task.Path, task.Enabled, task.LastRunTime, task.LastTaskResult, task.NextRunTime, task.State);
                }
            }
            return null;
        }

        private void InitTaskSharp()
        {
            if (!init) { taskService = new TaskScheduler.TaskScheduler();taskService.Connect(); init = true; }
        }
        private void EnumFolderTasks(ITaskFolder fld)
        {
            if (ActOnStart != null)
                ActOnStart();
            current = fld;
            if (ActOnFolder != null)
               foreach (ITaskFolder sfld in fld.GetFolders(TASK_FLAG_HIDDEN))
                {
                    ActOnFolder(sfld);
                }
            if (ActOnTask != null)
                foreach (IRegisteredTask task in fld.GetTasks(TASK_FLAG_HIDDEN))
                {
                    ActOnTask(task);
                }
            if (ActOnProgress != null)
                ActOnProgress();
            parent = taskService.GetFolder(fld.Path.Remove(current.Path.LastIndexOf('\\')));
        }


        /// <summary>
        /// Получение задач и папок в определенной директории 
        /// </summary>
        /// <exception cref="IOException"></exception>
        public void EnumAllTasks(string path = "\\")
        {
            InitTaskSharp();
            ITaskFolder rootFolder;
            rootFolder = taskService.GetFolder(path);
            current = rootFolder;
            if (current.Path.Length < current.Path.LastIndexOf('\\'))
                parent = taskService.GetFolder(path.Remove(current.Path.LastIndexOf('\\')));
            else
                parent = taskService.GetFolder("\\");
            EnumFolderTasks(rootFolder);
        }
    }
    /// <summary>
    /// Экземпляр класса возвращаемый при вызове <seealso cref="TaskCharp.GetTask"/>
    /// </summary>
    public class TaskInfo
    {
        public string Name;
        public string Path;
        public bool Enabled;
        public DateTime LastRunTime;
        public int LastRunResult;
        public DateTime NextRunTime;
        public string State;
        public TaskInfo(string name, string path, bool enabled, DateTime lastruntime, int lastrunresult, DateTime nextruntime, _TASK_STATE state)
        {
            Name = name;
            Path = path;
            Enabled = enabled;
            LastRunTime = lastruntime;
            LastRunResult = lastrunresult;
            NextRunTime = nextruntime;
            switch (state)
            {
                case _TASK_STATE.TASK_STATE_UNKNOWN:
                    State = "Неизвестно";
                    break;
                case _TASK_STATE.TASK_STATE_DISABLED:
                    State = "Выключена";
                    break;
                case _TASK_STATE.TASK_STATE_QUEUED:
                    State = "В очереди";
                    break;
                case _TASK_STATE.TASK_STATE_READY:
                    State = "Готово";
                    break;
                case _TASK_STATE.TASK_STATE_RUNNING:
                    State = "Работает";
                    break;
                default:
                    break;
            }
        }
    }
}
