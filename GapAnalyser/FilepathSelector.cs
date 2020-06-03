using System;
using System.Collections.Generic;
using System.Text;
using Foundations;

namespace GapAnalyser
{
    public sealed class FilepathSelector : BindableBase
    {
        public string DailyDataFileName
        {
            get => _dailyDatafileName;
            set => SetProperty(ref _dailyDatafileName, value);
        }

        public string MinuteDataFileName
        {
            get => _minuteDatafileName;
            set => SetProperty(ref _minuteDatafileName, value);
        }

        public bool DeriveDailyFromMinute
        {
            get => _deriveDailyFromMinute;
            set => SetProperty(ref _deriveDailyFromMinute, value);
        }

        private string _dailyDatafileName;
        private string _minuteDatafileName;
        private bool _deriveDailyFromMinute;
    }
}
