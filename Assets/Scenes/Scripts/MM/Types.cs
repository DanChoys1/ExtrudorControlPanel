namespace Types
{
    public struct RESULT
    {
        public string
            Designation;

        public double
            // { ��������� ���������� �������������� ��������� }
            Mu0,
            b,
            n,
            T0,
            //{ �����������- � ���������������� ��������� }
            a,
            Lam;
        public int
            //{����� �������� ��������� ������� Q }
            n_Q;
        public double[]
            //{ ��������� �������������� ��������� ������ } 
            MQ,
            MP,
            MT;
    }

    //QPT Types
    public struct DOP_DATA
    {
        public string
            Designation;

        public double 
            Q; //���� Q ��������
        public double
            Alfa_min, //{ (Q/Q_b)_min ����� �������� ����� ����� �������; }
            Alfa_max; //{ (Q/Q_b)_max ����� �������� ����� ����� �������; }
        public int
            n_Alfa; //{ ����� ����� �� Q/Q_b }
    }

    public struct SECT //= ARRAY[1..29] of RECORD          { ��������� ������ ������� }
    {
        public string
            Designation;

        public int
            S_Type,                 //{ ��� ������: 1 - � ��������, 2 - ������� }
            Order,                   //{ � � � � � � � � � �  � � � � �  ������ }
            n_cykle,               //{ ����� ������ �������������� ����� ������ }
            Monolit,        //{ � � � � � � �  ������������ (1) ��� �������� (0)}
            n_Line,                            //{ ����� ������� �������� ����� }
            R_or_L,                      //{ ��� �������: 1 - ������, 2 - ����� }
            p_W;      //{ �������: 1 - ����������� ���������; 2 - ������������� }

        public double
            D_st,            //{ ���������� ������� ������� � ������ ������, �� }
            D_fin,            //{ ���������� ������� ������� � ����� ������, �� }
            L_sect,                                        //{ ����� ������, �� }
            step_,                           //{ ��� ������� �������� �����, �� }
            H_st,           //{ ������� ��������� ������ (��� ���������� �����) }
                                //{ �  � � � � � �  ������, �� }
            H_fin,          //{ ������� ��������� ������ (��� ���������� �����) }
                                //{ �  � � � � �  ������, �� }
            e_st,                        //{ ������� ������ � ������ ������, �� }
            e_fin,                        //{ ������� ������ � ����� ������, �� }
            delta,                                     //{ ���������� �����, �� }
            d_Fi,      //{ ���� �������� ������ �� ��������� � ����������, ����.}
            W_a;               //{ ��������� ������ ������ �������� �������, �� }
    }

    public struct DATA_
    {
        public string
            Designation;

        public int
            nS_1,     //{ ����� ������ c �������� ������� }
            nS_2,     //{ ����� ������� ������ ������� }
            nS_korp,  //{ ����� ������ ������� � ������ ������������ }
            Var_Tscr, //{ ��� ��������� ���������� ������� �� ����������� ������� }
            n_Integr, //{ ����� ������ �������������� ����� ��� ������� }
            n_Graph;  //{ ����� ������ ����� ��� ������� ��� ��� ��������� }

        public double
            T0_,    //{ ������������� �������� ����������� ��������� }
            Mu0_,   //{ ����������� ������������ ��� ����������� T0_, ���*�^n }
            b_,     //{ ������� ������� ����������� �� �������� ��������, 1/K }
            m_,     //{ ������ ������� �������� }
            N_,     //{ ������� �������� �������, ��/��� }
            Ro_,    //{ ��������� ����������� ���������, ��/�^3 }
            Ro_gran_,//{ ��������� ������ ������, ��/�^3 }
            T_PL_,   //{ ����������� ��������� (��� �����������) ��������,
                         //��.������� }
            K_PL_ ,   //{ ������� ������� ��������� ��������, ���/�� }
            T_St,   //{ ��������� ����������� ��������, ��.������� }
            a_T_,   //{ ����������� ���������������������� ��������, �^2/� }
            Lam_T_, //{ ����������� ���������������� ��������, ��/(�*�) }
            Lam_k,  //{����������� ���������������� ��������� �������,��/(�*�)}
            Lam_s,  //{����������� ���������������� ��������� �������,��/(�*�)}
            Al_kor, //{����������� ����������� �������� � �������, ��/(�^2*�)}
            Al_scr, //{����������� ����������� �������� � �������, ��/(�^2*�)}
            Del_s,  //{������� ��������������� ������ �������, ��}
            Al_W_s, //{����������� ����������� ������������� � �������,
                        //��/(�^2*�)}
            T_W_s,  //{����������� ������������� � ������� �������, ����.�. }
            a_W,    //{����������� ���������������������� ������������� � �������
                        //�������, �^2/�}
            Lam_W,  //{����������� ���������������� ������������� � �������
                        //�������, ��/(�*�)}
            Q_W_s,  //{�������� ������ �������������, ������������ ������,
                        //��^3/���}
            T_scr;  //{�������������� ����������� �������, ����.�.}

        public double[]
            Lam_kor,   //{ ���������� ������ ������ �������, �� }
            T_kor;     //{ ����������� ������ �������, ����.������� }
    }

    public struct CYLINDER //= ARRAY[1..14] of RECORD
    {
        public string
            Designation;

        public int
            Var_T;    //{��� ��������� ���������� �������: 1 ��� 2}

        public double
            L_sec,    //{����� ������ �������, ��}
            Del_k_,   //{������� ��������������� ������ �������, ��}
            T_W_k_,  // {����������� ������������� ��� �������, ����.�.}
            Al_W_k_,  //{����������� ����������� ������������� � �������,
                          // ��/(�^2*�)}
            a_W_k_,   //{����������� ���������������������� �������������, �^2/�}
            Lam_W_k_, //{����������� ���������������� �������������, ��/(�*�)}
            Q_W_k_,   //{�������� ������ �������������, ������������ ������,
                          //  ��^3/���}
            q_int_k,  //{��������� ������������ �������� ����� �����
                          // ��������������� ������, ��}
            dT_W_k;   //{��������� ����������� ������������� � ������� ���������
                          //�� ������ �������, ����.�}
    }

    public struct S_CUT       //{ ��������� ������ ������� � �������� }
    {
        public int Num_C;
        public SECT[] S;
    }
    public struct S_SM           //{ ��������� ������� ������ ������� }
    {
        public int Num_S;
        public SECT[] S;
    }
    public struct S_BAR                             //{ ������ ������� }
    {
        public int Num_B;
        public CYLINDER[] S;
    }

    //DIE Types
    public struct FluxData
    {
        public string
            Designation;

        public int
            N, //{ ����� ����������� ������������ ��������������� ����� }
            iR;  //{    ����� ���������� ����� �� r/R ��� ���������� v(r) }
        public double
            AH, AB, //{ ������������ ��������������� ���� �� "alpha" }
            kH, kB;  //{     ������������ ��������������� ���� �� r/R }
    }

    //{ ��������� ������ ������ }
    public struct SECTIONS // ARRAY[1..29] Of Record
    {
        public string
            Designation;

        public int
            PRIZNAK,
            Order, //{   � � � � � � � � � �  � � � � �  ������ }
            n_cykle; //{ ����� ������ �������������� ����� ������ }

        public double
            D_st, //{   �������� ������� ������ � ������ ������, �� }
            D_fin, //{    �������� ������� ������ � ����� ������, �� }
            D_B_st, //{ ���������� ������� ������ � ������ ������, �� }
            D_B_fin, //{  ���������� ������� ������ � ����� ������, �� }
            L_sect, //{ ����� ������, �� }
            T_st, //{   ����������� ������ ������ � �������� ������ }
            T_B; //{      ����������� ���������� � �������� ������ }
    }

    //{ ��������� ������ ������ ������� }
    public struct S_KG
    {
        public string
            Designation;

        public int Num_S;
        public SECTIONS[] S;
    }

    public struct RESULT_D
    {
        public double
            v_profil,   // { �������� �������� �������� �������, �/� }
            D_Last,     // { ������� ������������� ������ ��������, �� }
            Delta,      // { ������� ������ ������ ��������, �� }
            a, Lam;      //{ �����������- � ���������������� �������� }

        public int
            n_slices;   // { ����� ����������� ������������ ����� �������� }

        public double[]
            T_START,    // { ������� �������� ����������� ������� T(r/R) }
            R_rel;      // { ������� �������� ��������� r/R }
    }

    public struct Res_fin
    {
        public double
            Q_fin,          // �������� ������: ��^3/���
            v_fin,          // �������� �������� ��������������� �������: �/���
            T_mid,          // ����������� ����� ��� ������ �� �������: " + T_f + " ��������
            R_inter,        // ���������� ������ ������ � ����� ������: ��
            delta;          // ������� �������� � ����������� �������� ������ � ����� ������: ��
    }     
    
    // ����������
    public struct Train
    {
        public string
            Designation;

        public double
            Time,
            G_max,
            G_min,
            Id_max,
            Fs_max;
    }
}