using System;
using TypesDual;

namespace Program
{
    class RectanDual
    {
        public RectanDual() { }

        public void init(InitData iniData)
        {
            init(iniData.resDual, iniData.datDual, iniData.s_kDual, iniData.vulcanDual);
        }

        public void init(RESULT res, FluxData dat, S_KG s_k, VULCAN vul)
        {
            Res = res;
            DAT = dat;
            S_K = s_k;
            VUL = vul;

            RES_V.t_eq_int = new double[VUL.n_inter];
            RES_V.E_R_int = new double[VUL.n_inter];
            RES_V.n_inter = VUL.n_inter;
            for (i = 0; i < RES_V.n_inter; i++)
            {
                RES_V.t_eq_int[i] = VUL.t_eq_int[i];
                RES_V.E_R_int[i] = VUL.E_R_int[i];
            }

            k_Max = 0;
            // { Заголовок цикла по группам секций }
            k_Min = k_Max + 1;
            k_Max = k_Max + 7;
            if (k_Max >= S_K.Num_S)
            {
                k_Max = S_K.Num_S;
            }
            j = 1;
            k = j - 1 + k_Min;

            for (j = 0; j < S_K.Num_S; j++)
            {
                SEC[j] = S_K.S[j];
            }

            // { Перевод миллиметров в метры }
            for (i = 0; i <= S_K.Num_S; i++)
            {
                S_K.S[i].H_st = S_K.S[i].H_st / 1000.0;
                S_K.S[i].H_fin = S_K.S[i].H_fin / 1000.0;
                S_K.S[i].W_st = S_K.S[i].W_st / 1000.0;
                S_K.S[i].W_fin = S_K.S[i].W_fin / 1000.0;
                S_K.S[i].L_sect = S_K.S[i].L_sect / 1000.0;
            }
            XM[0] = 0;
            HM[0] = S_K.S[0].H_st;
            WM[0] = S_K.S[0].W_st;
            for (i = 1; i <= S_K.Num_S; i++)
            {
                HM[i] = S_K.S[i - 1].H_fin;
                WM[i] = S_K.S[i - 1].W_fin;
                XM[i] = XM[i - 1] + S_K.S[i - 1].L_sect;
            }
            T_eq = Res.T_eqv;
            t_ind_eq = Res.t_in_eq;
            E_R = RES_V.E_R_int[1];
            Console.Write("Данные о состоянии материала перед головкой переданные,");
            Console.WriteLine(" программой расчета червячной машины:");
            Console.WriteLine("       i       Q        p       T");
            Console.WriteLine("       -    см^3/c     МПа     гр.Ц");
            for (i = 0; i <= Res.n_Q; i++)
            {
                Console.WriteLine($"{i} {Res.MQ[i]} {Res.MP[i]} {Res.MT[i]}");
            }

            Step(-1, 1, ref DAT.iR, DAT.k_R, DAT.k_R, ref yR);
            Step(0, 1, ref DAT.N, DAT.k_Al, DAT.k_Al, ref Alfa);
            n_Omit = 1;
            jM = 0;
            for (i = 1; i <= S_K.Num_S; i++)
            {
                jM = jM + S_K.S[i - 1].n_cykle;
            }
            if (jM > 40) n_Omit = (int)(jM / 40) + 1;
            MZ[0] = 0;
            iX_M[0] = 0;
            i = 0;
            j = 1;
            jM = 0;
            k = 0;
            x_M[0] = 0;
            do
            {
                jM = jM + S_K.S[j - 1].n_cykle;
                MZ[j] = MZ[j - 1] + S_K.S[j - 1].L_sect;
                d_X = S_K.S[j - 1].L_sect / S_K.S[j - 1].n_cykle;
                do
                {
                    i_pred = i;
                    i = i + n_Omit;
                    if (i > jM) i = jM;
                    if (i <= jM)
                    {
                        k = k + 1;
                        iX_M[k] = i;
                        x_M[k] = x_M[k - 1] + d_X * (i - i_pred);
                    }
                }
                while (i != jM);
                j = j + 1;
            }
            while (j <= S_K.Num_S);

            // { Начало циклов по объемному расходу 
            for (i_Q = 0; i_Q <= Res.n_Q; i_Q++)
            {
                Q = Res.MQ[i_Q] * 1E-06;
                T_enter = Res.MT[i_Q];
                for (i = 0; i <= DAT.iR; i++)
                {
                    TR[i] = T_enter;
                }
                for (i = 0; i <= DAT.N; i++)
                {
                    T[i] = T_enter;
                }
                p = 0;
                for (i = 0; i <= DAT.N; i++)
                {
                    time[i] = 0;
                    time_eq[i] = 0;
                }
                k = 0;
                p_M[0] = 0;

                // { Начало циклоа по секциям головки }
                for (jZ = 1; jZ <= S_K.Num_S; jZ++)
                {
                    if (jZ == 1) T[0] = S_K.S[jZ - 1].T_up;
                    else T[0] = T_enter;
                    T[DAT.N] = S_K.S[jZ - 1].T_up;
                    TR[0] = T[0];
                    TR[DAT.iR] = T[DAT.N];
                    j = S_K.S[jZ - 1].n_cykle;
                    Z = MZ[jZ - 1];
                    DZ = (MZ[jZ] - Z) / j;
                    DH = (S_K.S[jZ - 1].H_st - S_K.S[jZ - 1].H_fin) / 2.0;
                    TETA = Math.Abs(Math.Atan(DH / (MZ[jZ] - Z)));
                    sn = Math.Sin(TETA);
                    cs = Math.Cos(TETA);

                    // { Начало циклов вдоль секции }
                    for (j = 0; j <= S_K.S[jZ - 1].n_cykle; j++)
                    {
                        hj = LineInterpol(Z, 0, S_K.Num_S, XM, HM, ref pr) / 2.0;
                        wj = LineInterpol(Z, 0, S_K.Num_S, XM, WM, ref pr);
                        Qt = Q / wj;
                        sum = 0.00001;
                        transit = false;
                        Chorda(0, 0.1, 0.001, ref y0, ref sum, Root1);
                        if (transit)
                        {
                            sum = 0.00001;
                            Chorda(-1, 1, 0.001, ref y0, ref sum, Root1);
                        }
                        Tau1 = -hj * dp * (1 + y0);
                        Tau2 = hj * dp * (1 - y0);
                        for (i = 0; i <= DAT.N; i++)
                        {
                            pr = FixPr;
                            y_Alpha[i] = LineInterpol(Alfa[i], 0, DAT.iR, I3, yR, ref pr);
                            pr = FixPr;
                            vX2[i] = LineInterpol(y_Alpha[i], 0, DAT.iR, yR, vX, ref pr);
                            pr = FixPr;
                            qv2[i] = LineInterpol(y_Alpha[i], 0, DAT.iR, yR, qv, ref pr);
                        }
                        for (i = 0; i <= DAT.N; i++)
                        {
                            s2[i] = y_Alpha[i] * hj;
                            if (Math.Abs(TETA) > 0.05)
                            {
                                rk2 = hj / Math.Sin(TETA);
                                s2[i] = rk2 * Math.Atan(s2[i] / rk2 / cs);
                            }
                        }
                        if (j == 0) goto m1;
                        if (Math.Abs(TETA) > 0.05)
                        {
                            for (i = 0; i <= DAT.N; i++)
                            {
                                dTau[i] = Math.Sqrt(Math.Pow(DZ - rk1 * (Math.Cos(s1[i] / rk1) - cs) +
                                                                rk2 * (Math.Cos(s2[i] / rk2) - cs), 2) +
                                                    Math.Pow(rk1 * Math.Sin(s1[i] / rk1) -
                                                                rk2 * Math.Sin(s2[i] / rk2), 2));
                                if (i > 0) dY[i] = (s2[i] - s2[i - 1] + s1[i] - s1[i - 1]) / 2;
                            }
                            for (i = DAT.N; i >= 1; i--)
                            {
                                dTau[i] = (dTau[i] + dTau[i - 1]) / 2 * dY[i] / Qt;
                                dTau[i] = dTau[i] / (Alfa[i] - Alfa[i - 1]);
                            }
                        }
                        else
                        {
                            PRdTau();
                        }

                        p = p + (dp1 + dp) / 2 * DZ / Fp(2 * hj / wj);
                        Vulcan[i_Q] = 0;
                        for (i = 1; i <= DAT.N; i++)
                        {
                            qvT[i] = (qv1[i] + qv1[i - 1] + qv2[i] + qv2[i - 1]) / 4.0;
                            time[i] = time[i] + dTau[i];
                            time_eq[i] = time_eq[i] +
                                            Math.Exp(E_R / (T_eq + 273) / ((T[i] + T[i - 1]) / 2 + 273) *
                                                        ((T[i] + T[i - 1]) / 2 - T_eq)) *
                                            dTau[i];
                            Vulcan[i_Q] = Vulcan[i_Q] + time_eq[i] / t_ind_eq * 100 * (Alfa[i] - Alfa[i - 1]);
                        }
                        Test_Fo(DAT.N, 1, ref dY, ref T, ref dTau, ref qvT);
                        pr = FixPr;
                        for (i = 0; i <= DAT.iR; i++)
                        {
                            TR[i] = Spline(yR[i], 0, DAT.N, y_Alpha, T, ref pr);
                        }
                    m1:
                        dp1 = dp;
                        Tau1f = Tau1;
                        rk1 = rk2;
                        pf = p;
                        Z = Z + DZ;
                        for (i = 0; i <= DAT.N; i++)
                        {
                            s1[i] = s2[i];
                            qv1[i] = qv2[i];
                            vX1[i] = vX2[i];
                        }
                    } // {cycles j};
                } // { cycles by jZ };

                T_out[i_Q] = 0;
                for (i = 1; i <= DAT.N; i++)
                {
                    T_out[i_Q] = T_out[i_Q] + (T[i - 1] + T[i]) / 2 * (Alfa[i] - Alfa[i - 1]);
                }
                M_Q[i_Q] = Q * 1E+6;
                M_P[i_Q] = -p * 1E-6;
                M_T[i_Q] = Res.MT[i_Q];
                if (i_Q == 0)
                {
                    Console.WriteLine("       i       Q        p       T      T_out  t_eq/t_eq_ind");
                    Console.WriteLine("       -    см^3/с     МПа     гр.Ц     гр.Ц        % ");
                }
                Console.WriteLine($"{i_Q} {M_Q[i_Q]} {M_P[i_Q]} {M_T[i_Q]} {T_out[i_Q]} {Vulcan[i_Q]}");
            } // {by i_Q};

            Console.WriteLine("Показатели состояние материала в головке:");
            Console.WriteLine("Press <ENTER> to go on");

            MinMax(0, Res.n_Q, M_Q, ref fMin, ref fMax);
            MinMax(0, Res.n_Q, M_P, ref sMin, ref sMax);
            MinMax(0, Res.n_Q, Res.MP, ref dMin, ref dMax);
            if (dMin < sMin) sMin = dMin;
            if (dMax > sMax) sMax = dMax;

            MinMax(0, Res.n_Q, Res.MT, ref dMin, ref dMax);
            MinMax(0, Res.n_Q, T_out, ref sMin, ref sMax);
            if (sMax > dMax) dMax = sMax;
            if (sMin < dMin) dMin = sMin;

            transit = true;
            d_f = 0.0001;
            Chorda(M_Q[0], (M_Q[Res.n_Q] - M_Q[0]), 0.001, ref Q_f, ref d_f, Dif);
            i = S_K.Num_S;
            s_fin = SEC[i - 1].W_fin * SEC[i - 1].H_fin / 100;
            v_fin = Q_f / s_fin * 0.6;
            pr = FixPr;
            T_f = Spline(Q_f, 0, Res.n_Q, M_Q, T_out, ref pr);
            pr = FixPr;
            Vul_f = Spline(Q_f, 0, Res.n_Q, M_Q, Vulcan, ref pr);
            pr = FixPr;
            Vul_f_S = Spline(Q_f, 0, Res.n_Q, M_Q, Res.MVUL, ref pr);
            pr = FixPr;
            p_f = Spline(Q_f, 0, Res.n_Q, M_Q, Res.MP, ref pr);
            pr = FixPr;
            T_f_in = Spline(Q_f, 0, Res.n_Q, M_Q, Res.MT, ref pr);

            Console.WriteLine("РАССЧИТАННЫЕ ПАРАМЕТРЫ ЭКСТРУЗИИ:");
            Console.WriteLine($"Объемный расход: {Q_f} см^3/с");
            Console.WriteLine($"Линейная скорость экструдирования профиля: v = {v_fin} м/мин");
            Console.WriteLine($"Площадь сечения канала головки перед выходом профиля: S = {s_fin} см^2");
            Console.WriteLine($"Температура смеси при выходе из головки: {T_f} град.Ц");
            Console.WriteLine($"Температура смеси перед головкой: {T_f_in} град.Ц");
            Console.WriteLine($"Расходование индукционного периода в машине: {Vul_f_S} %");
            Console.WriteLine($"Расходование индукционного периода в головке: {Vul_f} %");
            Console.WriteLine($"Удельное давление перед головкой: p = {p_f} МПа");
        }

