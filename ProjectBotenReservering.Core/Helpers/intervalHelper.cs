namespace ProjectBotenReservering.Core.Helpers;

public static class IntervalHelper {

    // Convert datetimes to a float so we can easilier check for overlaps
    private static float dateTimeToFloat(DateTime date) {
        return (float)date.TimeOfDay.TotalMinutes;
    }

    public static float[] TimeSlotToInterval(DateTime StartTime, DateTime EndTime) {
        return new float[] { dateTimeToFloat(StartTime), dateTimeToFloat(EndTime) };
    }

    public static float[][] TimeSlotListToIntervalList(DateTime[] startTimes, DateTime[] endTimes) {
        List<float[]> intervalList = new List<float[]>();
        // there will always be as much start times as end times so no check is needed
        for (int i = 0; i < startTimes.Length; i++) {
        intervalList.Add(TimeSlotToInterval(startTimes[i], endTimes[i]));
        }
        return intervalList.ToArray();
    }

    private static bool isIntersecting(float[] a, float[] b) {
        if (a[1] < b[0] || b[1] < a[0])
            return false;
        return true;
    }

    public static bool isIntersectingWithIntervalList(float[] a, float[][] intervalList ) {
        for (int i = 0; i < intervalList.Length; i++) {
            if(isIntersecting(a, intervalList[i])) 
                return true;
        }
        return false;
    }
}
