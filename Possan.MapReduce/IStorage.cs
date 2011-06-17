using System.Collections.Generic;

namespace Possan.MapReduce
{
	public interface IStorage
	{
		string CreateFilename();
		string CreateBatch();

		void DeleteBatch(string batch);

		IEnumerable<string> GetSubBatches(string parentbatch);
		IEnumerable<string> GetAllFilesInBatch(string batch);
		
		string Get(string batch, string filename);
		void Put(string batch, string filename, string content);
		 
		void Copy(string inputbatch, string inputfilename, string outputbatch, string outputfilename);
	}
}