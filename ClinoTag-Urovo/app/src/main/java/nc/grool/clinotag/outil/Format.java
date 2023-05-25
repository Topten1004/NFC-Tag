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

    public static String AfficherDateHeureMilliEpoch(String sTicks) {

        if(sTicks.equals("0")) return "0";
        long TICKS_AT_EPOCH = 621355968000000000L;
        long TICKS_PER_MILLISECOND = 10000;
        long ticks = Long.parseLong(sTicks);
        Date date = new Date(((ticks - (3600 * 11 * 1000 * TICKS_PER_MILLISECOND)) - TICKS_AT_EPOCH) / TICKS_PER_MILLISECOND);
        SimpleDateFormat simpleDate =  new SimpleDateFormat("dd/MM/yyyy HH:mm");
        return simpleDate.format(date);

    }

    public static String AfficherDateMilliEpoch(String sTicks) {

        if(sTicks.equals("0")) return "0";
        long TICKS_AT_EPOCH = 621355968000000000L;
        long TICKS_PER_MILLISECOND = 10000;
        long ticks = Long.parseLong(sTicks);
        Date date = new Date(((ticks - (3600 * 11 * 1000 * TICKS_PER_MILLISECOND)) - TICKS_AT_EPOCH) / TICKS_PER_MILLISECOND);
        SimpleDateFormat simpleDate =  new SimpleDateFormat("dd/MM/yyyy");
        return simpleDate.format(date);

    }

    public static String AfficherHeureMilliEpoch(String sTicks) {

        if(sTicks.equals("0")) return "0";
        long TICKS_AT_EPOCH = 621355968000000000L;
        long TICKS_PER_MILLISECOND = 10000;
        long ticks = Long.parseLong(sTicks);
        Date date = new Date(((ticks - (3600 * 11 * 1000 * TICKS_PER_MILLISECOND)) - TICKS_AT_EPOCH) / TICKS_PER_MILLISECOND);
        SimpleDateFormat simpleDate =  new SimpleDateFormat("HH:mm");
        return simpleDate.format(date);

    }

    public static String AfficherDateHeureUnix(String sTicks) {

        if(sTicks.equals("0")) return "0";

        long ticks = Long.parseLong(sTicks);
        long ticksToMicrotime = ticks / 10000;
        long epochMicrotimeDiff = 621355968000000000L; //2208988800000L;

        Date tickDate = new Date(ticksToMicrotime - epochMicrotimeDiff);
        SimpleDateFormat simpleDate =  new SimpleDateFormat("dd/MM/yyyy HH:mm");
        return simpleDate.format(tickDate);

    }

    public static String AfficherHeureUnix(String sTicks) {

        if(sTicks.equals("0")) return "0";

        long ticks = Long.parseLong(sTicks);
        long ticksToMicrotime = ticks / 10000;
        long epochMicrotimeDiff = 2208988800000L;

        Date tickDate = new Date(ticksToMicrotime - epochMicrotimeDiff);
        SimpleDateFormat simpleDate =  new SimpleDateFormat("HH:mm");
        return simpleDate.format(tickDate);

    }

    public static String AfficherHeureMilli(String sTicks) {

        if(sTicks.equals("0")) return "0";
        long ticks = Long.parseLong(sTicks);
        Date date = new Date(ticks);
        SimpleDateFormat simpleDate =  new SimpleDateFormat("HH:mm");
        return simpleDate.format(date);

    }

//    public static String AfficherDateMilli(String sTicks) {
//
//        if(sTicks == "0") return "0";
//        Long ticks = Long.parseLong(sTicks);
//        Date date = new Date(ticks);
//        SimpleDateFormat simpleDate =  new SimpleDateFormat("dd/MM/yyyy");
//        return simpleDate.format(date);
//
//    }

//    public static String AfficherDateMilli(String sTicks) {
//
//        if(sTicks == "0") return "0";
//        long TICKS_AT_EPOCH = 621355968000000000L;
//        long TICKS_PER_MILLISECOND = 10000;
//        Long ticks = Long.parseLong(sTicks);
//        Date date = new Date(((ticks - (3600 * 11 * 1000 * TICKS_PER_MILLISECOND)) - TICKS_AT_EPOCH) / TICKS_PER_MILLISECOND);
//        SimpleDateFormat simpleDate =  new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS");
//        return simpleDate.format(date);
//    }

    public static Date DateMilliEpoch(String sTicks) {

        long TICKS_AT_EPOCH = 621355968000000000L;
        long TICKS_PER_MILLISECOND = 10000;
        long ticks = Long.parseLong(sTicks);
        return new Date(((ticks - (3600 * 11 * 1000 * TICKS_PER_MILLISECOND)) - TICKS_AT_EPOCH) / TICKS_PER_MILLISECOND);
    }

    public static Date DateMilli(String sTicks) {

        long ticks = Long.parseLong(sTicks);
        return new Date(ticks);

    }

    public static String ConvertMilliEpoch(String sTicks) {
        if(sTicks.equals("0")) return sTicks;
        long TICKS_AT_EPOCH = 621355968000000000L;
        long TICKS_PER_MILLISECOND = 10000;
        long ticks = Long.parseLong(sTicks);
        return String.valueOf(((ticks - (3600 * 11 * 1000 * TICKS_PER_MILLISECOND)) - TICKS_AT_EPOCH) / TICKS_PER_MILLISECOND);
    }

