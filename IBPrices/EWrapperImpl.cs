using IBApi;
using Mongo.Common;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace IBPrices
{
    public class EWrapperImpl : EWrapper
    {
        public readonly EReaderSignal signal;
        public readonly EClientSocket clientSocket;
        public readonly EReader reader;
        public string accounts;
        public int nextOrderId;

        public EWrapperImpl()
        {
            signal = new EReaderMonitorSignal();
            clientSocket = new EClientSocket(this, signal);
            clientSocket.eConnect("127.0.0.1", 7496, 0);
            //Create a reader to consume messages from the TWS. The EReader will consume the incoming messages and put them in a queue
            reader = new EReader(clientSocket, signal);
            reader.Start();
            //Once the messages are in the queue, an additional thread can be created to fetch them
            new Thread(() => { while (clientSocket.IsConnected()) { signal.waitForSignal(); reader.processMsgs(); } }) { IsBackground = true }.Start();
        }

        public void accountDownloadEnd(string account)
        {
            throw new NotImplementedException();
        }

        public void accountSummary(int reqId, string account, string tag, string value, string currency)
        {
            throw new NotImplementedException();
        }

        public void accountSummaryEnd(int reqId)
        {
            throw new NotImplementedException();
        }

        public void accountUpdateMulti(int requestId, string account, string modelCode, string key, string value, string currency)
        {
            throw new NotImplementedException();
        }

        public void accountUpdateMultiEnd(int requestId)
        {
            throw new NotImplementedException();
        }

        public void bondContractDetails(int reqId, ContractDetails contract)
        {
            throw new NotImplementedException();
        }

        public void commissionReport(CommissionReport commissionReport)
        {
            //throw new NotImplementedException();
        }

        public void completedOrder(Contract contract, Order order, OrderState orderState)
        {
            throw new NotImplementedException();
        }

        public void completedOrdersEnd()
        {
            //throw new NotImplementedException();
        }

        public void connectAck()
        {
            Console.WriteLine("Connect ack.");
        }

        public void connectionClosed()
        {
            throw new NotImplementedException();
        }

        public void contractDetails(int reqId, ContractDetails contractDetails)
        {
            Console.WriteLine("ContractDetails begin. ReqId: " + reqId);
            printContractMsg(contractDetails.Contract);
            printContractDetailsMsg(contractDetails);
            Console.WriteLine("ContractDetails end. ReqId: " + reqId);
        }

        private void printContractDetailsMsg(ContractDetails contractDetails)
        {
        }

        private void printContractMsg(Contract contract)
        {
            Console.WriteLine("ConId :" + contract.ConId);
            Console.WriteLine("Symbol :" + contract.Symbol);
            Console.WriteLine("SecType :" + contract.SecType);
            Console.WriteLine("Exchange :" + contract.Exchange);
            Console.WriteLine("Currency :" + contract.Currency);
            Console.WriteLine("LocalSymbol :" + contract.LocalSymbol);
            Console.WriteLine();
        }

        public void contractDetailsEnd(int reqId)
        {
            Console.WriteLine("ContractDetailsEnd. " + reqId + "\n");
        }

        public void currentTime(long time)
        {
            throw new NotImplementedException();
        }

        public void deltaNeutralValidation(int reqId, DeltaNeutralContract deltaNeutralContract)
        {
            throw new NotImplementedException();
        }

        public void displayGroupList(int reqId, string groups)
        {
            throw new NotImplementedException();
        }

        public void displayGroupUpdated(int reqId, string contractInfo)
        {
            throw new NotImplementedException();
        }

        public void error(Exception e)
        {
            Console.WriteLine(e.Message);
        }

        public void error(string str)
        {
            throw new NotImplementedException();
        }

        public void error(int id, int errorCode, string errorMsg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{id}) {DateTime.Now:hh:mm:ss} errorCode:{errorCode} {errorMsg}");
            Console.ForegroundColor = ConsoleColor.White; 
            if (errorCode == 2104) return;
            if (errorCode == 2106) return;
            if (errorCode == 2158) return;
            if (Program.allReports == null) return;
            if (Program.allReports == null || !Program.allReports.TryRemove(id, out EarningEvent currentEarning)) return;
            if (currentEarning == null) return;
            if (errorCode == 162)
            {
                if (currentEarning != null)
                {
                    currentEarning.Error = errorMsg;
                    Program.releaseCollection.InsertOneAsync(currentEarning);
                }
                return;
            }
            
            currentEarning.Error = errorMsg;
            Program.releaseCollection.ReplaceOneAsync(Builders<EarningEvent>.Filter.Eq(x => x.Id, currentEarning.Id), currentEarning);
            if (errorMsg.StartsWith("No security definition") ||
                errorMsg.StartsWith("Historical Market Data Service error") ||
                errorMsg.StartsWith("The contract description specified for"))
            {
                if (Program.badTickerCollection.CountDocuments(x => x.Ticker == currentEarning.Symbol) > 0) return;
                Program.badTickerCollection.InsertOneAsync(new IBBadTicker
                {
                    Ticker = currentEarning.Symbol,
                    Reason = errorMsg
                });
                Console.WriteLine($"Adding {currentEarning.Symbol} to bad symbol list.");
            }
            if (errorMsg.Contains("Duplicate ticker")) Program.pause = true;
        }

        public void execDetails(int reqId, Contract contract, Execution execution)
        {
            throw new NotImplementedException();
        }

        public void execDetailsEnd(int reqId)
        {
            throw new NotImplementedException();
        }

        public void familyCodes(FamilyCode[] familyCodes)
        {
            throw new NotImplementedException();
        }

        public void fundamentalData(int reqId, string data)
        {
            throw new NotImplementedException();
        }

        public void headTimestamp(int reqId, string headTimestamp)
        {
            throw new NotImplementedException();
        }

        public void histogramData(int reqId, HistogramEntry[] data)
        {
            throw new NotImplementedException();
        }

        public void historicalData(int reqId, Bar bar)
        {
            if (Program.allReports == null) return;
            Program.allReports.TryGetValue(reqId, out EarningEvent currentEarning);
            if (currentEarning.Candles == null)
                currentEarning.Candles = new List<Candle>();
            currentEarning.Candles.Add(new Candle
            {
                Time = DateTime.ParseExact(bar.Time, "yyyyMMdd  HH:mm:ss", CultureInfo.InvariantCulture).ToUniversalTime(),
                Open = bar.Open,
                High = bar.High,
                Low = bar.Low,
                Close = bar.Close,
                Count = bar.Count,
                Volume = bar.Volume,
                Wap = bar.WAP
            });
        }

        public void historicalDataEnd(int reqId, string start, string end)
        {
            if (Program.allReports == null || !Program.allReports.TryRemove(reqId, out EarningEvent currentEarning)) return;
            Program.releaseCollection.InsertOneAsync(currentEarning);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{currentEarning.TickerID}) {DateTime.Now:hh:mm:ss} {currentEarning.Symbol} {currentEarning.Date:yyyy.MM} / queue:{Program.allReports.Count} [{currentEarning.Candles.Count}]");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void historicalDataUpdate(int reqId, Bar bar)
        {
            throw new NotImplementedException();
        }

        public void historicalNews(int requestId, string time, string providerCode, string articleId, string headline)
        {
            throw new NotImplementedException();
        }

        public void historicalNewsEnd(int requestId, bool hasMore)
        {
            Console.WriteLine("historicalNewsEnd id:{0} hasMore:{1}", requestId, hasMore);
        }

        public void historicalTicks(int reqId, HistoricalTick[] ticks, bool done)
        {
            throw new NotImplementedException();
        }

        public void historicalTicksBidAsk(int reqId, HistoricalTickBidAsk[] ticks, bool done)
        {
            throw new NotImplementedException();
        }

        public void historicalTicksLast(int reqId, HistoricalTickLast[] ticks, bool done)
        {
            throw new NotImplementedException();
        }

        public void managedAccounts(string accountsList)
        {
            accounts = accountsList;
            Console.WriteLine("Accounts: " + accountsList);
        }

        public void marketDataType(int reqId, int marketDataType)
        {
            Console.WriteLine("marketDataType reqID:{0} marketDataType:{1}", reqId, marketDataType);

        }

        public void marketRule(int marketRuleId, PriceIncrement[] priceIncrements)
        {
            throw new NotImplementedException();
        }

        public void mktDepthExchanges(DepthMktDataDescription[] depthMktDataDescriptions)
        {
            throw new NotImplementedException();
        }

        public void newsArticle(int requestId, int articleType, string articleText)
        {
            throw new NotImplementedException();
        }

        public void newsProviders(NewsProvider[] newsProviders)
        {
            Console.WriteLine("News Providers:");
            foreach (var newsProvider in newsProviders)
            {
                Console.WriteLine("News provider: providerCode - {0}, providerName - {1}",
                    newsProvider.ProviderCode, newsProvider.ProviderName);
            }
            Console.WriteLine();
        }

        public void nextValidId(int orderId)
        {
            nextOrderId = orderId;
            Console.WriteLine("nextValidId: " + orderId);
        }

        public void openOrder(int orderId, Contract contract, Order order, OrderState orderState)
        {
            Console.WriteLine( $"openOrder {contract.Symbol}");
        }

        public void openOrderEnd()
        {
            throw new NotImplementedException();
        }

        public void orderBound(long orderId, int apiClientId, int apiOrderId)
        {
            throw new NotImplementedException();
        }

        public void orderStatus(int orderId, string status, double filled, double remaining, double avgFillPrice, int permId, int parentId, double lastFillPrice, int clientId, string whyHeld, double mktCapPrice)
        {
            //throw new NotImplementedException();
        }

        public void pnl(int reqId, double dailyPnL, double unrealizedPnL, double realizedPnL)
        {
            throw new NotImplementedException();
        }

        public void pnlSingle(int reqId, int pos, double dailyPnL, double unrealizedPnL, double realizedPnL, double value)
        {
            throw new NotImplementedException();
        }

        public void position(string account, Contract contract, double pos, double avgCost)
        {
            throw new NotImplementedException();
        }

        public void positionEnd()
        {
            throw new NotImplementedException();
        }

        public void positionMulti(int requestId, string account, string modelCode, Contract contract, double pos, double avgCost)
        {
            throw new NotImplementedException();
        }

        public void positionMultiEnd(int requestId)
        {
            throw new NotImplementedException();
        }

        public void realtimeBar(int reqId, long date, double open, double high, double low, double close, long volume, double WAP, int count)
        {
            throw new NotImplementedException();
        }

        public void receiveFA(int faDataType, string faXmlData)
        {
            throw new NotImplementedException();
        }

        public void replaceFAEnd(int reqId, string text)
        {
            throw new NotImplementedException();
        }

        public void rerouteMktDataReq(int reqId, int conId, string exchange)
        {
            throw new NotImplementedException();
        }

        public void rerouteMktDepthReq(int reqId, int conId, string exchange)
        {
            throw new NotImplementedException();
        }

        public void scannerData(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark, string projection, string legsStr)
        {
            throw new NotImplementedException();
        }

        public void scannerDataEnd(int reqId)
        {
            throw new NotImplementedException();
        }

        public void scannerParameters(string xml)
        {
            throw new NotImplementedException();
        }

        public void securityDefinitionOptionParameter(int reqId, string exchange, int underlyingConId, string tradingClass, string multiplier, HashSet<string> expirations, HashSet<double> strikes)
        {
            throw new NotImplementedException();
        }

        public void securityDefinitionOptionParameterEnd(int reqId)
        {
            throw new NotImplementedException();
        }

        public void smartComponents(int reqId, Dictionary<int, KeyValuePair<string, char>> theMap)
        {
            throw new NotImplementedException();
        }

        public void softDollarTiers(int reqId, SoftDollarTier[] tiers)
        {
            throw new NotImplementedException();
        }

        public void symbolSamples(int reqId, ContractDescription[] contractDescriptions)
        {
            throw new NotImplementedException();
        }

        public void tickByTickAllLast(int reqId, int tickType, long time, double price, int size, TickAttribLast tickAttribLast, string exchange, string specialConditions)
        {
            throw new NotImplementedException();
        }

        public void tickByTickBidAsk(int reqId, long time, double bidPrice, double askPrice, int bidSize, int askSize, TickAttribBidAsk tickAttribBidAsk)
        {
            throw new NotImplementedException();
        }

        public void tickByTickMidPoint(int reqId, long time, double midPoint)
        {
            throw new NotImplementedException();
        }

        public void tickEFP(int tickerId, int tickType, double basisPoints, string formattedBasisPoints, double impliedFuture, int holdDays, string futureLastTradeDate, double dividendImpact, double dividendsToLastTradeDate)
        {
            throw new NotImplementedException();
        }

        public void tickGeneric(int tickerId, int field, double value)
        {
            throw new NotImplementedException();
        }

        public void tickNews(int tickerId, long timeStamp, string providerCode, string articleId, string headline, string extraData)
        {
            Console.WriteLine("Tick News. Ticker Id: {0}, Time Stamp: {1}, Provider Code: {2}, Article Id: {3}, headline: {4}, extraData: {5}", tickerId, timeStamp, providerCode, articleId, headline, extraData);
        }

        public void tickOptionComputation(int tickerId, int field, int tickAttrib, double impliedVolatility, double delta, double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice)
        {
            throw new NotImplementedException();
        }

        public void tickPrice(int tickerId, int field, double price, TickAttrib attribs)
        {
            throw new NotImplementedException();
        }

        public void tickReqParams(int tickerId, double minTick, string bboExchange, int snapshotPermissions)
        {
            Console.WriteLine("tickReqParams id={0} minTick = {1} bboExchange = {2} snapshotPermissions = {3}", tickerId, minTick, bboExchange, snapshotPermissions);
        }

        public void tickSize(int tickerId, int field, int size)
        {
            throw new NotImplementedException();
        }

        public void tickSnapshotEnd(int tickerId)
        {
            throw new NotImplementedException();
        }

        public void tickString(int tickerId, int field, string value)
        {
            throw new NotImplementedException();
        }

        public void updateAccountTime(string timestamp)
        {
            throw new NotImplementedException();
        }

        public void updateAccountValue(string key, string value, string currency, string accountName)
        {
            throw new NotImplementedException();
        }

        public void updateMktDepth(int tickerId, int position, int operation, int side, double price, int size)
        {
            throw new NotImplementedException();
        }

        public void updateMktDepthL2(int tickerId, int position, string marketMaker, int operation, int side, double price, int size, bool isSmartDepth)
        {
            throw new NotImplementedException();
        }

        public void updateNewsBulletin(int msgId, int msgType, string message, string origExchange)
        {
            throw new NotImplementedException();
        }

        public void updatePortfolio(Contract contract, double position, double marketPrice, double marketValue, double averageCost, double unrealizedPNL, double realizedPNL, string accountName)
        {
            throw new NotImplementedException();
        }

        public void verifyAndAuthCompleted(bool isSuccessful, string errorText)
        {
            throw new NotImplementedException();
        }

        public void verifyAndAuthMessageAPI(string apiData, string xyzChallenge)
        {
            throw new NotImplementedException();
        }

        public void verifyCompleted(bool isSuccessful, string errorText)
        {
            throw new NotImplementedException();
        }

        public void verifyMessageAPI(string apiData)
        {
            throw new NotImplementedException();
        }
    }
}
