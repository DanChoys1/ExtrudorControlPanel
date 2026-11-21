using Extruder;
using Program;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Types;
using UnityEngine;
using Vector = System.Numerics.Vector2;

public class Recalculates : MonoBehaviour
{
    [SerializeField] private EntryValuePanel _entrPanel;
    [SerializeField] private CurrTimeUpdate _operTime;

    [SerializeField] private TMP_Text _shnekSpeed;
    [SerializeField] private TMP_Text _shnekSpeedTarget;
    [SerializeField] private TMP_Text _voronkaSpeed;
    [SerializeField] private TMP_Text _voronkaSpeedTarget;
    [SerializeField] private TMP_Text _voronkaFr;
    [SerializeField] private TMP_Text _knifeSpeed;
    [SerializeField] private TMP_Text _knifeSpeedTarget;
    [SerializeField] private TMP_Text _knifeFr;

    [SerializeField] private TMP_Text _temp1;
    [SerializeField] private TMP_Text _tempTarget1;
    [SerializeField] private TMP_Text _tempDopLabel11;
    [SerializeField] private TMP_Text _tempDopLabel12;
    [SerializeField] private MeasuringtTemp _measTemp1;

    [SerializeField] private TMP_Text _temp2;
    [SerializeField] private TMP_Text _tempTarget2;
    [SerializeField] private TMP_Text _tempDopLabel21;
    [SerializeField] private TMP_Text _tempDopLabel22;
    [SerializeField] private MeasuringtTemp _measTemp2;

    // Третья зона термостатирования корпуса — используется только моделью Dual
    // (у двухшнекового экструдера по умолчанию 3 зоны корпуса, а не 2).
    [SerializeField] private TMP_Text _temp3;
    [SerializeField] private TMP_Text _tempTarget3;
    [SerializeField] private TMP_Text _tempDopLabel31;
    [SerializeField] private TMP_Text _tempDopLabel32;
    [SerializeField] private MeasuringtTemp _measTemp3;

    [SerializeField] private TMP_Text       _scenarioLabel;

    [SerializeField] private TMP_Text       _resText;

    [SerializeField] private WindowGraph    _GTrendGraph;
    [SerializeField] private WindowGraph    _IdTrendGraph;
    [SerializeField] private WindowGraph    _FsTrendGraph;
    [SerializeField] private WindowGraph    _YTrendGraph;

    [SerializeField] private WindowGraph    _XGraph;
    [SerializeField] private WindowGraph    _PGraph;
    [SerializeField] private WindowGraph    _TGraph;
    [SerializeField] private WindowGraph    _WorkDotGraph;
    [SerializeField] private TableManager   _XPTTabel;

    [SerializeField] private TableManager   _logTabel;

    [SerializeField] private TMP_Text       _resTextInstructor;

    [SerializeField] private WindowGraph    _GTrendGraphInstructor;
    [SerializeField] private WindowGraph    _IdTrendGraphInstructor;
    [SerializeField] private WindowGraph    _FsTrendGraphInstructor;
    [SerializeField] private WindowGraph    _YTrendGraphInstructor;

    [SerializeField] private WindowGraph    _XGraphInstructor;
    [SerializeField] private WindowGraph    _PGraphInstructor;
    [SerializeField] private WindowGraph    _TGraphInstructor;
    [SerializeField] private WindowGraph    _WorkDotGraphInstructor;

    [SerializeField] private TableManager   _logTabelInstructor;

    ////////// Dual (двухшнековый экструдер) //////////
    [SerializeField] private TMP_Text       _resTextDual;

    [SerializeField] private WindowGraph    _GTrendGraphDual;
    [SerializeField] private WindowGraph    _IdTrendGraphDual;
    [SerializeField] private WindowGraph    _FsTrendGraphDual;
    [SerializeField] private WindowGraph    _YTrendGraphDual;

    [SerializeField] private WindowGraph    _PGraphDual;
    [SerializeField] private WindowGraph    _TGraphDual;
    [SerializeField] private TableManager   _XPTTabelDual;
    [SerializeField] private TableManager   _XPT_aTabelDual;

    [SerializeField] private TableManager   _logTabelDual;

    [SerializeField] private TMP_Text       _resTextDualInstructor;

