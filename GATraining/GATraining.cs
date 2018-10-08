using Encog.ML.Data;
using Encog.Neural.Networks;
using Encog.Persist;
using Encog.Util.Simple;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GATraining
{
	public class Results
	{
		public int articlesTraded = 0;
		public double[] profit;
		public int[] win;
		public int[] loss;
		public int[] goodDir;
		public int[] badDir;

		public Results(int outputs)
		{
			profit = new double[outputs];
			win = new int[outputs];
			loss = new int[outputs];
			goodDir = new int[outputs];
			badDir = new int[outputs];
		}
	}


	class GATraining
	{
		const int networkID = 1;
		const double moneyPerTrade = 10000;
		const double transactionFee = 10;
		
		static void Main(string[] args)
		{
			//using (var p = Process.GetCurrentProcess())
			//	p.PriorityClass = ProcessPriorityClass.Idle;

			FileInfo trainFile = new FileInfo("dataset.egb");
			FileInfo networkFile = new FileInfo($"ga_network{networkID}.nn");
			IMLDataSet trainingSet = LoadDataSet(trainFile);
			BasicNetwork network = LoadNetwork(networkFile, trainingSet);

			using (var p = Process.GetCurrentProcess())
				Console.WriteLine($"RAM usage: {p.WorkingSet64 / 1024 / 1024} MB.");

			CalculateScore(trainingSet, network);
			Console.ReadKey();
		}

		static private void CalculateScore(IMLDataSet trainingSet, BasicNetwork network)
		{
			Results res = new Results(trainingSet.IdealSize);
			CalculateProfit(trainingSet, network, res);

			for (int i = 0; i < trainingSet.IdealSize; i++)
			{ Console.Write($"{res.profit[i]:F0} "); }
			Console.WriteLine();

			Console.Write("Win: ");
			for (int i = 0; i < trainingSet.IdealSize; i++)
			{ Console.Write($"{res.win[i]} "); }
			Console.WriteLine();

			Console.Write("Loss: ");
			for (int i = 0; i < trainingSet.IdealSize; i++)
			{ Console.Write($"{res.loss[i]} "); }
			Console.WriteLine();

			Console.Write("Good direction: ");
			for (int i = 0; i < trainingSet.IdealSize; i++)
			{ Console.Write($"{100.0 * res.goodDir[i] / (res.badDir[i] + res.goodDir[i]):F3} "); }
			Console.WriteLine();

			Console.Write("Win rate: ");
			for (int i = 0; i < trainingSet.IdealSize; i++)
			{ Console.Write($"{100.0 * res.win[i] / (res.loss[i] + res.win[i]):F3} "); }
			Console.WriteLine();
			Console.WriteLine($"Articles traded: {res.articlesTraded}");
			Console.WriteLine("Total profit: " + res.profit.Sum());
		}

		private static void CalculateProfit(IMLDataSet trainingSet, BasicNetwork network, Results res)
		{
			foreach (var article in trainingSet)
			{
				bool tradeMade = false;
				var computed = network.Compute(article.Input);
				for (int i = 0; i < trainingSet.IdealSize; i++)
				{
					if (computed[i] > 0.2)
					{
						tradeMade = true;
						double reward = article.Ideal[i] * moneyPerTrade;
						if (reward > transactionFee)
							res.win[i]++;
						else
							res.loss[i]++;
						if (reward > 0)
							res.goodDir[i]++;
						else
							res.badDir[i]++;
						res.profit[i] += reward - transactionFee;
					}
					if (computed[i] < -0.2)
					{
						tradeMade = true;
						double reward = -article.Ideal[i] * moneyPerTrade;
						if (reward > transactionFee)
							res.win[i]++;
						else
							res.loss[i]++;
						if (reward > 0)
							res.goodDir[i]++;
						else
							res.badDir[i]++;
						res.profit[i] += reward - transactionFee;
					}
				}
				if (tradeMade)
				{
					res.articlesTraded++;
				}
			}
		}

		static private BasicNetwork LoadNetwork(FileInfo networkFile, IMLDataSet trainingSet)
		{
			if (networkFile.Exists)
			{
				Console.WriteLine($"Loading network {networkFile.FullName}");
				return (BasicNetwork)EncogDirectoryPersistence.LoadObject(networkFile);
			}
			else
			{
				Console.WriteLine("Creating NN.");
				var network = EncogUtility.SimpleFeedForward(input: trainingSet.InputSize, hidden1: 500, hidden2: 50, output: 3, tanh: true);
				network.Reset();
				return network;
			}
		}

		static private IMLDataSet LoadDataSet(FileInfo trainFile)
		{
			Console.WriteLine("Loading dataset.");
			if (!trainFile.Exists)
			{
				Console.WriteLine(@"File not found: " + trainFile);
				Console.ReadKey();
				Environment.Exit(0);
			}
			IMLDataSet trainingSet = EncogUtility.LoadEGB2Memory(trainFile);
			Console.WriteLine($"Loaded {trainingSet.Count} samples. Input size: {trainingSet.InputSize}, Output size: {trainingSet.IdealSize}");
			return trainingSet;
		}
	}
}
