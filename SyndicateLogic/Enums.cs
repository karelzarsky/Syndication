namespace SyndicateLogic
{
    public enum LogLevel : byte
    {
        Info = 1,
        Article = 2,
        Service = 3,
        Intrinio = 4,
        ShingleProcessing = 5,
        Feed = 6,
        Analysis = 7,
        Duplicate = 8,
        IntrinioError = 253,
        FeedError = 254,
        Error = 255

        //HashCollision = 6,
        //Summary = 3,
        //Fatal = 255
    }

    public enum ProcessState : byte
    {
        Waiting = 0,
        Done = 1,
        Running = 2
    }

    public enum ShingleKind : byte
    {
        newShingle = 0,
        common = 1,
        ticker = 2,
        currency = 3,
        currencyPair = 4,
        future = 5,
        upperCase = 6,
        interesting = 7,
        CEO = 8,
        companyName = 9,
        containCommon = 10,
        containTicker = 11
    }

    public enum InstrumentType : byte
    {
        unknown = 0,
        stock = 1,
        forex = 2,
        future = 3,
        stockIndex = 4,
        economyIndex = 5
    }

    public enum DirectionType
    {
        Sell = -1,
        Nothing = 0,
        Buy = 1
    }

    public enum ExitReason : byte
    {
        StopLoss = 0,
        TakeProfit = 1,
        Timeout = 2
    }
}