        void Chorda(double x0, double dx, double tx, ref double x, ref double y, Func<double> F)
        {
            double x1, x2, y1, y2, ty;
            byte k;
            bool v;
            ty = y;
            x = x0;
            y = F();
            if (Math.Abs(y) < ty)
                goto met;
            x1 = x;
            y1 = y;
            x = x0 + dx;
            y = F();
            if (Math.Abs(y) < ty)
                goto met;
            x2 = x;
            y2 = y;
            k = 2;
            do
            {
                if (transit)
                    x = (x1 + x2) / 2;      // { DIV 2 method }
                else
                    x = x1 - (x2 - x1) / (y2 - y1) * y1;    // { CHORDA method }

                y = F();
                k++;
                if (Math.Abs(y) < ty)
                    goto met;
                if (transit || (y1 / Math.Abs(y1) * y2 / Math.Abs(y2) < 0))
                {
                    if (y / Math.Abs(y) * y1 / Math.Abs(y1) < 0)
                    {
                        x2 = x;
                        y2 = y;
                    }
                    else
                    {
                        x1 = x;
                        y1 = y;
                    }
                }
                else if (Math.Abs(y1) < Math.Abs(y2))
                {
                    x2 = x;
                    y2 = y;
                }
                else
                {
                    x1 = x;
                    y1 = y;
                }
                v = (k > 30) && !transit;
            }
            while (!(((Math.Abs(x2 - x1) < tx) && transit) || (Math.Abs(y) <= ty) || v));

            if (v)
                transit = true;

            met:
            return;
        }

