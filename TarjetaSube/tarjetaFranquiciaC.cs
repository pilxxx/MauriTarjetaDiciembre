using System;

namespace TarjetaSube
{
    public class TarjetaFranquiciaCompleta : Tarjeta
    {
        public TarjetaFranquiciaCompleta(int idTarjeta = 0) : base(idTarjeta) { }

        public override decimal CalcularMontoACobrar(decimal montoBase, bool esTrasbordo)
        {
            return 0m; // Siempre gratis
        }

        public override bool PuedePagarEnHorario(DateTime fechaHora)
        {
            if (fechaHora.DayOfWeek == DayOfWeek.Saturday || fechaHora.DayOfWeek == DayOfWeek.Sunday)
                return false;
            return fechaHora.Hour >= 6 && fechaHora.Hour < 22;
        }

        public override bool DescontarSaldo(decimal monto) => true; // Nunca descuenta

        public override string ToString() => "Franquicia Completa";
    }
}