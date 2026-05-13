namespace Analisis_Numerico_2026.Helpers
{
    public static class NumberFormatter
    {
        /// <summary>
        /// Formatea un double sin notación científica. Cada '#' es un decimal 
        /// Centraliza el formato para toda la aplicación.
        /// </summary>
        public static string Formato(this double valor)
        {
            if (double.IsNaN(valor) || double.IsInfinity(valor))
                return valor.ToString();
            return valor.ToString("0.##########");
        }
    }
}