        void MinMax(int iMin, int iMax, double[] X, ref double xMin, ref double xMax)
        {
            int i;

            for (i = iMin; i <= iMax; i++)
            {
                if (i == iMin || X[i] < xMin)
                {
                    xMin = X[i];
                }
                if (i == iMin || X[i] > xMax)
                {
                    xMax = X[i];
                }
            }
        }

        public static double th(double x)
        {
            double a;
            if (x > 20)
            {
                a = 0;
            }
            else
            {
                a = Math.Exp(-2 * x);
            }
            return (1 - a) / (1 + a);
        }

        public static double Fp(double ff)
        {
            double f, nj, jk;
            int j;
            f = 0;
            for (j = 1; j <= 20; j++)
            {
                nj = Math.PI * (2 * j - 1);
                jk = nj * nj;
                f = f + th(nj / 2.0 / ff) / (jk * jk * nj);
            }
            return 1 - 192 * ff * f;
        }

        public double LineInterpol(double x, int iH, int iK, double[] mx, double[] my, ref double Pr)
        {
            int i = 0, j;
            double a = 0, b;

            for (j = iH; j <= iK - 1; j++)
            {
                b = x - mx[j];
                if (j > iH && Math.Abs(b) > Math.Abs(a))
                {
                    j = iK - 1;
                }
                else
                {
                    i = j;
                    a = b;
                }
            }

            b = x - mx[i];
            if (b < 0 && i > iH)
            {
                i--;
                b = x - mx[i];
            }

            Pr = (my[i + 1] - my[i]) / (mx[i + 1] - mx[i]);
            return my[i] + Pr * b;
        }

