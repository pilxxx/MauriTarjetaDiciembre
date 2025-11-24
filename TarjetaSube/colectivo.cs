using System;

namespace TarjetaSube
{
    public class Colectivo
    {
        protected string numeroLinea;
        protected decimal valorPasaje;

        public Colectivo(string linea, bool interurbano = false)
        {
            numeroLinea = linea;
            valorPasaje = interurbano ? 3000m : 1580m;
        }

        public string ObtenerLinea() => numeroLinea;

        public Boleto PagarCon(Tarjeta tarjeta, DateTime? fechaHora = null)
        {
            fechaHora ??= DateTime.Now;

            if (!tarjeta.PuedePagarEnHorario(fechaHora.Value))
                return null;

            bool esTrasbordo = tarjeta.PuedeHacerTrasbordo(numeroLinea, fechaHora.Value);
            decimal monto = tarjeta.CalcularMontoACobrar(valorPasaje, esTrasbordo);

            if (monto == 0 || tarjeta.DescontarSaldo(monto))
            {
                tarjeta.RegistrarViaje(numeroLinea, fechaHora.Value);
                return new Boleto(
                    linea: numeroLinea,
                    importe: monto,
                    saldo: tarjeta.ObtenerSaldo(),
                    idTarjeta: tarjeta.ObtenerId(),
                    tipoTarjeta: tarjeta.ToString(),
                    esTransbordo: esTrasbordo,
                    fechaHora: fechaHora
                );
            }

            return null;
        }
    }
}