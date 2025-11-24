using System;
using System.Collections.Generic;

namespace TarjetaSube
{
    public class Tarjeta
    {
        // Lista de montos permitidos para cargar (única instancia en toda la aplicación)
        protected static readonly List<decimal> montosPermitidos = new()
        {
            2000m, 3000m, 4000m, 5000m, 8000m,
            10000m, 15000m, 20000m, 25000m, 30000m
        };

        // Campos de la tarjeta
        protected decimal saldo;
        protected decimal saldoPendiente;
        protected readonly int id;

        private const decimal LIMITE_NEGATIVO = -1200m;
        private const decimal LIMITE_MAXIMO = 56000m;

        // Uso frecuente (30-59: 20% off, 60-80: 25% off)
        private int viajesDelMes;
        private DateTime ultimoMesRegistrado = DateTime.MinValue;

        // Trasbordo
        protected DateTime ultimoViajeParaTrasbordo = DateTime.MinValue;
        protected string ultimaLineaViajada = "";

        public Tarjeta(int idTarjeta = 0)
        {
            id = idTarjeta;
            saldo = 0m;
            saldoPendiente = 0m;
            viajesDelMes = 0;
        }

        public int ObtenerId() => id;
        public decimal ObtenerSaldo() => saldo;
        public decimal ObtenerSaldoPendiente() => saldoPendiente;

        // ===================================================================
        // CARGAR SALDO
        // ===================================================================
        public virtual bool CargarSaldo(decimal monto)
        {
            if (!montosPermitidos.Contains(monto))
                return false;

            // Si hay saldo pendiente, intentar acreditarlo primero
            if (saldoPendiente > 0)
                AcreditarCarga();

            decimal nuevoSaldo = saldo + monto;

            if (nuevoSaldo > LIMITE_MAXIMO)
            {
                decimal excedente = nuevoSaldo - LIMITE_MAXIMO;
                saldo = LIMITE_MAXIMO;
                saldoPendiente += excedente;
            }
            else
            {
                saldo = nuevoSaldo;
            }

            return true;
        }

        // ===================================================================
        // ACREDITAR SALDO PENDIENTE (cuando se gasta)
        // ===================================================================
        public virtual void AcreditarCarga()
        {
            if (saldoPendiente <= 0) return;

            decimal espacioDisponible = LIMITE_MAXIMO - saldo;

            if (espacioDisponible <= 0) return;

            if (saldoPendiente <= espacioDisponible)
            {
                saldo += saldoPendiente;
                saldoPendiente = 0m;
            }
            else
            {
                saldo = LIMITE_MAXIMO;
                saldoPendiente -= espacioDisponible;
            }
        }

        // ===================================================================
        // DESCONTAR SALDO (virtual para que franquicias lo sobreescriban)
        // ===================================================================
        public virtual bool DescontarSaldo(decimal monto)
        {
            decimal saldoResultante = saldo - monto;

            if (saldoResultante < LIMITE_NEGATIVO)
                return false;

            saldo = saldoResultante;

            // Después de gastar, acreditamos saldo pendiente si hay
            if (saldoPendiente > 0)
                AcreditarCarga();

            return true;
        }

        // ===================================================================
        // USO FRECUENTE (solo tarjetas normales)
        // ===================================================================
        protected virtual decimal AplicarDescuentoUsoFrecuente(decimal montoBase)
        {
            DateTime ahora = DateTime.Now;

            // Reiniciar contador si cambió el mes
            if (ultimoMesRegistrado == DateTime.MinValue ||
                ahora.Month != ultimoMesRegistrado.Month ||
                ahora.Year != ultimoMesRegistrado.Year)
            {
                viajesDelMes = 0;
                ultimoMesRegistrado = ahora;
            }

            viajesDelMes++;

            if (viajesDelMes >= 30 && viajesDelMes <= 59)
                return montoBase * 0.80m;   // 20% descuento
            if (viajesDelMes >= 60 && viajesDelMes <= 80)
                return montoBase * 0.75m;   // 25% descuento

            return montoBase; // 1-29 y 81+ → tarifa normal
        }

        // ===================================================================
        // MÉTODOS VIRTUALES QUE LAS SUBCLASES VAN A SOBREESCRIBIR
        // ===================================================================
        public virtual decimal CalcularMontoACobrar(decimal montoBase, bool esTrasbordo)
        {
            if (esTrasbordo) return 0m;
            return AplicarDescuentoUsoFrecuente(montoBase);
        }

        public virtual bool PuedePagarEnHorario(DateTime fechaHora)
        {
            // Tarjeta normal siempre puede pagar
            return true;
        }

        public virtual bool PuedeHacerTrasbordo(string lineaColectivo, DateTime fechaHora)
        {
            if (ultimoViajeParaTrasbordo == DateTime.MinValue)
                return false;

            TimeSpan diferencia = fechaHora - ultimoViajeParaTrasbordo;

            bool dentroDeUnaHora = diferencia.TotalMinutes <= 60;
            bool lineaDistinta = !string.Equals(lineaColectivo, ultimaLineaViajada, StringComparison.OrdinalIgnoreCase);
            bool diaValido = fechaHora.DayOfWeek >= DayOfWeek.Monday && fechaHora.DayOfWeek <= DayOfWeek.Saturday;
            bool horarioTrasbordo = fechaHora.Hour >= 7 && fechaHora.Hour < 22;

            return dentroDeUnaHora && lineaDistinta && diaValido && horarioTrasbordo;
        }

        public virtual void RegistrarViaje(string lineaColectivo, DateTime fechaHora)
        {
            ultimoViajeParaTrasbordo = fechaHora;
            ultimaLineaViajada = lineaColectivo;
        }

        // ===================================================================
        // Para debugging / tests
        // ===================================================================
        public int ObtenerViajesDelMes() => viajesDelMes;
        public DateTime ObtenerUltimoViajeTrasbordo() => ultimoViajeParaTrasbordo;
        public string ObtenerUltimaLinea() => ultimaLineaViajada;
    }
}