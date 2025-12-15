namespace ProjectBotenReservering.Core.Helpers;

public static class IntervalHelper {

    // Convert datetimes to a float so we can easilier check for overlaps 
    private static float DateTimeToFloat(DateTime date) {
        return (float)(date - DateTime.MinValue).TotalMinutes;
    }

    public static float[] TimeSlotToInterval(DateTime startTime, DateTime endTime) {
        return new float[] { DateTimeToFloat(startTime), DateTimeToFloat(endTime) };
    }

    public static float[][] TimeSlotListToIntervalList(DateTime[] startTimes, DateTime[] endTimes) {
        List<float[]> intervalList = new List<float[]>();
        // there will always be as many start times as end times so no check is needed
        for (int i = 0; i < startTimes.Length; i++) {
            intervalList.Add(TimeSlotToInterval(startTimes[i], endTimes[i]));
        }
        return intervalList.ToArray();
    }

    private static bool IsIntersecting(float[] a, float[] b) {
        if (a[1] <= b[0] || b[1] <= a[0])
            return false;
        return true;
    }

    public static int CountIntersectionsWithIntervalList(float[] a, float[][] intervalList )
    {
        int intersectionCount = 0;
        for (int i = 0; i < intervalList.Length; i++) {
            if(IsIntersecting(a, intervalList[i])) 
                intersectionCount++;
        }
        return intersectionCount;
    }
}
