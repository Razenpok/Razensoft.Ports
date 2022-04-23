namespace Razensoft.Ports
{
    public interface IBaseRequest { }

    public interface IRequest<TResponse> : IBaseRequest { }

    public interface IRequest : IRequest<Unit> { }
}