        public double Ht(double x)
        {
            double pr = FixPr;
            return LineInterpol(x, 0, S_K.Num_S, XM, HM, ref pr);
        }

        public double Wt(double x)
        {
            double pr = FixPr;
            return LineInterpol(x, 0, S_K.Num_S, XM, WM, ref pr);
        }

        public void Step(double rB, double rH, ref int N, double kB, double kH, ref double[] R)
        {
            int i, c;
            double d;

            d = N;
            d = d / 2;
            if (d - N / 2 > 0.1)
                N = N - 1;
            c = N / 2;
            d = 1;
            R[0] = 0;
            for (i = 1; i <= c; i++)
            {
                R[i] = R[i - 1] + d;
                d = d * kB;
            }
            for (i = 0; i <= c; i++)
                R[i] = rB + R[i] / R[c] * (rH - rB) / 2;
            R[N] = 0;
            d = 1;
            for (i = N - 1; i >= c; i--)
            {
                R[i] = R[i + 1] + d;
                d = d * kH;
            }
            for (i = N; i >= c; i--)
                R[i] = rH - R[i] / R[c] * (rH - rB) / 2;
        }

        public double a_T(double T, int i)
        {
            return Res.a;
        }

        public double Lambda_T(double T, int i)
        {
            return Res.Lam;
        }

