using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using WebApplicationAnalisisNumerico.Models;

namespace WebApplicationAnalisisNumerico.Services
{
    public class RootFinder
    {
        // Numerical derivative as fallback
        private static double NumericalDerivative(Func<double, double> f, double x)
        {
            double h = 1e-4;
            try
            {
                double f1 = f(x + h);
                double f0 = f(x);
                return (f1 - f0) / h;
            }
            catch
            {
                return double.NaN;
            }
        }

        public RootFindingResult FindRoot(RootFindingInput input)
        {
            var result = new RootFindingResult();

            // Basic validation
            if (string.IsNullOrWhiteSpace(input.Function))
            {
                result.Message = "Función vacía";
                return result;
            }

            // Use the raw function string for Calculus
            string func = input.Function;

            // Prepare evaluator and derivative using Calculus (required)
            Func<double, double> evaluator = null;
            Func<double, double> derivative = null;

            // Try to explicitly load Calculus/Calculous.dll from common locations, then find a type named 'Calculo' in loaded assemblies
            Type calcType = null;
            try
            {
                var candidateNames = new[] { "Calculus.dll", "calculs.dll", "Calculous.dll", "calculous.dll" };
                var probeDirs = new[]
                {
                    AppContext.BaseDirectory,
                    Directory.GetCurrentDirectory(),
                    Path.Combine(AppContext.BaseDirectory, "bin"),
                    Path.Combine(Directory.GetCurrentDirectory(), "bin")
                };

                foreach (var dir in probeDirs)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
                            continue;
                    }
                    catch
                    {
                        continue;
                    }

                    foreach (var name in candidateNames)
                    {
                        var p = Path.Combine(dir, name);
                        if (File.Exists(p))
                        {
                            try
                            {
                                // Load assembly if not already loaded
                                if (!AppDomain.CurrentDomain.GetAssemblies().Any(a =>
                                        {
                                            try { return string.Equals(a.Location, p, StringComparison.OrdinalIgnoreCase); } catch { return false; }
                                        }))
                                {
                                    Assembly.LoadFrom(p);
                                }
                                // If load succeeded, break probing for files in this dir
                                break;
                            }
                            catch
                            {
                                // ignore load failures, try next
                            }
                        }
                    }
                }

