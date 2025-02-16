using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class TimeUtil : MonoBehaviour
{

    public static string TimeToString(double inputTime)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(inputTime);
        if (timeSpan.TotalDays >= 1)
        {
            return string.Format("{0:D1}D:{1:D2}H:{2:D2}M", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
        }
        else if (timeSpan.TotalHours >= 1)
        {
            return string.Format("{0:D1}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }
        else if (timeSpan.TotalMinutes >= 1)
        {
            return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }

        return string.Format("00:{0:D2}", timeSpan.Seconds);
    }


    public static string TimeToNewDay()
    {
        DateTime currentTime = DateTime.Now;
        DateTime newDayTime = currentTime.AddDays(1);
        newDayTime = new DateTime(newDayTime.Year, newDayTime.Month, newDayTime.Day, 0, 0, 0);
        TimeSpan timeSpan = newDayTime.Subtract(currentTime);
        return string.Format("{0:D1}h: {1:D2}m: {2:D2}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
    public static DateTime GetNextDate()
    {
        DateTime now = DateTime.Now;
        DateTime temp = now.AddDays(1);
        return new DateTime(temp.Year, temp.Month, temp.Day);
    }

    public static string GetTimeStringFromSecond(double time)
    {
        int totalSecond = (int)time;
        int min = totalSecond / 60;
        int sec = totalSecond % 60;
        int hour = min / 60;
        min = min % 60;
        return (hour >= 10 ? hour.ToString() : "0" + hour.ToString()) + ":" + (min >= 10 ? min.ToString() : "0" + min.ToString()) + ":" + (sec >= 10 ? sec.ToString() : "0" + sec.ToString());
    }
    public static string DateTimeToString(DateTime dateTime)
    {
        return dateTime.ToString(new CultureInfo("en-US"));
    }
    public static DateTime StringToDateTime(string dateTime)
    {
        return DateTime.Parse(dateTime, new CultureInfo("en-US"));
    }
    public static bool IsNewDay(DateTime lastTime)
    {
        TimeSpan ts = DateTime.Now - lastTime;
        return ts.TotalSeconds >= 0;
    }
}