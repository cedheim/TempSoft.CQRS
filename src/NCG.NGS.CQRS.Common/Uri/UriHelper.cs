using System;
using System.Collections.Generic;

namespace NCG.NGS.CQRS.Common.Uri
{
    public class UriHelper : IUriHelper
    {
        private Dictionary<Type, System.Uri> _uris = new Dictionary<Type, System.Uri>();

        public void RegisterUri<TService>(System.Uri uri)
        {
            _uris.Add(typeof(TService), uri);
        }

        public System.Uri GetUriForSerivce<TService>()
        {
            return _uris.ContainsKey(typeof(TService)) ? _uris[typeof(TService)] : default(System.Uri);
        }
    }
}