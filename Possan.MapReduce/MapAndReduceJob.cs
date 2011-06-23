using Possan.MapReduce.IO;

namespace Possan.MapReduce
{
	public class MapAndReduceJob
	{
		public IPartitioner InputPartitioner { get; set; }
		public IReducer Reducer { get; set; }
		public IPartitioner ShufflePartitioner { get; set; }
		public IMapper Mapper { get; set; }
		public bool CombineOutput { get; set; }
		public IFileSource<string, string> Input;
		public IFileDestination<string, string> Output;
		public bool InputFitsInMemory { get; set; }
		public bool MapperFitsInMemory { get; set; }
		public bool ShufflerFitsInMemory { get; set; }
		public bool ReducerFitsInMemory { get; set; }

		internal MapAndReduceJob()
		{
			InputPartitioner = null;
			Mapper = null;
			Reducer = null;
			ShufflePartitioner = null;
			CombineOutput = false;
			Input = null;
			Output = null;
			InputFitsInMemory = false;
			MapperFitsInMemory = false;
			ShufflerFitsInMemory = false;
			ReducerFitsInMemory = false;
		}

		IFileSourceAndDestination<string, string> CreateTemporaryStorage(bool inmemory)
		{
			if (inmemory)
				return new InMemoryTempFolder();
			return new TempFolder();
		}

		public void Run()
		{
			var mapperoutput = CreateTemporaryStorage(MapperFitsInMemory);

			if (InputPartitioner != null)
			{
				var temp1 = CreateTemporaryStorage(InputFitsInMemory);
				Util.Splitter.Split(Input, temp1, InputPartitioner);
				Util.Mapper.Map(temp1, mapperoutput, Mapper, Reducer);
			}
			else
			{
				Util.Mapper.Map(Input, mapperoutput, Mapper, Reducer);
			}

			// samma

			if (ShufflePartitioner != null)
			{
				var shuffleroutput = CreateTemporaryStorage(ShufflerFitsInMemory);
				var reducertemp = CreateTemporaryStorage(ReducerFitsInMemory);
				Util.Splitter.Split(mapperoutput, shuffleroutput, ShufflePartitioner);
				Util.Reducer.Reduce(shuffleroutput, reducertemp, Reducer);
				Util.Combiner.Combine(reducertemp, Output);
			}
			else
			{
				Util.Reducer.Reduce(mapperoutput, Output, Reducer);
			}

		}
	}
}