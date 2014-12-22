using System;
using System.Collections.Generic;
using System.Reactive.Threading.Tasks;
using EsendexApi.Structures;

namespace EsendexApi.Clients
{
    public class AccountClient
    {
        private readonly IRestFactory _restFactory;

        public AccountClient(IRestFactory restFactory)
        {
            _restFactory = restFactory;
        }

        public IObservable<IEnumerable<Account>> GetAccounts()
        {
             return _restFactory.CreateGetRequest("/v1.0/accounts")
                                .ExecuteAsync<Accounts>()
                                .ContinueWith(res => res.Result.AccountDetails)
                                .ToObservable();
        }
    }
}
