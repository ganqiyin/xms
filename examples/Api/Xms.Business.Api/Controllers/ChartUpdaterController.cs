﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xms.Api.Core.Controller;
using Xms.Business.Api.Models;
using Xms.Business.DataAnalyse.Visualization;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Mvc;

namespace Xms.Business.Api.Controllers
{
    [Route("{org}/api/chart/update")]
    [ApiController]
    public class ChartUpdaterController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IChartCreater _chartCreater;
        private readonly IChartUpdater _chartUpdater;
        private readonly IChartFinder _chartFinder;
        private readonly IChartDeleter _chartDeleter;
        public ChartUpdaterController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            , IChartCreater chartCreater
            , IChartUpdater chartUpdater
            , IChartFinder chartFinder
            , IChartDeleter chartDeleter)
            : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _chartCreater = chartCreater;
            _chartUpdater = chartUpdater;
            _chartFinder = chartFinder;
            _chartDeleter = chartDeleter;
        }

        [HttpPost]        
        [Description("图表信息保存")]
        public IActionResult Post(EditChartModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _chartFinder.FindById(model.ChartId);
                model.CopyTo(entity);
                entity.OrganizationId = CurrentUser.OrganizationId;
                _chartUpdater.Update(entity);
                return UpdateSuccess(new { id = entity.ChartId });
            }
            var msg = GetModelErrors(ModelState);
            return UpdateFailure(msg);
        }

        [Description("设置图表可用状态")]
        [HttpPost("setstate")]
        public IActionResult SetChartState(SetChartStateModel model)
        {
            return _chartUpdater.UpdateState(model.RecordId, model.IsEnabled).UpdateResult(T);
        }

    }
}