    [SerializeField] private WindowGraph    _GTrendGraphDualInstructor;
    [SerializeField] private WindowGraph    _IdTrendGraphDualInstructor;
    [SerializeField] private WindowGraph    _FsTrendGraphDualInstructor;
    [SerializeField] private WindowGraph    _YTrendGraphDualInstructor;

    [SerializeField] private WindowGraph    _PGraphDualInstructor;
    [SerializeField] private WindowGraph    _TGraphDualInstructor;
    [SerializeField] private TableManager   _XPTTabelDualInstructor;
    [SerializeField] private TableManager   _XPT_aTabelDualInstructor;

    [SerializeField] private TableManager   _logTabelDualInstructor;

    // Переключают видимость панелей обучаемого/инструктора в зависимости
    // от выбранной модели экструдера (Single/Dual).
    [SerializeField] private GameObject     _singlePanel;
    [SerializeField] private GameObject     _singlePanelInstructor;
    [SerializeField] private GameObject     _dualPanel;
    [SerializeField] private GameObject     _dualPanelInstructor;

    [SerializeField] private CollectData collectData;

    [SerializeField] private CreateDuplicates _createDuplicates;

    private QPT_DIE_Adapter _qdAdapter =  new QPT_DIE_Adapter();

    void Start()
    {
        _entrPanel.OnChangeVal += EntrPanelOnChangeVal;

        /// Show G Id Fs Trend Graphs
        //_GTrendGraph .AddData(new Vector(0, 0));
        //_YTrendGraph .AddData(new Vector(0, 0));
        //_IdTrendGraph.AddData(new Vector(0, 0));
        //_FsTrendGraph.AddData(new Vector(0, 0));
    }

    public void SetInitData()
    {
        Clear();

        _qdAdapter.initData = collectData.GetInitData();
        InitData initData = _qdAdapter.initData;
        Train train = initData.train;

        bool isDual = initData.model == ExtruderModel.Dual;
        SetPanelsVisible(isDual);

        _scenarioLabel.text = $"{train.G0:f2}\n" +
                              $"{train.Id_max:f2}\n" +
                              $"{train.Fs_max:f2}\n" +
                              $"{train.Is0:f2}\n";

        _operTime.SetTime(TimeSpan.FromMinutes(train.Time));

        if (isDual)
        {
            _GTrendGraphDual.SetYMaxLine((float)train.G0);
            _YTrendGraphDual.SetYMaxLine((float)train.Is0);
            _IdTrendGraphDual.SetYMaxLine((float)train.Id_max);
            _FsTrendGraphDual.SetYMaxLine((float)train.Fs_max);

            _GTrendGraphDualInstructor.SetYMaxLine((float)train.G0);
            _YTrendGraphDualInstructor.SetYMaxLine((float)train.Is0);
            _IdTrendGraphDualInstructor.SetYMaxLine((float)train.Id_max);
            _FsTrendGraphDualInstructor.SetYMaxLine((float)train.Fs_max);

            _shnekSpeed.text = initData.dataDual.N_.ToString();
            RecalcShnekSpeed();
            _temp1.text = initData.cylinderDual[0].T_W_k_.ToString();
            SetTempTarget1();
            _temp2.text = initData.cylinderDual[1].T_W_k_.ToString();
            SetTempTarget2();
            _temp3.text = initData.cylinderDual[2].T_W_k_.ToString();
            SetTempTarget3();
        }
        else
        {
            _GTrendGraph.SetYMaxLine((float)train.G0);
            _YTrendGraph.SetYMaxLine((float)train.Is0);
            _IdTrendGraph.SetYMaxLine((float)train.Id_max);
            _FsTrendGraph.SetYMaxLine((float)train.Fs_max);

            _GTrendGraphInstructor.SetYMaxLine((float)train.G0);
            _YTrendGraphInstructor.SetYMaxLine((float)train.Is0);
            _IdTrendGraphInstructor.SetYMaxLine((float)train.Id_max);
            _FsTrendGraphInstructor.SetYMaxLine((float)train.Fs_max);

            _shnekSpeed.text = initData.data.N_.ToString();
            RecalcShnekSpeed();
            _temp1.text = initData.cyl[0].T_W_k_.ToString();
            SetTempTarget1();
            _temp2.text = initData.cyl[1].T_W_k_.ToString();
            SetTempTarget2();
        }

        RecalcOutData();
    }

