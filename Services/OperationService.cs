namespace ClinicalTrialsApi.Services;

public interface ITransientOperation
{
    Guid OperationId { get; }
}

public interface IScopedOperation
{
    Guid OperationId { get; }
}

public interface ISingletonOperation
{
    Guid OperationId { get; }
}


public class Operation : ITransientOperation, IScopedOperation, ISingletonOperation
{
    public Guid OperationId { get; } = Guid.NewGuid();
}