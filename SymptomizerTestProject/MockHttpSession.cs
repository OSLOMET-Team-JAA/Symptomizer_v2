using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymptomizerTestProject
{
    public class MockHttpSession : ISession
    {
        private Dictionary<string, object> _sessionStorage = new Dictionary<string, object>();

        public object this[string name]
        {
            get { return _sessionStorage[name]; }
            set { _sessionStorage[name] = value; }
        }

        void ISession.Set(string key, byte[] value)
        {
            _sessionStorage[key] = value;
        }

        bool ISession.TryGetValue(string key, out byte[] value)
        {
            if (_sessionStorage[key] != null)
            {
                value = Encoding.ASCII.GetBytes(_sessionStorage[key].ToString());
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
        // de underligggende metodene er ikke nødvendige for mocking 

        IEnumerable<string> ISession.Keys
        {
            get { throw new NotImplementedException(); }
        }

        string ISession.Id
        {
            get { throw new NotImplementedException(); }
        }

        bool ISession.IsAvailable
        {
            get { throw new NotImplementedException(); }
        }

        void ISession.Clear()
        {
            throw new NotImplementedException();
        }

        void ISession.Remove(string key)
        {
            throw new NotImplementedException();
        }

        Task ISession.CommitAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task ISession.LoadAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
