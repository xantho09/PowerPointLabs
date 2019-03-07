﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Office.Interop.PowerPoint;

using PowerPointLabs.ELearningLab.ELearningWorkspace.Model;
using PowerPointLabs.Models;

namespace PowerPointLabs.ELearningLab.ELearningWorkspace.ModelFactory
{
    public class CustomItemFactory : AbstractItemFactory
    {
        public CustomItemFactory(IEnumerable<Effect> effects, PowerPointSlide slide):base(effects, slide)
        { }
        protected override ClickItem CreateBlock()
        {
            if (effects.Count() == 0)
            {
                return null;
            }
            ObservableCollection<CustomSubItem> customItems = new ObservableCollection<CustomSubItem>();
            foreach (Effect effect in effects)
            {
                customItems.Add(new CustomSubItem(effect.Shape, effect));
            }
            return new CustomClickItem(customItems);
        }
    }
}