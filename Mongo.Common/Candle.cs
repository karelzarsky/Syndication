using System;

namespace Mongo.Common
{
    public class Candle
    {
        public DateTime Time;
        public double Open;
        public double High;
        public double Low;
        public double Close;
        public long Volume;
        public int Count;
        public double Wap;
    }
}