    // Показывает панель, соответствующую выбранной модели, и прячет другую.
    void SetPanelsVisible(bool isDual)
    {
        if (_singlePanel != null) _singlePanel.SetActive(!isDual);
        if (_singlePanelInstructor != null) _singlePanelInstructor.SetActive(!isDual);
        if (_dualPanel != null) _dualPanel.SetActive(isDual);
        if (_dualPanelInstructor != null) _dualPanelInstructor.SetActive(isDual);

        if (_temp3 != null) _temp3.transform.parent.gameObject.SetActive(isDual);
    }

    void Clear()
    {
        /// Show res
        _resText.text = $"{0:f2}\n" +
                        $"{0:f2}\n" +
                        $"{0:f2}\n" +
                        $"{0:f2}\n";
        _resTextInstructor.text = $"{0:f2}\n" +
                                  $"{0d:f2}\n" +
                                  $"{0:f2}\n" +
                                  $"{0:f2}\n";

        //Обучаемый
        _GTrendGraph.Clear();
        _IdTrendGraph.Clear();
        _FsTrendGraph.Clear();
        _YTrendGraph.Clear();

        _XGraph.Clear();
        _PGraph.Clear();
        _TGraph.Clear();
        _WorkDotGraph.Clear();
        _XPTTabel.Clear();

        _logTabel.Clear();

        //Инструктор
        _GTrendGraphInstructor.Clear();
        _IdTrendGraphInstructor.Clear();
        _FsTrendGraphInstructor.Clear();
        _YTrendGraphInstructor.Clear();

        _XGraphInstructor.Clear();
        _PGraphInstructor.Clear();
        _TGraphInstructor.Clear();
        _WorkDotGraphInstructor.Clear();

        _logTabelInstructor.Clear();

        /// Dual - res
        _resTextDual.text = $"{0:f2}\n" +
                            $"{0:f2}\n" +
                            $"{0:f2}\n" +
                            $"{0:f2}\n";
        _resTextDualInstructor.text = $"{0:f2}\n" +
                                      $"{0d:f2}\n" +
                                      $"{0:f2}\n" +
                                      $"{0:f2}\n";

        //Dual - Обучаемый
        _GTrendGraphDual.Clear();
        _IdTrendGraphDual.Clear();
        _FsTrendGraphDual.Clear();
        _YTrendGraphDual.Clear();

        _PGraphDual.Clear();
        _TGraphDual.Clear();
        _XPTTabelDual.Clear();
        _XPT_aTabelDual.Clear();

        _logTabelDual.Clear();

        //Dual - Инструктор
        _GTrendGraphDualInstructor.Clear();
        _IdTrendGraphDualInstructor.Clear();
        _FsTrendGraphDualInstructor.Clear();
        _YTrendGraphDualInstructor.Clear();

        _PGraphDualInstructor.Clear();
        _TGraphDualInstructor.Clear();
        _XPTTabelDualInstructor.Clear();
        _XPT_aTabelDualInstructor.Clear();

        _logTabelDualInstructor.Clear();
    }

    void EntrPanelOnChangeVal(TMP_Text valText)
    {
        if (valText == _shnekSpeed)
        {
            RecalcShnekSpeed();
            RecalcOutData();
        }
        else if (valText == _voronkaSpeed)
        {
            _voronkaSpeedTarget.text = _voronkaSpeed.text;
            RecalcVoronkaFr();
        }
        else if (valText == _voronkaFr)
        {
            RecalcVoronkaSpeed();
        }
        else if (valText == _knifeSpeed)
        {
            _knifeSpeedTarget.text = _knifeSpeed.text;
            RecalcKnifeFr();
        }
        else if (valText == _knifeFr)
        {
            RecalcKnifeSpeed();
        }
        else if (valText == _temp1)
        {
            SetTempTarget1();
            RecalcOutData();
        }
        else if (valText == _temp2)
        {
            SetTempTarget2();
            RecalcOutData();
        }
        else if (valText == _temp3)
        {
            SetTempTarget3();
            RecalcOutData();
        }
    }

