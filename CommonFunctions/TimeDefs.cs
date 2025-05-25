public class TdsDefs   // also defined in AbstractRobot.cs, sorry
{
    // 1 sec = 1000 millisec = 1,000,000 microsec = 1,000,000,000 nanosec = 10,000,000 100-nanosec  
    // 1 microsec = 1000 nanosec = 10 100-nanosec 
    // 100 nanosec = 0.1 microsec
    /// <summary>
    /// Milliseconds per second.
    /// </summary>
    public const int MILLISEC_PER_SEC = (1000);

    /// <summary>
    /// Microseconds per second.
    /// </summary>
    public const int MICROSEC_PER_SEC = (1000000);

    /// <summary>
    /// Nanoseconds per second.
    /// </summary>
    public const int NANOSEC_PER_SEC = (1000000000);

    /// <summary>
    /// Hectonanoseconds per second.
    /// </summary>
    public const int HECTONANOSEC_PER_SEC = (10000000);

    /// <summary>
    /// Nanoseconds per millisecond.
    /// </summary>
    public const int NANOSEC_PER_MILLISEC = (1000000);

    /// <summary>
    /// Nanoseconds per microsecond.
    /// </summary>
    public const int NANOSEC_PER_MICROSEC = (1000);

    /// <summary>
    /// Hours per day.
    /// </summary>
    public const int HOUR_PER_DAY = (24);

    /// <summary>
    /// Minutes per hour.
    /// </summary>
    public const int MIN_PER_HOUR = (60);

    /// <summary>
    /// Minutes per day.
    /// </summary>
    public const int MIN_PER_DAY = (MIN_PER_HOUR * HOUR_PER_DAY);

    /// <summary>
    /// Seconds per minute.
    /// </summary>
    public const int SEC_PER_MINUTE = (60);

    /// <summary>
    /// Seconds per hour.
    /// </summary>
    public const int SEC_PER_HOUR = (MIN_PER_HOUR * SEC_PER_MINUTE);

    /// <summary>
    /// Seconds per day.
    /// </summary>
    public const int SEC_PER_DAY = (SEC_PER_HOUR * 24);

    /// <summary>
    /// Seconds per day.
    /// </summary>
    public const int SEC_PER_WEEK = (SEC_PER_DAY * 7);

    /// <summary>
    /// MT4 Epoc datetime starts on 1.1.1970 what was a Thursday (WeekOfDay == 4). 
    /// Add this offset to get Sunday
    /// </summary>
    public const int EPOC_WEEKDAY_SEC_OFFSET = (3 * SEC_PER_DAY);
}
