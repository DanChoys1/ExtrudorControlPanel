using Types;

namespace Program
{
    public class InitData
    {
        ////////// QPT //////////
        public DATA_ data = new DATA_();
        public SECT[] sect = new SECT[3];
        public CYLINDER[] cyl = new CYLINDER[2];
        public DOP_DATA dop = new DOP_DATA();
        public RESULT res = new RESULT();

        ////////// DIE //////////
        public FluxData fluxData = new FluxData();
        public S_KG S_K = new S_KG();

        ////////// Train //////////
        public Train train = new Train();

        public InitData(bool isInit = true)
        {
            if (!isInit) return;

            ////////// QPT //////////

            // DATA
            data.Designation = "data1";
            data.nS_1           = 2;
            data.nS_2           = 1;
            data.nS_korp        = 2;
            data.Var_Tscr       = 2;
            data.n_Integr       = 200;
            data.n_Graph        = 1000;

            data.T0_            = 135;
            data.Mu0_           = 10;
            data.b_             = 0.01;
            data.m_             = 0.505;
            data.N_             = 10;
            data.Ro_            = 980;
            data.Ro_gran_       = 650;
            data.T_PL_          = 110;
            data.K_PL_          = 0;
            data.T_St           = 50;
            data.a_T_           = 8;
            data.Lam_T_         = 0.16;
            data.Lam_k          = 12.7;
            data.Lam_s          = 12.7;
            data.Al_kor         = 100;
            data.Al_scr         = 100;
            data.Del_s          = 10;
            data.Al_W_s         = 1;
            data.T_W_s          = 120;
            data.a_W            = 0.00002;
            data.Lam_W          = 0.026;
            data.Q_W_s          = 2;
            data.T_scr          = 140;

            //SCT
            sect[0].Designation = "sect1";
            sect[0].S_Type      = 1;
            sect[0].Order       = 1;
            sect[0].Monolit     = 0;
            sect[0].n_cykle     = 0;
            sect[0].n_Line      = 1;
            sect[0].R_or_L      = 1;
            sect[0].p_W         = 0;
            sect[0].D_st        = 90;
            sect[0].D_fin       = 90;
            sect[0].L_sect      = 450;
            sect[0].step_       = 90;
            sect[0].H_st        = 12;
            sect[0].H_fin       = 5;
            sect[0].e_st        = 15;
            sect[0].e_fin       = 15;
            sect[0].delta       = 0.05;
            sect[0].d_Fi        = 0;
            sect[0].W_a         = 0;

            sect[1].Designation = "sect2";
            sect[1].S_Type      = 1;
            sect[1].Order       = 2;
            sect[1].Monolit     = 0;
            sect[1].n_cykle     = 0;
            sect[1].n_Line      = 1;
            sect[1].R_or_L      = 1;
            sect[1].p_W         = 0;
            sect[1].D_st        = 90;
            sect[1].D_fin       = 90;
            sect[1].L_sect      = 350;
            sect[1].step_       = 90;
            sect[1].H_st        = 5;
            sect[1].H_fin       = 5;
            sect[1].e_st        = 15;
            sect[1].e_fin       = 15;
            sect[1].delta       = 0.1;
            sect[1].d_Fi        = 0;
            sect[1].W_a         = 0;

            sect[2].Designation = "sect3";
            sect[2].S_Type      = 2;
            sect[2].Order       = 3;
            sect[2].Monolit     = 0;
            sect[2].n_cykle     = 20;
            sect[2].n_Line      = 0;
            sect[2].R_or_L      = 0;
            sect[2].p_W         = 0;
            sect[2].D_st        = 90;
            sect[2].D_fin       = 90;
            sect[2].L_sect      = 100;
            sect[2].step_       = 0;
            sect[2].H_st        = 6;
            sect[2].H_fin       = 6;
            sect[2].e_st        = 0;
            sect[2].e_fin       = 0;
            sect[2].delta       = 0;
            sect[2].d_Fi        = 0;
            sect[2].W_a         = 0;

            //sect[3].S_Type      = 2;
            //sect[3].Order       = 3;
            //sect[3].Monolit     = 0;
            //sect[3].n_cykle     = 20;
            //sect[3].n_Line      = 0;
            //sect[3].R_or_L      = 0;
            //sect[3].p_W         = 0;
            //sect[3].D_st        = 120;
            //sect[3].D_fin       = 90;
            //sect[3].L_sect      = 80;
            //sect[3].step_       = 0;
            //sect[3].H_st        = 10;
            //sect[3].H_fin       = 10;
            //sect[3].e_st        = 0;
            //sect[3].e_fin       = 0;
            //sect[3].delta       = 0;
            //sect[3].d_Fi        = 0;
            //sect[3].W_a         = 0;

            //SCT_C
            cyl[0].Designation = "cyl1";
            cyl[0].Var_T        = 1;
            cyl[0].L_sec        = 450;
            cyl[0].Del_k_       = 10;
            cyl[0].T_W_k_       = 190;
            cyl[0].Al_W_k_      = 2000;
            cyl[0].a_W_k_       = 0.00000014;
            cyl[0].Lam_W_k_     = 0.6;
            cyl[0].Q_W_k_       = 2;
            cyl[0].q_int_k      = 0;
            cyl[0].dT_W_k       = 0;
                                
            cyl[1].Designation = "cyl2";
            cyl[1].Var_T        = 1;
            cyl[1].L_sec        = 480;
            cyl[1].Del_k_       = 10;
            cyl[1].T_W_k_       = 150;
            cyl[1].Al_W_k_      = 350;
            cyl[1].a_W_k_       = 0.00000014;
            cyl[1].Lam_W_k_     = 0.6;
            cyl[1].Q_W_k_       = 2;
            cyl[1].q_int_k      = 0;
            cyl[1].dT_W_k       = 0;

            //QPT
            dop.Designation = "dop1";
            dop.Alfa_min        = 0.8;
            dop.Alfa_max        = 0.9;
            dop.n_Alfa          = 2;

            //QPT_R
            //res.Mu0 = 10000;
            //res.b = 0.01;
            //res.n = 0.505;
            //res.T0 = 135;
            //res.a = 0.00000008;
            //res.Lam = 0.16;
            //res.n_Q = 2;
            //res.MQ = new double[3];
            //res.MQ[0] = 6.4182761356;
            //res.MQ[1] = 6.819418394;
            //res.MQ[2] = 7.2205606525;
            //res.MP = new double[3];
            //res.MP[0] = 8.89566982;
            //res.MP[1] = 7.2073293496;
            //res.MP[2] = 5.6127581505;
            //res.MT = new double[3];
            //res.MT[0] = 156.20711721;
            //res.MT[1] = 154.6121707;
            //res.MT[2] = 153.11101985;

            ////////// DIE //////////

            //DIE
            fluxData.Designation = "fluxData1";
            fluxData.N          = 14; 
            fluxData.iR         = 28;  
            fluxData.AH         = 1.04;
            fluxData.AB         = 1.02; 
            fluxData.kH         = 1.02;
            fluxData.kB         = 1.01;

            //DIE_S
            S_K.Designation = "S_K1";
            S_K.Num_S           = 4;
            S_K.S = new SECTIONS[S_K.Num_S];
            S_K.S[0].Designation = "S_K.S1";
            S_K.S[0].PRIZNAK    = 1;
            S_K.S[0].Order      = 1;
            S_K.S[0].n_cykle    = 40;
            S_K.S[0].D_st       = 60;
            S_K.S[0].D_fin      = 10;
            S_K.S[0].D_B_st     = 0;
            S_K.S[0].D_B_fin    = 0;
            S_K.S[0].L_sect     = 80;
            S_K.S[0].T_st       = 120;
            S_K.S[0].T_B        = 0;

            S_K.S[1].Designation = "S_K.S2";
            S_K.S[1].PRIZNAK    = 1;
            S_K.S[1].Order      = 2;
            S_K.S[1].n_cykle    = 10;
            S_K.S[1].D_st       = 10;
            S_K.S[1].D_fin      = 10;
            S_K.S[1].D_B_st     = 0;
            S_K.S[1].D_B_fin    = 0;
            S_K.S[1].L_sect     = 10;
            S_K.S[1].T_st       = 120;
            S_K.S[1].T_B        = 0;

            S_K.S[2].Designation = "S_K.S3";
            S_K.S[2].PRIZNAK    = 2;
            S_K.S[2].Order      = 3;
            S_K.S[2].n_cykle    = 50;
            S_K.S[2].D_st       = 12;
            S_K.S[2].D_fin      = 20;
            S_K.S[2].D_B_st     = 4;
            S_K.S[2].D_B_fin    = 14;
            S_K.S[2].L_sect     = 50;
            S_K.S[2].T_st       = 120;
            S_K.S[2].T_B        = 120;

            S_K.S[3].Designation = "S_K.S4";
            S_K.S[3].PRIZNAK    = 2;
            S_K.S[3].Order      = 4;
            S_K.S[3].n_cykle    = 20;
            S_K.S[3].D_st       = 20;
            S_K.S[3].D_fin      = 20;
            S_K.S[3].D_B_st     = 14;
            S_K.S[3].D_B_fin    = 14;
            S_K.S[3].L_sect     = 20;
            S_K.S[3].T_st       = 120;
            S_K.S[3].T_B        = 120;

            ////////// Train //////////
            train.Designation = "train1";
            train.Time = 99999999999;
            train.G0 = 50;
            train.Id_max = 5;
            train.Fs_max = 0.1;
            train.Is0 = 1650;
    }
    }
}