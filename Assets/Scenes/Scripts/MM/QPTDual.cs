using System;
using TypesDual;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;

namespace Program
{
    class QPTDual
    {
        public QPTDual(){}

        public void init(InitData iniData, bool isAloneQ = false)
        {
            init(iniData.dataDual, iniData.sectDual, iniData.sect_aDual,
                          iniData.cylinderDual, iniData.vulcanDual, iniData.dop_dataDual);
        }

        public void init(DATA_ data_, SECT[] sect, SECT_a[] sect_a, CYLINDER[] cylinder, VULCAN vulcan, DOP_DATA dop_data, bool isAloneQ = false)
        {
            DataRec = data_;
            sect.CopyTo(SCT, 0);
            sect_a.CopyTo(SCT_a, 0);
            cylinder.CopyTo(CYL, 0);
            VUL = vulcan;
            DOP = dop_data;

            // READdata
            // { Назначенте переменной для числа секций первого червяка }
            nSect = DataRec.nS_1+DataRec.nS_2;

            // { Назначенте переменной для числа секций второго червяка }
            nSect_a = DataRec.nS_1a+DataRec.nS_2a;

            // { Назначенте отдельной переменной для числа секций корпуса }
            nCYL = DataRec.nS_korp;

            // AddedData
            DOP.k_V_air = DOP.k_V_air/100.0; // { Перевод % в относит.единицы }
            f = 1-DOP.k_V_air;

            OrderSCT();
            Initial();
            Put_QM(isAloneQ);
            for (i_Alfa = 0; i_Alfa <= DOP.n_Alfa; i_Alfa++)
            {
                TETA = 1 - DOP.Alfa_min - (DOP.Alfa_max - DOP.Alfa_min) * i_Alfa / DOP.n_Alfa;
                Q_M = Res.MQ[i_Alfa] * 1E-6;
                Q_total = 2 * Q_M;
                Q1 = Q_M;
                Q2 = Q_M;
                Table_Titul();
                SCR_Num = 1;
                k = -1;
                T = T_Start;
                iSect = 1;
                Lm = 0;
                p = 0;
                q_int_s = 0;
                for (i = 1; i <= nCYL; i++)
                {
                    CYL[i-1].q_int_k = 0;
                }
                nPrint = (int)Math.Round(nLm_a / 15.0);
                ic = nPrint;
                Capacity = 0;
                Energy = 0;
                Energy_C = 0;
                Enrg_fix = 1000;
                time_eqv = 0;
                time = 0;
                i_print = 0;
                Volume = 0;
                for (iSect = 1; iSect <= nSect; iSect++)
                {
                    if (SCT[iSect-1].S_Type == 1)
                    {
                        //Console.Write("Задайте соотношение объемных расходов: Q1/Q2 = ?");
                        //Q1Q2 = double.Parse(Console.ReadLine());
                        Q1Q2 = 0.85;
                        Q1 = 2 * Q_M / (1 + 1 / Q1Q2);
                        Q2 = 2 * Q_M / (1 + Q1Q2);
                        Mix.Q1Q2[iSect-1, i_Alfa] = Q1Q2;
                        Mix.Lambda[iSect] = Lambda[iSect];
                        Start_SCT(iSect);
                        for (i = 0; i <= SCT[iSect-1].n_cykle; i++)
                        {
                            i_0(iSect);
                            if (i > 0)
                            {
                                i_Var(iSect);
                                if (Math.Abs(1 - Q / Q_B) < 0.2)
                                {
                                    func = 0.001;
                                    Chorda(Q_delta0, dQ_delta, tQ_delta, ref Q_delta, ref func, find_Q_delta);
                                }
                                else
                                {
                                    test = 0;
                                    do
                                    {
                                        i_Var_2(iSect);
                                    }
                                    while (!(Math.Abs((Q_delta - Qj_delta) / Q_B) < 0.001 || test < 15));
                                }
                                i_Var_3(iSect);
                            }
                            if (ic == nPrint || i == SCT[iSect-1].n_cykle)
                            {
                                ic = 0;
                                text += $"{Lm * 1000} {p * 1E-6} {T} {H * 1000} {W * 1000} {Q1Q2}\n";
                                k++;
                                ZM[k] = Lm;
                                TM[k] = T;
                                PM[k] = p * 1E-6;
                                ++i_print;
                            }
                            ic++;
                        }
                    }
                    else
                    {
                        //Console.Write("Задайте соотношение объемных расходов: Q1/Q2 = ?");
                        //Q1Q2 = double.Parse(Console.ReadLine());
                        Q1Q2 = 0.85;
                        Q1 = 2 * Q_M / (1 + 1 / Q1Q2);
                        Q2 = 2 * Q_M / (1 + Q1Q2);
                        Mix.Q1Q2[iSect-1, i_Alfa] = Q1Q2;
                        Mix.Lambda[iSect] = Lambda[iSect];
                        Ro = Ro_M;
                        Lam_T = Lam_T_M;
                        dLm = SCT[iSect-1].L_sect / SCT[iSect-1].n_cykle;
                        for (i = 0; i <= SCT[iSect-1].n_cykle; i++)
                        {
                            i_0(iSect);
                            if (i > 0)
                            {
                                i_Var_Smooth(iSect, i);
                            }
                            if (ic == nPrint || i == SCT[iSect-1].n_cykle)
                            {
                                ic = 0;
                                text += $"{Lm * 1000} {p * 1E-6} {T} {(rH - rB) * 1000}    - {Q1Q2}\n";
                                k++;
                                ZM[k] = Lm;
                                TM[k] = T;
                                PM[k] = p * 1E-6;
                                ++i_print;
                            }
                            ic++;
                        }
                        Res.MT[i_Alfa] = T;
                        Res.MP[i_Alfa] = p * 1E-6;
                        q_int_s1 = q_int_s;
                        q_int_s = 0;
                        Volume_1 = Volume;
                        time_1 = time;
                        Energy_1 = Energy;
                        Ener_C_1 = Energy_C;
                    }
                }
                SCR_Num = 2;
                // if (i_print == 19)
                // {
                //     Console.SetWindowSize(80, 25);
                //     Console.SetBufferSize(80, 25);
                //     Console.SetWindowPosition(39, 4);
                // }
                // else
                // {
                //     Console.SetWindowSize(80, 25);
                //     Console.SetBufferSize(80, 25);
                //     Console.SetWindowPosition(39, 7);
                // }
                k = -1;
                T = T_Start;
                iSect = 1;
                Lm = 0;
                p = 0;
                q_int_s = 0;
                for (i = 1; i <= nCYL; i++)
                {
                    CYL[i-1].q_int_k = 0;
                }
                nPrint = (int)Math.Round(nLm / 15.0);
                ic = nPrint;
                Capacity = 0;
                Energy = 0;
                Energy_C = 0;
                Enrg_fix = 1000;
                time_eqv = 0;
                time = 0;
                i_print = 0;
                Volume = 0;
                Mix.i_Sect = 1;
                Q1Q2 = Mix.Q1Q2[1-1, i_Alfa];
                Q1 = 2 * Q_M / (1 + 1 / Q1Q2);
                Q2 = 2 * Q_M / (1 + Q1Q2);
                for (iSect = 1; iSect <= nSect_a; iSect++)
                {
                    if (SCT_a[iSect-1].S_Type == 1)
                    {
                        Start_SCT(iSect);
                        for (i = 0; i <= SCT_a[iSect-1].n_cykle; i++)
                        {
                            i_0(iSect);
                            if (i > 0)
                            {
                                i_Var(iSect);
                                if (Math.Abs(1 - Q / Q_B) < 0.2)
                                {
                                    func = 0.001;
                                    Chorda(Q_delta0, dQ_delta, tQ_delta, ref Q_delta, ref func, find_Q_delta);
                                }
                                else
                                {
                                    test = 0;
                                    do
                                    {
                                        i_Var_2(iSect);
                                    } while (!(Math.Abs((Q_delta - Qj_delta) / Q_B) < 0.001 || test < 15));
                                }
                                i_Var_3(iSect);
                            }
                            if (ic == nPrint || i == SCT_a[iSect-1].n_cykle)
                            {
                                ic = 0;
                                text += $"{Lm * 1000} {p * 1E-6} {T} {H * 1000} {W * 1000} {Q1Q2}\n";
                                k++;
                                ZM_a[k] = Lm;
                                TM_a[k] = T;
                                PM_a[k] = p * 1E-6;
                                ++i_print;
                            }
                            ic++;
                            if (Math.Abs(Lm - Mix.Lambda[Mix.i_Sect]) < dLm)
                            {
                                Mix.i_Sect++;
                                Q1Q2 = Mix.Q1Q2[Mix.i_Sect-1, i_Alfa];
                                Q1 = 2 * Q_M / (Q1Q2 + 1) * Q1Q2;
                                Q2 = 2 * Q_M / (1 + Q1Q2);
                            }
                        }
                    }
                    else
                    {
                        Ro = Ro_M;
                        Lam_T = Lam_T_M;
                        dLm = SCT_a[iSect-1].L_sect / SCT_a[iSect-1].n_cykle;
                        for (int i = 0; i <= SCT_a[iSect-1].n_cykle; i++)
                        {
                            i_0(iSect);
                            if (i > 0)
                            {
                                i_Var_Smooth(iSect, i);
                            }
                            if (ic == nPrint || i == SCT_a[iSect-1].n_cykle)
                            {
                                ic = 0;
                                text += $"{Lm * 1000} {p * 1E-6} {T} {(rH - rB) * 1000}    - {Q1Q2}\n";
                                k++;
                                ZM_a[k] = Lm;
                                TM_a[k] = T;
                                PM_a[k] = p * 1E-6;
                                ++i_print;
                            }
                            ic++;
                            if (Math.Abs(Lm - Mix.Lambda[Mix.i_Sect]) < dLm)
                            {
                                Mix.i_Sect++;
                                if (Mix.i_Sect < nSect)
                                {
                                    Q1Q2 = Mix.Q1Q2[Mix.i_Sect-1, i_Alfa];
                                    Q1 = 2 * Q_M / (1 + 1 / Q1Q2);
                                    Q2 = 2 * Q_M / (1 + Q1Q2);
                                }
                            }
                        }
                    }
                }
                Res.MT[i_Alfa] = (Res.MT[i_Alfa] + T) / 2;
                Res.MP[i_Alfa] = (Res.MP[i_Alfa] + p * 1E-6) / 2;
                Volume += Volume_1;
                Energy = (Energy_1 * Q1 + Energy * Q2) / 2 / Q_M;
                Energy_C = (Energy_C * Q1 + Ener_C_1 * Q2) / 2 / Q_M;
                Fin_Q();

                //Данные для графика
                PM_LIST.Add(PM);
                ZM_LIST.Add(ZM);
                TM_LIST.Add(TM);
                //X_OTN_LIST.Add(X_OTN);

                PZ.Add(new List<Vector2>());
                TZ.Add(new List<Vector2>());
                //XZ.Add(new List<Vector2>());
                ZXPT.Add(new List<List<double>>());
                for (int i = 0; i < k + 1; ++i)
                {
                    PZ.Last().Add(new Vector2((float)(ZM[i] * 1e3), (float)PM[i]));
                    TZ.Last().Add(new Vector2((float)(ZM[i] * 1e3), (float)TM[i]));
                    //XZ.Last().Add(new Vector2((float)(ZM[i] * 1e3), (float)X_PL_LIST.Last()[i] * 1000));
                    ZXPT.Last().Add(new List<double>
                    {
                        ZM[i] * 1e3,
                        //XZ.Last().Last().Y,
                        PZ.Last().Last().Y,
                        TZ.Last().Last().Y,
                    });
                }

                PM_a_LIST.Add(PM_a);
                ZM_a_LIST.Add(ZM_a);
                TM_a_LIST.Add(TM_a);
                //X_OTN_a_LIST.Add(X_OTN_a);

                PZ_a.Add(new List<Vector2>());
                TZ_a.Add(new List<Vector2>());
                //XZ_a.Add(new List<Vector2>());
                ZXPT_a.Add(new List<List<double>>());
                for (int i = 0; i < k + 1; ++i)
                {
                    PZ_a.Last().Add(new Vector2((float)(ZM_a[i] * 1e3), (float)PM_a[i]));
                    TZ_a.Last().Add(new Vector2((float)(ZM_a[i] * 1e3), (float)TM_a[i]));
                    //XZ_a.Last().Add(new Vector2((float)(ZM_a[i] * 1e3), (float)X_PL_a_LIST.Last()[i] * 1000));
                    ZXPT_a.Last().Add(new List<double>
                    {
                        ZM_a[i] * 1e3,
                        //XZ_a.Last().Last().Y,
                        PZ_a.Last().Last().Y,
                        TZ_a.Last().Last().Y,
                    });
                }
            }
            Remain();
        }