    //////////////// RECALC OUT DATA ////////////////
    void RecalcOutData()
    {
        if (_qdAdapter.initData.model == ExtruderModel.Dual)
        {
            RecalcOutDataDualAsync();
        }
        else
        {
            RecalcOutDataSingleAsync();
        }
    }

    async void RecalcOutDataSingleAsync()
    {
        _qdAdapter.initData.data.N_       = Math.Max(double.Parse(_shnekSpeed.text), 0.1);
        _qdAdapter.initData.cyl[0].T_W_k_ = double.Parse(_temp1.text);
        _qdAdapter.initData.cyl[1].T_W_k_ = double.Parse(_temp2.text);
        await Task.Run(() => _qdAdapter.init());

        Train train = _qdAdapter.initData.train;
        float g = G();
        float dG = g / (float)Math.Max(train.G0, 1e-5) * 100 - 100;
        float id = Id();
        float dId = Math.Max(id / (float)train.Id_max * 100 - 100, 0);
        float fs = Fs();
        float dFs = Math.Max(fs / (float)train.Fs_max * 100 - 100, 0);
        float y = Dmix();
        float dY = y / (float)Math.Max(train.Is0, 1e-5) * 100 - 100;

        _createDuplicates.setTransitionValue(id);
        Debug.Log(id);

        /// Show res
        _resText.text = $"{g:f2}\n" +
                        $"{id:f2}\n" +
                        $"{fs:f2}\n" +
                        $"{y:f2}\n";
        _resTextInstructor.text = $"{g:f2}\n" +
                                  $"{id:f2}\n" +
                                  $"{fs:f2}\n" +
                                  $"{y:f2}\n";

        /// Show X P T WorkDot Graphs
        _XGraph.SetData(_qdAdapter.qpt.XZ.Last());
        _PGraph.SetData(_qdAdapter.qpt.PZ.Last());
        _TGraph.SetData(_qdAdapter.qpt.TZ.Last());

        _XGraphInstructor.SetData(_qdAdapter.qpt.XZ.Last());
        _PGraphInstructor.SetData(_qdAdapter.qpt.PZ.Last());
        _TGraphInstructor.SetData(_qdAdapter.qpt.TZ.Last());

        List<Vector> mt = new List<Vector>();
        List<Vector> tout = new List<Vector>();
        for (int i = 0; i <= _qdAdapter.die.Res.n_Q; i++)
        {
            mt.Add(  new((float)(_qdAdapter.die.M_Q[i] * 1e2), (float)_qdAdapter.die.Res.MP[i]));
            tout.Add(new((float)(_qdAdapter.die.M_Q[i] * 1e2), (float)_qdAdapter.die.M_P[i]));
        }
        _WorkDotGraph.SetData(mt, tout);
        _WorkDotGraphInstructor.SetData(mt, tout);

        /// Show X P T Table
        _XPTTabel.SetData(_qdAdapter.qpt.ZXPT.Last());

        /// Show G Id Fs Y Trend Graphs
        _GTrendGraph .AddData(new Vector((float)_operTime.GetMin(), g));
        _IdTrendGraph.AddData(new Vector((float)_operTime.GetMin(), id));
        _FsTrendGraph.AddData(new Vector((float)_operTime.GetMin(), fs));
        _YTrendGraph .AddData(new Vector((float)_operTime.GetMin(), y));

        _GTrendGraphInstructor .AddData(new Vector((float)_operTime.GetMin(), g));
        _IdTrendGraphInstructor.AddData(new Vector((float)_operTime.GetMin(), id));
        _FsTrendGraphInstructor.AddData(new Vector((float)_operTime.GetMin(), fs));
        _YTrendGraphInstructor .AddData(new Vector((float)_operTime.GetMin(), y));

        /// Log 
        _logTabel.AddData(new List<object>
        {
            _operTime.GetTimeStr(),
            _qdAdapter.initData.data.N_,
            _qdAdapter.initData.cyl[0].T_W_k_,
            _qdAdapter.initData.cyl[1].T_W_k_,
            g,
            dG,
            id,
            dId,
            fs,
            dFs,
            y,
            dY
        });

        _logTabelInstructor.AddData(new List<object>
        {
            _operTime.GetTimeStr(),
            _qdAdapter.initData.data.N_,
            _qdAdapter.initData.cyl[0].T_W_k_,
            _qdAdapter.initData.cyl[1].T_W_k_,
            g,
            dG,
            id,
            dId,
            fs,
            dFs,
            y,
            dY
        });
    }