        public double Sigma(double Fo)
        {
            return 0.5;
        }

        void PRdTau()
        {
            for (int i = 0; i <= DAT.N; i++)
            {
                dTau[i] = Math.Sqrt(DZ * DZ + Math.Pow(s1[i] - s2[i], 2));
                if (i > 0)
                    dY[i] = (s2[i] - s2[i - 1] + s1[i] - s1[i - 1]) / 2.0;
            }
            for (int i = DAT.N; i >= 1; i--)
            {
                dTau[i] = (dTau[i] + dTau[i - 1]) / 2 * dY[i] / Qt;
                dTau[i] = dTau[i] / (Alfa[i] - Alfa[i - 1]);
            }
        }

        double Dif()
        {
            double pr, a, b;
            pr = FixPr;
            a = Spline(Q_f, 0, Res.n_Q, M_Q, Res.MP, ref pr);
            pr = FixPr;
            b = Spline(Q_f, 0, Res.n_Q, M_Q, M_P, ref pr);
            return a - b;
        }

        public double Spline(double x, int iH, int iK, double[] xM, double[] yM, ref double Pr)
        {
            bool a, b, z;
            int i = 0, j;
            double r = 0, d;


            a = x > xM[iH];
            b = x < xM[iK];
            z = xM[iH] < xM[iK];
            if (Pr == FixPr)
                kSpline(iH, iK, xM, yM, ref aSpline, ref bSpline, ref cSpline, ref dSpline);
            if ((a && b) || (!a && !b))
            {
                for (j = iH; j <= iK; j++)
                {
                    d = x - xM[j];
                    if (j > iH && Math.Abs(d) > Math.Abs(r))
                    {
                        j = iK;
                    }
                    else
                    {
                        i = j;
                        r = d;
                    }
                }
                if ((z && r < 0) || (!z && r >= 0 && i > iH))
                {
                    i = i - 1;
                }
                d = x - xM[i];
                Pr = bSpline[i] + d * (2 * cSpline[i] + d * 3 * dSpline[i]);
                return aSpline[i] + d * (bSpline[i] + d * (cSpline[i] + d * dSpline[i]));
            }
            else if ((z && !a) || (!z && a))
            {
                Pr = bSpline[iH];
                return aSpline[iH] + Pr * (x - xM[iH]);
            }
            else
            {
                d = xM[iK] - xM[iK - 1];
                Pr = bSpline[iK - 1] + d * (2 * cSpline[iK - 1] + d * 3 * dSpline[iK - 1]);
                return yM[iK] + Pr * (x - xM[iK]);
            }
        }