        void OrderSCT() //{ Расстановка секций двух червяков }
        {
            int i=0,j=0;
            for(j=1; j <= nSect_a; ++j)
            {
                i = 0;
                do
                {
                    i = i+1;
                    if(SCT_a[i-1].Order==j)
                    {
                        SCT_a[25-1] = SCT_a[j-1];
                        SCT_a[j-1] = SCT_a[i-1];
                        SCT_a[i-1] = SCT_a[25-1];
                    }
                }
                while (!((SCT_a[i-1].Order==j) || (i==nSect_a)));
            }
            for (j=1; j <= nSect; ++j)
            {
                i = 0;
                do
                {
                    i = i+1;
                    if (SCT[i-1].Order==j)
                    {
                        SCT[25-1] = SCT[j-1];
                        SCT[j-1] = SCT[i-1];
                        SCT[i-1] = SCT[25-1];
                    }
                }
                while (!((SCT[i-1].Order==j) || (i==nSect)));
            }
        }

        void Initial() //{ Начальные присвоения }
        {
            int i, j;
            L = 0;
            Lambda[0] = 0;
            //{ Для первого червяка }
            for (i = 1; i <= nSect; i++)
            {
                SCT[i-1].D_st = SCT[i-1].D_st / 1000.0;
                SCT[i-1].D_fin = SCT[i-1].D_fin / 1000.0;
                SCT[i-1].L_sect = SCT[i-1].L_sect / 1000.0;
                SCT[i-1].H_st = SCT[i-1].H_st / 1000.0;
                SCT[i-1].H_fin = SCT[i-1].H_fin / 1000.0; //{ Перевод из м в мм }
                if (SCT[i-1].S_Type == 1)
                {
                    SCT[i-1].step_ = SCT[i-1].step_ / 1000.0;
                    SCT[i-1].e_st = SCT[i-1].e_st / 1000.0;
                    SCT[i-1].e_fin = SCT[i-1].e_fin / 1000.0;
                    SCT[i-1].delta = SCT[i-1].delta / 1000.0;
                    SCT[i-1].W_a = SCT[i-1].W_a / 1000.0; //{ Перевод из м в мм }
                    SCT[i-1].d_Fi = SCT[i-1].d_Fi / 180.0 * Math.PI; //{ Перевод из градусов в радианы }
                }
                L = L + SCT[i-1].L_sect; //{ Расчет длины рабочей части первого червяка }
                Lambda[i] = L; //{ Разбивка длины первого червяка на секции }
            }
            L_a = 0;
            Lambda_a[0] = 0; //{ Для второго червяка }
            for (i = 1; i <= nSect_a; i++)
            {
                SCT_a[i-1].D_st = SCT_a[i-1].D_st / 1000.0;
                SCT_a[i-1].D_fin = SCT_a[i-1].D_fin / 1000.0;
                SCT_a[i-1].L_sect = SCT_a[i-1].L_sect / 1000.0;
                SCT_a[i-1].H_st = SCT_a[i-1].H_st / 1000.0;
                SCT_a[i-1].H_fin = SCT_a[i-1].H_fin / 1000.0; // { Перевод из м в мм }
                if (SCT_a[i-1].S_Type == 1)
                {
                    SCT_a[i-1].step_ = SCT_a[i-1].step_ / 1000.0;
                    SCT_a[i-1].e_st = SCT_a[i-1].e_st / 1000.0;
                    SCT_a[i-1].e_fin = SCT_a[i-1].e_fin / 1000.0;
                    SCT_a[i-1].delta = SCT_a[i-1].delta / 1000.0;
                    SCT_a[i-1].W_a = SCT_a[i-1].W_a / 1000.0; // { Перевод из м в мм }
                    SCT_a[i-1].d_Fi = SCT_a[i-1].d_Fi / 180.0 * Math.PI; //{ Перевод из градусов в радианы }
                }
                L_a = L_a + SCT_a[i-1].L_sect; //{Расчет длины рабочей части второго червяка }
                Lambda_a[i] = L_a; // { Разбивка длины второго червяка на секции }
            }
            DataRec.Lam_kor = new double[41];
            DataRec.Lam_kor[0] = 0;
            L_CYL = 0; //{ Длина корпуса }
            for (i = 1; i <= nCYL; i++)
            {
                CYL[i-1].L_sec = CYL[i-1].L_sec / 1000.0;
                CYL[i-1].Del_k_ = CYL[i-1].Del_k_ / 1000.0; //{Перевод мм в м}
                L_CYL = L_CYL + CYL[i-1].L_sec; //{ Подсчет общей длины корпуса }
                CYL[i-1].Q_W_k_ = CYL[i-1].Q_W_k_ / 1000.0 / 60.0; //{ Перевод дм^3/мин в см^3/с }
                //{ Сохранение границ секций корпуса }
                DataRec.Lam_kor[i-1] = L_CYL * 1000.0; //{ Размерность мм }
            }
            n_Eta = 60; //{ Число шагов интегрирования по глубине канала }
            for (i = 0; i <= n_Eta; i++) //{ Назначение чисел циклов интегрирования по секциям 2-го червяка }
            {
                Eta[i] = (double)i / n_Eta;
            }
            nLm_a = DataRec.n_Integr;
            j = 0;
            for (i = 1; i <= nSect_a; i++)
            {
                SCT_a[i-1].n_cykle = (int)Math.Round(nLm_a * SCT_a[i-1].L_sect / L_a);
                if (SCT_a[i-1].n_cykle == 0)
                {
                    SCT_a[i-1].n_cykle = 1;
                }
                j = j + SCT_a[i-1].n_cykle;
            }
            if (j > nLm_a) //{ Назначение чисел циклов интегрирования по секциям 1-го червяка }
            {
                nLm_a = j;
            }
            nLm = DataRec.n_Integr;
            j = 0;
            for (i = 1; i <= nSect; i++)
            {
                SCT[i-1].n_cykle = (int)Math.Round(nLm * SCT[i-1].L_sect / L);
                if (SCT[i-1].n_cykle == 0)
                {
                    SCT[i-1].n_cykle = 1;
                }
                j = j + SCT[i-1].n_cykle;
            }
            if (j > nLm)
            {
                nLm = j;
            }
            Mu0_temp = DataRec.Mu0_max * 1000.0;
            T0 = DataRec.T0_;
            b = DataRec.b_;
            m = DataRec.m_;
            N = DataRec.N_;
            Ro = DataRec.Ro_;
            T = DataRec.T_St;
            T_screw = DataRec.T_scr;
            Al_korp = DataRec.Al_kor;
            Al_screw = DataRec.Al_scr;
            a_T = DataRec.a_T_;
            Lam_T = DataRec.Lam_T_;
            T_Start = DataRec.T_St;
            DataRec.Del_s = DataRec.Del_s / 1000.0;
            DataRec.Q_W_s = DataRec.Q_W_s / 1000.0 / 60;
            DataRec.Del_s_a = DataRec.Del_s_a / 1000.0;
            DataRec.Q_W_s_a = DataRec.Q_W_s_a / 1000.0 / 60;
            Ro_M = Ro;
            Lam_T_M = Lam_T;
            Mu0 = Mu0_temp;

            Mix.Q1Q2 = new double[28, 7];
            Mix.Lambda = new double[29];

            Res.MQ = new double[61]; 
            Res.MP = new double[61];
            Res.MT = new double[61]; 
            Res.MVUL = new double[61];
        }