    async void RecalcOutDataDualAsync()
    {
        _qdAdapter.initData.dataDual.N_          = Math.Max(double.Parse(_shnekSpeed.text), 0.1);
        _qdAdapter.initData.cylinderDual[0].T_W_k_ = double.Parse(_temp1.text);
        _qdAdapter.initData.cylinderDual[1].T_W_k_ = double.Parse(_temp2.text);
        _qdAdapter.initData.cylinderDual[2].T_W_k_ = double.Parse(_temp3.text);
        await Task.Run(() => _qdAdapter.initDual());

        Train train = _qdAdapter.initData.train;
        float g = GDual();
        float dG = g / (float)Math.Max(train.G0, 1e-5) * 100 - 100;
        float id = IdDual();
        float dId = Math.Max(id / (float)train.Id_max * 100 - 100, 0);
        float fs = FsDual();
        float dFs = Math.Max(fs / (float)train.Fs_max * 100 - 100, 0);
        float y = DmixDual();
        float dY = y / (float)Math.Max(train.Is0, 1e-5) * 100 - 100;

        _createDuplicates.setTransitionValue(id);

        /// Show res
        _resTextDual.text = $"{g:f2}\n" +
                            $"{id:f2}\n" +
                            $"{fs:f2}\n" +
                            $"{y:f2}\n";
        _resTextDualInstructor.text = $"{g:f2}\n" +
                                      $"{id:f2}\n" +
                                      $"{fs:f2}\n" +
                                      $"{y:f2}\n";

        /// Show P T Graphs (для шнека 1 и шнека 2)
        _PGraphDual.SetData(_qdAdapter.qptDual.PZ.Last(), _qdAdapter.qptDual.PZ_a.Last());
        _TGraphDual.SetData(_qdAdapter.qptDual.TZ.Last(), _qdAdapter.qptDual.TZ_a.Last());

        _PGraphDualInstructor.SetData(_qdAdapter.qptDual.PZ.Last(), _qdAdapter.qptDual.PZ_a.Last());
        _TGraphDualInstructor.SetData(_qdAdapter.qptDual.TZ.Last(), _qdAdapter.qptDual.TZ_a.Last());

        /// Show X P T Table (для шнека 1 и шнека 2)
        _XPTTabelDual.SetData(_qdAdapter.qptDual.ZXPT.Last());
        _XPT_aTabelDual.SetData(_qdAdapter.qptDual.ZXPT_a.Last());

        _XPTTabelDualInstructor.SetData(_qdAdapter.qptDual.ZXPT.Last());
        _XPT_aTabelDualInstructor.SetData(_qdAdapter.qptDual.ZXPT_a.Last());

        /// Show G Id Fs Y Trend Graphs
        _GTrendGraphDual .AddData(new Vector((float)_operTime.GetMin(), g));
        _IdTrendGraphDual.AddData(new Vector((float)_operTime.GetMin(), id));
        _FsTrendGraphDual.AddData(new Vector((float)_operTime.GetMin(), fs));
        _YTrendGraphDual .AddData(new Vector((float)_operTime.GetMin(), y));

        _GTrendGraphDualInstructor .AddData(new Vector((float)_operTime.GetMin(), g));
        _IdTrendGraphDualInstructor.AddData(new Vector((float)_operTime.GetMin(), id));
        _FsTrendGraphDualInstructor.AddData(new Vector((float)_operTime.GetMin(), fs));
        _YTrendGraphDualInstructor .AddData(new Vector((float)_operTime.GetMin(), y));

        /// Log
        _logTabelDual.AddData(new List<object>
        {
            _operTime.GetTimeStr(),
            _qdAdapter.initData.dataDual.N_,
            _qdAdapter.initData.cylinderDual[0].T_W_k_,
            _qdAdapter.initData.cylinderDual[1].T_W_k_,
            _qdAdapter.initData.cylinderDual[2].T_W_k_,
            g,
            dG,
            id,
            dId,
            fs,
            dFs,
            y,
            dY
        });

        _logTabelDualInstructor.AddData(new List<object>
        {
            _operTime.GetTimeStr(),
            _qdAdapter.initData.dataDual.N_,
            _qdAdapter.initData.cylinderDual[0].T_W_k_,
            _qdAdapter.initData.cylinderDual[1].T_W_k_,
            _qdAdapter.initData.cylinderDual[2].T_W_k_,
            g,
            dG,
            id,
            dId,
            fs,
            dFs,
            y,
            dY
        });
    }

