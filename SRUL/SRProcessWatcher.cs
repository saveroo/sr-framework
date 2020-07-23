using System;
using System.Management;
using System.Windows.Threading;

namespace SRUL
{
      class SRProcessWatcher {
 
        private readonly string _processName;
 
        private ManagementEventWatcher _processStartEvent;
        private ManagementEventWatcher _processStopEvent;
 
        public event EventHandler ProcessStarted;
        public event EventHandler ProcessStopped;
 
        private DispatcherTimer _processStartedTimer;
        
        public SRProcessWatcher(string processToWatch) {
            _processName = processToWatch;
 
            // start watching only 1 second after creation of the object
            _processStartedTimer = new DispatcherTimer();
            _processStartedTimer.Interval = TimeSpan.FromSeconds(1);
            _processStartedTimer.Tick += ProcessStartedTimer_Tick;
            _processStartedTimer.Start();
        }
 
        private void ProcessStartedTimer_Tick(object sender, EventArgs e) {
            // Check if process is already running and raise an even that it started
            if(System.Diagnostics.Process.GetProcessesByName($"{_processName}").Length != 0)
                RaiseProcessStartedEvent();
 
            // hook event handlers for when process is launched and stopped
            _processStartEvent = new ManagementEventWatcher(new WqlEventQuery($"SELECT * FROM Win32_ProcessStartTrace WHERE ProcessName = '{_processName}.exe'"));
            _processStopEvent = new ManagementEventWatcher(new WqlEventQuery($"SELECT * FROM Win32_ProcessStopTrace WHERE ProcessName = '{_processName}.exe'"));
 
            _processStartEvent.EventArrived += new EventArrivedEventHandler(ProcessStartEvent_EventArrived);
            _processStartEvent.Start();
            _processStopEvent.EventArrived += new EventArrivedEventHandler(ProcessStopEvent_EventArrived);
            _processStopEvent.Start();
 
            // Then stop the timer and use EventArrivedEventHandler
            _processStartedTimer.Stop();
        }
 
        private void ProcessStopEvent_EventArrived(object sender, EventArrivedEventArgs e) {
            RaiseProcessStoppedEvent();
        }
 
 
        private void ProcessStartEvent_EventArrived(object sender, EventArrivedEventArgs e) {
            RaiseProcessStartedEvent();
        }
 
 
        protected virtual void RaiseProcessStartedEvent() {
            ProcessStarted?.Invoke(this, EventArgs.Empty);
        }
 
 
        protected virtual void RaiseProcessStoppedEvent() {
            ProcessStopped?.Invoke(this, EventArgs.Empty);
        }
 
    }
}