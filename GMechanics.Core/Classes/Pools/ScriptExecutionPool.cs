using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using GMechanics.Core.GameScript;

namespace GMechanics.Core.Classes.Pools
{
    internal delegate void ScriptExecutionPoolEmptyEvent();

    internal class ScriptExecutionPool
    {

#region Private members

        private readonly Queue _objectPool = Queue.Synchronized(new Queue());
        private readonly Semaphore _semaphore = new Semaphore(0, int.MaxValue);
        private readonly Semaphore _singleActivityEvent = new Semaphore(0, int.MaxValue);
        private readonly List<ManualResetEvent> _activityEvents = new List<ManualResetEvent>();
        private ManualResetEvent[] _activityEventsArray;

        private readonly HashSet<long> _scriptExecutionLinkedDataInProgress = new HashSet<long>();

#endregion

#region Static members

        public static readonly ScriptExecutionPool Instance = new ScriptExecutionPool();

#endregion

#region Properties

        public ScriptExecutionDataPool ScriptExecutionDataPool
        {
            get { return ScriptExecutionDataPool.Instance; }
        }

#endregion

#region Class functions

        private object ExecuteScript(ScriptExecutionLinkedData data, out bool cancelled)
        {
            //Set 'cancelled' flag to false
            cancelled = false;

            //By default result equal to original object member value
            object result = data.Value;

            //Store intermediate value
            object intermediateValue = data.Value;

            //Foreach of ScriptExecutionLinkedData
            do
            {
                //If operation has not been cancelled by previous execution
                if (!cancelled)
                {
                    //Get hash code for current data
                    long dataHash = data.GetHashCode();

                    //If current data is not in progress
                    if (!_scriptExecutionLinkedDataInProgress.Contains(dataHash))
                    {
                        //Set progress mark for current data
                        _scriptExecutionLinkedDataInProgress.Add(dataHash);

                        //Set value for current data object equal to intermediate value
                        data.Value = intermediateValue;

                        //Execute script and store result into intermediate value
                        intermediateValue = data.Recipient.DoInteractive(data);

                        //Remove progress mark for current data
                        _scriptExecutionLinkedDataInProgress.Remove(dataHash);

                        //Set calncelled flag
                        cancelled = data.Cancelled;
                    }
                }

                //Returns ScriptExecutionLinkedData object to the pool
                ScriptExecutionDataPool.Push(data);

                //Get next data element
                data = data.NextElement;
            }
            while (data != null);

            if (!cancelled)
            {
                result = intermediateValue;
            }

            //Return result
            return result;
        }

        private void ThreadPoolProc(object state)
        {
            Thread currentThread = Thread.CurrentThread;
            ManualResetEvent activityEvent = (ManualResetEvent)state;

            while (currentThread.IsAlive)
            {
                //Wait for new script object in the pool
                _semaphore.WaitOne();

                //For now I`m an active
                activityEvent.Reset();

                //ExecuteAsync can get control
                _singleActivityEvent.Release();
               
                //Pop first script object from the pool
                ScriptExecutionLinkedData data = (ScriptExecutionLinkedData) _objectPool.Dequeue();

                //Execute script
                bool cancelled;
                ExecuteScript(data, out cancelled);

                //Set inactive state for self
                activityEvent.Set();
            }
        }

        private void Init(int workerThreadCount)
        {
            workerThreadCount = workerThreadCount > 0
                                    ? workerThreadCount
                                    : Environment.ProcessorCount;

            for (int i = 0; i < workerThreadCount; i++)
            {
                // Create activity events for thread
                ManualResetEvent e = new ManualResetEvent(true);
                _activityEvents.Add(e);

                //Activate new ThreadPoolProc
                ThreadPool.QueueUserWorkItem(ThreadPoolProc, e);
            }

            _activityEventsArray = _activityEvents.ToArray();
        }

        public ScriptExecutionPool()
        {
            Init(0);
        }

        public ScriptExecutionPool(int workerThreadCount)
        {
            Init(workerThreadCount);
        }

        public object ExecuteSync(ScriptExecutionLinkedData data, out bool cancelled)
        {
            return ExecuteScript(data, out cancelled);
        }

        public void ExecuteAsync(ScriptExecutionLinkedData data)
        {
            bool cancelled;
            ExecuteScript(data, out cancelled);
            _objectPool.Enqueue(data);
            _semaphore.Release();
            _singleActivityEvent.WaitOne();
        }

        public void WaitForEmpty(ScriptExecutionPoolEmptyEvent callBack)
        {
            if (callBack == null)
            {
                WaitHandle.WaitAll(_activityEventsArray);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    WaitHandle.WaitAll(_activityEventsArray);
                    try
                    {
                        callBack();
                    }
                    catch
                    {
                    }
                });
            }
        }

        public void WaitForEmpty()
        {
            WaitForEmpty(null);
        }

        private void StartScriptExecutionPool() { }

        public void Initialize()
        {
            StartScriptExecutionPool();
        }

        public void ClosePool()
        {
            /*_semaphore.Close();
            _singleActivityEvent.Close();
            foreach (ManualResetEvent e in _activityEvents)
            {
                e.Close();
            }*/
        }

#endregion

    }
}
