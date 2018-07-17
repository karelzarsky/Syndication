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
	class GATraining
	{
		const int networkID = 1;
		const double moneyPerTrade = 1000;
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

			var score = CalculateScore(trainingSet, network);
			Console.WriteLine("Total profit: " + score.Sum());
			Console.ReadKey();
		}

		static private double[] CalculateScore(IMLDataSet trainingSet, BasicNetwork network)
		{
			double[] profit = new double[trainingSet.IdealSize];
			int[] win = new int[trainingSet.IdealSize];
			int[] loss = new int[trainingSet.IdealSize];
			int[] goodDir = new int[trainingSet.IdealSize];
			int[] badDir = new int[trainingSet.IdealSize];

			foreach (var article in trainingSet)
			{
				bool tradeMade = false;
				var res = network.Compute(article.Input);
				for (int i = 0; i < trainingSet.IdealSize; i++)
				{
					if (res[i] > 0.1)
					{
						tradeMade = true;
						double reward = article.Ideal[i] * moneyPerTrade;
						if (reward > transactionFee)
							win[i]++;
						else
							loss[i]++;
						if (reward > 0)
							goodDir[i]++;
						else
							badDir[i]++;
						profit[i] += reward - transactionFee;
					}
					if (res[i] < - 0.1)
					{
						tradeMade = true;
						double reward = -article.Ideal[i] * moneyPerTrade;
						if (reward > transactionFee)
							win[i]++;
						else
							loss[i]++;
						if (reward > 0)
							goodDir[i]++;
						else
							badDir[i]++;
						profit[i] += reward - transactionFee;
					}
				}
				if (tradeMade)
				{
					for (int i = 0; i < trainingSet.IdealSize; i++)
					{
						Console.Write($"{profit[i]:F0} ");
					}
					Console.WriteLine();
				}
			}

			Console.Write("Win: ");
			for (int i = 0; i < trainingSet.IdealSize; i++)
			{
				Console.Write($"{win[i]} ");
			}
			Console.WriteLine();

			Console.Write("Loss: ");
			for (int i = 0; i < trainingSet.IdealSize; i++)
			{
				Console.Write($"{loss[i]} ");
			}
			Console.WriteLine();

			Console.Write("Good direction: ");
			for (int i = 0; i < trainingSet.IdealSize; i++)
			{
				Console.Write($"{100.0*goodDir[i]/(badDir[i]+goodDir[i]):F3} ");
			}
			Console.WriteLine();

			Console.Write("Win rate: ");
			for (int i = 0; i < trainingSet.IdealSize; i++)
			{
				Console.Write($"{100.0 * win[i] / (loss[i] + win[i]):F3} ");
			}
			Console.WriteLine();

			return profit;
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
