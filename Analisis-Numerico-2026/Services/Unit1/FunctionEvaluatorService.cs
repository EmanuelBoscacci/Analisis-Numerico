using Calculus;

namespace Analisis_Numerico_2026.Services.Unit1
{
    public class FunctionEvaluatorService
    {
        private Calculo? _calculo;
        private bool _funcionValida;

        /// <summary>
        /// Inicializa el cálculo con la función.
        /// Sintaxis() se llama UNA SOLA VEZ acá.
        /// </summary>
        public bool InicializarFuncion(string funcion)
        {
            _calculo = new Calculo();
            _funcionValida = _calculo.Sintaxis(funcion, 'x');
            return _funcionValida;
        }

        /// <summary>
        /// Evalúa la función en un punto x.
        /// NO vuelve a llamar a Sintaxis().
        /// </summary>
        public double Evaluar(double x)
        {
            if (_calculo == null || !_funcionValida)
                throw new InvalidOperationException("Debes inicializar la función primero.");

            return _calculo.EvaluaFx(x);
        }

        /// <summary>
        /// Valida la sintaxis de una función (uso independiente).
        /// </summary>
        public bool ValidarSintaxis(string funcion)
        {
            var temp = new Calculo();
            return temp.Sintaxis(funcion, 'x');
        }

        /// <summary>
        /// Calcula la derivada en un punto.
        /// NO vuelve a llamar a Sintaxis().
        /// </summary>
        public double CalcularDerivada(double x)
        {
            if (_calculo == null || !_funcionValida)
                throw new InvalidOperationException("Debes inicializar la función primero.");

            return _calculo.Dx(x);
        }
    }
}