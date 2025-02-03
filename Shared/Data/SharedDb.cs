using Domain.Models;
using System.Collections.Concurrent;

namespace Domain.Data
{
    public class SharedDb
    {
        private readonly ConcurrentDictionary<string, UserConnection> _connection = new();

        public ConcurrentDictionary<string, UserConnection> Connection => _connection;

    }
}
