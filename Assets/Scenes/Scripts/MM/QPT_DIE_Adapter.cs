using Program;
using System;
using UnityEngine;

namespace Extruder
{
    internal class QPT_DIE_Adapter
    {
        public void init(InitData id)
        {
            try
            {
                qpt = new QPT();
                die = new DIE();

                qpt.init(id);

                id.res = qpt.Res;
                die.init(id);

                id.dop.Q = die.RES_f.Q_fin * 1e-6;
                qpt.init(id, true);
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void init()
        {
            init(initData);
        }

        public void initDual(InitData id)
        {
            try
            {
                qptDual = new QPTDual();
                qptDual.init(id);
                id.resDual = qptDual.Res;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void initDual()
        {
            initDual(initData);
        }

        public InitData initData;

        public QPT qpt = new QPT();
        public DIE die = new DIE();

        public QPTDual qptDual = new QPTDual();

    }
}
