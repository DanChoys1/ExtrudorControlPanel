﻿using Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scenes.Scripts.UI
{
    public class DatabaseCollectData
    {
        ExtrusionContext ec = new();

        private static readonly string DesignName = "Designation";
        private static readonly string ConfName = "Configuration";

        private Dictionary<string, List<Dictionary<string, object>>> datas = new();

        public DatabaseCollectData()
        {
            //var screwConfigs = ec.ScrewPossibleСonfigurations
            //    .Include(s => s.IdScrewNavigation)
            //        .ThenInclude(s => s.ScrewParametrValues)
            //        .ThenInclude(s => s.IdParametrNavigation)
            //    .Include(s => s.IdConfigurationNavigation)
            //        .ThenInclude(s => s.ScrewElementInСonfigurations)
            //        .ThenInclude(s =Waiting for pgAdmin 4> s.IdElementNavigation)
            //        .ThenInclude(sp => sp.ScrewElementParametrValues)
            //        .ThenInclude(spv => spv.IdParametrNavigation).ToList<IPossibleConfig>();


            //var s = ec.BarrelPossibleСonfigurations
            //    .Include(s => s.IdBodyNavigation)
            //        .ThenInclude(s => s.BarrelParametrValues)
            //        .ThenInclude(s => s.IdParametrNavigation)
            //    .Include(s => s.IdConfigurationNavigation)
            //        .ThenInclude(s => s.BarrelSectionInСonfigurations)
            //        .ThenInclude(s => s.IdElementNavigation)
            //        .ThenInclude(sp => sp.BarrelSectionParametrValues)зы
            //        .ThenInclude(spv => spv.IdParametrNavigation).ToList<IPossibleConfig>();

            //var b = ec.BarrelSections
            //    .Include(s => s.BarrelSectionParametrValues)
            //        .ThenInclude(s => s.IdParametrNavigation).ToList<IElement>();

            ec.Barrels.Load();
            ec.BarrelParametrs.Load();
            ec.BarrelParametrValues.Load();
            ec.BarrelPossibleСonfigurations.Load();
            ec.BarrelSections.Load();
            ec.BarrelSectionInСonfigurations.Load();
            ec.BarrelSectionParametrs.Load();
            ec.BarrelSectionParametrValues.Load();
            ec.BarrelСonfigurations.Load();

            ec.Screws.Load();
            ec.ScrewParametrs.Load();
            ec.ScrewParametrValues.Load();
            ec.ScrewPossibleСonfigurations.Load();
            ec.ScrewElements.Load();
            ec.ScrewElementInСonfigurations.Load();
            ec.ScrewElementParametrs.Load();
            ec.ScrewElementParametrValues.Load();
            ec.ScrewСonfigurations.Load();

            ec.Dies.Load();
            ec.DieElements.Load();
            ec.DieElementInСonfigurations.Load();
            ec.DieElementParametrs.Load();
            ec.DieElementParametrValues.Load();

            DropdownDatasEvents.AddDataEvent += AddData;
            DropdownDatasEvents.SaveDataEvent += SaveData;
            DropdownDatasEvents.RemoveDataEvent += RemoveData;
        }

        public Dictionary<string, List<IValue>> GetDatas(string name)
        {
            //if (database.ContainsKey(name))
            //    return database[name];

            if (name == "BarrelSection")
            {
                return GetElemParametrs(ec.BarrelSections);
            }
            else if (name == "BarrelConfiguration")
            {
                return GetConfParametrs(ec.BarrelPossibleСonfigurations);
            }
            else if (name == "ScrewElement")
            {
                return GetElemParametrs(ec.ScrewElements);
            }
            else if (name == "ScrewConfiguration")
            {
                return GetConfParametrs(ec.ScrewPossibleСonfigurations);
            }
            else if (name == "DieElement")
            {
                return GetElemParametrs(ec.DieElements);
            }
            else if (name == "DieConfiguration")
            {
                return GetConfParametrs(ec.Dies);
            }

            return null;
        }

        void AddData(object _, string nameDataGroup, Dictionary<string, object> dataFields)
        {
            if (nameDataGroup == "BarrelSection")
            {
                InitElement(dataFields, ec.BarrelSections, ec.BarrelSectionParametrs);
            }
            else if (nameDataGroup == "BarrelConfiguration")
            {
                InitConfig(dataFields, ec.BarrelPossibleСonfigurations, ec.BarrelСonfigurations, ec.BarrelSections, ec.Barrels, ec.BarrelParametrs);
            }
            else if (nameDataGroup == "ScrewElement")
            {
                InitElement(dataFields, ec.ScrewElements, ec.ScrewElementParametrs);
            }
            else if (nameDataGroup == "ScrewConfiguration")
            {
                InitConfig(dataFields, ec.ScrewPossibleСonfigurations, ec.ScrewСonfigurations, ec.ScrewElements, ec.Screws, ec.ScrewParametrs);
            }
            else if (nameDataGroup == "DieElement")
            {
                InitElement(dataFields, ec.DieElements, ec.DieElementParametrs);
            }
            else if (nameDataGroup == "DieConfiguration")
            {
                InitConfig(dataFields, ec.Dies, ec.DieElements);
            }

            ec.SaveChanges();
        }

        void SaveData(object _, string nameDataGroup, string oldDataName, Dictionary<string, object> dataFields)
        {
            if (nameDataGroup == "BarrelSection")
            {
                SaveElement(dataFields, oldDataName, ec.BarrelSections);
            }
            else if (nameDataGroup == "BarrelConfiguration")
            {
                SaveConfiguration(dataFields, oldDataName, ec.BarrelPossibleСonfigurations, ec.BarrelSections);
            }
            else if (nameDataGroup == "ScrewElement")
            {
                SaveElement(dataFields, oldDataName, ec.ScrewElements);
            }
            else if (nameDataGroup == "ScrewConfiguration")
            {
                SaveConfiguration(dataFields, oldDataName, ec.ScrewPossibleСonfigurations, ec.ScrewElements);
            }
            else if (nameDataGroup == "DieElement")
            {
                SaveElement(dataFields, oldDataName, ec.DieElements);
            }
            else if (nameDataGroup == "DieConfiguration")
            {
                SaveConfiguration(dataFields, oldDataName, ec.Dies, ec.DieElements);
            }

            ec.SaveChanges();
        }

        void RemoveData(object _, string nameDataGroup, string dataName)
        {
            if (nameDataGroup == "BarrelSection")
            {
                RemoveElement(dataName, ec.BarrelSections);
            }
            else if (nameDataGroup == "BarrelConfiguration")
            {
                RemoveConfiguration(dataName, ec.BarrelPossibleСonfigurations, ec.Barrels, ec.BarrelСonfigurations);
            }
            else if (nameDataGroup == "ScrewElement")
            {
                RemoveElement(dataName, ec.ScrewElements);
            }
            else if (nameDataGroup == "ScrewConfiguration")
            {
                RemoveConfiguration(dataName, ec.ScrewPossibleСonfigurations, ec.Screws, ec.ScrewСonfigurations);
            }
            else if(nameDataGroup == "DieElement")
            {
                RemoveElement(dataName, ec.DieElements);
            }
            else if (nameDataGroup == "DieConfiguration")
            {
                RemoveConfiguration(dataName, ec.Dies);
            }

            ec.SaveChanges();
        }

        private List<IValue> GetParametrs(IElement element)
        {
            var pars = new List<IValue> { element };
            foreach (var pval in element.Parametrs)
            {
                pars.Add(pval);
            }

            return pars;
        }

        private Dictionary<string, List<IValue>> GetElemParametrs<T>(DbSet<T> element) where T : class, IElement, new()
        {
            var valsConfs = new Dictionary<string, List<IValue>>();
            foreach (var e in element)
            {
                valsConfs[e.Name] = GetParametrs(e);
            }

            return valsConfs;
        }

        private Dictionary<string, List<IValue>> GetConfParametrs<T>(DbSet<T> possConf) where T : class, IPossibleConfig, new()
        {
            var valsConfs = new Dictionary<string, List<IValue>>();
            foreach (var e in possConf)
            {
                List<IValue> pars = new();
                if (e.Element != null)
                    pars = GetParametrs(e.Element);
                pars.Add(e.Config);

                valsConfs[e.Name] = pars;
            }

            return valsConfs;
        }

        void RemoveElement<E>(string dataName, DbSet<E> els) where E : class, IElement, new()
        {
            foreach (var e in els)
            {
                if (e.Name == dataName)
                {
                    els.Remove(e);
                    break;
                }
            }
        }

        void RemoveConfiguration<P, C, E>(string dataName, DbSet<P> posConfs, DbSet<E> els, DbSet<C> confs) where P : class, IPossibleConfig, new() 
                                                                                                            where C : class, IConfig, new()
                                                                                                            where E : class, IElement, new()
        {
            foreach (var b in posConfs)
            {
                if (b.Name == dataName)
                {
                    posConfs.Remove(b);
                    els.Remove(b.Element as E);
                    confs.Remove(b.Config as C);
                    break;
                }
            }
        }

        void RemoveConfiguration<P>(string dataName, DbSet<P> confs) where P : class, IPossibleConfig, new()
        {
            foreach (var b in confs)
            {
                if (b.Name == dataName)
                {
                    confs.Remove(b);
                    break;
                }
            }
        }

        void SaveElement<E>(Dictionary<string, object> dataFields, string oldDataName, DbSet<E> els) where E : class, IElement, new()
        {
            foreach (var b in els)
            {
                if (b.Name == oldDataName)
                {
                    b.Name = dataFields[DesignName].ToString();
                    foreach (var p in b.Parametrs)
                    {
                        var v = dataFields[p.ValName];
                        if (v is int)
                            p.Value = (int)v;
                        if (v is double)
                            p.Value = (double)v;
                    }
                    break;
                }
            }
        }

        void SaveConfiguration<P, EC>(Dictionary<string, object> dataFields, string oldDataName, DbSet<P> conf, DbSet<EC> confElem) where P : class, IPossibleConfig, new()
                                                                                                                                    where EC : class, IElement, new()
        {
            P b = null;
            foreach (var d in conf)
            {
                if (d.Name == oldDataName)
                {
                    b = d;
                    break;
                }
            }

            if (b == null)
            {
                Debug.Log("Не была найдена возможная конфигурация для сохранения");
                return;
            }

            IElement e = b.Element;
            if (e != null)
            {
                foreach (var p in e.Parametrs)
                {
                    if (dataFields.ContainsKey(p.ValName))
                    {
                        var v = dataFields[p.ValName];
                        if (v is int)
                            p.Value = (int)v;
                        if (v is double)
                            p.Value = (double)v;
                    }
                    else
                    {
                        Debug.Log($"Нет парметра {p.ValName}");
                    }
                }
            }

            IConfig c = b.Config;
            foreach (var data in dataFields)
            {
                if (data.Value is MyList<string>)
                {
                    c.ConfigElements = CreateConfigElements(data.Value as MyList<string>, confElem);
                    break;
                }
            }

            b.Name = dataFields[DesignName].ToString();
        }

        void InitConfig<P, C, EC, E, T>(Dictionary<string, object> datas, DbSet<P> conf, DbSet<C> _, DbSet<EC> elemsConf, DbSet<E> elems, DbSet<T> parms)
                                                                                                            where P : class, IPossibleConfig, new() where C : class, IConfig, new()
                                                                                                            where EC : class, IElement, new() where E : class, IElement, new()
                                                                                                            where T : class, IPar
        {
            var elem = CreateElement<E, T>(datas, parms);
            elems.Add(elem);
            conf.Add(CreateConfig<P, C, EC, E>(datas, elemsConf, elem));
        }
        void InitConfig<P, EC>(Dictionary<string, object> datas, DbSet<P> conf, DbSet<EC> elemsConf) where P : class, IPossibleConfig, IConfig, new()
                                                                                                        where EC : class, IElement, new()
        {
            conf.Add(CreateConfig<P,EC>(datas, elemsConf));
        }

        void InitElement<E,T>(Dictionary<string, object> datas, DbSet<E> elems, DbSet<T> parms) where E : class, IElement, new() where T : class, IPar
        {
            elems.Add(CreateElement<E, T>(datas, parms));
        }

        P CreateConfig<P, C, EC, E>(Dictionary<string, object> datas, DbSet<EC> elemsConf, E elem) where P : class, IPossibleConfig, new()
                                                                                                    where C : class, IConfig, new()
                                                                                                    where EC : class, IElement, new()
                                                                                                    where E : class, IElement, new()
        {
            var confElems = new List<IConfigElement>();
            foreach (var field in datas)
            {
                if (field.Value is MyList<string>)
                {
                    confElems = CreateConfigElements(field.Value as MyList<string>, elemsConf);
                }
            }

            var conf = new C() { Name = ConfName, ConfigElements = confElems };

            return new P() { Config = conf, Element = elem };
        }

        P CreateConfig<P, EC>(Dictionary<string, object> datas, DbSet<EC> elemsConf) where P : class, IPossibleConfig, IConfig, new()
                                                                                            where EC : class, IElement, new()
        {
            var confElems = new List<IConfigElement>();
            foreach (var field in datas)
            {
                if (field.Value is MyList<string>)
                {
                    confElems = CreateConfigElements(field.Value as MyList<string>, elemsConf);
                }
            }

            IConfig newConf = new P() { ConfigElements = confElems };
            newConf.Name = datas[DesignName].ToString();
            return newConf as P;
        }

        List<IConfigElement> CreateConfigElements<E>(MyList<string> conf, DbSet<E> elements) where E : class, IElement
        {
            var newConf = new List<IConfigElement>();
            int i = 0;
            foreach (var name in conf)
            {
                E el = elements.FirstOrDefault(n => n.Name == name);
                if (el != null)
                {
                    newConf.Add(new ConfigElementInstance() { Number = ++i, IdElement = el.Id });
                    if (i == conf.Count) break;
                }
            }

            if (i != conf.Count)
            {
                Debug.Log($"Не было найдено элементов конфигурации {conf.Count - i} шт");
            }

            return newConf;
        }

        E CreateElement<E, T>(Dictionary<string, object> datas, DbSet<T> parms) where E : class, IElement, new()
                                                                                where T : class, IPar
        {
            E element = new E();
            SetElementValues(element, datas, parms);

            return element;
        }

        void SetElementValues<E, T>(E element, Dictionary<string, object> datas, DbSet<T> parms) where E : class, IElement, new()
                                                                                                    where T : class, IPar
        {
            var parVals = new List<IParametr>();
            foreach (var field in datas)
            {
                if (field.Value is double)
                {
                    parVals.Add(GetPar(parms, field.Key, (double)field.Value));
                }
                else if (field.Value is int)
                {
                    parVals.Add(GetPar(parms, field.Key, (int)field.Value));
                }
            }

            element.Name = datas[DesignName].ToString();
            element.Parametrs = parVals;
        }

        IParametr GetPar<T>(DbSet<T> parms, string des, double val) where T : class, IPar
        {
            foreach (var p in parms)
            {
                if (p.Designation == des)
                {
                    return new ParametrInstance() { IdParametr = p.Id, Value = val };
                }
            }

            Debug.Log($"Не было найдено параметра {des}");
            return new ParametrInstance();
        }

        public List<Dictionary<string, object>> GetDatas(Dictionary<string, List<string>> needsData)
        {
            int count = int.MaxValue;
            foreach (var needsFields in needsData)
            {
                count = Math.Min(count, datas[needsFields.Key].Count);
            }

            List<Dictionary<string, object>> res = new();
            for (int i = 0; i < count; i++)
            {
                res.Add(new());
            }

            foreach (var needsFields in needsData)
            {
                for (int i = 0; i < count; ++i)
                {
                    foreach (var needsField in needsFields.Value)
                    {
                        res[i][needsField] = datas[needsFields.Key][i][needsField];
                    }
                }
            }

            return res;
        }

        void InitDatas<T>(T[] fields)
        {
            string fieldName = typeof(T[]).ToString()[6..];
            datas[fieldName] = GetFields(fields);
        }
        
        void InitDatas<T>(T fields)
        {
            string fieldName = typeof(T).ToString()[6..];
            datas[fieldName] = new() { GetFields(fields) };
        }

        List<Dictionary<string, object>> GetFields<T>(T[] data)
        {
            var vals = new List<Dictionary<string, object>>();
            foreach (var field in data)
            {
                vals.Add(GetFields(field));
            }

            return vals;
        }

        Dictionary<string, object> GetFields<T>(T data)
        {
            var vals = new Dictionary<string, object>();
            var fields = typeof(T).GetFields();
            foreach (var fild in fields)
            {
                vals[fild.Name] = fild.GetValue(data);
            }

            return vals;
        }
    }
}
