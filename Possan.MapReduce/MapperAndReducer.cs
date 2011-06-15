using System;
using System.Collections.Generic;
using System.Linq;

namespace Possan.MapReduce
{
	public class MapperAndReducer
	{
		public static IList<string> MapAndReduce(IStorage storage, string[] inputbatches, IMapper mapper, IReducer reducer)
		{
			string prefix = Guid.NewGuid().ToString().Replace("-", "");

			// map input batch
			var mapresults = Mapper.Map(storage, inputbatches, prefix + "mapper-output", mapper);
			//			foreach (var mapresult in mapresults)
			//			Console.WriteLine("Mapper resulted in: " + mapresult);

			//		Console.WriteLine();
			//	Console.WriteLine("----------------------------------------------------------");
			//Console.WriteLine();

			var shuffledfolders = Shuffler.Shuffle(storage, mapresults.ToArray(), prefix + "shuffler-output");
			//foreach (var shuffledfolder in shuffledfolders)
			//		Console.WriteLine("Shuffled to folder: " + shuffledfolder);

			//		Console.WriteLine();
			//	Console.WriteLine("----------------------------------------------------------");
			// Console.WriteLine();


			var reducerresults = Reducer.Reduce(storage, shuffledfolders.ToArray(), prefix + "reducer-output", reducer);
			//	foreach (var reduceresult in reducerresults)
			//	Console.WriteLine("Reduced to: " + reduceresult);

			return reducerresults;
		}
	}
}