                calcType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a =>
                    {
                        try { return a.GetTypes(); } catch { return Array.Empty<Type>(); }
                    })
                    .FirstOrDefault(t => t.Name == "Calculo");
            }
            catch
            {
                calcType = null;
            }

            object calcInstance = null;
            MethodInfo sintaxisMethod = null;
            MethodInfo evaluaFxMethod = null;
            MethodInfo dxMethod = null;

            if (calcType != null)
            {
                try
                {
                    calcInstance = Activator.CreateInstance(calcType);
                    sintaxisMethod = calcType.GetMethod("Sintaxis", new Type[] { typeof(string), typeof(char) });
                    evaluaFxMethod = calcType.GetMethod("EvaluaFx", new Type[] { typeof(double) }) ?? calcType.GetMethod("EvaluaFx");
                    // Try common derivative method name(s)
                    dxMethod = calcType.GetMethod("Dx", new Type[] { typeof(double) }) ?? calcType.GetMethod("Derivada", new Type[] { typeof(double) });
                }
                catch
                {
                    calcInstance = null;
                    sintaxisMethod = null;
                    evaluaFxMethod = null;
                    dxMethod = null;
                }
            }

            // Enforce using Calculus library as requested
            if (calcInstance == null || sintaxisMethod == null || evaluaFxMethod == null)
            {
                result.Message = "Library Calculus (Calculus.dll) no encontrada o no compatible. Copiar Calculus.dll en 'libs' y volver a compilar.";
                return result;
            }

            // Validate syntax using Calculus
            try
            {
                var sint = sintaxisMethod.Invoke(calcInstance, new object[] { func, 'x' });
                if (!(sint is bool bs && bs))
                {
                    result.Message = "Error de sintaxis según Calculus.";
                    return result;
                }

                evaluator = (x) => Convert.ToDouble(evaluaFxMethod.Invoke(calcInstance, new object[] { x }));
                if (dxMethod != null)
                {
                    derivative = (x) => Convert.ToDouble(dxMethod.Invoke(calcInstance, new object[] { x }));
                }
                else
                {
                    derivative = (x) => NumericalDerivative(evaluator, x);
                }
            }
            catch (TargetInvocationException tie)
            {
                result.Message = "Error al invocar métodos de Calculus: " + tie.InnerException?.Message;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = "Error al inicializar Calculus: " + ex.Message;
                return result;
            }

            // Now proceed with closed and open methods
            // Initial evaluation at Xi
            double fxi;
            try
            {
                fxi = evaluator(input.Xi);
            }
            catch
            {
                result.Message = "Error al evaluar la función en xi";
                return result;
            }

            if (double.IsNaN(fxi))
            {
                result.Message = "La evaluación en xi dio NaN";
                return result;
            }

            if (Math.Abs(fxi) < input.Tolerance)
            {
                result.Converged = true;
                result.Root = input.Xi;
                result.Message = "xi es raíz";
                return result;
            }

            // If closed method, need xd
            bool isClosed = input.Method == "Biseccion" || input.Method == "ReglaFalsa";

            if (isClosed)
            {
                double fxd;
                try
                {
                    fxd = evaluator(input.Xd);
                }
                catch
                {
                    result.Message = "Error al evaluar la función en xd";
                    return result;
                }

                if (double.IsNaN(fxd))
                {
                    result.Message = "La evaluación en xd dio NaN";
                    return result;
                }

                if (fxi * fxd > 0)
                {
                    result.Message = "f(xi) * f(xd) > 0. Ingrese un intervalo donde haya cambio de signo.";
                    return result;
                }

                if (Math.Abs(fxi) < input.Tolerance)
                {
                    result.Converged = true;
                    result.Root = input.Xi;
                    result.Message = "xi es raíz";
                    return result;
                }

                if (Math.Abs(fxd) < input.Tolerance)
                {
                    result.Converged = true;
                    result.Root = input.Xd;
                    result.Message = "xd es raíz";
                    return result;
                }
            }

            // For open methods, handle secante/tangente as before
            double xr = 0;
            double xrPrev = double.NaN;
            double error = double.PositiveInfinity;

            for (int i = 1; i <= input.MaxIterations; i++)
            {
                if (input.Method == "Tangente")
                {
                    double deriv;
                    try
                    {
                        deriv = derivative(input.Xi);
                    }
                    catch
                    {
                        deriv = double.NaN;
                    }

                    if (double.IsNaN(deriv) || Math.Abs(deriv) < 1e-12)
                    {
                        result.Message = "La derivada es cero o NaN, el método diverge.";
                        result.Converged = false;
                        result.Root = double.NaN;
                        return result;
                    }

                    double fxi2;
                    try
                    {
                        fxi2 = evaluator(input.Xi);
                    }
                    catch
                    {
                        result.Message = "Error al evaluar la función en xi (Tangente)";
                        return result;
                    }

                    xr = input.Xi - (fxi2 / deriv);
                }
                else if (input.Method == "Secante")
                {
                    double fxi3, fxd3;
                    try
                    {
                        fxi3 = evaluator(input.Xi);
                        fxd3 = evaluator(input.Xd);
                    }
                    catch
                    {
                        result.Message = "Error al evaluar la función en xi o xd (Secante)";
                        return result;
                    }

                    double denom = (fxd3 - fxi3);
                    if (Math.Abs(denom) < 1e-15)
                    {
                        result.Message = "Denominador cercano a cero. El método diverge.";
                        result.Converged = false;
                        result.Root = double.NaN;
                        return result;
                    }

                    xr = (fxd3 * input.Xi - fxi3 * input.Xd) / denom;
                }
                else if (input.Method == "Biseccion")
                {
                    xr = (input.Xi + input.Xd) / 2.0;
                }
                else if (input.Method == "ReglaFalsa")
                {
                    double fxi4, fxd4;
                    try
                    {
                        fxi4 = evaluator(input.Xi);
                        fxd4 = evaluator(input.Xd);
                    }
                    catch
                    {
                        result.Message = "Error al evaluar la función en xi o xd (Regla Falsa)";
                        return result;
                    }

                    double denom = (fxd4 - fxi4);
                    if (Math.Abs(denom) < 1e-15)
                    {
                        result.Message = "Denominador cercano a cero. El método diverge.";
                        result.Converged = false;
                        result.Root = double.NaN;
                        return result;
                    }

                    xr = (fxd4 * input.Xi - fxi4 * input.Xd) / denom;
                }
                else
                {
                    result.Message = "Método no reconocido.";
                    return result;
                }

                if (double.IsNaN(xr) || double.IsInfinity(xr))
                {
                    result.Message = "El método diverge. No encuentra raíz (NaN/Inf).";
                    result.Converged = false;
                    result.Root = double.NaN;
                    return result;
                }

                double fxr;
                try
                {
                    fxr = evaluator(xr);
                }
                catch
                {
                    fxr = double.NaN;
                }

                // compute error using previous xrPrev; for first iteration keep error large
                if (double.IsNaN(xrPrev))
                {
                    error = double.PositiveInfinity;
                }
                else
                {
                    error = Math.Abs((xr - xrPrev) / (Math.Abs(xr) > double.Epsilon ? xr : 1.0));
                }

                var iter = new IterationResult
                {
                    Iteration = i,
                    Xi = input.Xi,
                    Xd = (input.Method == "Secante" || input.Method == "Biseccion" || input.Method == "ReglaFalsa") ? input.Xd : double.NaN,
                    Xr = xr,
                    Fx = fxr,
                    Error = error,
                    Note = string.Empty
                };

                result.Iterations.Add(iter);

                if (Math.Abs(fxr) < input.Tolerance || error < input.Tolerance)
                {
                    result.Converged = true;
                    result.Root = xr;
                    result.Message = $"Convergió en {i} iteraciones.";
                    return result;
                }

                // Update for next iteration (closed methods update interval endpoints)
                if (input.Method == "Tangente")
                {
                    input.Xi = xr;
                }
                else if (input.Method == "Secante")
                {
                    input.Xi = input.Xd;
                    input.Xd = xr;
                }
                else if (input.Method == "Biseccion" || input.Method == "ReglaFalsa")
                {
                    double fxi_curr = evaluator(input.Xi);
                    double fxr_curr = evaluator(xr);
                    if (fxi_curr * fxr_curr > 0)
                    {
                        input.Xi = xr;
                    }
                    else
                    {
                        input.Xd = xr;
                    }
                }

                xrPrev = xr;
            }

            // If reached max iter
            result.Converged = false;
            result.Root = xr;
            result.Message = "Se superaron las iteraciones sin converger.";
            return result;
        }
    }
}