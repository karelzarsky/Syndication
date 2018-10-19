using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyndicateLogic;
using SyndicateLogic.Entities;

namespace NNPrepareDataSet
{
    class NNPrepareDataSet
    {
        const int shinglesPerNetwork = 10;
        const int daysToForecast = 5;

        static void Main(string[] args)
        {
            using (var p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.Idle;

            using (var ctx = new Db())
            {
                var shingles = ctx.Shingles.Where(x => x.UseForML).ToArray();
                int NextNetworkID = ctx.TrainSchemas.Select(x => x.NetworkID).Max() + 1;
                for (int i = 0; i <= shingles.Length / shinglesPerNetwork; i++)
                {
                    int firstShingle = i * shinglesPerNetwork;
                    int shinglesCount = shingles.Length > firstShingle + shinglesPerNetwork
                        ? shinglesPerNetwork
                        : shingles.Length - firstShingle;

                    for (int inputNr = 0; inputNr < shinglesPerNetwork; inputNr++)
                    {
                        var ts = new TrainSchema
                        {
                            ForecastDays = daysToForecast,
                            NetworkID = NextNetworkID + i,
                            Input = true,
                            NeuronID = inputNr,
                            ShingleID = shingles[i * shinglesPerNetwork + inputNr].ID
                        };
                    }
                }
            }
        }
    }
}