        void Put_QM( bool isAloneQ = false) //{ Выбор сечения червяка для определения Q машины в целом }
        {
            double fi, uz;

            if (isAloneQ)
            {
                Q_M = DOP.Q;
                DOP.n_Alfa = 0;
                DOP.Alfa_max = 0;
                DOP.Alfa_min = 0;
                return;
            }

            for (i_Alfa = 0; i_Alfa <= DOP.n_Alfa; i_Alfa++) //по первому червяку 
            {
                TETA = 1 - DOP.Alfa_min - (DOP.Alfa_max - DOP.Alfa_min) * i_Alfa / DOP.n_Alfa;
                for (i = 1; i <= nSect; i++)
                {
                    if (SCT[i-1].S_Type == 1)
                    {
                        D = SCT[i-1].D_st;
                        pd = Math.PI * D;
                        u = pd * N / 60.0;
                        fi = Math.Atan(SCT[i-1].step_ / pd);
                        uz = u * Math.Cos(fi);
                        W_st = pd * Math.Sin(fi) / SCT[i-1].n_Line - SCT[i-1].e_st * Math.Cos(fi);
                        S_st = SCT[i-1].H_st * W_st * SCT[i-1].n_Line;
                        W_fin = pd * Math.Sin(fi) / SCT[i-1].n_Line - SCT[i-1].e_fin * Math.Cos(fi);
                        S_Fin = SCT[i-1].H_fin * W_fin * SCT[i-1].n_Line;
                        if (i == 1 || S_st < SQ)
                        {
                            SQ = S_st;
                            Q_M = (1 - TETA) * uz * W_st * SCT[i-1].H_st * SCT[i-1].n_Line / 2.0;
                        }
                        if (S_Fin < SQ)
                        {
                            SQ = S_Fin;
                            Q_M = (1 - TETA) * uz * W_fin * SCT[i-1].H_fin * SCT[i-1].n_Line / 2.0;
                        }
                    }
                }
                Res.MQ[i_Alfa] = Q_M * 1E+6;
            }
        }

