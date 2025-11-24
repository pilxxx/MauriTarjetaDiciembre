using System;

namespace TarjetaSube
{
    public class TarjetaBoletoGratuito : Tarjeta
    {
        private int viajesHoy;
        private DateTime ultimaFecha;

        public TarjetaBoletoGratuito(int idTarjeta = 0) : base(idTarjeta)
        {
            viajesHoy = 0;
            ultimaFecha = DateTime.MinValue;
        }

        public override bool PuedePagarEnHorario(DateTime fechaHora)
        {
            if (fechaHora.DayOfWeek == DayOfWeek.Saturday || fechaHora.DayOfWeek == DayOfWeek.Sunday)
                return false;
            return fechaHora.Hour >= 6 && fechaHora.Hour < 22;
        }

        public override decimal CalcularMontoACobrar(decimal montoBase, bool esTrasbordo)
        {
            if (esTrasbordo) return 0m;

            DateTime hoy = fechaHora.Date;
            if (ultimaFecha.Date != hoy)
                viajesHoy = 0;

            return viajesHoy < 2 ? 0m : montoBase;
        }

        public override void RegistrarViaje(string lineaColectivo, DateTime fechaHora)
        {
            base.RegistrarViaje(lineaColectivo, fechaHora);
            DateTime hoy = fechaHora.Date;
            if (ultimaFecha.Date != hoy)
                viajesHoy = 0;
            viajesHoy++;
            ultimaFecha = fechaHora;
        }

        public override string ToString() => "Boleto Educativo Gratuito";
    }
}