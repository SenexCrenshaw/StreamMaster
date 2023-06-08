using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamMasterDomain.Dto;

public class EPGChannel
{
    public required string UUID { get; set; }
    public required string Logo { get; set; }
}

public class EPGProgram {

    public required string   Id { get; set; }
    public required string ChannelUuid { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime  Since { get; set; }
    public DateTime Till { get; set; }
    public required string Image  { get; set; }
        }