using UnityEngine;

public static class GameStatsManager
{
    public static string endType = "time";

    public static int streamTimeSec;
    public static int peakViewers;
    public static int subscribers;
    public static int totalDonations;
    public static int totalEarnings;
    public static int totalExpenses;
    public static int foodConsumed;

    private const string BestKeyPrefix = "Best_";

    public static class PreviousBest
    {
        public static int StreamTime;
        public static int PeakViewers;
        public static int Subscribers;
        public static int Donations;
        public static int Earnings;
        public static int Food;
    }

    public static void SaveCurrentStats()
    {
        PlayerPrefs.SetInt("Current_StreamTimeSec", streamTimeSec);
        PlayerPrefs.SetInt("Current_PeakViewers", peakViewers);
        PlayerPrefs.SetInt("Current_Subscribers", subscribers);
        PlayerPrefs.SetInt("Current_Donations", totalDonations);
        PlayerPrefs.SetInt("Current_Earnings", totalEarnings);
        PlayerPrefs.SetInt("Current_Food", foodConsumed);

        PreviousBest.StreamTime = GetBest("StreamTime");
        PreviousBest.PeakViewers = GetBest("PeakViewers");
        PreviousBest.Subscribers = GetBest("Subscribers");
        PreviousBest.Donations = GetBest("Donations");
        PreviousBest.Earnings = GetBest("Earnings");
        PreviousBest.Food = GetBest("Food");

        SaveIfRecord("StreamTime", streamTimeSec);
        SaveIfRecord("PeakViewers", peakViewers);
        SaveIfRecord("Subscribers", subscribers);
        SaveIfRecord("Donations", totalDonations);
        SaveIfRecord("Earnings", totalEarnings);
        SaveIfRecord("Food", foodConsumed);

        PlayerPrefs.Save();
    }

    private static void SaveIfRecord(string key, int newValue)
    {
        string fullKey = BestKeyPrefix + key;
        int oldValue = PlayerPrefs.GetInt(fullKey, 0);
        if (newValue > oldValue)
        {
            PlayerPrefs.SetInt(fullKey, newValue);
        }
    }

    public static int GetBest(string key) => PlayerPrefs.GetInt(BestKeyPrefix + key, 0);
}