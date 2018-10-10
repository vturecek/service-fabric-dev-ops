// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace VotingWeb.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Description;
    using System.Fabric.Query;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PollsController : Controller
    {
        private readonly FabricClient fabricClient;
        private readonly StatelessServiceContext serviceContext;

        public PollsController(StatelessServiceContext context, FabricClient fabricClient)
        {
            this.fabricClient = fabricClient;
            this.serviceContext = context;
        }

        // GET: api/polls
        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            ServiceList serviceList =
               await this.fabricClient.QueryManager.GetServiceListAsync(
                   new Uri(this.serviceContext.CodePackageActivationContext.ApplicationName));

            return Json(serviceList
                .Where (x => x.ServiceName.AbsolutePath.StartsWith(VotingWeb.GetVotingDataServiceName(this.serviceContext, "").AbsolutePath))
                .Select(x =>
                    new
                    {
                        name = x.ServiceName.Segments.Last(),
                        service = x.ServiceName
                    }));
              
        }

        // PUT: api/polls/poll
        [HttpPut("{poll}")]
        public async Task<IActionResult> Put(string poll)
        {
            Uri serviceName = VotingWeb.GetVotingDataServiceName(this.serviceContext, poll);

            ServiceList serviceList =
               await this.fabricClient.QueryManager.GetServiceListAsync(
                   new Uri(this.serviceContext.CodePackageActivationContext.ApplicationName),
                   serviceName);

            if (!serviceList.Any())
            {
                await this.fabricClient.ServiceManager.CreateServiceAsync(
                    new StatefulServiceDescription()
                    {
                        ApplicationName = new Uri(this.serviceContext.CodePackageActivationContext.ApplicationName),
                        HasPersistedState = true,
                        MinReplicaSetSize = 1,
                        TargetReplicaSetSize = 1,
                        ServiceName = serviceName,
                        ServiceTypeName = "VotingDataType",
                        ServicePackageActivationMode = ServicePackageActivationMode.ExclusiveProcess,
                        PartitionSchemeDescription = new UniformInt64RangePartitionSchemeDescription(3, 0, 25)
                    });
                
            }

            return Ok();

        }

        // DELETE: api/polls/poll
        [HttpDelete("{poll}")]
        public async Task<IActionResult> Delete(string poll)
        {
            Uri serviceName = VotingWeb.GetVotingDataServiceName(this.serviceContext, poll);

            await this.fabricClient.ServiceManager.DeleteServiceAsync(new DeleteServiceDescription(serviceName)
            {
                ForceDelete = true
            });

            return Ok();
        }
    }
}