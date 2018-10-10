// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace VotingWeb.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Fabric;
    using System.Fabric.Query;
    using System.Threading.Tasks;
    using System.Fabric.Description;

    public class HomeController : Controller
    {
        private readonly FabricClient fabricClient;
        private readonly StatelessServiceContext serviceContext;

        public HomeController(StatelessServiceContext serviceContext, FabricClient fabricClient)
        {
            this.fabricClient = fabricClient;
            this.serviceContext = serviceContext;
        }

        public IActionResult Poll(string poll)
        {
            Uri serviceName = VotingWeb.GetVotingDataServiceName(this.serviceContext, poll);
            
            this.ViewData["Poll"] = serviceName;

            return this.View();
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Error()
        {
            return this.View();
        }
    }
}