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

namespace NNTest
{
	class NNTest
	{
		const string fmt = "{0:+00.00‰;-00.00‰; 00.00‰} ";

		static void Main(string[] args)
		{
			Console.Write("Enter network ID: ");
			int networkID = int.Parse(Console.ReadLine());

			FileInfo trainFile = new FileInfo("dataset.egb");
			FileInfo networkFile = new FileInfo($"network{networkID}.nn");

			IMLDataSet trainingSet = LoadDataSet(trainFile);
			BasicNetwork network = LoadNetwork(networkFile);

			using (var p = Process.GetCurrentProcess())
				Console.WriteLine($"RAM usage: {p.WorkingSet64 / 1024 / 1024} MB.");

			foreach (var sample in trainingSet)
			{
				Console.WriteLine("------------");
				for (int i = 0; i < sample.Ideal.Count; i++)
				{
					Console.Write(fmt, sample.Ideal[i]);
				}
				Console.WriteLine();
				//Console.WriteLine(sample.Ideal.ToString());
				IMLData res = network.Compute(sample.Input);
				for (int i = 0; i < sample.Ideal.Count; i++)
				{
					Console.Write(fmt, res[i]);
				}
				Console.WriteLine();
				//Console.WriteLine(res.ToString());
				Console.ReadKey();
			}
		}

		private static BasicNetwork LoadNetwork(FileInfo networkFile)
		{
			if (networkFile.Exists)
			{
				Console.WriteLine($"Loading network {networkFile.FullName}");
				return (BasicNetwork)EncogDirectoryPersistence.LoadObject(networkFile);
			}
			else
			{
				Console.WriteLine(@"File not found: " + networkFile.FullName);
				Console.ReadKey();
				Environment.Exit(0);
			}
			return null;
		}

		private static IMLDataSet LoadDataSet(FileInfo trainFile)
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
