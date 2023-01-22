using System;

namespace Repository.Infrastructure;

public class NotFoundException : Exception
{
    public NotFoundException() : base() { }

    public NotFoundException(string message) : base(message) { }
}

public class AlreadyExistsException : Exception
{
    public AlreadyExistsException() : base() { }

    public AlreadyExistsException(string message) : base(message) { }
}