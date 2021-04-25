using Mongo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUIGraph
{
    class Model
    {
        public const string CollectionName = "TradesThroughZeroMinus1Plus05";
        public double gapMin = -5;
        public double gapMax = -4;
        public double[] targets;
        public double[] stopLoses;
        public CellData[,] cellData;
        public CellData best;
        public CellData worst;

        readonly DataLayer data = new();

        public void Calculate()
        {
            best = null;
            worst = null;
            var trades = data.GetTradesByGap(CollectionName, gapMin, gapMax);
            stopLoses = trades.Select(t => t.StopLossPct).Distinct().OrderBy(s => s).ToArray();
            targets = trades.Select(t => t.TargerPct).Distinct().OrderBy(s => s).ToArray();
            cellData = new CellData[stopLoses.Length, targets.Length];
            List<Trade> bestList = new();

            for (int c = 0; c < stopLoses.Length; c++)
            {
                for (int r = 0; r < targets.Length; r++)
                {
                    var selectedTrades = trades.Where(t => t.StopLossPct == stopLoses[c] && t.TargerPct == targets[r]).ToList();
                    cellData[c, r] = new()
                    {
                        Column = c,
                        Row = r,
                    };
                    if (selectedTrades.Count() > 0)
                    {
                        cellData[c, r].AvgDays = selectedTrades.Select(t => (t.EndTime - t.StartTime).Days).Average();
                        cellData[c, r].Trades = selectedTrades.Count();
                        cellData[c, r].StopLossCount = selectedTrades.Where(t => t.EndReason.Contains("SL")).Count();
                        cellData[c, r].TimeoutCount = selectedTrades.Where(t => t.EndReason.Equals("OUT")).Count();
                        cellData[c, r].TakeProfitCount = selectedTrades.Where(t => t.EndReason.Contains("TP")).Count();
                        if (cellData[c, r].StopLossCount > 0)
                            cellData[c, r].AvgDaysSL = selectedTrades.Where(t => t.EndReason.Contains("SL")).Select(t => (t.EndTime - t.StartTime).Days).Average();
                        if (cellData[c, r].TimeoutCount > 0)
                            cellData[c, r].AvgDaysTimeout = selectedTrades.Where(t => t.EndReason.Equals("OUT")).Select(t => (t.EndTime - t.StartTime).Days).Average();
                        if (cellData[c, r].TakeProfitCount > 0)
                            cellData[c, r].AvgDaysTP = selectedTrades.Where(t => t.EndReason.Contains("TP")).Select(t => (t.EndTime - t.StartTime).Days).Average();
                        cellData[c, r].RealizedProfitValueAvg = selectedTrades.Select(t => t.RealizedProfitValue).Average();
                        cellData[c, r].RealizedProfitPctAvg = selectedTrades.Select(t => t.RealizedProfitPct).Average();
                        cellData[c, r].RealizedProfitValueSum = selectedTrades.Select(t => t.RealizedProfitValue).Sum();
                        cellData[c, r].RiskValueAvg = selectedTrades.Select(t => t.RiskValue).Average();
                        cellData[c, r].UsedMarginAvg = selectedTrades.Select(t => t.UsedMargin).Average();
                        cellData[c, r].GapPercentAvg = selectedTrades.Select(t => t.GapPercent).Average();
                        if (best == null || best.RealizedProfitValueAvg < cellData[c, r].RealizedProfitValueAvg)
                            best = cellData[c, r];
                        if (worst == null || worst.RealizedProfitValueAvg > cellData[c, r].RealizedProfitValueAvg)
                            worst = cellData[c, r];
                    }
                }
            }
        }
    }
}
