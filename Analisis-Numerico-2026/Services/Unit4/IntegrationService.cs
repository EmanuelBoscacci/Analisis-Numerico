using System;
using Calculus;
using Analisis_Numerico_2026.Models.Unit4;

namespace Analisis_Numerico_2026.Services.Unit4
{
    public class IntegrationService
    {
        public IntegrationResult Integrar(IntegrationInput input)
        {
            var result = new IntegrationResult();
            var calculo = new Calculo();

            // 1. Validar la sintaxis de la función
            if (!calculo.Sintaxis(input.Function, 'x'))
            {
                result.Success = false;
                result.Method = ObtenerNombreMetodo(input.Method);
                result.Message = "Función mal ingresada. Compruebe la sintaxis matemática.";
                return result;
            }

            result.Function = input.Function;
            result.Method = ObtenerNombreMetodo(input.Method);

            try
            {
                double xi = input.Xi;
                double xd = input.Xd;
                int n = input.Subintervalos;
                double area = 0;

                switch (input.Method)
                {
                    case "TrapeciosSimple":
                        {
                            double fXi = calculo.EvaluaFx(xi);
                            double fXd = calculo.EvaluaFx(xd);
                            area = ((fXi + fXd) * (xd - xi)) / 2.0;
                            break;
                        }

                    case "TrapeciosMultiple":
                        {
                            if (n < 1)
                            {
                                result.Success = false;
                                result.Message = "La cantidad de subintervalos debe ser de al menos 1.";
                                return result;
                            }
                            double h = (xd - xi) / n;
                            double sum = 0;
                            for (int i = 1; i < n; i++)
                            {
                                sum += calculo.EvaluaFx(xi + h * i);
                            }
                            double fXi = calculo.EvaluaFx(xi);
                            double fXd = calculo.EvaluaFx(xd);
                            area = (h / 2.0) * (fXi + 2.0 * sum + fXd);
                            break;
                        }

                    case "Simpson13Simple":
                        {
                            double h = (xd - xi) / 2.0;
                            double fXi = calculo.EvaluaFx(xi);
                            double fMid = calculo.EvaluaFx(xi + h);
                            double fXd = calculo.EvaluaFx(xd);
                            area = (h / 3.0) * (fXi + 4.0 * fMid + fXd);
                            break;
                        }

                    case "Simpson13Multiple":
                        {
                            if (n < 1)
                            {
                                result.Success = false;
                                result.Message = "La cantidad de subintervalos debe ser de al menos 1.";
                                return result;
                            }
                            if (n % 2 != 0)
                            {
                                result.Success = false;
                                result.Message = "Para el método de Simpson 1/3 Múltiple, la cantidad de subintervalos (n) debe ser un número par.";
                                return result;
                            }
                            double h = (xd - xi) / n;
                            double sumPares = 0;
                            double sumImpares = 0;
                            for (int i = 1; i < n; i++)
                            {
                                double val = calculo.EvaluaFx(xi + h * i);
                                if (i % 2 == 0)
                                {
                                    sumPares += val;
                                }
                                else
                                {
                                    sumImpares += val;
                                }
                            }
                            double fXi = calculo.EvaluaFx(xi);
                            double fXd = calculo.EvaluaFx(xd);
                            area = (h / 3.0) * (fXi + 4.0 * sumImpares + 2.0 * sumPares + fXd);
                            break;
                        }

                    case "Simpson38Simple":
                        {
                            double h = (xd - xi) / 3.0;
                            double fXi = calculo.EvaluaFx(xi);
                            double f1 = calculo.EvaluaFx(xi + h);
                            double f2 = calculo.EvaluaFx(xi + 2.0 * h);
                            double fXd = calculo.EvaluaFx(xd);
                            area = (3.0 * h / 8.0) * (fXi + 3.0 * f1 + 3.0 * f2 + fXd);
                            break;
                        }

                    case "SimpsonCombinado":
                        {
                            if (n < 1)
                            {
                                result.Success = false;
                                result.Message = "La cantidad de subintervalos debe ser de al menos 1.";
                                return result;
                            }

                            double h = (xd - xi) / n;
                            double area38 = 0;
                            double area13 = 0;
                            int n13 = n;
                            double xd13 = xd;

                            // Si n es impar, aplicamos Simpson 3/8 en los últimos 3 subintervalos
                            if (n % 2 != 0)
                            {
                                if (n < 3)
                                {
                                    result.Success = false;
                                    result.Message = "Para el método combinado, el número de subintervalos (n) debe ser al menos 3.";
                                    return result;
                                }

                                double nuevoXi = xi + h * (n - 3);
                                double fNuevoXi = calculo.EvaluaFx(nuevoXi);
                                double f1 = calculo.EvaluaFx(nuevoXi + h);
                                double f2 = calculo.EvaluaFx(nuevoXi + 2.0 * h);
                                double fXd = calculo.EvaluaFx(xd);

                                area38 = (3.0 * h / 8.0) * (fNuevoXi + 3.0 * f1 + 3.0 * f2 + fXd);

                                n13 = n - 3;
                                xd13 = nuevoXi;
                            }

                            // Aplicar Simpson 1/3 Múltiple en los subintervalos restantes (n13, que es par)
                            if (n13 > 0)
                            {
                                double sumPares = 0;
                                double sumImpares = 0;
                                for (int i = 1; i < n13; i++)
                                {
                                    double val = calculo.EvaluaFx(xi + h * i);
                                    if (i % 2 == 0)
                                    {
                                        sumPares += val;
                                    }
                                    else
                                    {
                                        sumImpares += val;
                                    }
                                }
                                double fXi = calculo.EvaluaFx(xi);
                                double fXd13 = calculo.EvaluaFx(xd13);
                                area13 = (h / 3.0) * (fXi + 4.0 * sumImpares + 2.0 * sumPares + fXd13);
                            }

                            area = area13 + area38;
                            break;
                        }

                    default:
                        {
                            result.Success = false;
                            result.Message = $"Método '{input.Method}' no soportado.";
                            return result;
                        }
                }

                if (double.IsNaN(area) || double.IsInfinity(area))
                {
                    result.Success = false;
                    result.Message = "El cálculo de la integral produjo un valor indeterminado o infinito.";
                }
                else
                {
                    result.Area = area;
                    result.Success = true;
                    result.Message = "Area Calculada Exitosamente";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ocurrió un error durante la evaluación: {ex.Message}";
            }

            return result;
        }

        private string ObtenerNombreMetodo(string method)
        {
            return method switch
            {
                "TrapeciosSimple" => "Trapecios Simple",
                "TrapeciosMultiple" => "Trapecios Múltiple",
                "Simpson13Simple" => "Simpson 1/3 Simple",
                "Simpson13Multiple" => "Simpson 1/3 Múltiple",
                "Simpson38Simple" => "Simpson 3/8 Simple",
                "SimpsonCombinado" => "Simpson 1/3 y 3/8 Combinado",
                _ => method
            };
        }
    }
}
