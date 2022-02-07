using BankingServiceASM.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BankingServiceASM
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBankingService" in both code and config file together.
    [ServiceContract]
    public interface IBankingService
    {
        [OperationContract]
        TransactionHistoryDto RequireTransaction(AccountTransactionDto accountTransaction);
        [OperationContract]
        ICollection<TransactionHistoryDto> ShowTransactionHistory(AccountGetTransactionHistoryDto acc);
        [OperationContract]
        AccountDto ShowAccountInformation(AccountLoginDto account);
        [OperationContract]
        bool SendConfirmationPinCode(AccountLoginDto account);
    }
}
