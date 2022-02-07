using BankingServiceASM.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BankingServiceASM
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBankingAuthService" in both code and config file together.
    [ServiceContract]
    public interface IBankingAuthService
    {
        [OperationContract]
        AccountDto RegisterAccount(AccountRegisterDto newAccount); 
    }
}
