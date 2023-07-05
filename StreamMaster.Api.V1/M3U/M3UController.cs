using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirect;

using StreamMasterInfrastructure;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StreamMaster.Api.V1.M3U;

[V1ApiController]
public class M3UController : Controller
{
    [HttpPost]
    public ActionResult SetM3UFields(M3UFieldSettings m3UFieldSettings)
    {
        return Ok();
    }
}