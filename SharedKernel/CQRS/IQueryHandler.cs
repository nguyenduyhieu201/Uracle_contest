namespace SharedKernel.CQRS
{
    public interface IQueryHandler <in TQuery, TResponse>
        where TQuery : IQuery<TResponse>
        where TResponse : notnull
    {
    }
}
