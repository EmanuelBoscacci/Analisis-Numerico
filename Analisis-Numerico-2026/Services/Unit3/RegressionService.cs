using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Analisis_Numerico_2026.Models.Unit3;
using Analisis_Numerico_2026.Helpers;

namespace Analisis_Numerico_2026.Services.Unit3
{
    public class RegressionService
    {
        public RegressionResult CalcularRegresion(RegressionInput input)
        {
            var result = new RegressionResult();
            
            // 1. Parsear los puntos de entrada
            var puntos = ParsearPuntos(input.RawPoints);
            result.Puntos = puntos;

            if (puntos.Count < 2)
            {
                result.Success = false;
                result.Message = "Se requieren al menos 2 puntos válidos para realizar la regresión.";
                return result;
            }

            result.Success = true;

            // 2. Calcular Regresión Lineal
            CalcularRegresionLinealInterno(puntos, input.Tolerancia, result);

            // 3. Calcular Regresión Polinomial
            CalcularRegresionPolinomialInterno(puntos, input.Grado, input.Tolerancia, result);

            return result;
        }

        private void CalcularRegresionLinealInterno(List<double[]> puntos, double tolerancia, RegressionResult result)
        {
            int n = puntos.Count;
            double sumX = 0;
            double sumY = 0;
            double sumXY = 0;
            double sumX2 = 0;

            foreach (var p in puntos)
            {
                double x = p[0];
                double y = p[1];

                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
            }

            double denominador = n * sumX2 - sumX * sumX;
            if (Math.Abs(denominador) < 1e-9)
            {
                result.LinearSuccess = false;
                result.LinearMessage = "Error: El denominador es cero. Los puntos cargados no permiten definir una recta única (por ejemplo, están alineados verticalmente).";
                return;
            }

            double a1 = (n * sumXY - sumX * sumY) / denominador;
            double a0 = (sumY / n) - a1 * (sumX / n);

            result.LinearCoeficientes = new List<double> { a0, a1 };

            double yMedia = sumY / n;
            double st = 0;
            double sr = 0;

            foreach (var p in puntos)
            {
                double x = p[0];
                double y = p[1];

                st += Math.Pow(yMedia - y, 2);
                sr += Math.Pow(a1 * x + a0 - y, 2);
            }

            double r = 0;
            if (st > 0)
            {
                double ratio = (st - sr) / st;
                if (ratio < 0) ratio = 0;
                r = Math.Sqrt(ratio) * 100;
            }
            else if (Math.Abs(sr) < 1e-9)
            {
                r = 100;
            }

            result.LinearR = r;
            result.LinearPearsonR = (a1 >= 0 ? 1.0 : -1.0) * (r / 100.0);

            double tol = tolerancia;
            if (tol <= 1.0)
            {
                tol *= 100;
            }

            result.LinearAjusteAceptable = r >= tol;
            result.LinearSuccess = true;

            string a1Str = a1.Formato();
            string a0Str = Math.Abs(a0).Formato();

            if (Math.Abs(a1) < 1e-9)
            {
                result.LinearFuncionObtenida = $"y = {a0.Formato()}";
            }
            else
            {
                string sign = a0 >= 0 ? "+" : "-";
                if (Math.Abs(a0) < 1e-9)
                {
                    result.LinearFuncionObtenida = $"y = {a1Str}x";
                }
                else
                {
                    result.LinearFuncionObtenida = $"y = {a1Str}x {sign} {a0Str}";
                }
            }

            result.LinearMessage = result.LinearAjusteAceptable 
                ? "El ajuste es aceptable en relación con la tolerancia indicada." 
                : "El ajuste no es aceptable ya que la correlación es menor que la tolerancia indicada.";
        }

