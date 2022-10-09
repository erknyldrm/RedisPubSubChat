namespace ElasticCore
{
    public interface IElasticCoreService<T> where T : class
    {
        public IReadOnlyCollection<ChatModel> SearchChatLog(int rowCount);
        public void CheckExistsAndInsertLog(T logModel, string indexName);
    }
}
