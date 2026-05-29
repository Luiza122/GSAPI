namespace AgroOrbit.Api;

public class RegraNegocioException : Exception
{
    public RegraNegocioException(string message) : base(message)
    {
    }
}

public class RecursoNaoEncontradoException : Exception
{
    public RecursoNaoEncontradoException(string message) : base(message)
    {
    }
}
