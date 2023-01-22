using System;

namespace Models;

public class TransactionCode
{
    public char? SendingRole { get; }
    public char? ReceivingRole { get; }

    public TransactionCode(char? sendingRole, char? receivingRole)
    {
        SendingRole = sendingRole;
        ReceivingRole = receivingRole;
    }
}