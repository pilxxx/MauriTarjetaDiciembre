using System;

namespace TarjetaSube
{
    public class TarjetaMedioBoleto : Tarjeta
    {
        private int viajesMedioHoy;
        private DateTime ultimaFechaMedio;
        private DateTime ultimoViaje;

        public TarjetaMedioBoleto(int idTarjeta = 0) : base(idTarjeta)
        {
            viajesMedioHoy = 0;
            ultimaFechaMedio = DateTime.MinValue;
            ultimoViaje = DateTime.MinValue;
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
            if (ultimaFechaMedio.Date != hoy)
                viajesMedioHoy = 0;

            // Regla de los 5 minutos
            if (ultimoViaje != DateTime.MinValue)
            {
                if ((fechaHora - ultimoViaje).TotalMinutes < 5)
                    return montoBase; // Cobra completo si intenta viajar antes de 5 min
            }

            return viajesMedioHoy < 2 ? montoBase / 2 : montoBase;
        }

        public override void RegistrarViaje(string lineaColectivo, DateTime fechaHora)
        {
            base.RegistrarViaje(lineaColectivo, fechaHora);

            DateTime hoy = fechaHora.Date;
            if (ultimaFechaMedio.Date != hoy)
                viajesMedioHoy = 0;

            if (viajesMedioHoy < 2)
                viajesMedioHoy++;

            ultimoViaje = fechaHora;
            ultimaFechaMedio = fechaHora;
        }

        public override string ToString() => "Medio Boleto";
    }
}