        public void kSpline(int iH, int iK, double[] xM, double[] yM, ref double[] a, ref double[] b, ref double[] c, ref double[] d)
        {
            int i;
            double dx1 = 0, dx2, dy1 = 0, dy2;
            double[] MD = new double[60];
            double[] BD = new double[60];

            for (i = iH; i < iK; i++)
            {
                a[i] = yM[i];
                dx2 = xM[i + 1] - xM[i];
                dy2 = yM[i + 1] - yM[i];
                if (i > iH)
                {
                    MD[i] = dx2;
                    BD[i] = 2 * (dx1 + dx2);
                    c[i] = 3 * (dy2 / dx2 - dy1 / dx1);
                }
                dx1 = dx2;
                dy1 = dy2;
            }

            TriDag(iH + 1, iK - 1, BD, MD, ref c);

            c[iH] = 0;
            c[iK] = 0;

            for (i = iH; i < iK; i++)
            {
                dx2 = xM[i + 1] - xM[i];
                b[i] = (yM[i + 1] - yM[i]) / dx2 - dx2 * (c[i + 1] + 2 * c[i]) / 3.0;
                d[i] = (c[i + 1] - c[i]) / 3.0 / dx2;
            }
        }

        void Test_Fo(int n, int GN, ref double[] dY, ref double[] T, ref double[] dTau, ref double[] qvT)
        {
            int i, iq;
            double[] Tq = new double[61];
            double[] qvq = new double[61];

            for (i = 1; i <= DAT.N; i++)
            {
                dFo = dTau[i] * a_T((T[i - 1] + T[i]) / 2.0, i - 1) / dY[i] / dY[i];
                if (i == 1 || dFo > MaxFo)
                    MaxFo = dFo;
                if (i == 1 || qvT[i] > qvMax)
                    qvMax = qvT[i];
            }
            if (qvMax > 1E+08)
            {
                for (i = 1; i <= DAT.N; i++)
                {
                    if (i > 0)
                    {
                        dTau[i] = dTau[i] / 10.0;
                        Tq[i] = T[i];
                        qvq[i] = qvT[i];
                    }
                }
                MaxFo = MaxFo / 10.0;
                for (i = 1; i <= 10; i++)
                {
                    TRTV(1, 1, DAT.N, ref T, dY, qvT, dTau, 0, 0, 0, 0, a_T, Lambda_T, Sigma);
                    for (iq = 1; iq <= DAT.N; iq++)
                    {
                        time[i] = time[i] + dTau[i];
                        qvT[iq] = qvq[iq] * Math.Exp(-Res.b * (T[iq] + T[iq - 1] - Tq[iq] - Tq[iq - 1]) / 2.0);
                    }
                }
            }
            else
            {
                TRTV(1, 1, DAT.N, ref T, dY, qvT, dTau, 0, 0, 0, 0, a_T, Lambda_T, Sigma);
                for (i = 1; i <= DAT.N; i++)
                    time[i] = time[i] + dTau[i];
            }
        }

