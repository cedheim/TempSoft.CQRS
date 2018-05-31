using System;
using System.Collections.Generic;

namespace TempSoft.CQRS.ServiceFabric.Tools
{
    public class UriHelper : IUriHelper
    {
        private readonly Dictionary<Type, System.Uri> _uris = new Dictionary<Type, System.Uri>();

        public System.Uri GetUriFor<TService>()
        {
            return _uris.ContainsKey(typeof(TService)) ? _uris[typeof(TService)] : default(System.Uri);
        }

        public void RegisterUri<TService>(System.Uri uri)
        {
            _uris.Add(typeof(TService), uri);
        }
    }
}