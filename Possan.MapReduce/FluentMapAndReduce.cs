using System;

namespace Possan.MapReduce
{
	public class FluentMapAndReduce
	{
		private MapAndReduceJob job;

		public FluentMapAndReduce()
		{
			job = new MapAndReduceJob();
		}
		
		public FluentMapAndReduce Input(IFileSource<string, string> input)
		{
			job.Input = input;
			return this;
		}

		public FluentMapAndReduce WithMapper(IMapper mapper)
		{
			job.Mapper = mapper;
			return this;
		}

		public FluentMapAndReduce WithInMemoryMapper(IMapper mapper)
		{
			job.Mapper = mapper;
			job.MapperFitsInMemory = true;
			return this;
		}
		
		public FluentMapAndReduce PartitionInputInMemoryUsing(IPartitioner partitioner)
		{
			job.InputPartitioner = partitioner;
			job.InputFitsInMemory = true;
			return this;
		}
		
		public FluentMapAndReduce PartitionInputUsing(IPartitioner partitioner)
		{
			job.InputPartitioner = partitioner;
			return this;
		}

		public FluentMapAndReduce ReduceAfterEveryMapper()
		{
			job.RunReducerAfterEveryMapper = true;
			return this;
		}

		public FluentMapAndReduce ReduceUsing(IReducer reducer)
		{
			job.Reducer = reducer;
			return this;
		}

		public FluentMapAndReduce ReduceInMemoryUsing(IReducer reducer)
		{
			job.Reducer = reducer;
			job.ReducerFitsInMemory = true;
			return this;
		}

		public FluentMapAndReduce ShuffleUsing(IPartitioner partitioner)
		{
			job.ShufflePartitioner = partitioner;
			return this;
		}

		public FluentMapAndReduce ShuffleInMemoryUsing(IPartitioner partitioner)
		{
			job.ShufflePartitioner = partitioner;
			job.ShufflerFitsInMemory = true;
			return this;
		}

		public void CombineTo(IFileDestination<string, string> output)
		{
			job.CombineOutput = true;
			job.Output = output;
			job.Run();
		}

		public void To(IFileDestination<string, string> output)
		{
			job.CombineOutput = false;
			job.Output = output;
			job.Run();
		}
	}
}