//    public static String MilliMVC(String sTicks){
//        long TICKS_AT_EPOCH = 621355968000000000L;
//        long TICKS_PER_MILLISECOND = 10000;
//        Long ticks = Long.parseLong(sTicks);
//        return String.valueOf(ticks * TICKS_PER_MILLISECOND + TICKS_AT_EPOCH);
//    }

//    public static String AfficherHeureString(String DH) {
//
//        DateFormat formatter = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS");
//        Date date = null;
//        try {
//            date = formatter.parse(DH);
//        } catch (ParseException e) {
//            e.printStackTrace();
//        }
//        SimpleDateFormat simpleDate =  new SimpleDateFormat("HH:mm");
//        return simpleDate.format(date);
//    }

//    public static String ccDate(Date date) {
//        SimpleDateFormat sdfDate = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS");//dd/MM/yyyy
//        String strDate = sdfDate.format(date);
//        return strDate;
//    }

    public static String AfficherCourteDateHeure(String DH) {
        SimpleDateFormat formatter = new SimpleDateFormat("EEE MMM dd HH:mm:ss zzz yyyy", Locale.ENGLISH);
        Date date = null;
        try {
            date = formatter.parse(DH);
        } catch (ParseException e) {
            e.printStackTrace();
        }
        SimpleDateFormat simpleDate =  new SimpleDateFormat("dd/MM HH:mm");
        return simpleDate.format(date);
    }

    public static String AfficherCourteDateHeure(Date date) {
        SimpleDateFormat sdfDate = new SimpleDateFormat("dd/MM HH:mm");//dd/MM/yyyy
        String strDate = sdfDate.format(date);
        return strDate;
    }

    public static String AfficherHeure(Date date) {
        SimpleDateFormat sdfDate = new SimpleDateFormat("HH:mm:ss");//dd/MM/yyyy
        String strDate = sdfDate.format(date);
        return strDate;
    }

    public static String AfficherDate(Date date) {
        SimpleDateFormat sdfDate = new SimpleDateFormat("dd-MM-yyyy");//dd/MM/yyyy
        String strDate = sdfDate.format(date);
        return strDate;
    }

    public static String AfficherDate(String DH) {
        DateFormat formatter = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS");
        Date date = null;
        try {
            date = formatter.parse(DH);
        } catch (ParseException e) {
            e.printStackTrace();
        }
        SimpleDateFormat simpleDate =  new SimpleDateFormat("dd-MM-yyyy");
        return simpleDate.format(date);
    }

//    public static Date ObtenirDate(String DH){
//        if(DH == null) return new Date(Long.MIN_VALUE);;
//        DateFormat formatter = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS");
//        try {
//            return formatter.parse(DH);
//        } catch (ParseException e) {
//            e.printStackTrace();
//        }
//        return null;
//    }
//
//    public static String ObtenirMillisecond(String DH){
//        if(DH == null) return "0";
//        DateFormat formatter = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS");
//        try {
//            return String.valueOf(formatter.parse(DH).getTime());
//        } catch (ParseException e) {
//            e.printStackTrace();
//        }
//        return null;
//    }

    public static Boolean MemeJour(Date date1, Date date2) {
        SimpleDateFormat fmt = new SimpleDateFormat("yyyyMMdd");
        return fmt.format(date1).equals(fmt.format(date2));
    }

    public static Date StringToDate(String DH) {
//        DateFormat formatter = new SimpleDateFormat("EEE, dd MMM yyyy HH:mm:ss Z", Locale.US);
        DateFormat formatter = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'", Locale.US);
        Date date = null;
        try {
            date = formatter.parse(DH);
        } catch (ParseException e) {
            e.printStackTrace();
        }

        return date;
    }

    public static Date ddMMyyyyToDate(String DH) {
//        DateFormat formatter = new SimpleDateFormat("EEE, dd MMM yyyy HH:mm:ss Z", Locale.US);
        DateFormat formatter = new SimpleDateFormat("dd-MM-yyyy", Locale.US);
        Date date = null;
        try {
            date = formatter.parse(DH);
        } catch (ParseException e) {
            e.printStackTrace();
        }

        return date;
    }

    public static Date Maintenant(int nbMinute) {
        Calendar date = Calendar.getInstance();
        long timeInSecs = date.getTimeInMillis();
        return  new Date(timeInSecs + (nbMinute * 60 * 1000));
    }
}
