using System;

namespace SystemTestService.Utility
{
    public class Formator
    {        
        //yyyy-MM-dd
        public static string FormatDate(DateTime dateTime)
        {            
            return string.Format("{0:0000}-{1:00}-{2:00}", dateTime.Year, dateTime.Month, dateTime.Day);
        }

        //yyyy-MM-dd HH:mm:ss
        public static string FormatDateTime(DateTime dateTime)
        {
            return string.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
        }

        //<contributor>-<yyyymmdd_hour_min_second>.json.gz
        //Example: systemtest-20140508_17_49_22.json.gz
        public static string FormatFileName(DateTime dateTime)
        {
            return string.Format("systemtest-{0:0000}{1:00}{2:00}_{3:00}_{4:00}_{5:00}", dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
        }

        public static string FormatOpsFileName(DateTime dateTime)
        {
            return string.Format("ops-{0:0000}{1:00}{2:00}_{3:00}_{4:00}_{5:00}", dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
        }
    }
}