        public void TRTV(int G0, int GN, int N,
            ref double[] T, double[] DY, double[] Q, double[] DTAU,
            double T0, double TN, double AL0, double ALN,
            Func<double, int, double> A, Func<double, int, double> L, Func<double, double> SIGMA)
        {
            int i, j, c;
            double r1, r2 = 0, r3, r4, r5, r6, r7, r8, d;
            double[] B = new double[60];
            double[] M = new double[60];
            double[] DT = new double[60];

            if (G0 == 3)
            {
                r1 = -AL0;
                r2 = AL0 * (T[0] - T0);
            }
            else
            {
                r1 = 0;
            }

            if (G0 == 2)
            {
                r2 = -AL0;
            }

            r7 = 0;

            for (i = 0; i <= N - 1; i++)
            {
                r3 = (T[i] + T[i + 1]) / 2.0;
                r4 = A(r3, i) / (DY[i + 1] * DY[i + 1]) * DTAU[i + 1];
                r5 = L(r3, i) / DY[i + 1];
                r8 = Q[i + 1] / 2.0 * DY[i + 1];
                d = SIGMA(r4);
                r6 = -r5 * (0.5 / r4 + d);
                r4 = r5 * (T[i + 1] - T[i]);
                M[i] = r5 * d;

                if (i == 0 && G0 == 1)
                {
                    goto m1;
                }

                B[i] = r1 + r6;
                DT[i] = r2 - r4 - r7 - r8;

                if (i == 1 && G0 == 1)
                {
                    DT[1] = DT[1] - M[0] * T0;
                }

                if (i == N - 1 && GN == 1)
                {
                    DT[i] = DT[i] - M[i] * TN;
                }

            m1: r2 = r4;
                r1 = r6;
                r7 = r8;
            }

            if (GN > 1)
            {
                c = N;

                if (GN == 2)
                {
                    B[N] = r1;
                    DT[N] = r2 - ALN - r7;
                }
                else
                {
                    B[N] = r1 - ALN;
                    DT[N] = r2 + ALN * (T[N] - TN) - r7;
                }
            }
            else
            {
                c = N - 1;
            }

            if (G0 == 1)
            {
                j = 1;
            }
            else
            {
                j = 0;
            }

            TriDag(j, c, B, M, ref DT);

            for (i = j; i <= c; i++)
            {
                T[i] = T[i] + DT[i];
            }

            if (G0 == 1)
            {
                T[0] = T[0] + T0;
            }

            if (GN == 1)
            {
                T[N] = T[N] + TN;
            }
        }

        public void TriDag(int H, int K,
                             double[] B, double[] M, ref double[] C)
        {
            double r;
            int i;

            r = B[H];
            C[H] = C[H] / r;

            for (i = H; i <= K - 1; i++)
            {
                B[i] = M[i] / r;
                r = B[i + 1] - M[i] * B[i];
                C[i + 1] = (C[i + 1] - M[i] * C[i]) / r;
            }

            for (i = K - 1; i >= H; i--)
            {
                C[i] = C[i] - B[i] * C[i + 1];
            }
        }

        public void integral(ref double[] a, ref double[] b)
        {
            b[0] = 0;
            for (int k = 1; k <= DAT.iR; k++)
            {
                b[k] = b[k - 1] + (a[k] + a[k - 1]) / 2.0 * (yR[k] - yR[k - 1]);
            }
        }