    private float G()
    {
        return (float)(_qdAdapter.die.RES_f.Q_fin * (_qdAdapter.qpt.DataRec.Ro_* 3600 * 1e-6));
    }
    
    private float Id()
    {
        const double T_kel = 273.15;
        const double R = 8.31;

        //const double t_d = 600;
        //const double E_d = 165000;
        //const double T_d = 220 + T_kel;
        const double t_d = 300;
        const double E_d = 80000;
        const double T_d = 200 + T_kel;

        double T_ext = _qdAdapter.qpt.T + T_kel;
        return (float)(t_av() / t_d * Math.Exp(E_d / (R * T_ext * T_d) * (T_ext - T_d)) * 100);
    }

    private float Fs()
    {
        double Q = _qdAdapter.die.RES_f.Q_fin * 1e3;
        double Q_s = _qdAdapter.initData.sect.Last().H_fin * _qdAdapter.qpt.X_PL * 1e3 * _qdAdapter.qpt.v_SZ * 1e3;

        if (Q == 0) return 0;

        return (float)(Q_s / Q);
    }

    private float Dmix()
    {
        return (float)(_qdAdapter.qpt.Sav * t_av());
    }

    private double t_av()
    {
        double V = 0;
        foreach (var sect in _qdAdapter.initData.sect)
        {
            if (sect.S_Type == 1)
            {
                double z = sect.L_sect / _qdAdapter.qpt.sn;
                V += z * _qdAdapter.qpt.W * 1e3 * ((sect.H_st + sect.H_fin) / 2);
            }
            else
            {
                double D = (sect.D_st + sect.D_fin) / 2;
                double d = sect.D_st - 2 * sect.H_st;
                V += 0.25 * Math.PI * sect.L_sect * (D*D - d*d);
            }
        }

        return V / (_qdAdapter.die.RES_f.Q_fin * 1e3);
    }

    //////////////// RECALC OUT DATA (Dual) ////////////////
    // G/Id переносят формулы одношнековой модели по прямой физической аналогии:
    // RectanDual.Q_f — объёмный расход через головку (аналог die.RES_f.Q_fin),
    // QPTDual.T — температура расплава на выходе из шнеков (аналог qpt.T).
    private float GDual()
    {
        return (float)(_qdAdapter.rectanDual.Q_f * (_qdAdapter.qptDual.DataRec.Ro_ * 3600 * 1e-6));
    }

    private float IdDual()
    {
        const double T_kel = 273.15;
        const double R = 8.31;

        const double t_d = 300;
        const double E_d = 80000;
        const double T_d = 200 + T_kel;

        double T_ext = _qdAdapter.qptDual.T + T_kel;
        return (float)(t_avDual() / t_d * Math.Exp(E_d / (R * T_ext * T_d) * (T_ext - T_d)) * 100);
    }

    // TODO(dual): грубое приближение. В одношнековой модели Fs строится на X_PL
    // (степень заполнения зоны дозирования расплавом) и v_SZ (скорость в этой
    // зоне), которых QPTDual не вычисляет. Здесь вместо X_PL*v_SZ используется
    // W (ширина канала последней секции), оставшееся в QPTDual от последнего
    // просчитанного шага интегрирования — то есть подача оценивается без учёта
    // степени заполнения канала расплавом. Заменить точной формулой, когда она
    // будет определена для двухшнекового процесса.
    private float FsDual()
    {
        double Q = _qdAdapter.rectanDual.Q_f * 1e3;
        if (Q == 0) return 0;

        var lastSect = _qdAdapter.initData.sectDual.Last();
        double Q_s = lastSect.H_fin * _qdAdapter.qptDual.W * 1e3 * _qdAdapter.qptDual.u * 1e3;

        return (float)(Q_s / Q);
    }

