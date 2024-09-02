package nc.grool.clinotag.outil;

import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.Locale;

public class Format {

    public static long DifferenceMinute(Date d1, Date d2)
    {
//        SimpleDateFormat formatter = new SimpleDateFormat("EEE MMM dd HH:mm:ss zzz yyyy", Locale.ENGLISH);
//        Date d1 = null;
//        try {
//            d1 = formatter.parse(start_date);
//        } catch (ParseException e) {
//            e.printStackTrace();
//        }
        // Calucalte time difference
        // in milliseconds
        long difference_In_Time
                = d2.getTime() - d1.getTime();

        // Calucalte time difference in
        // seconds, minutes, hours, years,
        // and days
        long difference_In_Seconds
                = (difference_In_Time
                / 1000)
                % 60;

        long difference_In_Minutes
                = (difference_In_Time
                / (1000 * 60))
                % 60;

        long difference_In_Hours
                = (difference_In_Time
                / (1000 * 60 * 60))
                % 24;

        long difference_In_Years
                = (difference_In_Time
                / (1000l * 60 * 60 * 24 * 365));

        long difference_In_Days
                = (difference_In_Time
                / (1000 * 60 * 60 * 24))
                % 365;

        return difference_In_Minutes;

    }

    public static String bytesToHexString(byte[] src) {
        StringBuilder stringBuilder = new StringBuilder("0x");
        if (src == null || src.length <= 0) {
            return null;
        }

        char[] buffer = new char[2];
        for (int i = 0; i < src.length; i++) {
            buffer[0] = Character.forDigit((src[i] >>> 4) & 0x0F, 16);
            buffer[1] = Character.forDigit(src[i] & 0x0F, 16);
            System.out.println(buffer);
            stringBuilder.append(buffer);
        }

        return stringBuilder.toString();
    }

    public static Date DateMilliEpoch(String sTicks) {

        long TICKS_AT_EPOCH = 621355968000000000L;
        long TICKS_PER_MILLISECOND = 10000;
        long ticks = Long.parseLong(sTicks);
        return new Date(((ticks - (3600 * 11 * 1000 * TICKS_PER_MILLISECOND)) - TICKS_AT_EPOCH) / TICKS_PER_MILLISECOND);
    }

    public static String ShowShortDateTime(Date date) {
        SimpleDateFormat sdfDate = new SimpleDateFormat("HH:mm");//dd/MM/yyyy
        String strDate = sdfDate.format(date);
        return strDate;
    }
}
