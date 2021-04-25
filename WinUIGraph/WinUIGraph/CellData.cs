using System;

namespace WinUIGraph
{
    public class CellData
    {
        public int Column;
        public int Row;
        public double AvgDays;
        public int Trades;
        public int StopLossCount;
        public int TimeoutCount;
        public int TakeProfitCount;
        public double AvgDaysSL;
        public double AvgDaysTimeout;
        public double AvgDaysTP;
        public double RealizedProfitPctAvg;
        public double RealizedProfitValueAvg;
        public double RealizedProfitValueSum;
        public double RiskValueAvg;
        public double UsedMarginAvg;
        public double GapPercentAvg;

    }
}