        public void Table_Titul()
        {
            text += "Удельное давление, температура материала и " +
                            $"геометрия каналапри объемной производительности машины Q={2*Q_M*1E+6}\n" +
                            "For a Screw Number 1                       For a Screw Number 2\n" +
                            "Z    p      T      H    W   Q1/Q2          Z    p      T      H    W   Q1/Q2\n";
        }

        void Start_SCT(int i_SCT)
        {
            Ro = Ro_M;
            Lam_T = Lam_T_M;
            if (SCR_Num == 1)
            {
                D = SCT[i_SCT-1].D_st;
            }
            else
            {
                D = SCT_a[i_SCT-1].D_st;
            }
            if (SCR_Num == 1)
            {
                dLm = SCT[i_SCT-1].L_sect / SCT[i_SCT-1].n_cykle;
            }
            else
            {
                dLm = SCT_a[i_SCT-1].L_sect / SCT_a[i_SCT-1].n_cykle;
            }
            if (SCR_Num == 1)
            {
                nL = SCT[i_SCT-1].n_Line;
            }
            else
            {
                nL = SCT_a[i_SCT-1].n_Line;
            }
            pd = Math.PI * D;
            st = stl(Lm, SCR_Num);
            fi = Math.Atan(st / pd);
            cs = Math.Cos(fi);
            sn = Math.Sin(fi);
            u = pd * N / 60.0;
            uX = u * sn;
            dz = dLm / sn;
            a_ = pd / cs;
            e = et(Lm, SCR_Num);
            W = pd * sn / nL - e * cs;
            H = Ht(Lm, SCR_Num);
            if (SCR_Num == 1)
            {
                g_ = uX * SCT[i_SCT-1].delta / 2.0;
            }
            else
            {
                g_ = uX * SCT_a[i_SCT-1].delta / 2.0;
            }
            Q_delta0 = a_ * g_;
            Q_delta = Q_delta0;
            Q_a = 0;
        }

        //{ Fluent step of a screw flight }
        public double stl(double Lm, int Scr_Num)
        {
            if (Scr_Num == 1)
            {
                return SCT[iSect-1].step_;
            }
            else
            {
                return SCT_a[iSect-1].step_;
            }
        }

        void i_0(int i_SCT)
        {
            Mu0 = Mu0_temp;
            if (((SCR_Num == 1) && (SCT[i_SCT-1].Monolit != 1) &&
                (((SCT[i_SCT-1].S_Type == 1) && (i_SCT == 1)) || (SCT[i_SCT-1].S_Type == 2))) ||
                ((SCR_Num == 2) && (SCT_a[i_SCT-1].Monolit != 1) &&
                (((SCT_a[i_SCT-1].S_Type == 1) && (i_SCT == 1)) || (SCT_a[i_SCT-1].S_Type == 2))))
            {
                Mu0 /= 1000.0;
                Ro *= f;
                Lam_T *= f;
            }
            korpus();
            if (SCR_Num == 1)
                pr = DataRec.Al_W_s * DataRec.Del_s / DataRec.Lam_s + DataRec.Al_W_s / Al_screw;
            else
                pr = DataRec.Al_W_s_a * DataRec.Del_s_a / DataRec.Lam_s + DataRec.Al_W_s_a / Al_screw;
            if (SCR_Num == 1)
                TB_s = (T + pr * DataRec.T_W_s) / (pr + 1);
            else
                TB_s = (T + pr * DataRec.T_W_s_a) / (pr + 1);
            if (SCR_Num == 1)
                q_Ws = DataRec.Al_W_s * (TB_s - DataRec.T_W_s);
            else
                q_Ws = DataRec.Al_W_s_a * (TB_s - DataRec.T_W_s_a);
            if (SCR_Num == 1)
                T_screw = T - DataRec.Al_W_s / Al_screw * (TB_s - DataRec.T_W_s);
            else
                T_screw = T - DataRec.Al_W_s_a / Al_screw * (TB_s - DataRec.T_W_s_a);
        }

        void i_Var(int i_SCT)
        {
            CYL[iCYL-1].q_int_k += q_Wk * Math.PI * (D + 2 * Del_k) * dLm;
            q_int_s += q_Ws * Math.PI * (D - 2 * H - 2 * DataRec.Del_s) * dLm;
            Lm += dLm;
            H = Ht(Lm, SCR_Num);
            e = et(Lm, SCR_Num);
            Mu = Mu0 * Math.Exp(-b * (T - T0));
            W = pd * sn / nL - e * cs;
            uZ = u * cs;
            uX = u * sn;
            Q_B = uZ * W * H / 2;
            dQ_delta = 0.5 * Q_delta0;
            tQ_delta = 0.0001 * Q_B;
            if (dQ_delta < 0.01 * Q_B)
                dQ_delta = 0.01 * Q_B;
            MuEf_Fix = Mu * Math.Exp((m - 1) * Math.Log(u / H));
            if (SCR_Num == 1)
                delt = SCT[i_SCT-1].delta;
            else
                delt = SCT_a[i_SCT-1].delta;
            Mu_delta = Mu0 * Math.Exp(-b * (T_korp - T0)) * Math.Exp((m - 1) * Math.Log(u / delt));
            b_ = st * delt * delt * delt / (12 * Mu_delta * nL * sn * (e * cs + delt));
            if (SCR_Num == 1)
                Q = (Q1 + Q_delta + Q_a) / nL;
            else
                Q = (Q2 + Q_delta + Q_a) / nL;
            Mu_a = Mu * Math.Exp((m - 1) * Math.Log(u / H));
        }

        void i_Var_2(int i_SCT)
        {
            double QM;
            if (SCR_Num == 1)
                QM = Q1;
            else
                QM = Q2;
            Q = (QM + Q_delta + Q_a) / nL;
            TETA = 1 - Q / Q_B;
            AL_Z = Q / Q_B;
            Q_B_X = H * uX / 2;
            Q_ut = Q_delta / a_;
            AL_X = Q_ut / Q_B_X;
            iter_screw();
            fd_fp();
            Qj_delta = Q_delta;
            b_a = st * H * H * H / (12 * Mu_a * nL * sn * (e * cs + H));
            if (dp_dz == 0)
                sign_dp = 0;
            else
                sign_dp = dp_dz / Math.Abs(dp_dz);
            if (SCR_Num == 1)
                Wa = SCT[iSect-1].W_a;
            else
                Wa = SCT_a[iSect-1].W_a;
            if (dp_dz > 0)
                dp_dz = (nL * Q_B * fd - QM - a_ * g_) / (nL * (Q_B - Q) * fp / dp_dz + a_ * b_ + Wa * b_a) * sign_dp;
            Q_delta = a_ * (b_ * dp_dz + g_);
            Q_a = Wa * b_a * dp_dz;
            test++;
        }

