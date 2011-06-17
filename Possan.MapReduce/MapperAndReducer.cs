using System;

namespace Possan.MapReduce
{
	public class MapperAndReducer
	{
		public static void MapAndReduce(IRecordReader<string, string> inputreader, IRecordWriter<string, string> outputwriter, IMapper mapper, IReducer reducer)
		{
			string prefix = Guid.NewGuid().ToString().Replace("-", "");
		//	(new TextFileRecordDumper("c:\\temp\\MR\\_" + prefix + "_input.txt")).Dump(inputreader, "MapAndReduce Mapper input");
			var mapperoutput = new MemoryKeyValueReaderWriter();
			using (new Timing("Mapper"))
			{
				Mapper.Map(inputreader, mapperoutput, mapper);
			}
		//	(new TextFileRecordDumper("c:\\temp\\MR\\_" + prefix + "_mapped.txt")).Dump(mapperoutput, "MapAndReduce Mapper output");
			var reduceroutput = new NonLockingMemoryKeyValueReaderWriter();
			using (new Timing("Reducer"))
			{
				Reducer.Reduce(mapperoutput, reduceroutput, reducer);
			}
		//	(new TextFileRecordDumper("c:\\temp\\MR\\_" + prefix + "_reduced.txt")).Dump(reduceroutput, "Reducer output");
			using (new Timing("Copy output"))
			{
				outputwriter.Write(reduceroutput);
			}
		}

	}
}
