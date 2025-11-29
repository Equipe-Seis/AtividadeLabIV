using Metalama.Framework.Aspects;

namespace AtividadeLabIV.Aspects.Auth;

public class AutenticadoAttribute : OverrideMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        if (meta.This is IComponenteAutenticado { IdSessao: not null})
        {
            Console.WriteLine("[AUTH] Sessao autenticada.");
            return meta.Proceed();
        }

        Console.WriteLine("[AUTH](ERROR) Sessao nao autenticada.");
        return default;
    }
}