        public double Root1()
        {
            int i;
            double c, d, v, z, c1, d1;
            double[] F3 = new double[61];
            double[] F4 = new double[61];
            double[] I4 = new double[61];

            for (i = 0; i <= DAT.iR; i++)
            {
                c = yR[i] - y0;
                d = Math.Abs(c);
                if (d < eps)
                {
                    F3[i] = 0;
                }
                else
                {
                    v = (Math.Log(d) + Res.b * (TR[i] - Res.T0)) / Res.n;
                    if (v < -20)
                    {
                        F3[i] = 0;
                    }
                    else
                    {
                        F3[i] = Math.Exp(v) * c / d;
                    }
                }
                F4[i] = yR[i] * F3[i];
            }

            integral(ref F4, ref I4);
            if (Math.Abs(I4[DAT.iR]) < eps)
            {
                I4[DAT.iR] = eps;
            }

            integral(ref F3, ref I3);
            z = I3[DAT.iR];

            for (i = 0; i <= DAT.iR; i++)
            {
                I4[i] = I3[i] - I4[i];
            }

            c1 = Qt / hj;
            v = Qt / hj / I4[DAT.iR];
            d1 = Res.n * Math.Log(Math.Abs(v) / hj);

            if (d1 < -30)
            {
                dp = 0;
            }
            else
            {
                dp = Res.Mu0 / hj * Math.Exp(d1) * v / Math.Abs(v);
            }

            for (i = 0; i <= DAT.iR; i++)
            {
                vX[i] = v * I3[i];
            }

            for (i = 0; i <= DAT.iR; i++)
            {
                I3[i] = 1 / I4[DAT.iR] * ((yR[i] - 1) * I3[i] + I4[i]);
                Mu = Res.Mu0 * Math.Exp(-Res.b * (TR[i] - Res.T0));
                d = Math.Abs(dp * (yR[i] - y0) * hj / Mu);
                if (d < eps || (1.0 / DAT.N + 1) * Math.Log(d) < -30)
                {
                    qv[i] = 0;
                }
                else
                {
                    qv[i] = Mu * Math.Exp((1.0 / DAT.N + 1) * Math.Log(d));
                }
            }

            c = c1 * z;
            return c;
        }

        public int
            i, j, k, k_Min, k_Max;
        public bool
            transit;
        public int
            jM, i_Q, n_Omit;

        public double
            MaxFo, y0, Q, Qt, hj, dp, dp1, Mu, pr,
            Tau1, Tau2, wj, rk1, rk2, p, pf, Tau1f,
            dFo, qvMax, T_enter, fMin, fMax,
            sMin, sMax, dMin, dMax, E_R, T_eq, t_ind_eq;

        public double[]
            yR = new double[61], Alfa = new double[61], TR = new double[61], TimeFin = new double[61],
            vX = new double[61], qv = new double[61], XM = new double[61], HM = new double[61],
            WM = new double[61], y_Alpha = new double[61], s1 = new double[61],
            s2 = new double[61], dTau = new double[61], dY = new double[61],
            qvT = new double[61], qv1 = new double[61], qv2 = new double[61],
            vX1 = new double[61], vX2 = new double[61], T = new double[61],
            M2X = new double[61], M2Y = new double[61], time = new double[61],
            time_eq = new double[61], M_Q = new double[61], M_P = new double[61],
            M_T = new double[61], T_out = new double[61], x_M = new double[61],
            H_M = new double[61], W_M = new double[61], p_M = new double[61],
            MZ = new double[61], Vulcan = new double[61], I3 = new double[61];

        public double[]
            aSpline = new double[61], bSpline = new double[61],
            cSpline = new double[61], dSpline = new double[61];

        public int[] iX_M = new int[61];

        public int i_pred, jZ;
        public double
            d_X, Z, DZ, DH, TETA, sn, cs, sum, s_fin, v_fin,
            d_f, Q_f, T_f, Vul_f, Vul_f_S, p_f, T_f_in;

        public SECTIONS[] SEC = new SECTIONS[30];
        public RES_VUL RES_V;

        public RESULT Res;
        public FluxData DAT;
        public S_KG S_K;
        public VULCAN VUL;

        const double eps = 1E-08;
        const int FixPr = 4738;

        // RESULT Res_p_q;
    }
}