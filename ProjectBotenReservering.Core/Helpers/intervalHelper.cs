namespace ProjectBotenReservering.Core.Helpers;

public static class IntervalHelper {

    private static float dateTimeToFloat(DateTime date) {
        return ((float)startTime.Hour + ((float)startTime.Minute * 0.01f))
    }

    public static float[] TimeSlotToInterval(DateTime StartTime, DateTime EndTime) {
        return [startTime.dateTimeToFloat(), endTime.dateTimeToFloat()]
    }

    public static float[][] TimeSlotListToIntervalList() {
        
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
