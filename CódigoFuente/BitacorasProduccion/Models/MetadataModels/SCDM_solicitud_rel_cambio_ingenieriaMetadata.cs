﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class SCDM_solicitud_rel_cambio_ingenieriaMetadata
    {

    }

    [MetadataType(typeof(SCDM_solicitud_rel_cambio_ingenieriaMetadata))]
    public partial class SCDM_solicitud_rel_cambio_ingenieria
    {
        [NotMapped]
        public int num_fila { get; set; }
    }
}