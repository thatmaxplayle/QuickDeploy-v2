using System;
using System.ComponentModel;

namespace QuickDeploy.Backend.MVVM.ViewModels
{
    internal class CrashVM : VMBase
    {

        public CrashVM(Exception e)
        {
            this.ExceptionTypeName = e.GetType().Name;
            this.ExceptionStackTrace = e.StackTrace;
            this.ExceptionMessage = e.Message;
        }

        private string? _exceptionTypeName;
        public string? ExceptionTypeName
        {
            get => _exceptionTypeName;
            set
            {
                if (value != _exceptionTypeName)
                {
                    _exceptionTypeName = value;
                    OnPropertyChanged(nameof(ExceptionTypeName));
                }
            }
        }

        private string? _exceptionStackTrace;
        public string? ExceptionStackTrace
        {
            get => _exceptionStackTrace;
            set
            {
                if (value != _exceptionStackTrace)
                {
                    _exceptionStackTrace = value;
                    OnPropertyChanged(nameof(ExceptionStackTrace));
                }
            }
        }

        private string? _exceptionMessage;
        public string? ExceptionMessage
        {
            get => _exceptionMessage;
            set
            {
                if (value != _exceptionMessage)
                {
                    _exceptionMessage = value;
                    OnPropertyChanged(nameof(ExceptionMessage));
                }
            }
        }

    }
}