        private void CalcularRegresionPolinomialInterno(List<double[]> puntos, int grado, double tolerancia, RegressionResult result)
        {
            result.PolyGrado = grado;
            int n = puntos.Count;

            if (n < grado + 1)
            {
                result.PolySuccess = false;
                result.PolyMessage = $"Se requieren al menos {grado + 1} puntos válidos para realizar una regresión polinomial de grado {grado}.";
                return;
            }

            double[,] matriz = GenerarMatrizPolinomial(grado, puntos);
            double[]? solucion = ResolverGaussJordan(matriz, grado + 1);

            if (solucion == null)
            {
                result.PolySuccess = false;
                result.PolyMessage = "Error: El sistema de ecuaciones polinomiales no tiene una solución única. Compruebe los puntos ingresados.";
                return;
            }

            result.PolyCoeficientes = solucion.ToList();

            string funcion = string.Empty;
            string signo = string.Empty;
            for (int i = 0; i < solucion.Length; i++)
            {
                double ai = Math.Round(solucion[i], 4);
                if (ai != 0)
                {
                    if (i == 0)
                    {
                        funcion = $"{ai}";
                    }
                    else if (i == 1)
                    {
                        string space = string.IsNullOrEmpty(signo) ? "" : " ";
                        funcion = $"{ai}x {signo}{space}{funcion}";
                    }
                    else
                    {
                        string space = string.IsNullOrEmpty(signo) ? "" : " ";
                        funcion = $"{ai}x^{i} {signo}{space}{funcion}";
                    }
                }
                signo = ai > 0 ? "+" : string.Empty;
            }

            if (string.IsNullOrEmpty(funcion))
            {
                funcion = "0";
            }
            result.PolyFuncionObtenida = $"y = {funcion}";

            double sumY = puntos.Sum(p => p[1]);
            double yMedia = sumY / n;
            double st = 0;
            double sr = 0;

            foreach (var p in puntos)
            {
                double x = p[0];
                double y = p[1];

                double suma = 0;
                for (int i = 0; i < solucion.Length; i++)
                {
                    suma += solucion[i] * Math.Pow(x, i);
                }

                sr += Math.Pow(suma - y, 2);
                st += Math.Pow(yMedia - y, 2);
            }

            double r = 0;
            if (st > 0)
            {
                double ratio = (st - sr) / st;
                if (ratio < 0) ratio = 0;
                r = Math.Sqrt(ratio) * 100;
            }
            else if (Math.Abs(sr) < 1e-9)
            {
                r = 100;
            }

            result.PolyR = r;
            result.PolyPearsonR = r / 100.0;

            double tol = tolerancia;
            if (tol <= 1.0)
            {
                tol *= 100;
            }

            result.PolyAjusteAceptable = r >= tol;
            result.PolySuccess = true;

            result.PolyMessage = result.PolyAjusteAceptable 
                ? "El ajuste es aceptable en relación con la tolerancia indicada." 
                : "El ajuste no es aceptable ya que la correlación es menor que la tolerancia indicada.";
        }

        private List<double[]> ParsearPuntos(string rawPoints)
        {
            var puntos = new List<double[]>();
            if (string.IsNullOrWhiteSpace(rawPoints))
                return puntos;

            var lineas = rawPoints.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var linea in lineas)
            {
                var partes = linea.Split(new[] { ',', ';', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length >= 2)
                {
                    if (double.TryParse(partes[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x) &&
                        double.TryParse(partes[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
                    {
                        puntos.Add(new double[] { x, y });
                    }
                    else if (double.TryParse(partes[0].Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double x2) &&
                             double.TryParse(partes[1].Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double y2))
                    {
                        puntos.Add(new double[] { x2, y2 });
                    }
                }
            }
            return puntos;
        }

        private double[,] GenerarMatrizPolinomial(int grado, List<double[]> puntosCargados)
        {
            int dimension = grado + 1;
            double[,] matriz = new double[dimension, dimension + 1];
            
            foreach (var punto in puntosCargados)
            {
                double x = punto[0];
                double y = punto[1];

                for (int fila = 0; fila < dimension; fila++)
                {
                    for (int col = 0; col < dimension; col++)
                    {
                        matriz[fila, col] += Math.Pow(x, fila + col);
                    }
                    matriz[fila, dimension] += y * Math.Pow(x, fila);
                }
            }

            return matriz;
        }

        private double[]? ResolverGaussJordan(double[,] matriz, int dimension)
        {
            double[][] m = new double[dimension][];
            for (int i = 0; i < dimension; i++)
            {
                m[i] = new double[dimension + 1];
                for (int j = 0; j <= dimension; j++)
                {
                    m[i][j] = matriz[i, j];
                }
            }

            for (int rowDiag = 0; rowDiag < dimension; rowDiag++)
            {
                int maxRow = rowDiag;
                double maxVal = Math.Abs(m[rowDiag][rowDiag]);
                for (int i = rowDiag + 1; i < dimension; i++)
                {
                    if (Math.Abs(m[i][rowDiag]) > maxVal)
                    {
                        maxVal = Math.Abs(m[i][rowDiag]);
                        maxRow = i;
                    }
                }

                if (maxRow != rowDiag)
                {
                    double[] temp = m[rowDiag];
                    m[rowDiag] = m[maxRow];
                    m[maxRow] = temp;
                }

                double pivote = m[rowDiag][rowDiag];
                if (Math.Abs(pivote) < 1e-15)
                {
                    return null; // El sistema no tiene solución única
                }

                for (int col = 0; col <= dimension; col++)
                {
                    m[rowDiag][col] /= pivote;
                }

                for (int row = 0; row < dimension; row++)
                {
                    if (row == rowDiag) continue;
                    double coef = m[row][rowDiag];
                    for (int col = 0; col <= dimension; col++)
                    {
                        m[row][col] -= coef * m[rowDiag][col];
                    }
                }
            }

            double[] solucion = new double[dimension];
            for (int i = 0; i < dimension; i++)
            {
                solucion[i] = m[i][dimension];
            }
            return solucion;
        }
    }
}
