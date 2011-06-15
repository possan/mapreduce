using NUnit.Framework;
using Possan.MapReduce.Impl;

namespace Possan.MapReduce.Tests
{
	[TestFixture]
	public class MemoryKeyValueStoreTests
	{
		IKeyValueStore CreateStore()
		{
			return new DebuggingKeyValueStore(new MemoryKeyValueStore());
		}

		[Test]
		public void Can_write_and_read_back_single_value()
		{
			var store = CreateStore();
			store.Put("key", "value");
			Assert.AreEqual("value", store.Get("key", ""));
		}

		[Test]
		public void Can_write_and_read_back_single_value_from_many()
		{
			var store = CreateStore();
			store.Put("keyx", "valuex");
			store.Put("keyy", "valuey");
			store.Put("key", "value");
			store.Put("keyz", "valuez");
			Assert.AreEqual("value", store.Get("key", ""));
		}

		[Test]
		public void Cant_read_non_existing_data()
		{
			var store = CreateStore();
			store.Put("key", "value");
			Assert.AreEqual("defaultvalue", store.Get("anotherkey", "defaultvalue"));
		}
	}
}