using System.Collections.Generic;
using NUnit.Framework;
using Possan.MapReduce.Impl;

namespace Possan.MapReduce.Tests
{
	[TestFixture]
	public class KeyValueStoreTests
	{
		IStorage CreateStorage()
		{
			return new GenericKeyValueStorage(new DebuggingKeyValueStore(new MemoryKeyValueStore()));
		}

		[Test]
		public void Can_store_and_find_files_in_batch_1()
		{
			var storage = CreateStorage();
			storage.Put("batch1", "file1", "content1");
			Assert.AreEqual("content1", storage.Get("batch1", "file1"));
		}

		[Test]
		public void Cant_find_an_file_in_an_deleted_batch()
		{
			var storage = CreateStorage();
			storage.Put("batch1", "file1", "content1");
			storage.DeleteBatch("batch1");
			Assert.AreEqual("", storage.Get("batch1", "file1"));
		}

		[Test]
		public void Can_store_and_find_file_list_in_batch()
		{
			var storage = CreateStorage();
			storage.Put("batch1", "file1", "content1");
			storage.Put("batch1", "file2", "content2");
			storage.Put("batch2", "file3", "content3");
			Assert.AreEqual("content1", storage.Get("batch1", "file1"));
		}

		[Test]
		public void Can_store_and_find_files_in_deep_batch_1()
		{
			var storage = CreateStorage();
			storage.Put("batch1/batch2/batch3", "file1", "content1");
			Assert.AreEqual("content1", storage.Get("batch1/batch2/batch3", "file1"));
		}

		[Test]
		public void Cant_find_an_file_in_an_recursive_deleted_batch()
		{
			var storage = CreateStorage();
			storage.Put("batch1/batch2/batch3", "file1", "content1");
			storage.DeleteBatch("batch1/batch2");
			Assert.AreEqual("", storage.Get("batch1/batch2/batch3", "file1"));
		}

		[Test]
		public void Can_find_files_in_batch()
		{
			var storage = CreateStorage();
			storage.Put("batch1", "file1", "content1");
			storage.Put("batch1", "file2", "content2");
			storage.Put("batch2", "file3", "content3");
			var list = new List<string>( storage.GetAllFilesInBatch("batch1"));
			Assert.AreEqual(2,list.Count);
			Assert.IsTrue(list.Contains("file1"));
			Assert.IsTrue(list.Contains("file2"));
		}

		[Test]
		public void Returns_filenames_in_batch_sorted()
		{
			var storage = CreateStorage();
			storage.Put("batch1", "file4", "content1");
			storage.Put("batch1", "file1", "content2");
			storage.Put("batch1", "file3", "content2");
			storage.Put("batch1", "file2", "content2");
			storage.Put("batch2", "filex", "content3");
			var list = new List<string>(storage.GetAllFilesInBatch("batch1"));
			Assert.AreEqual(4, list.Count);
			Assert.AreEqual("file1", list[0]);
			Assert.AreEqual("file2", list[1]);
			Assert.AreEqual("file3", list[2]);
			Assert.AreEqual("file4", list[3]);
		}

		[Test]
		public void Returns_subfolders()
		{
			var storage = CreateStorage();
			storage.Put("folder1/folder5/folder3", "file1", "content1");
			storage.Put("folder1/folder5/folder3", "file2", "content2");
			storage.Put("folder1/folder5/folder3", "file3", "content2");
			storage.Put("folder1/folder4/folder5", "file1", "content1");
			storage.Put("folder1/folder4/folder5", "file2", "content2");
			storage.Put("folder1/folder4/folder5", "file3", "content2");
			var list = new List<string>(storage.GetSubBatches("folder1/folder5"));
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("folder3", list[0]);
			list = new List<string>(storage.GetSubBatches("folder1"));
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("folder4", list[0]);
			Assert.AreEqual("folder5", list[1]);
		}

		
	}
}
