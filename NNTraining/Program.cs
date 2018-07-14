using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Util.File;
using Encog.Util.Simple;
using SyndicateLogic;
using SyndicateLogic.Entities;

namespace NNTraining
{
	class Program
	{
		const int networkID = 1;

		static void Main(string[] args)
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
					Console.WriteLine($"Article {articleID}: {shingles} shingles, {idealsArray[outputsCount-1]*100:N2}");
				}
				else
				{
					Console.WriteLine($"Article {articleID} NOT complete !!!");
				}
			}
			EncogUtility.SaveEGB(new FileInfo("dataset.egb"), dataset);
			Console.ReadKey();
		}
	}
}