        void i_Var_3(int i_SCT)
        {
            double QM;
            if (SCR_Num == 1)
                QM = Q1;
            else
                QM = Q2;
            tkz = H * dp_dz * (1 - Eta_Z);
            tkx = H * dp_dx * (1 - Eta_X);
            TAU = Math.Sqrt(tkz * tkz + tkx * tkx);
            p = p + dp_dz * dz;
            dn = (uZ * tkz + uX * tkx) * W;
            Capacity = Capacity + dn * dz * nL;
            dVolume = H * W * dz * nL;
            Volume = Volume + dVolume;
            d_time = dVolume / QM;
            T = T + (dn + W * (Al_korp * (T_korp - T) + Al_screw * (T_screw - T)) - dp_dz * Q) / Q * a_T / Lam_T * dz;
            if (SCR_Num == 1)
                Mnlit = SCT[iSect-1].Monolit;
            else
                Mnlit = SCT_a[iSect-1].Monolit;
            if (Mnlit == 1)
                Energy = Energy + dn * dz * nL / QM;
            Energy_C = (T - T_Start) * Lam_T / a_T;
            E_R = E_R_Fluent(time_eqv);
            if (T >= DataRec.T_ini)
                time_eqv = time_eqv + Math.Exp(E_R / (T + 273) / (DataRec.T_eqv + 273) * (T - DataRec.T_eqv)) * d_time;
            time = time + d_time;
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
                    x = (x1 + x2) / 2;
                else
                    x = x1 - (x2 - x1) / (y2 - y1) * y1;

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

        double find_Q_delta()
        {
            double QM;
            if (SCR_Num == 1)
                QM = Q1;
            else
                QM = Q2;
            Q = (Q_M + Q_delta + Q_a) / nL;
            TETA = 1 - Q / Q_B;
            AL_Z = Q / Q_B;
            Q_B_X = H * uX / 2;
            Q_ut = Q_delta / a_;
            AL_X = Q_ut / Q_B_X;
            
            if (i == 1)
                MuEf_a();

            itera = 0;

            do
            {
                itera++;
                Del = 0;
                Int_i(ref I0, ref I1, ref I2);
                Roots();
                MuEff();
                MinMax(0, n_Eta, Mu_ef, ref fMin, ref fMax);
                MinMax(0, n_Eta, Mu_eff, ref fMin, ref ffMax);
                if (ffMax > fMax)
                    fMax = ffMax;
                for (int j = 0; j <= n_Eta; j++)
                    Del += Math.Pow((Mu_ef[j] - Mu_eff[j]) / fMax, 2);
                Del = Math.Sqrt(Del / n_Eta);
                IterEnd = (itera == 40) || (Del < 0.002);
                for (j = 0; j <= n_Eta; j++)
                    Mu_ef[j] = Mu_eff[j];
            } 
            while (!IterEnd);

            Qj_delta = Q_delta;
            Q_delta = a_ * (b_ * dp_dz + g_);
            b_a = st * H * H * H / (12 * Mu_a * nL * sn * (e * cs + H));
            Q_a = SCT_a[iSect-1].W_a * b_a * dp_dz;

            return (Q_delta - Qj_delta) / Q_B;
        }

        void i_Var_Smooth(int i_SCT, int i)
        {
            double QM;
            if (SCR_Num == 1)
            {
                QM = Q1;
            }
            else
            {
                QM = Q2;
            }
            if (SCR_Num == 1)
            {
                Dst = SCT[i_SCT-1].D_st;
                Dfin = SCT[i_SCT-1].D_fin;
                nCykle = SCT[i_SCT-1].n_cykle;
                Mnlit = SCT[i_SCT-1].Monolit;
            }
            else
            {
                Dst = SCT_a[i_SCT-1].D_st;
                Dfin = SCT_a[i_SCT-1].D_fin;
                nCykle = SCT_a[i_SCT-1].n_cykle;
                Mnlit = SCT_a[i_SCT-1].Monolit;
            }
            D = Dst + i * (Dfin - Dst) / nCykle;
            Lm += dLm;
            Mu = Mu0 * Math.Exp(-b * (T - T0));
            rH = D / 2;
            rB = rH - Ht(Lm, SCR_Num);
            MuEf_Fix = Mu * Math.Exp((m - 1) * Math.Log(u / (rH - rB)));
            for (j = 0; j <= n_Eta; j++)
            {
                r[j] = rB + j * (rH - rB) / n_Eta;
            }
            iter_smooth();
            Tau_rt_B = Tau_rt_H * rH / rB;
            S_round = (rH - rB) * (rH + rB);
            dVolume = Math.PI * S_round * dLm;
            d_time = dVolume / QM;
            dp_dz = 2 * (Tau_rz_H * rH - Tau_rz_B * rB) / S_round;
            p += dp_dz * dLm;
            dn = u * Tau_rt_H;
            TAU = Tau_rt_H;
            T += (2 * Math.PI * rH * (dn + Al_korp * (T_korp - T) + rB / rH * Al_screw * (T_screw - T)) - dp_dz * QM) / QM * a_T / Lam_T * dLm;
            Volume += dVolume;
            if (Mnlit == 1)
            {
                Energy += 2 * Math.PI * rH * dn * dLm / QM;
            }
            Energy_C = (T - T_Start) * Lam_T / a_T;
            E_R = E_R_Fluent(time_eqv);
            // расчет по функции E_R_Fluent
            if (T >= DataRec.T_eqv)
            {
                time_eqv += Math.Exp(E_R / (T + 273) / (DataRec.T_eqv + 273) * (T - DataRec.T_eqv)) * d_time;
            }
            time += d_time;
        }

        void Remain()
        {
            Res.Mu0 = Mu0_temp;
            Res.b = b;
            Res.n = m;
            Res.T0 = T0;
            Res.a = a_T;
            Res.Lam = Lam_T;
            Res.n_Q = DOP.n_Alfa;
            Res.T_eqv = DataRec.T_eqv;
            Res.t_in_eq = DataRec.t_in_eq;
            Res.T_ini = DataRec.T_ini;
            Res.t_op_eq = DataRec.t_op_eq;
            for (i = 0; i <= DOP.n_Alfa; i++)
            {
                Res.MQ[i] *= 2;
            }
            MinMax(0, DOP.n_Alfa, Res.MP, ref fMin, ref fMax);
            MinMax(0, DOP.n_Alfa, Res.MQ, ref ffMin, ref ffMax);
            // Final();
            // using (var QPT_Res = new StreamWriter(DirPath + "QPT_Res.rec"))
            // {
            //     QPT_Res.Write(Res);
            // }
            // using (var Mix_File = new StreamWriter(DirPath + "QPT_Mix.rec"))
            // {
            //     Mix_File.Write(Mix);
            // }
        }

        double et(double Lm, int Scr_Num)
        {
            if (Scr_Num == 1)
            {
                return SCT[iSect-1].e_st + (SCT[iSect-1].e_fin - SCT[iSect-1].e_st) / 
                        SCT[iSect-1].L_sect * (Lm - Lambda[iSect - 1]);
            }
            else
            {
                return SCT_a[iSect-1].e_st + (SCT_a[iSect-1].e_fin - SCT_a[iSect-1].e_st) / 
                        SCT_a[iSect-1].L_sect * (Lm - Lambda_a[iSect - 1]);
            }
        }

        double Ht(double Lm, int Scr_Num)
        {
            if (Scr_Num == 1)
            {
                return SCT[iSect-1].H_st + (SCT[iSect-1].H_fin - SCT[iSect-1].H_st) / 
                        SCT[iSect-1].L_sect * (Lm - Lambda[iSect - 1]);
            }
            else
            {
                return SCT_a[iSect-1].H_st + (SCT_a[iSect-1].H_fin - SCT_a[iSect-1].H_st) / 
                            SCT_a[iSect-1].L_sect * (Lm - Lambda_a[iSect - 1]);
            }
        }

        void korpus()
        {
            DLINA = 0;
            iCYL = 0;

            do
            {
                iCYL++;
                DLINA += CYL[iCYL-1].L_sec;
            } 
            while (!(DLINA >= Lm || iCYL == nCYL));

            AL_W_k = CYL[iCYL-1].Al_W_k_;
            Del_k = CYL[iCYL-1].Del_k_;
            T_W_k = CYL[iCYL-1].T_W_k_;
            Q_W_k = CYL[iCYL-1].Q_W_k_;
            pr = AL_W_k * Del_k / DataRec.Lam_k + AL_W_k / Al_korp;
            TB_k = (T + pr * T_W_k) / (pr + 1);
            q_Wk = AL_W_k * (TB_k - T_W_k);
            T_korp = T - AL_W_k / Al_korp * (TB_k - T_W_k);
        }

        void iter_screw()
        {
            if (i == 1)
            {
                MuEf_a();
            }

            itera = 0;
            Del = 0;

            do
            {
                itera++;
                Del = 0;
                Int_i(ref I0, ref I1, ref I2);
                Roots();
                MuEff();
                MinMax(0, n_Eta, Mu_ef, ref fMin, ref fMax);
                MinMax(0, n_Eta, Mu_eff, ref fMin, ref ffMax);

                if (ffMax > fMax)
                {
                    fMax = ffMax;
                }

                for (j = 0; j <= n_Eta; j++)
                {
                    Del += Math.Pow((Mu_ef[j] - Mu_eff[j]) / fMax, 2);
                }

                Del = Math.Sqrt(Del / n_Eta);
                IterEnd = (itera == 40) || (Del < 0.002);

                for (j = 0; j <= n_Eta; j++)
                {
                    Mu_ef[j] = Mu_eff[j];
                }
            } 
            while (!IterEnd);
        }

        void fd_fp()
        {
            ff = H / W;
            fd = 0;
            fp = 0;

            for (j = 1; j <= 20; j++)
            {
                double n = Math.PI * (2 * j - 1);
                double k = n * n;
                fd += th(n / 2 * ff) / (n * k);
                fp += th(n / 2 / ff) / (k * k * n);
            }

            fd = 16 / ff * fd;
            fp = 1 - 192 * ff * fp;
        }

        double E_R_Fluent(double time_eqv)
        {
            double E_R = VUL.E_R_int[0]; // TODO было 1, я заменил на 0

            for (int i = 0; i < VUL.n_inter; i++)
            {
                if (time_eqv >= VUL.t_eq_int[i])
                {
                    E_R = VUL.E_R_int[i];
                }
            }

            return E_R;
        }

        void MuEf_a()
        {
            for (int i = 0; i <= n_Eta; i++)
            {
                Mu_ef[i] = 1;
            }
        }

        void Int_i(ref double I0, ref double I1, ref double I2)
        {
            double f1=0, f2=0, f3=0, f4=0, f5=0, f6=0, d=0;
            int i;

            I0 = 0;
            I1 = 0;
            I2 = 0;
            d = 1.0 / n_Eta / 2.0;

            for (i = 0; i <= n_Eta; i++)
            {
                f2 = 1.0 / Mu_ef[i] / MuEf_Fix;
                f4 = f2 * Eta[i];
                f6 = f4 * Eta[i];

                if (i > 0)
                {
                    I0 += (f1 + f2) * d;
                    I1 += (f3 + f4) * d;
                    I2 += (f5 + f6) * d;
                }

                f1 = f2;
                f3 = f4;
                f5 = f6;
            }
        }

        void Roots()
        {
            Eta_Z = (I1 - I2 - AL_Z * I1 / 2.0) / (I0 - I1 - AL_Z * I0 / 2.0);
            dp_dz = uZ / (H * H * (I1 - Eta_Z * I0));
            Eta_X = (I1 - I2 - AL_X * I1 / 2.0) / (I0 - I1 - AL_X * I0 / 2.0);
            dp_dx = uX / (H * H * (I1 - Eta_X * I0));
        }

        void MuEff()
        {
            int i;
            double a;
            double[] Gamma_X = new double[61];
            double[] Gamma_Z = new double[61];

            for (i = 0; i <= n_Eta; i++)
            {
                a = Mu_ef[i] * MuEf_Fix;
                Gamma_X[i] = H / a * dp_dx * (Eta[i] - Eta_X);
                Gamma_Z[i] = H / a * dp_dz * (Eta[i] - Eta_Z);
                Mu_eff[i] = Math.Sqrt(Math.Pow(Gamma_X[i], 2) + Math.Pow(Gamma_Z[i], 2));
                Mu_eff[i] = Mu * Math.Exp((m - 1) * Math.Log(Mu_eff[i])) / MuEf_Fix;
            }
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

        void iter_smooth()
        {   
            if (i == 1)
            {
                MuEf_a();
            }

            itera = 0;
            Del = 0;
            do
            {
                itera++;
                Del = 0;
                Int_5(ref I1, ref I2, ref I3, ref I4, ref I5);
                Roots_g();
                MuEff_g();
                MinMax(0, n_Eta, Mu_ef, ref fMin, ref fMax);
                MinMax(0, n_Eta, Mu_eff, ref fMin, ref ffMax);
                if (ffMax > fMax)
                {
                    fMax = ffMax;
                }
                for (j = 0; j <= n_Eta; j++)
                {
                    Del += Math.Pow((Mu_ef[j] - Mu_eff[j]) / fMax, 2);
                }
                Del = Math.Sqrt(Del / n_Eta);
                IterEnd = (itera == 40) || (Del < 0.002);
                for (j = 0; j <= n_Eta; j++)
                {
                    Mu_ef[j] = Mu_eff[j];
                }
            } 
            while (!IterEnd);
        }

        double th(double x)
        {
            double a;
            if (x > 40)
            {
                a = 0;
            }
            else
            {
                a = Math.Exp(-2 * x);
            }
            return (1.0 - a) / (1.0 + a);
        }

        void Int_5(ref double I1, ref double I2, ref double I3, ref double I4, ref double I5)
        {
            double f1=0, f2=0, f3=0, f4=0, f5=0, f6=0, f7=0, 
                    f8=0, f9=0, f10=0, f11=0, f12=0, d=0, a=0, b=0;
            int i;
            I1 = 0;
            I2 = 0;
            I3 = 0;
            I4 = 0;
            I5 = 0;
            d = (rH - rB) / n_Eta / 2.0;
            for (i = 0; i <= n_Eta; i++)
            {
                b = rH * rH - r[i] * r[i];
                f2 = rB / r[i] * b / (rH * rH - rB * rB);
                f1 = rH / r[i] * (r[i] * r[i] - rB * rB) / (rH * rH - rB * rB);
                a = Mu_ef[i] * MuEf_Fix;
                f4 = rH / (r[i] * a);
                f6 = f1 / a;
                f8 = f2 / a;
                f10 = b * f1 / a;
                f12 = b * f2 / a;
                if (i > 0)
                {
                    I5 += (f3 + f4) * d;
                    I1 += (f5 + f6) * d;
                    I2 += (f7 + f8) * d;
                    I3 += (f9 + f10) * d;
                    I4 += (f11 + f12) * d;
                }
                f3 = f4;
                f5 = f6;
                f7 = f8;
                f9 = f10;
                f11 = f12;
            }
        }

        void Roots_g()
        {
            double a, QM;
            if (SCR_Num == 1)
                QM = Q1;
            else
                QM = Q2;
            a = Math.PI * (I1 * I4 - I2 * I3);
            Tau_rz_H = -QM * I2 / a;
            Tau_rz_B = QM * I1 / a;
            Tau_rt_H = u / I5;
        }

        void MuEff_g()
        {
            double a, b, Tau_rz, Tau_rt, c, d, f1, f2;
            for (int i = 0; i <= n_Eta; i++)
            {
                a = Mu_ef[i] * MuEf_Fix;
                f1 = rH / r[i] * (r[i] * r[i] - rB * rB) / (rH * rH - rB * rB);
                f2 = rB / r[i] * (rH * rH - r[i] * r[i]) / (rH * rH - rB * rB);
                c = (f1 * Tau_rz_H + f2 * Tau_rz_B) / a;
                d = Tau_rt_H * rH / r[i] / a;
                b = Math.Sqrt(c * c + d * d);
                Mu_eff[i] = Mu * Math.Exp((m - 1) * Math.Log(b)) / MuEf_Fix;
            }
        }

        void Fin_Q()
        {
            for (iCYL = 1; iCYL <= nCYL; iCYL++)
            {
                CYL[iCYL-1].dT_W_k = CYL[iCYL-1].q_int_k / Q_W_k * CYL[iCYL-1].a_W_k_ / CYL[iCYL-1].Lam_W_k_;
            }

            dT_W_s1 = q_int_s1 / DataRec.Q_W_s * DataRec.a_W / DataRec.Lam_W;
            dT_W_s = q_int_s / DataRec.Q_W_s_a * DataRec.a_W_a / DataRec.Lam_W_a;

            // Befor_final();
            string s1 = (2 * Q_M * 1E+6).ToString("0.0");
            Fin_Q_text = $"ПАРАМЕТРЫ ПЕРЕРАБОТКИ ПРИ ОБЪЕМНОМ РАСХОДЕ Q = {s1} см^3/с\n"
            + "Температура, градусы Цельсия:\n"
            + $"         Температура материала, питающего машину:            T(0) = {DataRec.T_St.ToString()}\n"
            + $"         Температура материала при сходе с червяка:             T(L) = {T.ToString()}\n"
            + $"         Температура теплоносителя в полости 1-го червяка:   T_SW = { DataRec.T_W_s.ToString()}\n"
            + $"         Ее изменение за время пребывания в полости червяка: {dT_W_s1.ToString()}\n"
            + $"         Температура теплоносителя в полости 2-го червяка:   T_SW = {DataRec.T_W_s_a.ToString()}\n"
            + $"         Ее изменение за время пребывания в полости червяка: {dT_W_s.ToString()}\n"
            + "         Температура теплоносителя в секцях корпуса:\n";
            for (int iCYL = 1; iCYL <= nCYL; iCYL++)
            {
                Fin_Q_text += T_W_k.ToString() + " ";
            }
            Fin_Q_text += "\n";
            Fin_Q_text += "         Ее изменение за времена пребывания теплоносителя в секцях корпуса:\n      ";
            for (int iCYL = 1; iCYL <= nCYL; iCYL++)
            {
                Fin_Q_text += CYL[iCYL - 1].dT_W_k.ToString() + " ";
            }
            Fin_Q_text += "Интегральные характеристики:\n\n"
            + "         Объем материала в машине :                     V =" + (Volume * 1000).ToString() + " дм^3\n"
            + "         Массовый расход материала:                     G =" + (Ro * 2 * Q_M * 3600).ToString() + " кг/ч\n"
            + "         Масса материала в машине :                     m = " + (Volume * Ro).ToString() + " кг\n"
            + "         Среднее время пребывания материала в машине:   t = " + (Volume / (2 * Q_M) / 60).ToString() + " мин\n"
            + "         Плотность поглощенной механической энергии: " + (Energy * 1E-6).ToString() + " МДж/м^3\n"
            +"         Повышение теплосодержания:                    " + (Energy_C * 1E-6).ToString() + " МДж/м^3\n";
            Res.MVUL[i_Alfa] = time_eqv / DataRec.t_in_eq * 100;
            Fin_Q_text += "         Израсходованная доля индукционного периода:   " + Res.MVUL[i_Alfa].ToString() + " %\n";
            text += Fin_Q_text;
        }


        // void Befor_final()
        // {
        //     if (i_Alfa == 0)
        //     {
        //         OpenGraph();
        //     }
        //     else
        //     {
        //         SetGraphMode(ModeVar);
        //     }
        //     MinMax(0, k, PM, fMin, fMax);
        //     MinMax(0, k, PM_a, ffMin, ffMax);
        //     if (ffMax > fMax)
        //     {
        //         fMax = ffMax;
        //     }
        //     SetBkColor(White);
        //     SetColor(DarkGray);
        //     AxesXY(0, ZM[k], fMin, fMax, MidSize, MinSize, "", "", White, DarkGray, DarkGray);
        //     Next_Graphic(0, k, ZM, PM, 0, k, ZM, PM, DarkGray, 1, LineInterpol);
        //     Next_Graphic(0, k, ZM_a, PM_a, 0, k, ZM_a, PM_a, DarkGray, 2, LineInterpol);
        //     SetTextJustify(LeftText, CenterText);
        //     SetTextStyle(DefaultFont, 0, 1);
        //     MoveTo((int)(0.51 * GetMaxX()), (int)(0.722 * GetMaxY()));
        //     OutText("   Axe coordinate, m");
        //     SetTextStyle(DefaultFont, VertDir, 1);
        //     MoveTo((int)(0.15 * GetMaxX()), (int)(0.25 * GetMaxY()));
        //     OutText("p, MPa");
        //     OutStr(0.07, "SPECIFIC PRESSURE");
        //     OutStr(0.95, "Press <ENTER> to go on");
        //     Console.ReadLine();
        //     ClearViewPort();
        //     MinMax(0, k, TM, fMin, fMax);
        //     MinMax(0, k, TM_a, ffMin, ffMax);
        //     if (ffMax > fMax)
        //     {
        //         fMax = ffMax;
        //     }
        //     AxesXY(0, ZM[k], fMin, fMax, MidSize, MidSize, "", "", White, DarkGray, DarkGray);
        //     Next_Graphic(0, k, ZM, TM, 0, k, ZM, TM, DarkGray, 1, LineInterpol);
        //     Next_Graphic(0, k, ZM_a, TM_a, 0, k, ZM_a, TM_a, DarkGray, 2, LineInterpol);
        //     SetTextJustify(LeftText, CenterText);
        //     SetTextStyle(DefaultFont, 0, 1);
        //     MoveTo((int)(0.51 * GetMaxX()), (int)(0.83 * GetMaxY()));
        //     OutText("   Axe coordinate, m");
        //     SetTextStyle(DefaultFont, VertDir, 1);
        //     MoveTo((int)(0.15 * GetMaxX()), (int)(0.35 * GetMaxY()));
        //     OutText("  T, centigrade");
        //     OutStr(0.07, "TEMPERATURE of material");
        //     OutStr(0.95, "Press <ENTER> to go on");
        //     Console.ReadLine();
        // }

        // void Final()
        // {
        //     SetGraphMode(ModeVar);
        //     SetBkColor(White);
        //     SetColor(DarkGray);
        //     AxesXY(ffMin, ffMax, fMin, fMax, MidSize, MinSize, "", " p, MPa", White, DarkGray, DarkGray);
        //     Next_Graphic(0, Dop.n_Alfa, Res.MQ, Res.Mp, 0, Dop.n_Alfa, Res.MQ, Res.Mp, DarkGray, 1, Spline);
        //     SetTextJustify(LeftText, CenterText);
        //     SetTextStyle(DefaultFont, 0, 1);
        //     MoveTo((int)(0.38 * GetMaxX()), (int)(0.722 * GetMaxY()));
        //     OutText("Throughput Q, sm^3/sec");
        //     OutStr(0.035, "DEPENDANCE of SPECIFIC PRESSURE in front of a head");
        //     OutStr(0.08, "upon THROUGHPUT of material");
        //     OutStr(0.95, "Press <ENTER> to go on");
        //     Console.ReadLine();
        //     ClearViewPort();
        //     MinMax(0, Dop.n_Alfa, Res.MT, fMin, fMax);
        //     if (fMax - fMin < 10)
        //     {
        //         fMin = fMin - 7;
        //         fMax = fMax + 3;
        //     }
        //     AxesXY(ffMin, ffMax, fMin, fMax, MidSize, MinSize, "", " T, centigrade", White, DarkGray, DarkGray);
        //     Next_Graphic(0, Dop.n_Alfa, Res.MQ, Res.MT, 0, Dop.n_Alfa, Res.MQ, Res.MT, DarkGray, 1, Spline);
        //     SetTextJustify(LeftText, CenterText);
        //     SetTextStyle(DefaultFont, 0, 1);
        //     MoveTo((int)(0.38 * GetMaxX()), (int)(0.722 * GetMaxY()));
        //     OutText("THROUGHPUT Q, sm^3/sec");
        //     OutStr(0.04, "DEPENDANCE of TEMPERATURE of material in front of a head");
        //     OutStr(0.08, "upon its THROUGHPUT by MACHINE");
        //     OutStr(0.95, "Press <ENTER> to finish the programm");
        //     Console.ReadLine();
        //     CloseGraph();
        // }



        public int
            nLm,nLm_a,i,j,k,s,DriverVar,ModeVar,ErrorNumber,nL,ic,nPrint,test,
            n_Eta,nCYL,iCYL,itera,nSect,nSect_a,i_Alfa,iS_korp,
            SCR_Num, iSect;

        public double
            D,L,L_a,H0,H,TETA,e,u,fi,Mu0,T0,Mu,b,m,T,Lm,p,uX,uZ,db,Q_M,W,AL_Z,
            AL_X,cs,sn,fd,fp,ff,tkz,tkx,z,pr,f1,f2,Ro,MASSA,T_Start,a_T,Lam_T,
            T_korp,T_screw,Al_korp,Al_screw,pd,st,dz,dn,dLm,I0,I1,I2,I3,I4,I5,
            Mu_e,fMin,fMax,ffMin,ffMax,Eta_Z,Eta_X,dp_dx,dp_dz,sign_dp,Del,SQ,
            MuEf_Fix,g_,a_,b_,b_a,Q,Q_B,Q_B_X,Q_ut,Q_a,Mu_a,Mu_delta,func,E_R,
            Qj_delta,Q_delta,Q_delta0,dQ_delta,tQ_delta,Q_W_k,W_fin,S_st,W_st,
            S_Fin,rH,rB,Tau_rz_B,Tau_rz_H,Tau_rt_H,Tau_rt_B,AL_W_k,Capacity,N,
            DLINA,Del_k,T_W_k,TB_k,TB_s,q_Wk,q_Ws,f,L_CYL,Ro_M,q_int_s,dT_W_s,
            Mu0_temp,S_round,Lam_T_M,Energy,Energy_C,Enrg_fix,Volume,time_eqv,
            dVolume,time,d_time,TAU,c,Q_total,Q1,Q2;

        public double[] // от 0 до 60
            Eta = new double[61],Mu_ef = new double[61],ZM = new double[61],TM = new double[61],
            PM = new double[61],Mu_eff = new double[61],M_P = new double[61],r = new double[61],
            Mu_ef_a = new double[61],ZM_a = new double[61],TM_a = new double[61],
            PM_a = new double[61],Mu_eff_a = new double[61],M_P_a = new double[61],r_a = new double[61];

        public bool
            IterEnd,transit,VMenuResult,New;

        // sZ: NAMEst;
        // sC: COMENTst;
        // Titul_g,Titul_j: StringArr;

        public int i_Menu,j_Menu,i_print;

        public RESULT Res = new RESULT();
        public DOP_DATA DOP = new DOP_DATA();
        public DATA_ DataRec = new DATA_(); 
        public SECT[] SCT = new SECT[29]; //= ARRAY[1..28] of RECORD  { ГЕОМЕТРИЯ СЕКЦИЙ ПЕРВОГО ЧЕРВЯКА }
        public SECT_a[] SCT_a = new SECT_a[29]; //=ARRAY[1..28] of RECORD  { ГЕОМЕТРИЯ СЕКЦИЙ ВТОРОГО ЧЕРВЯКА }
        public CYLINDER[] CYL = new CYLINDER[15]; // = ARRAY[1..14] of RECORD
        public VULCAN VUL = new VULCAN();

        public double 
            delt,Wa,Dst,Dfin;
        public int
            Mnlit,nCykle;

        public double
            Q1Q2,q_int_s1,dT_W_s1,Volume_1,time_1,Energy_1,Ener_C_1;

        public int X_cursor,Y_cursor;

        public MIX_MACRO Mix = new MIX_MACRO();

        public double[] Lambda = new double[41], Lambda_a = new double[41]; // от 0 до 40

        //Рузультаты
        public string text = "";
        public string Fin_Q_text = "";

        public List<double[]> PM_LIST = new List<double[]>();
        public List<double[]> TM_LIST = new List<double[]>();
        public List<double[]> ZM_LIST = new List<double[]>();
        //public List<double[]> X_OTN_LIST = new List<double[]>();
        //public List<List<double>> X_PL_LIST = new List<List<double>>();
        public List<double[]> PM_a_LIST = new List<double[]>();
        public List<double[]> TM_a_LIST = new List<double[]>();
        public List<double[]> ZM_a_LIST = new List<double[]>();
        //public List<double[]> X_OTN_LIST = new List<double[]>();
        //public List<List<double>> X_PL_LIST = new List<List<double>>();

        public List<List<Vector2>> PZ = new List<List<Vector2>>();
        public List<List<Vector2>> TZ = new List<List<Vector2>>();
        //public List<List<Vector2>> XZ = new List<List<Vector2>>();
        public List<List<Vector2>> PZ_a = new List<List<Vector2>>();
        public List<List<Vector2>> TZ_a = new List<List<Vector2>>();
        //public List<List<Vector2>> XZ = new List<List<Vector2>>();

        public List<List<List<double>>> ZXPT = new List<List<List<double>>>();
        public List<List<List<double>>> ZXPT_a = new List<List<List<double>>>();

    }
}