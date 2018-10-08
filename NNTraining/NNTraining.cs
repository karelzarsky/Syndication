using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Persist;
using Encog.Util;
using Encog.Util.Simple;
using SyndicateLogic;

namespace NNTraining
{
	class NNTraining
	{
		const int networkID = 8;
		const int minutes = 240 * 60;

		static void Main(string[] args)
		{
			using (var p = Process.GetCurrentProcess())
				p.PriorityClass = ProcessPriorityClass.Idle;

			FileInfo dataSetFile = new FileInfo("dataset.egb");
			FileInfo networkFile = new FileInfo($"network{networkID}.nn");
			FileInfo trainFile = new FileInfo($"train{networkID}.tr");

			Console.WriteLine("Loading dataset.");
			if (!dataSetFile.Exists)
			{
				ExtractTrainData(dataSetFile);
				Console.WriteLine(@"Extracting dataset from database: " + dataSetFile);
				return;
			}
			var trainingSet = EncogUtility.LoadEGB2Memory(dataSetFile);
			Console.WriteLine($"Loaded {trainingSet.Count} samples. Input size: {trainingSet.InputSize}, Output size: {trainingSet.IdealSize}");

			BasicNetwork network;
			if (networkFile.Exists)
			{
				Console.WriteLine($"Loading network {networkFile.FullName}");
				network = (BasicNetwork) EncogDirectoryPersistence.LoadObject(networkFile);
			}
			else
			{
				Console.WriteLine("Creating NN.");
				network = EncogUtility.SimpleFeedForward(trainingSet.InputSize, 1000, 200, trainingSet.IdealSize, true);
				network.Reset();
			}

			using (var p = Process.GetCurrentProcess())
				Console.WriteLine($"RAM usage: {p.WorkingSet64 / 1024 / 1024} MB.");

			ResilientPropagation train = new ResilientPropagation(network, trainingSet)
			{
				ThreadCount = 0
			};
			if (trainFile.Exists)
			{
				TrainingContinuation cont = (TrainingContinuation)EncogDirectoryPersistence.LoadObject(trainFile);
				train.Resume(cont);
			}
			MyTrainConsole(train, network, trainingSet, minutes, networkFile, trainFile);
			Console.WriteLine(@"Final Error: " + train.Error);
			Console.WriteLine(@"Training complete, saving network.");
			EncogDirectoryPersistence.SaveObject(networkFile, network);
			Console.WriteLine(@"Network saved. Press s to stop.");
			ConsoleKeyInfo key;
			do
			{
				key = Console.ReadKey();
			}
			while (key.KeyChar != 's');
		}

		public static void MyTrainConsole(IMLTrain train, BasicNetwork network, IMLDataSet trainingSet, int minutes, FileInfo networkFile, FileInfo trainFile)
		{
			int epoch = 1;
			long remaining;
			Console.WriteLine(@"Beginning training...");
			long start = Environment.TickCount;
			do
			{
				train.Iteration();
				long current = Environment.TickCount;
				long elapsed = (current - start) / 1000;
				remaining = minutes - elapsed / 60;
				Console.WriteLine($@"Iteration #{Format.FormatInteger(epoch)} Error:{Format.FormatPercent(train.Error)} elapsed time = {Format.FormatTimeSpan((int)elapsed)} time left = {Format.FormatTimeSpan((int)remaining * 60)}");
				epoch++;
				EncogDirectoryPersistence.SaveObject(networkFile, network);
				TrainingContinuation cont = train.Pause();
				EncogDirectoryPersistence.SaveObject(trainFile, cont);
				train.Resume(cont);
				foreach (var x in cont.Contents)
				{
					Console.WriteLine($"{x.Key}: {((double[])x.Value).Average()}");
				}
			}
			while (remaining > 0 && !train.TrainingDone && !Console.KeyAvailable);
			Console.WriteLine("Finishing.");
			train.FinishTraining();
		}

		private static void ExtractTrainData(FileInfo trainFile)
		{
			var ctx = new Db();
			int inputsCount = ctx.TrainSchemas.Where(x => x.NetworkID == networkID && x.Input == true).Select(x => x.NeuronID).Distinct().Count();
			int outputsCount = ctx.TrainSchemas.Where(x => x.NetworkID == networkID && x.Input == false).Select(x => x.NeuronID).Distinct().Count();
			Console.WriteLine($"in: {inputsCount}, out: {outputsCount}");
			var dataset = new BasicMLDataSet();
			int[] articles = ctx.TrainValues.Where(x => x.NetworkID == networkID).Select(x => x.ArticleID).Distinct().OrderBy(i => i).ToArray();
			var schema = ctx.TrainSchemas.Where(x => x.NetworkID == networkID).ToArray();
			int shingles;
			int forecasts;
			foreach (int articleID in articles)
			{
				var values = ctx.TrainValues.Where(x => x.NetworkID == networkID && x.ArticleID == articleID);
				var inputsArray = new double[inputsCount];
				var idealsArray = new double[outputsCount];
				shingles = 0;
				forecasts = 0;
				foreach (var value in values)
				{
					if (schema.FirstOrDefault(x => x.NeuronID == value.NeuronID).Input)
					{
						shingles++;
						inputsArray[value.NeuronID - 1] = value.Value;
					}
					else
					{
						forecasts++;
						idealsArray[value.NeuronID - inputsCount - 2] = value.Value;
					}
				}
				if (shingles > 0 && forecasts == outputsCount)
				{
					dataset.Add(new BasicMLData(inputsArray), new BasicMLData(idealsArray));
					Console.WriteLine($"Article {articleID}: {shingles} shingles, {idealsArray[outputsCount - 1] * 100:N2}");
				}
				else
				{
					Console.WriteLine($"Article {articleID} NOT complete !!!");
				}
			}
			EncogUtility.SaveEGB(trainFile, dataset);
			Console.WriteLine($"Trainset written to: {trainFile.FullName}");
		}
	}
}
