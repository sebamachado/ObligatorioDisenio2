using System;
using System.Web;
using ModeloEF;

public static class ContextoAplicacion
{
    private const string ClaveContexto = "ContextoBiosMessenger";

    public static BiosMessengerContext ObtenerContexto()
    {
        var app = HttpContext.Current.Application;
        var contexto = app[ClaveContexto] as BiosMessengerContext;
        if (contexto == null)
        {
            contexto = new BiosMessengerContext();
            app[ClaveContexto] = contexto;
        }

        return contexto;
    }

    public static void Reiniciar()
    {
        var app = HttpContext.Current.Application;
        if (app[ClaveContexto] is BiosMessengerContext actual)
        {
            actual.Dispose();
        }

        app[ClaveContexto] = new BiosMessengerContext();
    }
}