    // TODO(dual): грубое приближение. Dmix в одношнековой модели использует Sav
    // (среднюю скорость сдвига в канале шнека), которую QPTDual не вычисляет и
    // не хранит. Здесь используется среднее время пребывания расплава в обоих
    // каналах шнеков как единственная доступная оценка интенсивности смешения.
    // Заменить точной формулой, когда появится аналог Sav для двухшнекового процесса.
    private float DmixDual()
    {
        return (float)t_avDual();
    }

    // Повторяет структуру t_av() из одношнековой модели: объём канала делится
    // на расход через головку. sn/W берутся из QPTDual так же, как t_av()
    // берёт их из QPT — это те же поля экземпляра, оставшиеся от последней
    // просчитанной секции, а не точная геометрия каждой секции в отдельности.
    // Суммируются секции обоих шнеков (sectDual и sect_aDual), так как оба
    // формируют общий поток, поступающий в RectanDual (Q_total = 2 * Q_M).
    private double t_avDual()
    {
        double V = 0;
        foreach (var sect in _qdAdapter.initData.sectDual)
        {
            if (sect.S_Type == 1)
            {
                double zLen = sect.L_sect / _qdAdapter.qptDual.sn;
                V += zLen * _qdAdapter.qptDual.W * 1e3 * ((sect.H_st + sect.H_fin) / 2);
            }
            else
            {
                double D = (sect.D_st + sect.D_fin) / 2;
                double d = sect.D_st - 2 * sect.H_st;
                V += 0.25 * Math.PI * sect.L_sect * (D * D - d * d);
            }
        }
        foreach (var sect in _qdAdapter.initData.sect_aDual)
        {
            if (sect.S_Type == 1)
            {
                double zLen = sect.L_sect / _qdAdapter.qptDual.sn;
                V += zLen * _qdAdapter.qptDual.W * 1e3 * ((sect.H_st + sect.H_fin) / 2);
            }
            else
            {
                double D = (sect.D_st + sect.D_fin) / 2;
                double d = sect.D_st - 2 * sect.H_st;
                V += 0.25 * Math.PI * sect.L_sect * (D * D - d * d);
            }
        }

        return V / (_qdAdapter.rectanDual.Q_f * 1e3);
    }

    //////////////// RECALC PARAMS ON CONTROL PANEL //////////////// 
    void RecalcShnekSpeed()
    {
        _shnekSpeedTarget.text = _shnekSpeed.text;

        RecalcVoronkaSpeed();
        RecalcKnifeSpeed();
    }

    void RecalcVoronkaSpeed()
    {
        double N = double.Parse(_shnekSpeed.text);
        double fr = double.Parse(_voronkaFr.text);
        _voronkaSpeed.text = string.Format("{0:f}", (1 + fr / 100) * N);
        _voronkaSpeedTarget.text = _voronkaSpeed.text;
    }

    void RecalcVoronkaFr()
    {
        double N = double.Parse(_shnekSpeed.text);
        double Nh = double.Parse(_voronkaSpeed.text);
        _voronkaFr.text = string.Format("{0:f}", (Nh / N - 1) * 100);
    }

    void RecalcKnifeSpeed()
    {
        double N = double.Parse(_shnekSpeed.text);
        double fr = double.Parse(_knifeFr.text);
        _knifeSpeed.text = string.Format("{0:f}", (1 + fr / 100) * N);
        _knifeSpeedTarget.text = _knifeSpeed.text;
    }

    void RecalcKnifeFr()
    {
        double N = double.Parse(_shnekSpeed.text);
        double Nh = double.Parse(_knifeSpeed.text);
        _knifeFr.text = string.Format("{0:f}", (Nh / N - 1) * 100);
    }

    void SetTempTarget1()
    {
        _tempTarget1.text = _temp1.text;
        _tempDopLabel11.text = _temp1.text;
        _tempDopLabel12.text = _temp1.text;
        _measTemp1.SetValue(_temp1.text);
    }

    void SetTempTarget2()
    {
        _tempTarget2.text = _temp2.text;
        _tempDopLabel21.text = _temp2.text;
        _tempDopLabel22.text = _temp2.text;
        _measTemp2.SetValue(_temp2.text);
    }

    void SetTempTarget3()
    {
        _tempTarget3.text = _temp3.text;
        _tempDopLabel31.text = _temp3.text;
        _tempDopLabel32.text = _temp3.text;
        _measTemp3.SetValue(_temp3.text);
    }
}
